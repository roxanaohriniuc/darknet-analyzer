using darknet_analyzer.Utilities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace darknet_analyzer.DataAccess
{
    public class BaseRepository : IDisposable
    {
        protected const int DuplicateKeyExceptionNumber = 2627;

        private SqlConnection connection { get; set; }

        public BaseRepository(string connectionString)
        {
            this.connection = new SqlConnection(connectionString);
        }

        public void Dispose()
        {
            this.connection.Close();
        }

        protected DataTable Query(string sql, params SqlParameter[] parameters)
        {
            return this.ExecuteSql(c => c.ExecuteReader().ToDataTable(), sql, parameters);
        }

        protected void NonQuery(string sql, params SqlParameter[] parameters)
        {
            this.ExecuteSql(c => c.ExecuteNonQuery(), sql, parameters);
        }

        protected Task NonQueryAsync(string command, params SqlParameter[] parameters)
        {
            return this.ExecuteSql(c => c.ExecuteNonQueryAsync(), command, parameters);
        }

        protected DataTable ExecuteStoredProcedure(string storedProcedureName, params SqlParameter[] parameters)
        {
            Func<SqlCommand, DataTable> commandAction = c =>
            {
                c.CommandType = System.Data.CommandType.StoredProcedure;
                return c.ExecuteReader().ToDataTable();
            };

            return this.ExecuteSql(commandAction, storedProcedureName, parameters);
        }

        protected T ExecuteSql<T>(Func<SqlCommand, T> commandAction, string sql, params SqlParameter[] parameters)
        {
            try
            {
                this.connection.Open();

                var command = new SqlCommand(sql, this.connection);

                if (parameters.Any())
                {
                    command.Parameters.AddRange(parameters);
                }

                return commandAction(command);
            }
            finally
            {
                this.connection.Close();
            }
        }
    }
}
