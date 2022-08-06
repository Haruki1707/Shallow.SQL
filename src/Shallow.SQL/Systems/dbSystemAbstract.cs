using Shallow.SQL.Structs;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Shallow.SQL.Systems
{
    internal abstract class dbSystemAbstract
    {
        internal int _count = 0;
        internal Mutex _mutex = new Mutex();
        internal string _database;
        internal DbConnection _connection;
        internal abstract DbCommand command(string commandText);

        internal virtual string _tableExistsQuery(string tableName)
            => "SELECT EXISTS ( SELECT `TABLE_NAME` FROM `INFORMATION_SCHEMA`.`TABLES` WHERE " +
                    $"`TABLE_NAME` = '{tableName}' " +
                    $"AND `TABLE_SCHEMA` = '{_database}' " +
                ") AS `is-exists`";
        internal virtual string _columnExistsQuery(string tableName, string columnName)
            => "SELECT EXISTS ( SELECT `COLUMN_NAME` FROM `INFORMATION_SCHEMA`.`COLUMNS` WHERE " +
                    $"`TABLE_SCHEMA` = '{_database}' " +
                    $"AND `TABLE_NAME` = '{tableName}' " +
                    $"AND `COLUMN_NAME` = '{columnName}' " +
                ") AS `is-exists`";

        internal virtual bool TableExists(string tableName)
        {
            bool result = false;

            OpenConnection();
            DbDataReader reader = command(_tableExistsQuery(tableName)).ExecuteReader();
            while (reader.Read())
                result = reader.GetBoolean(0);
            CloseConnection();

            return result;
        }

        internal virtual bool ColumnExists(string tableName, string columnName)
        {
            bool result = false;

            OpenConnection();
            DbDataReader reader = command(_columnExistsQuery(tableName, columnName)).ExecuteReader();
            while (reader.Read())
                result = reader.GetBoolean(0);
            CloseConnection();

            return result;
        }

        public virtual bool ExecuteNonQuery<U>(ModelTable<U> modelTable, string query, Column[] props, Type type, string table)
        {
            Stopwatch watch = Stopwatch.StartNew();
            bool result = true;
            DbCommand command = this.command(query);

            try
            {
                if (query.Contains("@"))
                    foreach (Column item in props.Where(p => p.VarName != "ID"))
                        addParameter(command, $"@{item.ColumnName}", type.GetProperty(item.VarName).GetValue(modelTable).ToString());

                OpenConnection();
                int execution = command.ExecuteNonQuery();
                CloseConnection();

                if (execution <= 0)
                    result = false;

                if (query.Contains("INSERT") && ColumnExists(table, "id"))
                {
                    OpenConnection();
                    DbDataReader lastID = this.command($"SELECT MAX(id) FROM {table}").ExecuteReader();
                    while (lastID.Read())
                        modelTable.ID = lastID.GetString(0);
                    CloseConnection();
                }
                if (query.Contains("DELETE"))
                    modelTable.AlreadyOnDB = false;
            }
            catch (Exception e)
            {
                Log($"{query}");
                throw e;
            }

            watch.Stop();
            Log($"{query} IN {watch.ElapsedMilliseconds}ms");
            return result;
        }

        public virtual U[] GetObjects<U>(string query, Column[] props, object insObject)
        {
            Stopwatch watch = Stopwatch.StartNew();
            List<U> result = new List<U>();
            props = props.Concat(new Column[] { new Column { ColumnName = "id", VarName = "ID" } }).ToArray();

            try
            {
                OpenConnection();
                DbDataReader reader = command(query).ExecuteReader();

                while (reader.Read())
                {
                    ObjectActivator<U> temp = new ObjectActivator<U>(insObject != null ? insObject : null);
                    temp.SetValue("AlreadyOnDB", true);
                
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var tempTo = props.Where(c => string.Equals(c.ColumnName, reader.GetName(i), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        temp.SetValue(
                            tempTo != null ? tempTo.VarName : reader.GetName(i),
                            reader.GetValue(i)
                        );
                    }
                    result.Add(temp.GetObject);
                }
                CloseConnection();
            }
            catch (Exception e)
            {
                Log($"{query}");
                throw e;
            }

            watch.Stop();
            Log($"{query} IN {watch.ElapsedMilliseconds}ms");
            return result.ToArray();
        }

        internal virtual void OpenConnection()
            => openCloseConnection();

        internal virtual void CloseConnection()
            => openCloseConnection(close: true);

        internal virtual void openCloseConnection(bool close = false)
        {
            _mutex.WaitOne();
            if (!close)
            {
                if (_count == 0)
                    _connection.Open();
                _count++;
                return;
            }

            _count--;
            if (_count == 0)
                _connection.Close();
            _mutex.ReleaseMutex();
        }

        private void Log(string message)
        {
            Debug.WriteLine($"Shallow.SQL query: {message}");
        }

        internal abstract void addParameter(DbCommand command, string parameter, object value);
    }
}
