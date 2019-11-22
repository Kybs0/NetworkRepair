using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkRepair.Business
{
    public static class LogHelper
    {
        public static void Log(string error, [CallerMemberName] string propertyName = null)
        {
            var logErrorPath = GetLogErrorPath();
            File.AppendAllText(logErrorPath, $"{DateTime.Now.ToString("u")},Method:{propertyName}\r\n{error}\r\n");
        }
        private static string GetLogErrorPath()
        {
            string currentDirectory = Path.Combine(@"C:\Users\" + Environment.UserName + @"\AppData\Roaming\NetworkRepair");
            var logFolder = Path.Combine(currentDirectory, "Log");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }
            var iniFilePath = Path.Combine(logFolder, "Error.txt");
            if (!File.Exists(iniFilePath))
            {
                var fileStream = File.Create(iniFilePath);
                fileStream.Close();
            }
            return iniFilePath;
        }
    }
}
