using Shallow.SQL.Test.Models;

namespace Shallow.SQL.Test
{
    [TestClass]
    public class MySQLTest : SQLsystemTest
    {
        public MySQLTest()
        {
            SQL.System = Systems.SystemType.MySQL;
            SQL.Server = "localhost";
            SQL.Database = "sharpquent";
            SQL.User = "root";
            users = User.Query.All();
        }
    }
}
