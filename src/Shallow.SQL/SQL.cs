using Shallow.SQL.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shallow.SQL.Structs;
using System.Diagnostics;
using Shallow.SQL.Exceptions;

namespace Shallow.SQL
{
    public static class SQL
    {
        public static string Server { get; set; }
        public static string Database { get; set; }
        public static string User { get; set; }
        public static string Password { get => s_password; set => s_password = value; }
        public static uint Port { get => s_port; set => s_port = value; }
        ///<summary>
        ///Indicates which SQL system to use, default value: <c>SystemType.MySQL</c>
        /// </summary>
        public static SystemType System { get; set; }

        private static string s_password = "";
        private static uint s_port = 3306;
        private static Dictionary<SystemType, dbSystemAbstract> systems = new Dictionary<SystemType, dbSystemAbstract>();

        private static dbSystemAbstract GetSystem()
        {
            if (string.IsNullOrEmpty(Server) && string.IsNullOrEmpty(Database))
                throw new ServerOrDatabaseNotFoundException("Define wheter the Server or the Database you are connecting to");

            if(!systems.ContainsKey(System))
                switch (System)
                {
                    case SystemType.MySQL:
                        systems.Add(System, new MySQL(Server, User, Password, Database, Port));
                        break;
                    case SystemType.SQL_Server:
                        systems.Add(System, new SQL_Server(Server, User, Password, Database, Port));
                        break;
                    case SystemType.SQLite:
                        systems.Add(System, new SQLite(Database, Password));
                        break;
                }
            return systems[System];
        }

        internal static bool TableExists(string tableName)
            => GetSystem().TableExists(tableName);
        internal static bool ColumnExists(string tableName, string columnName)
            => GetSystem().ColumnExists(tableName, columnName);
        internal static bool ExecuteNonQuery<U>(ModelTable<U> modelTable, string query, Column[] props, Type type, string table)
            => GetSystem().ExecuteNonQuery(modelTable, query, props, type, table);
        internal static U[] GetObjects<U>( string query, Column[] props, object insObject)
            => GetSystem().GetObjects<U>(query, props, insObject);
    }
}
