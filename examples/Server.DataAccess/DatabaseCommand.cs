using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters;
using System.Threading.Tasks;
using Kingo.Messaging.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kingo.Samples.Chess
{
    internal sealed class DatabaseCommand : IDisposable        
    {
        private static readonly string _ConnectionString = ConfigurationManager.ConnectionStrings["Server.Data.Sql"].ConnectionString;
        private readonly SqlConnection _connection;
        private readonly SqlCommand _command;        

        internal DatabaseCommand(string commandText)
        {
            _connection = new SqlConnection(_ConnectionString);
            _command = _connection.CreateCommand();
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = commandText;
        }        

        public void Dispose()
        {
            _command.Dispose();
            _connection.Dispose();
        }

        #region [====== Execution ======]

        internal SqlParameterCollection Parameters
        {
            get { return _command.Parameters; }
        }

        internal async Task<TValue> ExecuteScalarAsync<TValue>()
        {
            await _connection.OpenAsync();

            try
            {
                return (TValue) await _command.ExecuteScalarAsync();
            }
            finally
            {
                _connection.Close();
            }            
        }

        internal async Task ExecuteNonQueryAsync()
        {
            await _connection.OpenAsync();

            try
            {
                var rowCount = await _command.ExecuteNonQueryAsync();
                if (rowCount == 0)
                {
                    // TODO: throw concurrency exception.
                }
            }
            finally
            {
                _connection.Close();
            }            
        }

        internal async Task<SqlDataReader> ExecuteDataReader()
        {
            await _connection.OpenAsync();

            return await _command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
        }

        #endregion

        #region [====== Factory Methods ======]

        private const string _Key = "Key";
        private const string _Version = "Version";
        private const string _Value = "Value";        

        internal static DatabaseCommand CreateInsertCommand<TKey, TVersion, TAggregate>(string commandText, TAggregate aggregate)
            where TKey : struct, IEquatable<TKey>
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            where TAggregate : class, IVersionedObject<TKey, TVersion>
        {
            var command = new DatabaseCommand(commandText);
            command.Parameters.AddWithValue(_Key, aggregate.Key);
            command.Parameters.AddWithValue(_Version, aggregate.Version);
            command.Parameters.AddWithValue(_Value, Serialize(aggregate));
            return command;
        }

        internal static DatabaseCommand CreateUpdateCommand<TKey, TVersion, TAggregate>(string commandText, TAggregate aggregate, TVersion originalVersion)
            where TKey : struct, IEquatable<TKey>
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            where TAggregate : class, IVersionedObject<TKey, TVersion>
        {
            throw new NotImplementedException();
        }

        #endregion

        #region [====== Serialization ======]

        private static readonly JsonSerializerSettings _SerializerSettings = new JsonSerializerSettings
        {            
            ContractResolver = new DefaultContractResolver
            { IgnoreSerializableAttribute = false },
            Formatting = Formatting.None,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            TypeNameHandling = TypeNameHandling.None            
        };

        private static string Serialize(object aggregate)
        {                        
            return JsonConvert.SerializeObject(aggregate, _SerializerSettings);
        }

        private static TAggregate Deserialize<TAggregate>(string value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
