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
    public class aqxxptService : System.Web.Services.WebService
    {
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();

        //测酒查询处理数据
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public Department[] getAqxxptBm(int xxid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getAqxxptBm(xxid);
        }
        //

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public User[] getAqxxptShenHe()
        {

            if (!sjbgHeader.checkValid()) return null;
            return BLL.getAqxxptShenHe();

        }


        
        /// <summary>
        /// 提交安全信息申请，由段领导审核
        /// </summary>
        /// <param name="sender">申请提交人</param>
        /// <param name="auditor">审核人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="buMens">部门列表</param>
        /// <returns></returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT applyAqxx(string sender, string auditor, string title, string content, string buMens,string setTime,string lingDaos)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.ApplyAqxx(sender, auditor, title, content, buMens,setTime,lingDaos);
        }


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AqxxInfo[] getAqxxInfo(int uid,int xxid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getAqxxInfo(uid,xxid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public int getAqxxCount(int uid)
        {
            if (!sjbgHeader.checkValid()) return -1;
            return BLL.getAqxxCount(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AqxxInfo[] getAqxxInfos(int uid, int ksxh,int count)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getAqxxInfos(uid, ksxh,count);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AQXX[] getAqxxToAudit(int uid,int xxid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getAqxxToAudit(uid,xxid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AqxxDetail[] getAqxxDetail(int xxid ,int ksxh,int count)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getAqxxDetail(xxid ,ksxh,count);
        }


        /// <summary>
        /// 段领导审核安全信息
        /// </summary>
        /// <param name="xxid">安全信息id</param>
        /// <param name="auditor">审核人</param>
        /// <param name="result">审核结果，0：不通过；1：通过，直接发送；2：通过，由申请人发送</param>
        /// <param name="txt">审批意见</param>
        /// <returns></returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT auditAqxx(int xxid, string auditor, int result,string title,string txt)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.AuditAqxx(xxid, auditor, result,title, txt);
        }


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AQXX[] getAqxxContent(int xxid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getAqxxContent(xxid);
        }

    }


}
