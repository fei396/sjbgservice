using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace sjbgWebService
{
    /// <summary>
    /// yunyongService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://sjbg.xxjwd.org/")]

    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [SoapRpcService]
    public class jianxiuService : System.Web.Services.WebService
    {
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();

        
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public FeiYunYongJiChe[] getFyyjc(string jczt)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetFyyjc(jczt);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public string[] getFyyjcZt()
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetFyyjcZt();
        }
        
    }
}
