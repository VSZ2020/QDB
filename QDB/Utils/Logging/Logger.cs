using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Utils.Logging
{
    public static class Logger
    {
        public const int MAX_LOG_FILE_SIZE_KBYTES = 5012;
        public static string LogFilePath { get; set; }
        public static bool IsEnabled { get; set; }
        public static bool LogToFile { get; set; }

        static Logger()
        {
            IsEnabled = true;
            LogToFile = true;
            LogFilePath = Path.Combine(Environment.CurrentDirectory, "log.txt");
        }
        public static void Log(string Message, string? tag = null)
        {
            if (!IsEnabled)
                return;
            if (tag != null)
                tag = string.Format(" [{0}] ", tag);
            StringBuilder builder = new StringBuilder();
            builder.Append(GetDateTime());
            builder.Append(tag);
            builder.AppendLine(": " + Message);
            Trace.WriteLine(builder.ToString());
            if (LogToFile)
                WriteToFile(builder.ToString());
        }

        public static void Log(Exception exception, string? tag = null)
        {

        }

        public static void WriteToFile(string Message)
        {
            using (StreamWriter wr = new StreamWriter(LogFilePath, true))
            {
                wr.WriteLine(Message);
            }
        }
        private static string GetDateTime()
        {
            return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
        }
        private static void CheckFileOversized()
        {

        }
    }
}
