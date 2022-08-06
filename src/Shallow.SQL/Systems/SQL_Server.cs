using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using Shallow.SQL.Structs;
using System.Linq;
using System.Data.Common;

namespace Shallow.SQL.Systems
{
    internal class SQL_Server : dbSystemAbstract
    {
        public SqlConnection Connection => (SqlConnection)_connection;
        internal SQL_Server(string server, string user, string password, string db, uint port)
        {
            _database = db;
            _connection = new SqlConnection(new SqlConnectionStringBuilder
            {
                DataSource = $"{server},{port}",
                UserID = user,
                Password = password,
                InitialCatalog = db,
            }.ConnectionString);
        }

        public SqlCommand Command(string commandText)
            => (SqlCommand)command(commandText);
        internal override DbCommand command(string commandText)
            => new SqlCommand(commandText, Connection);

        internal override void addParameter(DbCommand command, string parameter, object value)
            => ((SqlCommand)command).Parameters.AddWithValue(parameter, value);
    }
}
