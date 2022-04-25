using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace dissertation.Models
{
    public class Log
    {
        public static void WriteToLog(string input)
        {
            string filePath = @"C:\Users\nabee\Documents\GitHub\Dissertation\dissertation\Logs\Logs.txt";
            List<string> logs = new List<string>();
            logs.Add(input);
            File.AppendAllLines(filePath, logs);
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            WriteToLog("TEST");
        }
    }
}