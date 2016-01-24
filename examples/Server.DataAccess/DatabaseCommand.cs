using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

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

        internal const string KeyParameter = "Key";
        internal const string VersionParameter = "Version";
        internal const string ValueParameter = "Value";
        internal const string TypeParameter = "Type";
        internal const string OriginalVersionParameter = "OriginalVersion";

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
    }
}
