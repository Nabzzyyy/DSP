using System;

namespace dissertation.Models
{
    public static class Logging
    {
        public static string LogPath;
        public static string Level;

        // <summary>
        /// Gets the minimum log level of items which should be written to the logs.
        /// </summary>
        /// <returns></returns>
        private static LogLevel GetMinLogLevel()
        {
            LogLevel minLevel;
            if (!Enum.TryParse(LogPath, out minLevel))
            {
                int i;
                if (int.TryParse(Level, out i))
                    minLevel = (LogLevel)i;
                else
                    minLevel = LogLevel.INFO;
            }
            return minLevel;
        }

        public static void Write(string module, string msg, LogLevel level)
        {
            try
            {
                if ((int)level < (int)GetMinLogLevel()) return;
                string LogFileFolder = System.IO.Path.GetFullPath(LogPath);
                System.IO.File.AppendAllText(LogFileFolder + "//" + module + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "--> " + msg + "\n");

                // If the level is ERROR or higher, then also log this to the error log
                if ((int)level >= (int)LogLevel.ERROR)
                    System.IO.File.AppendAllText(LogFileFolder + "//Error_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "--> " + msg + "\n");
            }
            catch
            {
            }
        }

        public static void Debug(string module, string msg)
        {
            Write(module, msg, LogLevel.DEBUG);
        }

        /// <summary>
        /// Writes a string to the log file, but with delayed evaluation of the string to increase performance if the string takes a long time to build and it would not be written because of the log level setting.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="getMessage"></param>
        public static void Debug(string module, Func<string> getMessage)
        {
            if (GetMinLogLevel() <= LogLevel.DEBUG)
                Debug(module, getMessage());
        }

        public static void Info(string module, string msg)
        {
            Write(module, msg, LogLevel.INFO);
        }

        public static void Warn(string module, string msg)
        {
            Write(module, msg, LogLevel.WARN);
        }

        public static void Error(string module, string msg)
        {
            Write(module, msg, LogLevel.ERROR);
        }

    }

    public enum LogLevel
    {
        DEBUG = 0,
        INFO = 10,
        WARN = 20,
        ERROR = 30
    }
}
