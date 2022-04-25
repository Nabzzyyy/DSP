using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client_Kidz_protection
{
    /// <summary>
    /// Client class to communicate with the rest API
    /// </summary>
    internal class Client
    {

        private HttpClient client = null;

        private static Client instance = null;

        private Client() { }

        public string GetClientHash()
        {
            return ConfigurationManager.AppSettings["ClientHash"];
        }

        //singleton
        public static Client GetInstance
        {
            get
            {
                if (instance == null) instance = new Client();
                return instance;
            }
        }

        /// <summary>
        /// Making the status call. This will let the server know I'm online
        /// </summary>
        /// <returns></returns>
        public void StatusCallAsync()
        {
            client = new HttpClient();
            var url = ConfigurationManager.AppSettings["url"] + "/StatusAPI";

            var request = new StatusRequestStruct() { ClientHash = GetClientHash() };
            
            var task = client.PostAsync(url, ConvertToByteContent(request));
            task.Wait();
            var result = task.Result;
            if (result != null);
        }

        /// <summary>
        /// Send client information to the server e.g OS, computer name etc.
        /// </summary>
        /// <returns></returns>
        public void CurrentSystemAsync()
        {
            client = new HttpClient();
            var url = ConfigurationManager.AppSettings["url"] + "/CurrentSystemAPI";
            string OS = "UNKNOWN";
            if (OperatingSystem.IsWindows())
            {
                OS = "Windows";
            }
            else if (OperatingSystem.IsLinux())
            {
                OS = "Linux";
            }
            else if (!OperatingSystem.IsMacOS())
            {
                OS = "Mac OS";
            }
            var request = new CurrentSystemStruct() { ClientHash = GetClientHash(), ComputerName = Environment.MachineName, OS = OS };
            _ = client.PostAsync(url, ConvertToByteContent(request));
        }

        /// <summary>
        /// returns any violations that may have occurred.
        /// </summary>
        /// <returns></returns>
        public void AlertAsync(Alert alert)
        {
            client = new HttpClient();
            var url = ConfigurationManager.AppSettings["url"] + "/AlertAPI/POST";
            Logging.Log.Debug("Sending violation", $"{JsonConvert.SerializeObject(alert)}");
            var task = client.PostAsync(url, ConvertToByteContent(alert));
            Task.WaitAll(task);
            if (task.Result.IsSuccessStatusCode)
            {
                Console.WriteLine("Success!!");
                Logging.Log.Debug("Sending violation", $"Success");
            }
            else
            {

                var e = task.Result.StatusCode;
                Console.WriteLine($"NOOO!! {e}");
                Logging.Log.Debug("Sending violation", $"{e}");
            }
        }

        /// <summary>
        /// Grabs all settings for this client
        /// </summary>
        /// <returns></returns>
        public void Setting()
        {
            client = new HttpClient();
            var url = ConfigurationManager.AppSettings["url"] + "/SettingsAPI";

            var request = new StatusRequestStruct() { ClientHash = GetClientHash() };
            var resposne = client.PostAsync(url, ConvertToByteContent(request));

            resposne.Wait();

            if (resposne.Result != null)
            {
                if (resposne.Result.IsSuccessStatusCode)
                {

                    var result = resposne.Result.Content.ReadAsStringAsync();
                    SettingsResponse settings = JsonConvert.DeserializeObject<SettingsResponse>(result.Result);


                    DNS.SetDNS(new string[] { settings.PreferDns, settings.AlternativeDns });

                    if (!(settings.TosSetting.Any(x => (DayOfWeek)x.Tos_Day == DateTime.Now.DayOfWeek && DateTime.Now.Date.AddMinutes(x.Tos_Start_Minute) < DateTime.Now && DateTime.Now.Date.AddMinutes(x.Tos_End_Minute) > DateTime.Now)))
                    {
                        //lock user pc
                       //Program.LockWorkStation();
                    }

                }
            }
        }

        /// <summary>
        /// Pass sentence for server to do tone analysation
        /// </summary>
        /// <returns></returns>
        public void IBMAsync(string message)
        {
            client = new HttpClient();
            var url = ConfigurationManager.AppSettings["url"] + "/IbmAPI";

            _ = client.PostAsync(url, ConvertToByteContent(new IBMStruct() { message = message, ClientHash = GetClientHash()}));
        }


        public ByteArrayContent ConvertToByteContent(object obj)
        {
            var myContent = JsonConvert.SerializeObject(obj);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return byteContent;
        }
    }
}
