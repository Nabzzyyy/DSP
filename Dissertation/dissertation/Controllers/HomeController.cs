using dissertation.Models;
using dissertation.ObjectModel;
using IPAddress_Location_MVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace dissertation.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Index page showing first webpage. 
        /// </summary>
        /// <returns>View for that webpage.</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Login page allowing user to login and register account. 
        /// </summary>
        /// <returns>View for that webpage.</returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Directs to login page which allows user to login and gets given a token unique to the user.
        /// </summary>
        /// <param name="userInput">Allows fields for user input such as email and password.</param> 
        /// <returns>Directs to home page if user inputs are valid</returns>
        [HttpPost]
        public ActionResult Login(LoginModel userInput)
        {
            if(ModelState.IsValid){
                var user = ObjectModel.User.Get(userInput.Email);
                if (user != null) {
                    if (user.UserPassword == userInput.Password)
                    {
                        //GENERATE, DELETE TOKEN
                        var token = Token.getInstance().GenerateToken(user.ID, "LOGIN");
                        var expiry = DateTime.Now.AddMinutes(60);
                        Token.getInstance().AddToken(user.ID, token, expiry);
                        HttpCookie myCookie = new HttpCookie("Authorisation");
                        myCookie.Value = token;
                        myCookie.Expires = expiry;
                        Response.Cookies.Add(myCookie);

                        //LOG IN  
                        return RedirectToAction("Homepage", "Home");
                    }
                }
            }
            return View(userInput);
        }

        /*
        public ActionResult Logout(LoginModel userInput) {
            if (ModelState.IsValid)
            {
                var user = ObjectModel.User.Get(userInput.Email);
                if (user != null)
                {
                    var token = Token.getInstance().GenerateToken(user.ID, "LOGIN");
                    var expiry = DateTime.Now.AddMinutes(0);
                    Token.getInstance().AddToken(user.ID, token, expiry);
                    HttpCookie myCookie = new HttpCookie("Authorisation");
                    myCookie.Value = token;
                    myCookie.Expires = expiry;
                    Response.Cookies.Add(myCookie);

                    //LOG OUT 
                    return RedirectToAction("Indez", "Index");
                }
            }
            return null;
        }
        */

        /// <summary>
        /// Logout functionaity to delete token and user gets redirected to index page to re-login. 
        /// </summary>
        /// <returns>redirected to index page</returns>
        public ActionResult Logout()
        {
            var cookie = Request.Cookies.Get("Authorisation");
            
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var userID = Int32.Parse(decodedToken[2]);
            cookie.Value = null;
            cookie.Expires = DateTime.Now.AddDays(-10);
            Token.getInstance().deleteToken(userID);
            Response.Cookies.Remove("Authorisation");
            Response.Cookies.Set(cookie);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Registeration for the parent which the password gets checked if it password matches with the second attempt.
        /// </summary>
        /// <param name="userInput">Gets parent input for their details.</param> 
        /// <returns>User input and directed to page.</returns>
        public ActionResult Register(RegisterModel userInput)
        {
            if (ModelState.IsValid)
            {
                if (userInput.Password == userInput.ConfirmPassword){
                    //Insert data into DB
                    ObjectModel.User.Insert(userInput.FirstName, userInput.LastName, userInput.Email, userInput.Password);
                }
            }
            return View(userInput);
        }

        /// <summary>
        /// Once parent is logged in then they get directed to the homepage
        /// </summary> 
        /// <returns>View page with list of all child belonging to the parent</returns>
        [CustomAuthorize]
        public ActionResult Homepage()
        {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var list = Client.getUniqueClient(Int32.Parse(decodedToken[2]));
            
            return View(list);
        }

        /// <summary>
        /// Displays all violations that has been caused from the parent's child.  
        /// </summary>
        /// <returns>view page with all clientList that caused a violation</returns>
        [CustomAuthorize]
        public ActionResult Overview()
        {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var list = Client.ViewClient(Int32.Parse(decodedToken[2]));
            list = list.Where(x => x.Keyword != "");

            List<ClientStruct> clientList = new List<ClientStruct>();

            foreach (var client in list.ToList())
            {
                //string screenshotBase64Encode = System.IO.File.ReadAllText(client.Screenshot);
                var updatedClient = client;
                //updatedClient.Screenshot = screenshotBase64Encode;

                clientList.Add(updatedClient);
                /*if (System.IO.File.Exists(client.Screenshot))
                {
                    string screenshotBase64Encode = System.IO.File.ReadAllText(client.Screenshot);
                    var updatedClient = client;
                    updatedClient.Screenshot = screenshotBase64Encode;

                    clientList.Add(updatedClient);
                }*/
            }

            return View(clientList);
        }

        /// <summary>
        /// reitreves all clients that belongs to the user.
        /// </summary>
        /// <param name="id">Passes client name into the function</param> 
        /// <returns>json format of the clients.</returns>
        [HttpPost]
        [CustomAuthorize]
        public string RetrieveClientInformation(string id) {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var allClients = Client.ViewClient(Int32.Parse(decodedToken[2]));

            var retval = allClients.Where(x => x.Name == id);
            var json = JsonConvert.SerializeObject(retval);
            return json;
        }

        /// <summary>
        /// location of directory foler to copy files/folders into these location
        /// </summary>
        /// <param name="sourceDirectory">...</param> 
        /// /// <param name="targetDirectory">...</param> 
        /// <returns></returns>
        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        /// <summary>
        /// Copying folders/files into new directory
        /// </summary>
        /// <param name="source">Source to get the files.</param> 
        /// <param name="target">Location to copy the files into target folder.</param> 
        /// <returns></returns>
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        
        static readonly string rootFolder = @"C:\Program Files\DspWebsite\client_software2";
        /// <summary>
        /// Gets the client's software project and alters app.config to change to clientHash and stores into another directory making the software unique to that specific client.
        /// </summary>
        /// <param name="clientName"></param> 
        /// <returns>null</returns>
        [HttpGet]
        [CustomAuthorize]
        public FileResult Software(string clientName)
        {
            try
            {

                /*
                 * Cloning file - Replicates existing folder to edit 
                 */
                string sourceDirectory = @"C:\Program Files\DspWebsite\client_software";
                string targetDirectory = @"C:\Program Files\DspWebsite\client_software2";

                Copy(sourceDirectory, targetDirectory);


                /*
                 * Pass clientName into the database - client information table (Pass user ID into stored procedure)
                 * View recent client details - Grab clientHash
                 * Update client information - adding clientHash
                 */

                var cookie = Request.Cookies.Get("Authorisation");
                var token = cookie.Value;
                var decodedToken = Base64Handler.Decoder(token).Split(',');
                var userID = Int32.Parse(decodedToken[2]);

                Client.Insert(clientName, "", "", "", userID, DateTime.Now.Round(new TimeSpan(0, 1, 0)), "");
                
                var clientID = Client.UniqueClient(userID);
                Logging.Log.Error("home", "client ID:" + clientID.ToString());
                var encode = userID.ToString() + "," + clientID.ToString();
                Logging.Log.Error("home", "user and client:" + encode.ToString());
                var clientHash = Base64Handler.Encoder(encode);
                Logging.Log.Error("home", "clientHash:" + clientHash.ToString());

                Client.UpdateClientHash(clientHash, clientID);

                /*
                 *Read config text file 
                 *Edit text file and change url and clientHash
                 *Remove old app.config file
                 *Add new app.config file + write variable into file
                 */

                string text = System.IO.File.ReadAllText(@"C:\Program Files\DspWebsite\client_software2\App.config");
                text = text.Replace("[SERVER_URL]", "http://192.168.1.132/DSP/api/").Replace("[CLIENT_HASH]", clientHash);

                string authorsFile = "App.config";

                if (System.IO.File.Exists(Path.Combine(rootFolder, authorsFile)))
                {

                    System.IO.File.Delete(Path.Combine(rootFolder, authorsFile));
                }

                string newTextFilePath = @"C:\Program Files\DspWebsite\client_software2\App.config";

                using (StreamWriter sw = System.IO.File.CreateText(newTextFilePath))
                {
                    sw.Write(text);
                }

                /*
                 * Create zip file 
                 * Store zip file into variable & removes temp clone to prevent memory issue
                 * Send variable to front-end
                 */
                string startPath = @"C:\Program Files\DspWebsite\client_software2";
                string zipPath = @"C:\Program Files\DspWebsite\client_software2.zip";

                ZipFile.CreateFromDirectory(startPath, zipPath);

                string[] files = Directory.GetFiles(rootFolder);
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }

                return File(zipPath, "application/zip", "client_software.zip");
            }
            catch(Exception ex)
            {
                Logging.Log.Error("home", ex.ToString());
            }
            return null;
        }

        /// <summary>
        ///  Download page allowing the parent to download the software.
        /// </summary>
        /// <returns>View page</returns>
        [CustomAuthorize]
        public ActionResult DownloadClient() 
        {
            return View();
        }

        /// <summary>
        ///  DNS settings page allowing the parent to set the DNS for their child.
        /// </summary>
        /// <returns>View page</returns>
        [CustomAuthorize]
        public ActionResult DnsSettings()
        {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var list = Client.getAllClientSettings(Int32.Parse(decodedToken[2]));
            return View(list);
        }

        /// <summary>
        /// DNS setting gets updated
        /// </summary>
        /// <param name="ClientName">Name of children to identify them</param> 
        /// <param name="DnsSetting">storing DNS value</param> 
        /// <returns>null.</returns>
        [HttpPost]
        [CustomAuthorize]
        public string UpdateDns(string ClientName, int DnsSetting) {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var user = new User(Int32.Parse(decodedToken[2]));
            var client = Client.Get(ClientName, user);
            
           var UpdatedDNS = Client.UpdateDnsSettings(client.ID, DnsSetting);

            return null;
        }

        /// <summary>
        /// Child's setting gets retrieved that belongs to the parent.
        /// </summary>
        /// <param name="id">parent unique id to identify them.</param> 
        /// <returns>JSON format of client setting.</returns>
        [HttpPost]
        [CustomAuthorize]
        public string RetrieveClientSettings(string id)
        {
            var setting = Client.getAllClientSettings(Int32.Parse(id));
            var json = JsonConvert.SerializeObject(setting);
            return json;
        }

        /// <summary>
        /// Collecting the time on screen for the child. 
        /// </summary>
        /// <returns>view page and list of children belonging to the parent that has time on screen settings</returns>
        [CustomAuthorize]
        public ActionResult TosSettings()
        {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var list = Client.getAllClientSettings(Int32.Parse(decodedToken[2]));
            return View(list);
        }

        /// <summary>
        /// Setting time on screen setting for children and removing old time on screen settings
        /// </summary>
        /// <param name="tosData">storing time on screen; start and end time</param> 
        /// <param name="ClientName">Name of children to identify them</param> 
        /// <returns>null</returns>
        [HttpPost]
        [CustomAuthorize]
        public string RetrieveClientTosSettings(string[][] tosData, string ClientName)
        {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var user = new User(Int32.Parse(decodedToken[2]));
            var client = Client.Get(ClientName, user);
            var counter = 0;
            if(tosData != null && tosData.Length > 0) Client.RemoveTosSettings(client.ID);
            foreach (var day in tosData) {
                var sdt = new TimeSpan(0, Int32.Parse(day[0]), 0);
                var edt = new TimeSpan(0, Int32.Parse(day[1]), 0);
                Client.InsertTos((int)sdt.TotalMinutes, (int)edt.TotalMinutes, counter, client.ID);
                counter++;
            }

            return null;
        }

        /// <summary>
        /// Calling ipstack API to get geo location from IP address such as country name and city.
        /// </summary>
        /// <returns>view page with geo location.</returns>
        [CustomAuthorize]
        public ActionResult GPS()
        {
            List<ClientGpsStruct> list = new List<ClientGpsStruct>();

            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var clients = Client.ViewClient(Int32.Parse(decodedToken[2]));

            foreach (var individual_client in clients) {
                ClientGpsStruct clientGPS = new ClientGpsStruct();
                clientGPS.Name = individual_client.Name;
                clientGPS.CurrentIP = individual_client.CurrentIP;

                LocationModel GpsLocation = new LocationModel();
                string url = string.Format("http://api.ipstack.com/{0}?access_key=640b3eadaaf0b4636c2d6707e9f36ef3", clientGPS.CurrentIP);
                using (WebClient client = new WebClient())
                {
                    string json = client.DownloadString(url);
                    GpsLocation = new JavaScriptSerializer().Deserialize<LocationModel>(json);
                }

                clientGPS.IP = GpsLocation.IP;
                clientGPS.Country_Code = GpsLocation.Country_Code;
                clientGPS.Country_Name = GpsLocation.Country_Name;
                clientGPS.Region_Code = GpsLocation.Region_Code;
                clientGPS.Region_Name = GpsLocation.Region_Name;
                clientGPS.City = GpsLocation.City;
                clientGPS.Zip_Code = GpsLocation.Zip_Code;
                clientGPS.Time_Zone = GpsLocation.Time_Zone;
                clientGPS.Latitude = GpsLocation.Latitude;
                clientGPS.Longitude = GpsLocation.Longitude;
                clientGPS.Metro_Code = GpsLocation.Metro_Code;
                list.Add(clientGPS);
            }
            return View(list);
        }

        /// <summary>
        /// Report generated page allowing parent to view child's behaviour online.
        /// </summary>
        /// <returns>view page with list of clients emotion</returns>
        [CustomAuthorize]
        public ActionResult AnalyticalReport()
        {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var clientEmotion = Client.GetClientEmotions(Int32.Parse(decodedToken[2]));

            return View(clientEmotion);
        }

        /// <summary>
        /// Creating a violation graph showing the amount of violations each child has created in bar graph form.
        /// </summary>
        /// <returns>null</returns>
        public ActionResult ViolationGraph()
        {
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var allClients = Client.getUniqueClient(Int32.Parse(decodedToken[2]));

            var clientList = allClients.Where(x => x.Name != "");



            List<string> ClientName = new List<string>();
            List<int> ClientViolation = new List<int>();

            foreach (var client in clientList.ToList())
            {
                ClientName.Add(client.Name);
                ClientViolation.Add(client.TotalViolations);
            }


            var myChart = new Chart(width: 600, height: 400)
            .AddTitle("Violations")
            .AddSeries(
                name: "Client Names",
                xValue: ClientName,
                yValues: ClientViolation)
            .Write("png");

            return null;
        }

        /// <summary>
        /// Creating a emotional graph showing each child's emotional behaviour on the system in pie chart form.
        /// </summary>
        /// <returns>NULL</returns>
        [HttpGet]
        public ActionResult EmotionGraph(string id)
        {
            int ClientID = Int32.Parse(id);
            var cookie = Request.Cookies.Get("Authorisation");
            var token = cookie.Value;
            var decodedToken = Base64Handler.Decoder(token).Split(',');
            var allClientEmotions = Client.GetClientEmotions(Int32.Parse(decodedToken[2]));

            var clientEmotions = allClientEmotions.GroupBy(x => x.ClientID);


            var emotions = clientEmotions.Where(x => x.Key == ClientID);
            List<string> IBMToneName = new List<string>();
            List<string> IBMScore = new List<string>();

            foreach (var emotion in emotions)
            {
                foreach (var em in emotion)
                {

                    IBMToneName.Add(em.ToneName);
                    IBMScore.Add(emotion.Where(x => x.ToneName == em.ToneName).Average(x => x.ToneScore).ToString());

                }


            }

            var myChart = new Chart(width: 600, height: 400)
               .AddTitle(Client.Get(ClientID).Nickname)
               .AddSeries("Default", chartType: "Pie",
                   xValue: IBMToneName, xField: "Tone Name",
                   yValues: IBMScore, yFields: "Score")
                   .Write("png");

            return null;
        }
    }


    public static class DateExtensions
    {

        /// <summary>
        /// Rounds time preventing fraction error when data getting inserted includes time.
        /// </summary>
        /// <param name="date">Calculates date.</param> 
        /// <param name="span">Rounds the time.</param> 
        /// <returns>rounded date and time.</returns>

        public static DateTime Round(this DateTime date, TimeSpan span)
        {
            long ticks = (date.Ticks + (span.Ticks / 2) + 1) / span.Ticks;
            return new DateTime(ticks * span.Ticks);
        }
    }

}