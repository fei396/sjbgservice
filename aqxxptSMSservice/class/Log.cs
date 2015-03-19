using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace aqxxptSMSservice
{


	class Log
	{
        static string strLogFile = System.Configuration.ConfigurationManager.AppSettings["LogFile"];

        public static void writeLog(string strContent)
        {
            FileInfo f = new FileInfo(strLogFile);

            StreamWriter sw;
            if (f.Exists)
            {
                sw = f.AppendText();
            }
            else
            {
                if (!f.Directory.Exists) System.IO.Directory.CreateDirectory(f.Directory.FullName);
                sw = f.CreateText();
            }
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "--" + strContent);
            sw.Close();
        }
	}
}
