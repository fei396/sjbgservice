using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace sjbgWebService
{
    /// <summary>
    /// tqyjService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://sjbg.xxjwd.org/")]

    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [SoapRpcService]
    public class tqyjService : System.Web.Services.WebService
    {
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();


		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public TeQing[] getTqByWorkno(int workno,int ksxh,int count)
        {
			if (!sjbgHeader.checkValid()) return null;
            return BLL.GetTeQingByWorkNo(workno,ksxh,count);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BOOLEAN replyTQ(int workno,int tid,string replayContent)
        {
			if (!sjbgHeader.checkValid()) return null;
            return  BLL.ReplyTq(workno,tid, replayContent);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public TeQing[] checkReply(int senderno,int ksxh,int count)
        {
			if (!sjbgHeader.checkValid()) return null;
            return BLL.CheckReply(senderno,ksxh,count);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public TeQing[] checkReplyDetails(int tid,int ksxh,int count)
        {
			if (!sjbgHeader.checkValid()) return null;
            return BLL.CheckReplyDetails(tid,ksxh,count);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT getTqLevel(int workno)
        {
			if (!sjbgHeader.checkValid()) return new INT( -999 ,SjbgConfig.StrInvlaidRequest);
            return BLL.GetTqLevel(workno);
        }

    }
}
