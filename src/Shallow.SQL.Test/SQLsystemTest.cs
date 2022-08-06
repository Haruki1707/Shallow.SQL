using Shallow.SQL.Test.Models;

namespace Shallow.SQL.Test
{
    public abstract class SQLsystemTest
    {
        internal User[] users;

        [TestMethod]
        public void canConnectToDatabaseAndObtainUsers()
        {
            Assert.IsNotNull(users);
        }

        [TestMethod]
        public void twoUsersOnDatabase()
        {
            Assert.AreEqual(2, users.Length);
        }

        [TestMethod]
        public void userContainsPhoneAndViceversa()
        {
            foreach (User user in users)
            {
                Assert.IsNotNull(user.PhoneNumber);
                Assert.AreEqual(user.PhoneNumber.Users.ID, user.ID);
            }
        }

        [TestMethod]
        public void userContainsProducts()
        {
            foreach (User user in users)
                foreach (Product product in user.Products)
                    Assert.IsNotNull(product);
        }

        [TestMethod]
        public void userCanRefresh()
        {
            string _testName = "Can Refresh";
            User user = users[0];
            user.Name = _testName;
            user.Refresh();
            Assert.AreNotEqual(user.Name, _testName);
        }

        [TestMethod]
        public void userCanUpdate()
        {
            string _originalName, _testName = "Can Update";
            User user = users[1];
#pragma warning disable CS8600
            _originalName = user.Name;
#pragma warning restore CS8600
            user.Name = _testName;
            user.Update();
            user.Refresh();
            Assert.AreEqual(user.Name, _testName);
            user.Name = _originalName;
            user.Update();
            user.Refresh();
            Assert.AreEqual(user.Name, _originalName);
        }

        [TestMethod]
        public void userCanBeCreatedAndDeleted()
        {
            User user = new User();
            user.Name = "Can Create";
            user.Email = "cc@gmail.com";
            Assert.IsTrue(user.Create());
            Assert.IsTrue(user.Delete());
        }
    }
}
