using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Net;
using System.IO;

namespace sjbgWebService
{
    /// <summary>
    /// scjbService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://sjbg.xxjwd.org/")]

    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [SoapRpcService]
    public class scjbService : System.Web.Services.WebService
    {
        public static string strScjbFilePath = System.Configuration.ConfigurationManager.AppSettings["ScjbFilePath"];
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public String[] getJianBaoBuMen()
        {
			if (!sjbgHeader.checkValid()) return null;
            return BLL.GetJianBaoBuMen();
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public JianBao[] getAllJianBaoByDate(string date)
        {
			if (!sjbgHeader.checkValid()) return null;
            try
            {
                Convert.ToDateTime(date);
            }
            catch
            {
                return null;
            }
            return BLL.GetAllJianBao(Convert.ToDateTime(date));
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public JianBao[] getJianBaoByDeptDate(string dept,string date)
        {
			if (!sjbgHeader.checkValid()) return null;
            try
            {
                Convert.ToDateTime(date);
            }
            catch
            {
                return null;
            }
            return BLL.GetJianBao(dept,Convert.ToDateTime(date));
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public JianBao[] getJianBaoByDeptDate1(JianBao j)
        {
            if (!sjbgHeader.checkValid()) return null;
           
            return BLL.GetJianBao(j.BuMen,Convert.ToDateTime(j.Date));
        }


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public string getScjbExcel(string jbrq)
        {
            string filePath = strScjbFilePath;
            string fileName = Convert.ToDateTime(jbrq).ToString("yyyy-M-d") + ".xls";
            string fileFullPath = filePath + fileName;
            if (!sjbgHeader.checkValid()) return null;
            //string fileName = filePath.Substring(filePath.LastIndexOf('/') + 1, filePath.Length - filePath.LastIndexOf('/') - 1);
            HttpWebRequest request; 
            WebResponse response;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(fileFullPath);
                response = request.GetResponse();
            }
            catch 
            {
                fileName = Convert.ToDateTime(jbrq).ToString("yyyy-MM-dd") + ".xls";
                fileFullPath = filePath + fileName;
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(fileFullPath);
                    response = request.GetResponse();
                }
                catch
                {
                    return"NotFound";
                }
            }
            
           
            //Stream stream = response.GetResponseStream();
            //if (!response.ContentType.ToLower().StartsWith("text/"))
            {
                byte[] buffer = new byte[1024];
                Stream outStream = System.IO.File.Create(gwxx.gwxxWebService.strFilePath + fileName);
                Stream inStream = response.GetResponseStream();
                
                int i;
                do
                {
                    i = inStream.Read(buffer, 0, buffer.Length);
                    if (i > 0)
                    {
                        outStream.Write(buffer, 0, i);
                    }
                }
                while (i > 0);


                outStream.Close();
                inStream.Close();

            }
            
            FileStream fs = new FileStream(gwxx.gwxxWebService.strFilePath + fileName, FileMode.Open, FileAccess.Read);

            byte[] b = new byte[(int)fs.Length];
            int k = fs.Read(b, 0, (int)fs.Length);
            fs.Close();
            string base64 = Convert.ToBase64String(b);
            return base64;
        }
    }
}
