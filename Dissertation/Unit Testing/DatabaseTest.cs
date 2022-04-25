using dissertation.Controllers;
using dissertation.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Testing
{
    [TestClass]
    public class DatabaseTest
    {

        [TestMethod]
        public void DatabaseTesting()
        {

            var clientInsert = Client.Insert("Daniel", "Witts", "127.0.0.1", "Windows-10", 1, DateTime.Now.Round(new TimeSpan(0, 1, 0)), "MSw5");
            Assert.AreEqual(clientInsert, true);

            var clientTos = Client.InsertTos(500, 1200, 0, 1018);
            Assert.AreEqual(clientTos, true);

            var UpdatedDNS = Client.UpdateDnsSettings(1, 1018);
            Assert.AreEqual(UpdatedDNS, true);

            var clientViolation = Client.AlertNotification("C:\\Screenshot_Location", "Fuck", "Bristol", DateTime.Now.Round(new TimeSpan(0, 1, 0)), 1, 1018);
            Assert.AreEqual(clientViolation, true);

            var clientStatus = Client.Get(9).StatusUpdate(DateTime.Now.Round(new TimeSpan(0, 1, 0)));
            Assert.AreEqual(clientStatus, true);

            var clientEmotion = Client.InsertClientEmotions("Excited", 0.923m, 1018, 1);
            Assert.AreEqual(clientEmotion, true);
        }
    }
}
