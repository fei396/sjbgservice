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
    /// 公文服务
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
				
			else return BLL.GetLeaderList();
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BuMenGw[] getDeptListById(int lbid)
        {
			if (!sjbgHeader.checkValid()) return null;
            return BLL.GetBmList(lbid);
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BuMenLeiBie[] getDeptTypeList()
        {
			if (!sjbgHeader.checkValid()) return null;
            return BLL.GetBmlbList();
        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public GongWen getGwxxByWh(string wh)
		{
			if (!sjbgHeader.checkValid()) return null;
			GongWen gw = BLL.GetGongWenByWh(wh);
			//string[] strs = b.makeGwString(gw).Split(new string[] { "^^" }, StringSplitOptions.None);
			//return b.makeGwString(gw);
			return gw;
		}

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BOOLEAN isSigned(string wh, int gh)
        {
			if (!sjbgHeader.checkValid()) return null;
			
			return  BLL.IsSigned(wh, gh);

        }

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public BOOLEAN signGw(string wh,int uid,string ins,string nextUsers)
		{
			if (!sjbgHeader.checkValid()) return null;
			return  BLL.SignGw(wh, uid, ins, nextUsers);
		}

	    /// <summary>
	    /// 获取符合条件的公文列表
	    /// </summary>
	    /// <param name="gh">工号</param>
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
			return BLL.GetGwlb(gh, lblx, gw, dw, ksxh, count);
		}
		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public Instruction[] getInstructions(string wh)
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.GetLdps(wh);
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
			return BLL.GetGwLevel(uid);
		}

		[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
		[SoapRpcMethod, WebMethod]
		public BOOLEAN signGwMiddle(string wh, int uid)
		{
			if (!sjbgHeader.checkValid()) return null;
			return BLL.SignGw(wh,uid,null,null);
        }


        #region 2016新版公文

	    /// <summary>
	    /// 发布新公文
	    /// </summary>
	    /// <param name="uid">发布人工号</param>
	    /// <param name="ht">文件红头</param>
	    /// <param name="dw">发文单位</param>
	    /// <param name="wh">文号</param>
	    /// <param name="bt">文件标题</param>
	    /// <param name="zw">正文</param>
	    /// <param name="yj">呈送意见</param>
	    /// <param name="xzid">公文性质,1行政，2党群</param>
	    /// <param name="lxid">公文类型，1路局，2段发</param>
	    /// <param name="jinji">公文紧急程度</param>
	    /// <param name="ip">发布人IP</param>
	    /// <param name="jsr">接收人工号列表</param>
	    /// <param name="gwfj">附件列表</param>
	    /// <returns></returns>
	    [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT addNewGongWen2016(int uid ,string ht,string dw,string wh,string bt,string zw,string yj,int xzid,int lxid,string jinji,string ip,string[] jsr,string[] gwfj )
        {
            if (dw == null) throw new ArgumentNullException(nameof(dw));
            //判断传过来的header是否合法
            if (!sjbgHeader.checkValid()) return new INT(-1, "非法接入程序");
            //直接调用业务逻辑层函数进行添加操作
            return BLL.AddNewGongWen2016(uid, ht, dw, wh, bt, zw, yj, xzid, lxid, jinji, ip, jsr, gwfj);
        }


        /// <summary>
        /// 补签公文，公文处理员对领导遗漏的人员进行补充添加
        /// </summary>
        /// <param name="gwid">公文ID</param>
        /// <param name="lzid">流转ID</param>
        /// <param name="fsr">发送人，指领导</param>
        /// <param name="buid">补签人工号，指进行补签操作的公文处理员</param>
        /// <param name="jsr">接收人，补充添加的人员工号数组</param>
        /// <returns>成功标识</returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT buGongWen2016(int gwid, int lzid, int fsr, int buid, string[] jsr)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.BuGongWen2016(gwid, lzid, fsr,buid, jsr);
        }

        /// <summary>
        /// 签收公文
        /// </summary>
        /// <param name="gwid">公文ID</param>
        /// <param name="lzid">流转ID</param>
        /// <param name="fsr">签收人工号</param>
        /// <param name="jsr">接收人工号数组</param>
        /// <param name="qsnr">签阅内容</param>
        /// <param name="zdybm">自定义部门（已取消，无效）</param>
        /// <param name="ip">ip地址</param>
        /// <returns></returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT signGongWen2016(int gwid, int lzid, int fsr, string[] jsr, string qsnr,int []zdybm,string ip)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.SignGongWen2016(gwid ,lzid,fsr ,jsr,qsnr,zdybm,"网页",ip);
        }

        /// <summary>
        /// 手机端签收公文
        /// </summary>
        /// <param name="gwid">公文ID</param>
        /// <param name="lzid">流转ID</param>
        /// <param name="fsr">签收人工号</param>
        /// <param name="jsr">接收人工号字符串</param>
        /// <param name="qsnr">签阅内容</param>
        /// <returns></returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT signGongWen2016Mobile(int gwid, int lzid, int fsr, string jsr, string qsnr)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.SignGongWen2016Mobile(gwid, lzid, fsr, jsr, qsnr);
        }

        /// <summary>
        /// 撤销签约公文
        /// </summary>
        /// <param name="uid">撤销人工号</param>
        /// <param name="lzid">流转ID</param>
        /// <returns></returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT undoGongWen2016(int uid,int lzid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.UndoSignGongWen2016(uid,lzid);
        }

        
        /// <summary>
        /// 获取公文性质列表
        /// </summary>
        /// <returns>公文性质列表</returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenXingZhi[] getXingZhi()
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetGongWenXingZhi();
        }

        /// <summary>
        /// 获取公文类型列表
        /// </summary>
        /// <returns>公文类型列表</returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenLeiXing[] getLeiXing()
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetGongWenLeiXing();
        }

        /// <summary>
        /// 获取领导信息
        /// </summary>
        /// <param name="roleid">要获取的领导的角色id列表</param>
        /// <returns></returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenYongHu[] getLingDao(int[] roleid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetGongWenYongHu(roleid);
        }


        /// <summary>
        /// 通过工号获取公文用户信息
        /// </summary>
        /// <param name="uid">工号</param>
        /// <returns>公文用户信息</returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenYongHu getGongWenYongHuByUid(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetGongWenYongHuByUid(uid);
        }

        /// <summary>
        /// 获取公文列表
        /// </summary>
        /// <param name="uid">接收人工号</param>
        /// <param name="fsr">发送人工号（暂时无用）</param>
        /// <param name="keyWord">关键字（文号或标题）</param>
        /// <param name="sTime">开始时间</param>
        /// <param name="eTime">截至时间</param>
        /// <param name="gwtype">文件类型，1：所有公文，0：未签公文</param>
        /// <param name="lxid">公文类型1:路局文，2:段发文</param>
        /// <param name="ksxh">开始序号</param>
        /// <param name="count">数量</param>
        /// <returns>公文列表</returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenList[] getGongWenList(int uid,string fsr ,string keyWord,string sTime,string eTime,int gwtype,int lxid,int ksxh,int count)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetGongWenList(uid , fsr, -1, lxid, keyWord, sTime, eTime, gwtype ,ksxh, count);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenList[] getDuanWenList(int uid,string keyWord, string sTime, string eTime, int ksxh, int count)

        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetDuanWenList(uid,keyWord, sTime, eTime,ksxh, count);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public int getDuanWenCount(int uid, string keyWord, string sTime, string eTime)

        {
            if (!sjbgHeader.checkValid()) return -1;
            return BLL.GetDuanWenCount(uid, keyWord, sTime, eTime);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public int getGongWenCount(int uid, string fsr, string keyWord, string sTime, string eTime, int gwtype,int lxid)
        {
            if (!sjbgHeader.checkValid()) return -1;
            return BLL.GetGongWenCount(uid, fsr, -1, lxid, keyWord, sTime, eTime, gwtype );
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWen2016 getGongWen2016ByID(int gwid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetGongWen2016ById(gwid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenLiuZhuan[] getLiuZhuanXian(bool sfbr,int lzlvl, int lzid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetLiuZhuanXianByLzId(sfbr,lzlvl ,lzid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenLiuZhuan[] getLingDaoPiShi(int uid,int gwid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetLingDaoPiShi(uid, gwid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenLiuZhuan[] getSuoYouWeiQian(int uid, int gwid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetSuoYouWeiQian(uid, gwid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenZiDingYiDuanYu[] getZiDingYiDuanYu(int uid,bool onlyPrivate)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetZiDingYiDuanYu(uid ,onlyPrivate);
        }
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT addDuanYu(int uid, string dynr)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.AddDuanYu(uid, dynr);
        }
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT deleteDuanYu(int id)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.DeleteDuanYu(id);
        }
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT updateDuanYu(int id, string newTxt)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.UpdateDuanYu(id, newTxt);
        }
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT addZdybm(int uid, string dynr)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.AddZdybm(uid, dynr);
        }
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT deleteZdybm(int id)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.DeleteZdybm(id);
        }
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT updateZdybm(int id, string newTxt)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.UpdateZdybm(id, newTxt);
        }
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenZiDingYiBuMen[] getZiDingYiBuMen(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetZiDingYiBuMen(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BuMenFenLei[] getBuMenFenLei(int uid,int rid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetBuMenFenLei(uid,rid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenGuiDangList[] getGongWenGuiDangList(int uid,int type,string keyWord,string sTime,string eTime,int ksxh,int count)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetGongWenGuiDangList(uid, type, keyWord,sTime,eTime, ksxh, count);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public int getGongWenGuiDangCount(int uid, int type, string keyWord, string sTime, string eTime)
        {
            if (!sjbgHeader.checkValid()) return -1;
            return BLL.GetGongWenGuiDangCount(uid, type, keyWord, sTime, eTime);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT makeCuiBan(int gwid,int rid)
        {
            if (!sjbgHeader.checkValid()) return new INT(-1);
            return BLL.makeCuiBan(gwid,rid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT makeCuiBanByRenYuan(int gwid, string[] jsr)
        {
            if (!sjbgHeader.checkValid()) return new INT(-1);
            return BLL.makeCuiBan(gwid, jsr);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenBuMenRenYuan[] getZiDingYiBuMenRenYuan(int zdybmid,bool added)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetZiDingYiBuMenRenYuan(zdybmid, added);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT setZiDingYiBuMenRenYuan(int zdybmid, string[] user_no)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.SetZiDingYiBuMenRenYuan(zdybmid, user_no);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenBuMenRenYuan[] getBuMenRenYuan(int bmid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetBuMenRenYuan(bmid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT addGongWenRenYuan(int uid,string gh , int rid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.AddGongWenRenYuan(uid,gh, rid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT deleteGongWenRenYuan(int uid, string gh, int rid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.DeleteGongWenRenYuan(uid, gh, rid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public GongWenYongHu getYongHuXinXiByGh(int uid,string gh)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetYongHuXinXiByGh(uid,gh);
        }


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public string getFuJianNeiRong(string fileName)
        {
            if (!sjbgHeader.checkValid()) return null;
            try
            {
                string fileFullName = SjbgConfig.FuJianPath + fileName;
                FileStream fs = new FileStream(fileFullName, FileMode.Open, FileAccess.Read);

                byte[] b = new byte[(int)fs.Length];
                int k = fs.Read(b, 0, (int)fs.Length);
                fs.Close();
                string base64 = Convert.ToBase64String(b);
                return base64;
            }
            catch
            {
                return null;
            }
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT deleteGongWen2016(int uid ,int gwid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.DeleteGongWen2016(uid, gwid);
        }

        #endregion

        #region 段内通知

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public TongZhiLeiXing[] getTongZhiLeiXing(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetTongZhiLeiXing(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BuMenFenLei[] getTongZhiBuMenFenLei(int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetTongZhiBuMenFenLei(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT addNewTongZhi2016(string bt, string zw, int fbrid, int lxid, int[] jsrid, string[] files, string ip,int sfgk)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.AddNewTongZhi2016(bt, zw, fbrid, lxid, jsrid, files, ip, sfgk);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT getAllTongZhiLeiXing()
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getAllTongZhiLeiXing();
        }
        #endregion
    }
}