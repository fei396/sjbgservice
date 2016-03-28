using System;
using System.IO;

namespace aqxxptSMSservice
{


	internal class Log
	{
        static readonly string StrLogFile = System.Configuration.ConfigurationManager.AppSettings["LogFile"];

        public static void WriteLog(string strContent)
        {
            FileInfo f = new FileInfo(StrLogFile);

            StreamWriter sw;
            if (f.Exists)
            {
                sw = f.AppendText();
            }
            else
            {
                if (f.Directory != null && !f.Directory.Exists) Directory.CreateDirectory(f.Directory.FullName);
                sw = f.CreateText();
            }
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "--" + strContent);
            sw.Close();
        }
	}
}
