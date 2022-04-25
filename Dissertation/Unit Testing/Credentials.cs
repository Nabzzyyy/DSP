using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using dissertation.Controllers;

namespace Unit_Testing
{
    [TestClass]
    public class Credentials
    {

        [TestMethod]
        public void RegisterUniqueCredentialsTest()
        {
            var FirstName = "Steven";
            var LastName = "Gerrad";
            var Email = "StevenGerrad@gmail.com";
            var Password = "StevenTest";

            var user = dissertation.ObjectModel.User.Insert(FirstName, LastName, Email, Password);
            

            Assert.AreEqual(user, true);
        }
    }

    [TestClass]
    public class LoginTest
    {

        [TestMethod]
        public void LoginCredentialsTest()
        {
            var Email = "JimmyNeutron@gmail.com";
            var Password = "JimmyTest";

            var user = dissertation.ObjectModel.User.Get(Email);

            Assert.AreEqual(user.Email, Email);
            Assert.AreEqual(user.UserPassword, Password);
        }
    }
}
