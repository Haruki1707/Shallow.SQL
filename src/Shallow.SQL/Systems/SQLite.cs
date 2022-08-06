using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Shallow.SQL.Structs;
using System.Diagnostics;
using System.Linq;
using System.Data.Common;

namespace Shallow.SQL.Systems
{
    internal class SQLite : dbSystemAbstract
    {
        public SQLiteConnection Connection => (SQLiteConnection)_connection;

        internal SQLite(string server, string password)
        {
            SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
            { 
                DataSource = server,
            };

            if(!string.IsNullOrWhiteSpace(password))
                connectionString.Password = password;
            
            _connection = new SQLiteConnection(connectionString.ConnectionString);
        }

        public SQLiteCommand Command(string commandText)
            => (SQLiteCommand)command(commandText);
        internal override DbCommand command(string commandText)
            => new SQLiteCommand(commandText, Connection);

        internal override string _tableExistsQuery(string tableName)
            => $"SELECT EXISTS(SELECT 1 FROM sqlite_master WHERE type='table' AND name='{tableName}' COLLATE NOCASE)";

        internal override string _columnExistsQuery(string tableName, string columnName)
            => $"SELECT EXISTS(SELECT 1 FROM pragma_table_info('{tableName}') WHERE name='{columnName}' COLLATE NOCASE)";

        internal override void addParameter(DbCommand command, string parameter, object value)
            => ((SQLiteCommand)command).Parameters.AddWithValue(parameter, value);
    }
}
