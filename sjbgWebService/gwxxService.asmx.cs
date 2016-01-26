using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using System.Web.Services.Protocols;
using System.Web.Services.Description;
using sjbgWebService.pub;
using System.Data;

namespace sjbgWebService.gwxx
{
	/// <summary>
	/// WebService1 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://sjbg.xxjwd.org/")]

	[System.ComponentModel.ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	// [System.Web.Script.Services.ScriptService]
	[WebServiceBinding(ConformsTo=WsiProfiles.None)]
	[SoapRpcService]
	public class gwxxWebService : System.Web.Services.WebService
	{
		public static string strFilePath = System.Configuration.ConfigurationManager.AppSettings["FuJian"];
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod,WebMethod]
        public UserGw[] getLeaderList()
        {
			if (!sjbgHeader.checkValid()) return null;
				
			else return BLL.getLeaderList();
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BuMenGw[] getDeptListById(int lbid)
        {
			if (!sjbgHeader.checkValid()) return null;
            return BLL.getBmList(lbid);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BuMenLeiBie[] getDeptTypeList()
        {
			if (!sjbgHeader.checkValid()) return null;
            return BLL.getBmlbList();
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public GongWen getGwxxByWh(string wh)
		{
			if (!sjbgHeader.checkValid()) return null;
			GongWen gw = BLL.getGongWenByWh(wh);
			//string[] strs = b.makeGwString(gw).Split(new string[] { "^^" }, StringSplitOptions.None);
			//return b.makeGwString(gw);
			return gw;
		}

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BOOLEAN isSigned(string wh, int gh)
        {
			if (!sjbgHeader.checkValid()) return null;
			
			return  BLL.isSigned(wh, gh);

        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public BOOLEAN signGw(string wh,int uid,string ins,string nextUsers)
		{
			if (!sjbgHeader.checkValid()) return null;
			return  BLL.signGw(wh, uid, ins, nextUsers);
		}

		/// <summary>
		/// 获取符合条件的公文列表
		/// </summary>
		/// <param name="lblx">列表类型，1：已批阅列表，0：未批阅列表</param>
		/// <param name="gwlx">公文类型，1：行政，0：党群</param>
		/// <param name="dwlx">发文单位类型，1：局文，0：段文</param>
		/// <param name="ksxh">列表开始序号，按时间倒叙</param>
		/// <param name="count">列表公文数量</param>
		/// <returns>公文列表</returns>
		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public GongWen[] getGwlb(int gh,int lblx, int gwlx, int dwlx, int ksxh, int count)
		{
			if (!sjbgHeader.checkValid()) return null;
			gwlx gw;
			dwlx dw;
            if (gwlx == 1) gw = gwxx.gwlx.XZ;
            else if (gwlx == 0) gw = gwxx.gwlx.DQ;
            else gw = gwxx.gwlx.ALL;
            if (dwlx == 1) dw = gwxx.dwlx.LJ;
            else if (dwlx == 0) dw = gwxx.dwlx.DW;
            else dw = gwxx.dwlx.ALL;
			return BLL.getGwlb(gh, lblx, gw, dw, ksxh, count);
		}
		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public Instruction[] getInstructions(string wh)
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.getLdps(wh);
		}


		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public string getAttachFile(string filePath)
		{
			if (!sjbgHeader.checkValid()) return null;
			string fileName = filePath.Substring(filePath.LastIndexOf('/') + 1, filePath.Length - filePath.LastIndexOf('/') - 1);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(filePath);
			WebResponse response = request.GetResponse();
			//Stream stream = response.GetResponseStream();
            //if (!response.ContentType.ToLower().StartsWith("text/"))
            {
                byte[] buffer = new byte[1024];
                Stream outStream = System.IO.File.Create(strFilePath + fileName);
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

			FileStream fs = new FileStream(strFilePath + fileName, FileMode.Open, FileAccess.Read);

			byte[] b = new byte[(int)fs.Length];
			int k = fs.Read(b, 0, (int)fs.Length);
			fs.Close();
			string base64 = Convert.ToBase64String(b);
			return base64;
		}

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public INT getGwLevel(int uid)
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.getGwLevel(uid);
		}

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public BOOLEAN signGwMiddle(string wh, int uid)
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.signGw(wh,uid,null,null);
        }


        #region 2016新版公文
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT addNewGongWen2016(int uid ,string ht,string wh,string bt,string zw,int xzid,int lxid,string ip,string jsr,string[] gwfj )
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.addNewGongWen2016(uid, ht, wh, bt, zw, xzid, lxid, ip, jsr, gwfj);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT signGongWen2016(int gwid, int lzid, string fsr, string[] jsr, string qsnr)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.signGongWen2016(gwid ,lzid,fsr ,jsr,qsnr);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenXingZhi[] getXingZhi()
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getGongWenXingZhi();
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenLeiXing[] getLeiXing()
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getGongWenLeiXing();
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenYongHu[] getLingDao()
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getGongWenYongHu(new int[]{21,22});
        }
        #endregion
    }
}
