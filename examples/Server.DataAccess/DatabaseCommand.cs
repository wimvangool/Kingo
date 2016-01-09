using System;
using System.Collections.Generic;
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

        internal async Task<TAggregate> ExecuteSnapshotAsync<TAggregate>()
        {
            return Deserialize<TAggregate>(await ExecuteScalarAsync<string>());
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

        internal async Task<SqlDataReader> ExecuteDataReaderAsync()
        {
            await _connection.OpenAsync();

            return await _command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
        }

        internal async Task<bool> ExecuteNonQueryAsync()
        {
            await _connection.OpenAsync();

            try
            {                
                return await _command.ExecuteNonQueryAsync() > 0;                
            }
            finally
            {
                _connection.Close();
            }
        }

        #endregion

        #region [====== Factory Methods ======]

        private const string _Key = "Key";
        private const string _Version = "Version";
        private const string _Value = "Value";
        private const string _EventsTableType = "dbo.Events";
       
        internal static DatabaseCommand CreateSelectByKeyCommand<TKey>(string commandText, TKey key)
        {
            var command = new DatabaseCommand(commandText);
            command.Parameters.AddWithValue(_Key, key);
            return command;
        }

        internal static DatabaseCommand CreateInsertCommand<TKey, TVersion, TAggregate>(string commandText, TAggregate aggregate)            
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
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            where TAggregate : class, IVersionedObject<TKey, TVersion>
        {
            var command = new DatabaseCommand(commandText);
            command.Parameters.AddWithValue(_Key, aggregate.Key);
            command.Parameters.AddWithValue("OldVersion", originalVersion);
            command.Parameters.AddWithValue("NewVersion", aggregate.Version);
            command.Parameters.AddWithValue(_Value, Serialize(aggregate));
            return command;
        }

        internal static DatabaseCommand CreateInsertEventsCommand<TKey, TVersion, TAggregate>(string commandText, TAggregate aggregate, IList<IVersionedObject<TKey, TVersion>> events, TVersion? originalVersion)            
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
            where TAggregate : class, IVersionedObject<TKey, TVersion>
        {
            var command = new DatabaseCommand(commandText);
            command.Parameters.AddWithValue(_Key, aggregate.Key);
            command.Parameters.AddWithValue("OldVersion", ValueOrDBNull(originalVersion));

            var eventsParameter = command.Parameters.AddWithValue("Events", ConvertToTable(events));
            eventsParameter.SqlDbType = SqlDbType.Structured;
            eventsParameter.TypeName = _EventsTableType;
            return command;
        }

        private static DataTable ConvertToTable<TKey, TVersion>(IEnumerable<IVersionedObject<TKey, TVersion>> events)
            where TVersion : struct, IEquatable<TVersion>, IComparable<TVersion>
        {
            var table = CreateEventsTable();

            foreach (var @event in events)
            {
                table.Rows.Add(@event.Version, Serialize(@event));
            }
            return table;
        }

        private static DataTable CreateEventsTable()
        {
            var table = new DataTable(_EventsTableType);
            table.Columns.Add(_Version);
            table.Columns.Add(_Value);
            return table;
        }

        private static object ValueOrDBNull(object value)
        {
            return value ?? DBNull.Value;
        }

        #endregion

        #region [====== Serialization ======]

        private static readonly JsonSerializerSettings _SerializerSettings = new JsonSerializerSettings
        {            
            ContractResolver = new DefaultContractResolver
            {
                IgnoreSerializableAttribute = false
            },
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
            return JsonConvert.DeserializeObject<TAggregate>(value, _SerializerSettings);
        }

        #endregion
    }
}
