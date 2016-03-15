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
	public class baseService : System.Web.Services.WebService
	{
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();
		public PreSoapHeader preHeader = new PreSoapHeader();


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [WebMethod]
        public bool setPassword(int uid,string password)
        {
            if (!sjbgHeader.checkValid()) return false;
            return DAL.SetPassword(uid,password);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[WebMethod]
		public bool initPassword()
		{
			if (!sjbgHeader.checkValid()) return false;
			return DAL.InitPassword();
		}
		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public bool setTqyjPassword(int uid, string newPass)
		{
			if (!sjbgHeader.checkValid()) return false;
			return DAL.SetTqjyPassword(uid, newPass);
		}
		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public BOOLEAN changePassword(int uid, string opass,string npass)
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.setNewPass(uid, opass,npass);
		}
		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public User getUserById(string user_no)
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.getUserByNum(user_no);
		}

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public UserRole[] getUserRolesById(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getUserRoleByNum(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public MenuItem[] getUserMenusById(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getUserMenuByNum(uid);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public Product getProductByPName(string pname)
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.getProductByPname(pname);
		}

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public ApkInfo getApkVerCode()
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.getApkInfo();

		}

        [SoapRpcMethod, WebMethod]
        public string getip()
        {
            if (Context.Request.ServerVariables["HTTP_VIA"] != null)
            {
                return Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                return Context.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
        }

		//[SoapHeader("preHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod,WebMethod]
		public INT login(string user_no, string user_pass, string code,string info,string version)
		{
            return BLL.login(user_no, user_pass, code,getip(), info, version);
		}

        [SoapRpcMethod, WebMethod]
        public INT loginDirect(string user_no, string user_pass, string code, string info, string version)
        {
            return BLL.loginDirect(user_no, user_pass, code, getip(), info, version);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public INT registerDevice(int workno,string mobile,string ucode,string rcode,string sq,string sa,string email)
		{
            if (!sjbgHeader.checkValid()) return null;
            RegisterInfo ri = new RegisterInfo(workno, mobile, ucode, rcode, sq, sa, email);
            if (ri.isRegisterValid() == false) return new INT(-999,SjbgConfig.StrInvalidParameter);

			return BLL.registerDevice(ri);
		}


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT SendFeedback(int uid, string txt)
        {
            if (!sjbgHeader.checkValid()) return null;
            if (!txt.isValidString()) return new INT(-1, "文本包含非法字符。");
            return BLL.SendFeedBack(uid, txt);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT setTopicsSubed(string tids)
        {
            if (!sjbgHeader.checkValid()) return null;
            if (!tids.isValidString()) return new INT(-1, "文本包含非法字符。");
            return BLL.setTopicsSubed(tids);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT setMqttStatus(int uid, int type ,string clientId)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.setMqttStaus(uid, type, clientId);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT getMqttStatus(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getMqttStaus(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public LoginInfo[] GetLoginRecord(int uid ,int ksxh ,int count)
        {
            if (!sjbgHeader.checkValid()) return null;

            return BLL.GetLoginRecord(uid, ksxh,count);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public MqttTopic[] getUnsubTopics(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;

            return BLL.getUnsubTopics(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public SystemMessage[] getSystemMessage(int uid ,int type,int ksxh,int count)
        {
            if (!sjbgHeader.checkValid()) return null;

            return BLL.getSystemMessage(uid,type,ksxh,count);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT readSystemMessage(int mid)
        {
            if (!sjbgHeader.checkValid()) return null;

            return BLL.readSystemMessage(mid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT readMqttMessage(int uid,int mid)
        {
            if (!sjbgHeader.checkValid()) return null;

            return BLL.readMqttMessage(uid,mid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public string[] getUnReadMqttMessage(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;

            return BLL.getUnReadMqttMessage(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT insertSystemMessage(int uid, int type)
        {
            if (!sjbgHeader.checkValid()) return null;

            return BLL.insertSystemMessage(uid, type);
        }

        //[SoapHeader("preHeader", Direction = SoapHeaderDirection.In)]
        //[SoapRpcMethod, WebMethod]
        //public INT checkMobile(registerInfo ri)
        //{
        //    if (ri.isRegisterValid() == false) return new INT(-1);
        //    return new INT(BLL.registerDevice(ri));
        //}

		//[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
        public INT requestRegisterCode(int workno, string mobile, string ucode)
		{
            //if (!sjbgHeader.checkValid()) return null;
            RegisterInfo ri = new RegisterInfo(workno, mobile, ucode);
            if (ri.isRegisterValid() == false) return new INT(-999, SjbgConfig.StrInvalidParameter);
            return BLL.requestRegisterCode(ri);
            
		}


     

        [WebMethod]
        public int isIn(int y, int x)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            System.Drawing.Region r = new System.Drawing.Region();
            System.Drawing.Point[] p = new System.Drawing.Point[4];
            p[0] = new System.Drawing.Point(113854964, 35307631);
            p[1] = new System.Drawing.Point(113858591, 35306449);
            p[2] = new System.Drawing.Point(113860329, 35311080);
            p[3] = new System.Drawing.Point(113856499, 35311982);
            gp.AddPolygon(p);
            r.MakeEmpty();
            r.Union(gp);
            if (r.IsVisible(new System.Drawing.Point(x,y))) return 1;
            else return 0;
        }



        [WebMethod]
        public YouJianDiZhi jiexiyoujian(string youjian)
        {
            YouJianDiZhi yjdz = new YouJianDiZhi();
            yjdz.LoadFrom(youjian);
            return yjdz;
        }

	}
}
