using MySqlConnector;
using Shallow.SQL.Structs;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Shallow.SQL.Systems
{
    internal class MySQL : dbSystemAbstract
    {
        public MySqlConnection Connection => (MySqlConnection)_connection;

        internal MySQL(string server, string user, string password, string db, uint port)
        {
            _database = db;
            _connection = new MySqlConnection(new MySqlConnectionStringBuilder
            {
                Server = server,
                UserID = user,
                Password = password,
                Database = db,
                Port = port
            }.ConnectionString);
        }

        public MySqlCommand Command(string commandText)
            => (MySqlCommand)command(commandText);
        internal override DbCommand command(string commandText)
            => new MySqlCommand(commandText, Connection);

        internal override void addParameter(DbCommand command, string parameter, object value)
            => ((MySqlCommand)command).Parameters.AddWithValue(parameter, value);
    }
}
