using System;
using System.Collections.Generic;
using System.Linq;
using dissertation.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unit_Testing
{
    [TestClass]
    public class HomepageWebsiteDisplayTest
    {

        [TestMethod]
        public void HomepageDisplayTest()
        {
            var userID = 1;

            var client = Client.getUniqueClient(userID);
            var name = "";
            DateTime lastStatus;
            var violation = 0;
            foreach (var c in client.ToList())
            {
                name = c.Name;
                lastStatus = c.LastStatus;
                violation = c.TotalViolations;

                Assert.IsTrue(client.Any(x => x.Name == name));
                Assert.IsTrue(client.Any(x => x.LastStatus == lastStatus));
                Assert.IsTrue(client.Any(x => x.TotalViolations == violation));


            }


        }
    }

    [TestClass]
    public class OverviewWebsiteDisplayTest
    {

        [TestMethod]
        public void OverviewDisplayTest()
        {
            var userID = 1;

            var client = Client.ViewClient(userID);

            var Name = "";
            var ComputerName = "";
            var CurrentIP = "";
            var OS = "";
            var LastStatus = "";
            var Screenshot = "";
            var Keyword = "";
            var Location = "";
            var AlertTime = "";

            foreach (var c in client.ToList())
            {
                Name = c.Name;
                ComputerName = c.ComputerName;
                CurrentIP = c.CurrentIP;
                OS = c.OS;
                LastStatus = c.LastStatus.ToString();
                Screenshot = c.Screenshot;
                Keyword = c.Keyword;
                Location = c.Location;
                AlertTime = c.AlertTime.ToString();


                Assert.IsTrue(client.Any(x => x.Name == Name));
                Assert.IsTrue(client.Any(x => x.ComputerName == ComputerName));
                Assert.IsTrue(client.Any(x => x.CurrentIP == CurrentIP));
                Assert.IsTrue(client.Any(x => x.OS == OS));
                Assert.IsTrue(client.Any(x => x.LastStatus.ToString() == LastStatus));
                Assert.IsTrue(client.Any(x => x.Screenshot == Screenshot));
                Assert.IsTrue(client.Any(x => x.Keyword == Keyword));
                Assert.IsTrue(client.Any(x => x.Location == Location));
                Assert.IsTrue(client.Any(x => x.AlertTime.ToString() == AlertTime));


            }

        }
    }

    public class IbmEmotionTest
    {

        [TestMethod]
        public void IbmEmotionDisplayTest()
        {
            var tone_name = "Excited";
            decimal score = 0.586m;
            var clientID = 1;
            var userID = 1;
            

            var client = Client.InsertClientEmotions(tone_name, score, clientID, userID);

            Assert.AreEqual(client, true);
        }
    }

}
