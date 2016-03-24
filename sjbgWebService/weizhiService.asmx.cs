using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace sjbgWebService.pub
{
	/// <summary>
	/// baseService 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://sjbg.xxjwd.org/")]

	[System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [SoapRpcService]
	public class weizhiService : System.Web.Services.WebService
	{
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();
		public PreSoapHeader preHeader = new PreSoapHeader();

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GpsData[] getGps(string gps_data)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetGpsByNum(gps_data);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT forGps(string work_no, string work_name, string JingDu, string WeiDu, string WeiZhi, string ShiJian)
        {
            //if (!sjbgHeader.checkValid()) return null;

            return DAL.Gpscs(work_no, work_name, JingDu, WeiDu, WeiZhi, ShiJian);

        }
    }
}
