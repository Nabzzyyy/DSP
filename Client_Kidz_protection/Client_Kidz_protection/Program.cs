using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client_Kidz_protection
{
    internal class Program
    {

        private List<string> SwearWordsList = new();
        private List<string> SexualWordsList = new();
        private Dictionary<int, string> WordUsed = new Dictionary<int, string>();
        private bool IsRunning = false;
        static void Main(string[] args)
        {
            
            Program p = new();

            p.loadToMemory();
            p.Run();
        }

        private void loadToMemory()
        {
            string swearWordFile = "./Word_Lists/Swear_word_RedList.txt";
            string sexualWordFile = "./Word_Lists/Sexual_RedList.txt";
            if (File.Exists(swearWordFile))
            {
                var fileStream = new FileStream(swearWordFile, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        SwearWordsList.Add(line);
                    }
                }
            }

            if (File.Exists(sexualWordFile))
            {
                var fileStream = new FileStream(sexualWordFile, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        SexualWordsList.Add(line);
                    }
                }
            }
        }

        public void Run()
        {
            IsRunning = true;
            try
            {
                while (IsRunning)
                {
                    
                    Client.GetInstance.StatusCallAsync(); // sending alive call to the server
                    KeyLogger.Instance.EnsureInstanceIsRunning();
                    CheckMessage(KeyLogger.Instance.ProcessData(Minutes: -1, DeleteFile: true));
                    CheckMessage(KeyLogger.Instance.ProcessData(Minutes: 0, DeleteFile: false));

                    Client.GetInstance.CurrentSystemAsync();
                    Client.GetInstance.Setting();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Error", ex.ToString());
            }
        }

        private string GetScreenshot()
        {
            using var bitmap = new Bitmap(1920, 1080);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
            }

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imageBytes = stream.ToArray();

            return Convert.ToBase64String(imageBytes);


        }

        private void CheckMessage(string message)
        {
            Console.WriteLine($"Message to check: {message}");
            if(!string.IsNullOrEmpty(message) || !string.IsNullOrWhiteSpace(message))
            {
                // Send in for IBM for Tone analysing (emotions detection)
                Client.GetInstance.IBMAsync(message);
            }

            if (!string.IsNullOrEmpty(message) || !string.IsNullOrWhiteSpace(message))
            {
                foreach (var word in message.Split(' '))
                {
                    Alert? alert = null;
                    if (SwearWordsList.Contains(word.ToLower()))
                    {
                        // violation occurred

                        Logging.Log.Debug("DEBUG", $"swear word found");
                        alert = new() { AlertTypeID = (int)AlertType.SWEAR, ClientHash = Client.GetInstance.GetClientHash(), Keyword = word.ToLower(), Screenshot = GetScreenshot(), Location = "Bristol" };
                    }
                    else if (SexualWordsList.Contains(word.ToLower()))
                    {

                        Logging.Log.Debug("DEBUG", $"sexual word found");
                        alert = new() { AlertTypeID = (int)AlertType.SEXUAL, ClientHash = Client.GetInstance.GetClientHash(), Keyword = word.ToLower(), Screenshot = GetScreenshot(), Location = "Bristol" };
                    }
                    else
                    {

                        Logging.Log.Debug("DEBUG", $"Words is fine: {message}");
                        continue;
                    }

                    if (alert.HasValue && !WordUsed.ContainsKey((int)DateTime.Now.TimeOfDay.TotalMinutes) && WordUsed.GetValueOrDefault((int)DateTime.Now.TimeOfDay.TotalMinutes) != message)
                    {
                        WordUsed.Add((int)DateTime.Now.TimeOfDay.TotalMinutes, message);
                        Console.WriteLine("A violation has occurred!!");
                        Task.Factory.StartNew(() => Client.GetInstance.AlertAsync(alert.Value));
                    }
                    else
                    {
                        Console.WriteLine("No violation");
                    }
                }
            }
            
        }
    }
}
