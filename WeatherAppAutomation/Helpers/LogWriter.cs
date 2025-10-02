using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAppAutomation.Helpers
{
    public class LogWriter
    {
        public static void WriteLineToTestLog(string message)
        {
            StreamWriter writer = File.AppendText("C:\\Windows\\Temp\\TestLog.txt");
            writer.WriteLine($"{DateTime.Now}: {message}");
            writer.Close();
        }
    }
}
