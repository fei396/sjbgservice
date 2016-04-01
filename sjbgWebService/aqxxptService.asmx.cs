using System.Web.Services;
using System.Web.Services.Protocols;

namespace sjbgWebService
{
    /// <summary>
    ///安全信息平台
    /// </summary>
    [WebService(Namespace = "http://sjbg.xxjwd.org/")]

    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [SoapRpcService]
    public class aqxxptService : WebService
    {
		public SjbgSoapHeader SjbgHeader = new SjbgSoapHeader();


        /// <summary>
        /// 获取安全信息平台可发送的部门信息
        /// </summary>
        /// <param name="xxid"></param>
        /// <returns></returns>
        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public Department[] GetAqxxptBm(int xxid)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetAqxxptBm(xxid);
        }
        //

        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public User[] GetAqxxptShenHe()
        {

            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetAqxxptShenHe();

        }


        /// <summary>
        /// 提交安全信息申请，由段领导审核
        /// </summary>
        /// <param name="sender">申请提交人</param>
        /// <param name="auditor">审核人</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="buMens">部门列表</param>
        /// <param name="setTime">设置的发送短信时间</param>
        /// <param name="lingDaos">随安全信息发送的领导列表</param>
        /// <returns></returns>
        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT ApplyAqxx(string sender, string auditor, string title, string content, string buMens, string setTime, string lingDaos)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.ApplyAqxx(sender, auditor, title, content, buMens, setTime, lingDaos);
        }


        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AqxxInfo[] GetAqxxInfo(int uid,int xxid)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetAqxxInfo(uid,xxid);
        }

        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public int GetAqxxCount(int uid)
        {
            if (!SjbgHeader.checkValid()) return -1;
            return BLL.GetAqxxCount(uid);
        }


        /// <summary>
        /// 获取已发送的安全信息统计结果
        /// </summary>
        /// <param name="uid">发送人工号，如果是段领导，则所有人发送的都能查询到</param>
        /// <param name="ksxh">开始序号</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AqxxInfo[] GetAqxxInfos(int uid, int ksxh,int count)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetAqxxInfos(uid, ksxh,count);
        }

        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AQXX[] GetAqxxToAudit(int uid,int xxid)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetAqxxToAudit(uid,xxid);
        }

        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AqxxDetail[] GetAqxxDetail(int xxid ,int type,int ksxh,int count)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetAqxxDetail(xxid ,type,ksxh,count);
        }


        /// <summary>
        /// 段领导审核安全信息
        /// </summary>
        /// <param name="xxid">安全信息id</param>
        /// <param name="auditor">审核人</param>
        /// <param name="result">审核结果，0：不通过；1：通过，直接发送；2：通过，由申请人发送</param>
        /// <param name="title">标题</param>
        /// <param name="txt">审批意见</param>
        /// <returns></returns>
        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT AuditAqxx(int xxid, string auditor, int result,string title,string txt)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.AuditAqxx(xxid, auditor, result,title, txt);
        }


        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public AQXX[] GetAqxxContent(int xxid)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetAqxxContent(xxid);
        }

    }


}
