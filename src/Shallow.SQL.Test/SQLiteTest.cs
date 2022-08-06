using Shallow.SQL.Test.Models;

namespace Shallow.SQL.Test
{
    [TestClass]
    public class SQLiteTest : SQLsystemTest
    {
        public SQLiteTest()
        {
            SQL.System = Systems.SystemType.SQLite;
            SQL.Database = $"{Environment.CurrentDirectory}\\..\\..\\..\\Shallow.SQL.db";
            users = User.Query.All();
        }
    }
}