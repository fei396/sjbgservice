using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using AE.Net.Mail.Imap;
using AE.Net.Mail;

namespace sjbgWebService.youjian
{
	/// <summary>
	/// youjianService 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://sjbg.xxjwd.org/")]

	[System.ComponentModel.ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	// [System.Web.Script.Services.ScriptService]
	[WebServiceBinding(ConformsTo = WsiProfiles.None)]
	[SoapRpcService]
    public class youjianService : System.Web.Services.WebService
	{
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();
		public PreSoapHeader preHeader = new PreSoapHeader();

		
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod ,WebMethod]
		public WenJianJia selectMailBox(int uid,string mailBoxName)
		{
			if (!sjbgHeader.checkValid()) return null;
            return DAL.SelectMailBox(uid, mailBoxName);
		}


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public YouJianSimple[] getMailMessageList(int uid, int muids, int muide, string mailBoxName)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.GetMailMessages(uid, muids, muide, mailBoxName);
        }


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public YouJian getMailMessage(int uid, int muid, string mailBoxName)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.GetMailMessage(uid,muid,mailBoxName);

        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public YouJianFuJian[] getAttachment(int uid, int muid, string mailBoxName,int pos)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.GetMailAttachment(uid, muid, mailBoxName,pos);

        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public WenJianJia[] listMailBoxes(int uid )
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.GetMailBoxList(uid);

        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT sendMail(int uid, YouJian mm)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.SendMail(uid, mm);

        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT sendMail1(int uid, int importance, string subject, string body,string from, string to, string cc, string bcc, string attachment)
        {
            if (!sjbgHeader.checkValid()) return null;

            return DAL.SendMail(uid, importance, subject, body, from ,to, cc, bcc, attachment);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT deleteMailMessage(int uid, int muid ,string mailBoxName)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.DeleteMailMessage(uid, muid, mailBoxName);

        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT moveMailMessage(int uid, int muid, string oldMailBox , string newMailBox)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.MoveMailMessage(uid, muid, oldMailBox, newMailBox);

        }


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT deleteMailBox(int uid, string mailBoxName)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.DeleteMailBox(uid, mailBoxName);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT renameMailBox(int uid, string oldMailBoxName , string newMailBoxName)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.RenameMailBox(uid, oldMailBoxName, newMailBoxName);
        }


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public YouJianSimple[] getMailMessageListTKMP(int uid, int start, int count,bool asc)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.GetMailMessagesTkmp(uid, start, count, asc);
        }


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public YouJian getMailMessageTKMP(int uid, int muid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.GetMailMessageTkmp(uid, muid);

        }
     
	}
}
