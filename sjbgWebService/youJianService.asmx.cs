using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace sjbgWebService.youjian
{
	/// <summary>
	/// youjianService 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://sjbg.xxjwd.org/")]

	[ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	// [System.Web.Script.Services.ScriptService]
	[WebServiceBinding(ConformsTo = WsiProfiles.None)]
	[SoapRpcService]
    public class YoujianService : WebService
	{
		public SjbgSoapHeader SjbgHeader = new SjbgSoapHeader();
        public static string StrFilePath = System.Configuration.ConfigurationManager.AppSettings["YouJianFuJian"];

        #region 旧手机办公邮件

        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public WenJianJia SelectMailBox(int uid, string mailBoxName)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.SelectMailBox(uid, mailBoxName);
	    }


	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public YouJianSimple[] GetMailMessageList(int uid, int muids, int muide, string mailBoxName)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.GetMailMessages(uid, muids, muide, mailBoxName);
	    }


	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public YouJian GetMailMessage(int uid, int muid, string mailBoxName)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.GetMailMessage(uid, muid, mailBoxName);

	    }

	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public YouJianFuJian[] GetAttachment(int uid, int muid, string mailBoxName, int pos)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.GetMailAttachment(uid, muid, mailBoxName, pos);

	    }

	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public WenJianJia[] ListMailBoxes(int uid)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.GetMailBoxList(uid);

	    }

	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public INT SendMail(int uid, YouJian mm)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.SendMail(uid, mm);

	    }

	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public INT SendMail1(int uid, int importance, string subject, string body, string from, string to, string cc,
	        string bcc, string attachment)
	    {
	        if (!SjbgHeader.checkValid()) return null;

	        return DAL.SendMail(uid, importance, subject, body, from, to, cc, bcc, attachment);
	    }

	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public INT DeleteMailMessage(int uid, int muid, string mailBoxName)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.DeleteMailMessage(uid, muid, mailBoxName);

	    }

	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public INT MoveMailMessage(int uid, int muid, string oldMailBox, string newMailBox)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.MoveMailMessage(uid, muid, oldMailBox, newMailBox);

	    }


	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public INT DeleteMailBox(int uid, string mailBoxName)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.DeleteMailBox(uid, mailBoxName);
	    }

	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public INT RenameMailBox(int uid, string oldMailBoxName, string newMailBoxName)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.RenameMailBox(uid, oldMailBoxName, newMailBoxName);
	    }


	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public YouJianSimple[] GetMailMessageListTkmp(int uid, int start, int count, bool asc)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.GetMailMessagesTkmp(uid, start, count, asc);
	    }


	    [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
	    [SoapRpcMethod, WebMethod]
	    public YouJian GetMailMessageTkmp(int uid, int muid)
	    {
	        if (!SjbgHeader.checkValid()) return null;
	        return DAL.GetMailMessageTkmp(uid, muid);

	    }

        #endregion


        #region 2016自制邮件系统

        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public YouJian2016 GetMailByMid2016(int uid, int mid)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetMailByMid2016(uid, mid);

        }


        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public YouJianList2016[] GetMailList2016(int uid, int type,int ksxh , int count)
        {
            if (!SjbgHeader.checkValid()) return null;
            return BLL.GetMailList2016(uid, type, ksxh, count);
        }

        [SoapHeader("SjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public YouJianFuJian GetMailFuJian2016(int uid ,int mid ,int pos)
        {

            if (!SjbgHeader.checkValid()) return null;
            return  BLL.GetYouJianFuJian(uid, mid, pos);
            
        }


        #endregion

    }
}
