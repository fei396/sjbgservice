using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using sjbgWebService.gwxx;
using sjbgWebService.pub;
using sjbgWebService.xwxx;
using System.IO;
using System.Web.Script.Serialization;

namespace sjbgWebService
{
    public static class BLL
    {

        public static MqttMessageType ToMqttMessageType(this int type)
        {
            switch(type)
            {
                case 1:
                    return MqttMessageType.CHAT_MESSAGE;
                case 2:
                    return MqttMessageType.REMIND_MESSAGE;
                case 3:
                    return MqttMessageType.ALARM_MESSAGE;
                default:
                    return MqttMessageType.CHAT_MESSAGE;
            }
        }

        public static string ToJsonString(this MqttMessage mm)
        {
            int types = mm.Type;
            
            JavaScriptSerializer j = new JavaScriptSerializer();
            string str = j.Serialize(mm);
            return str;
        }

        public static string ToMD5String(this string s)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToUpper();
        }

        public static string ToMD5String(this int i)
        {
            string s = i.ToString();
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToUpper();
        }
        private static bool isValidUserNo(this string str)
        {
            return true;
        }
        private static bool isValidPass(this string str)
        {
            return true;
        }
		public static bool isValidEmail(this string str)
		{
			return true;
		}
		public static bool isValidMobile(this string str)
		{
			return true;
		}
		public static bool isValidString(this string str)
		{
			return true;
		}
        public static string setEncryptPass(string gh, string pass)
        {
            return ToMD5String(gh + pass);
        }

        public static string toWorkNo(this int uid)
        {
            int length = 4;
            if (uid <= 0 || uid >= Math.Pow(10, length)) throw new ArgumentOutOfRangeException("uid", "工号只能在0001-9999中间") ;
            return uid.ToString().PadLeft(length, '0');
        }


        public static List<System.Net.Mail.MailAddress> ToSysMailAddress(string str)
        {
            if (str.Contains(SjbgConfig.FuHaoKaiShi) == false) return null;
            string[] strYjdzs = str.Split(new string[]{SjbgConfig.FuHaoYouJianDiZhi} , StringSplitOptions.RemoveEmptyEntries);
            List<System.Net.Mail.MailAddress> ma = new List<System.Net.Mail.MailAddress>();
            for (int i = 0; i < strYjdzs.Length; i++)
            {
                string address = strYjdzs[i].Substring(strYjdzs[i].IndexOf(SjbgConfig.FuHaoKaiShi) + SjbgConfig.FuHaoKaiShi.Length, strYjdzs[i].IndexOf(SjbgConfig.FuHaoFenGe) - strYjdzs[i].IndexOf(SjbgConfig.FuHaoKaiShi) - SjbgConfig.FuHaoKaiShi.Length);
                string displayname = strYjdzs[i].Substring( strYjdzs[i].IndexOf(SjbgConfig.FuHaoFenGe) + SjbgConfig.FuHaoFenGe.Length , strYjdzs[i].Length - SjbgConfig.FuHaoFenGe.Length - strYjdzs[i].IndexOf(SjbgConfig.FuHaoFenGe));
                ma.Add(new System.Net.Mail.MailAddress(address, displayname));
            }
            return ma;

        }

        public static List<System.Net.Mail.Attachment> ToSysAttachment(string strYjfj)
        {
            if (strYjfj.IndexOf(SjbgConfig.FuHaoYouJianDiZhi) <= 0) return null;
            string[] strYjfjs = strYjfj.Split(new string[] { SjbgConfig.FuHaoYouJianDiZhi }, StringSplitOptions.None);
            YouJianFuJian[] yjfj = new YouJianFuJian[strYjfjs.Length];
            for (int i = 0; i < strYjfjs.Length; i++)
            {
                yjfj[i] = new YouJianFuJian();
                yjfj[i].LoadFrom(strYjfjs[i]);
            }
            List<System.Net.Mail.Attachment> atts = new List<System.Net.Mail.Attachment>();
            foreach (YouJianFuJian fj in yjfj)
            {
                if (fj.FileName != string.Empty)
                {
                    atts.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(System.Convert.FromBase64String(fj.Base64Code)), fj.FileName));
                }
            }
            return atts;
        }

        public static GongWen getGongWenByWh(string wh)
        {

            GongWen gw = new GongWen();
            DataTable dt = DAL.getGwxxByWh(wh);
            if (dt.TableName.Equals("wh"))
            {
                gw.Id = Convert.ToInt32(dt.Rows[0]["id"]);
                gw.RedTitle = Convert.ToString(dt.Rows[0]["ht"]);
                gw.Number = Convert.ToString(dt.Rows[0]["wh"]);
                gw.Title = Convert.ToString(dt.Rows[0]["bt"]);
                gw.Content = Convert.ToString(dt.Rows[0]["zw"]);
                gw.SendDept = Convert.ToString(dt.Rows[0]["fwdw"]);
                gw.SendDate = Convert.ToString(dt.Rows[0]["fwrq"]);
                gw.Suggestion = Convert.ToString(dt.Rows[0]["csyj"]);
                gw.FileType = Convert.ToString(dt.Rows[0]["wjxz"]);
                gw.SendType = Convert.ToString(dt.Rows[0]["lwlx"]);
                gw.AttachPath01 = Convert.ToString(dt.Rows[0]["fj1"]);
                gw.AttachPath02 = Convert.ToString(dt.Rows[0]["fj2"]);
                gw.AttachPath03 = Convert.ToString(dt.Rows[0]["fj3"]);
                gw.AttachPath04 = Convert.ToString(dt.Rows[0]["fj4"]);
                gw.AttachPath05 = Convert.ToString(dt.Rows[0]["fj5"]);
                gw.AttachPath06 = Convert.ToString(dt.Rows[0]["fj6"]);
            }
            return gw;

        }

        public static UserGw getUserGwByUid(int uid)
        {
            DataTable dt = DAL.getUserGwByUid(uid);
            UserGw u = new UserGw();
            if (dt.TableName.Equals("usergw"))
            {
                u.Yhbh = Convert.ToInt32(dt.Rows[0]["yhbh"]);
                u.Yhmc = Convert.ToString(dt.Rows[0]["yhmc"]);
                u.Yhsm = Convert.ToString(dt.Rows[0]["yhsm"]);
                u.Ssbm = Convert.ToInt32(dt.Rows[0]["ssbm"]);
                u.Yhnc = Convert.ToString(dt.Rows[0]["yhnc"]);
                u.Yhqx = Convert.ToInt32(dt.Rows[0]["yhqx"]);
                u.ShuangQian = Convert.ToInt32(dt.Rows[0]["shuangqian"]);
                //u.Wjxz = Convert.ToInt32(dt.Rows[0]["wjxz"]);
                u.Ssxzbm = DAL.getSsxzbmBySsbm(u.Ssbm);

            }
            return u;
        }
        public static UserGw getUserGwByUserName(string userName)
        {

            DataTable dt = DAL.getUserGwByUserName(userName);
            UserGw u = new UserGw();
            if (dt.TableName.Equals("usergw"))
            {
                u.Yhbh = Convert.ToInt32(dt.Rows[0]["yhbh"]);
                u.Yhmc = Convert.ToString(dt.Rows[0]["yhmc"]);
                u.Yhsm = Convert.ToString(dt.Rows[0]["yhsm"]);
                u.Ssbm = Convert.ToInt32(dt.Rows[0]["ssbm"]);
                u.Yhnc = Convert.ToString(dt.Rows[0]["yhnc"]);
                u.Yhqx = Convert.ToInt32(dt.Rows[0]["yhqx"]);
                u.ShuangQian = Convert.ToInt32(dt.Rows[0]["shuangqian"]);
                //u.Wjxz = Convert.ToInt32(dt.Rows[0]["wjxz"]);
                u.Ssxzbm = DAL.getSsxzbmBySsbm(u.Ssbm);
            }
            return u;
        }

        public static BOOLEAN signGw(string wh, int gh, string insStr, string nextUids)
        {
            int uid;
            try
            {
                uid = Convert.ToInt32(DAL.getProductUserIdByBaseNum(gh.ToString().PadLeft(4, '0'), 2));
            }
            catch (Exception ex)
            {
                return new BOOLEAN(false, ex.Message);
            }
            GongWen gw = getGongWenByWh(wh);
            if (gw.Id == 0) return new BOOLEAN(false,"");
            UserGw user = getUserGwByUid(uid);
            if (user.Yhmc.Equals("")) return new BOOLEAN(false,"");
            if (user.Yhqx == 6) //领导权限
            {
                Instruction ins = new Instruction();
                ins.Content = insStr;
                string[] nextUid = nextUids.Split(new string[] { SjbgConfig.FuHaoLingDaoFenGe }, StringSplitOptions.RemoveEmptyEntries);

                UserGw[] nextUser = new UserGw[nextUid.Length];
                int j = 0;
                foreach (string uidStr in nextUid)
                {
                    nextUser[j] = new UserGw();
                    nextUser[j++].Yhbh = Convert.ToInt32(uidStr);
                }
                return DAL.leaderSign(gw, user, ins, nextUser);
            }
            else//中层签收
            {
                return DAL.sign(gw, user);
            }
           
        }

        public static BOOLEAN isSigned(string wh, int gh)
        {
            int uid;
            try
            {
                uid = Convert.ToInt32(DAL.getProductUserIdByBaseNum(gh.ToString().PadLeft(4, '0'), 2));
            }
            catch
            {
                return new BOOLEAN(true,"") ;
            }
            GongWen gw = getGongWenByWh(wh);
            UserGw user = getUserGwByUid(uid);
            return DAL.isSigned(gw, user);
        }

        public static Instruction[] getLdps(string wh)
        {
            GongWen gw = getGongWenByWh(wh);
            DataTable dt = DAL.getLdps(gw);

            Instruction[] instruction = new Instruction[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                instruction[i] = new Instruction();
                instruction[i].ID = Convert.ToInt32(dt.Rows[i]["id"]);
                instruction[i].Title = Convert.ToString(dt.Rows[i]["wh"]);
                instruction[i].Name = Convert.ToString(dt.Rows[i]["psr"]);
                instruction[i].Content = Convert.ToString(dt.Rows[i]["psnr"]);
                instruction[i].Date = Convert.ToString(dt.Rows[i]["psrq"]);
            }
            return instruction;
        }



        public static GongWen[] getGwlb(int gh, int lblx, gwlx gwlx, dwlx dwlx, int ksxh, int count)
        {
            int uid;
            try
            {
                uid = Convert.ToInt32(DAL.getProductUserIdByBaseNum(gh.ToString().PadLeft(4, '0'),2));
            }
            catch
            {
                return null;
            }
            UserGw user = getUserGwByUid(uid);
            DataTable dt = new DataTable();
            if (lblx == 1)//所有文件列表
            {
                dt = DAL.getAllGwlb(user, gwlx, dwlx);
            }
            else//未批阅或签收文件列表
            {
                dt = DAL.getUnfinishedGwlb(user, dwlx);
            }
            if (ksxh > dt.Rows.Count) return null;
            GongWen[] gws = new GongWen[ksxh+ count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < gws.Length; i++)
            {
                gws[i] = new GongWen();
                gws[i].Id = Convert.ToInt32(dt.Rows[i + ksxh - 1]["id"]);
                gws[i].SendDept = Convert.ToString(dt.Rows[i + ksxh - 1]["fwdw"]);
                gws[i].Title = Convert.ToString(dt.Rows[i + ksxh - 1]["bt"]);
                gws[i].Number = Convert.ToString(dt.Rows[i + ksxh - 1]["wh"]);
                gws[i].SendDate = Convert.ToString(dt.Rows[i + ksxh - 1]["fwrq"]);
            }
            return gws;
        }

        public static string makeGwString(GongWen gw)
        {
            if (gw == null || gw.Id == 0)
            {
                throw new ArgumentNullException("GongWen", "公文未初始化！");
            }
            string str = SjbgConfig.FuHaoKaiShi;
            str = str + SjbgConfig.FuHaoFenGe + gw.Id;
            str = str + SjbgConfig.FuHaoFenGe + gw.RedTitle;
            str = str + SjbgConfig.FuHaoFenGe + gw.Number;
            str = str + SjbgConfig.FuHaoFenGe + gw.Title;
            str = str + SjbgConfig.FuHaoFenGe + gw.Content;
            str = str + SjbgConfig.FuHaoFenGe + gw.SendDept;
            str = str + SjbgConfig.FuHaoFenGe + gw.SendDate;
            str = str + SjbgConfig.FuHaoFenGe + gw.Suggestion;
            str = str + SjbgConfig.FuHaoFenGe + gw.FileType;
            str = str + SjbgConfig.FuHaoFenGe + gw.SendType;
            str = str + SjbgConfig.FuHaoFenGe + gw.AttachPath01;
            str = str + SjbgConfig.FuHaoFenGe + gw.AttachPath02;
            str = str + SjbgConfig.FuHaoFenGe + gw.AttachPath03;
            str = str + SjbgConfig.FuHaoFenGe + gw.AttachPath04;
            str = str + SjbgConfig.FuHaoFenGe + gw.AttachPath05;
            str = str + SjbgConfig.FuHaoFenGe + gw.AttachPath06;
            return str;
        }

        public static User getUserByNum(string user_no)
        {
            DataTable dt = DAL.getUserByNum(user_no);
            User u = new User();
            if (dt.TableName.Equals("user"))
            {
                u.Uid = Convert.ToInt32(dt.Rows[0]["user_no"]);
                u.UserNo = Convert.ToString(dt.Rows[0]["user_no"]);
                u.UserName = Convert.ToString(dt.Rows[0]["user_name"]);
                u.UserDept = Convert.ToInt32(dt.Rows[0]["bumen_id"]);
                u.GwLevel = getGwLevel(u.Uid).Number;
                u.TqLevel = getTqLevel(u.Uid).Number;
            }
            return u;

        }



        public static INT login(string user_no, string user_pass,string code,string ip,string deviceInfo,string deviceVersion)
        {
            if (!isValidPass(user_pass)) return new INT(0,"工号或密码不正确");
            if (!isValidUserNo(user_no)) return new INT(0, "工号或密码不正确");
            return DAL.login(user_no, user_pass, code,ip, deviceInfo,deviceVersion);
        }

        public static INT loginDirect(string user_no, string user_pass, string code, string ip, string deviceInfo, string deviceVersion)
        {
            if (!isValidPass(user_pass)) return new INT(0, "工号或密码不正确");
            if (!isValidUserNo(user_no)) return new INT(0, "工号或密码不正确");
            return DAL.loginDirect(user_no, user_pass, code, ip, deviceInfo, deviceVersion);
        }

        public static XinWen[] getXinWen(int xwlx, int ksxh, int count)
        {
            DataTable dt = DAL.getXwlb(xwlx);
            XinWen[] xws = new XinWen[count < dt.Rows.Count ? count : dt.Rows.Count];
            for (int i = 0; i < xws.Length; i++)
            {
                xws[i] = new XinWen();
                xws[i].ID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["id"]);
                xws[i].Title = Convert.ToString(dt.Rows[i + ksxh - 1]["wybt"]);
                xws[i].TypeId = Convert.ToInt32(dt.Rows[i + ksxh - 1]["sclbid"]);
                xws[i].Date = Convert.ToString(dt.Rows[i + ksxh - 1]["scrq"]);
                xws[i].Publisher = Convert.ToString(dt.Rows[i + ksxh - 1]["yhbm"]);
                xws[i].Content = Convert.ToString(dt.Rows[i + ksxh - 1]["wjnr"]);
                xws[i].AttachFiles = Convert.ToString(dt.Rows[i + ksxh - 1]["wjmc"]);
                xws[i].Path = Convert.ToString(dt.Rows[i + ksxh - 1]["path"]);
            }
            return xws;
        }

        public static XinWenLeiXing[] getXinWenLeiXing()
        {
            DataTable dt = DAL.getXwlx();
            XinWenLeiXing[] xwlx = new XinWenLeiXing[dt.Rows.Count];
            for (int i = 0; i < xwlx.Length; i++)
            {
                xwlx[i] = new XinWenLeiXing();
                xwlx[i].ID = Convert.ToInt32(dt.Rows[i]["id"]);
                xwlx[i].Type = Convert.ToString(dt.Rows[i]["sclb"]);
            }
            return xwlx;
        }

        public static Product getProductByPname(string pname)
        {
            DataTable dt = DAL.getProductByPid(pname);
            Product p = new Product();
            if (dt.TableName.Equals("product"))
            {
                p.Pid = Convert.ToInt32(dt.Rows[0]["xt_id"]);
                p.PName = Convert.ToString(dt.Rows[0]["xt_mc"]);
                p.ServicePage = Convert.ToString(dt.Rows[0]["webServiceURL"]);
            }
            return p;
        }

        public static ZbPerson[] getZbPersons(DateTime dateTime, int isNight)
        {
            DataTable dt = DAL.getZbPerson(dateTime, isNight);
            ZbPerson[] persons = new ZbPerson[dt.Rows.Count];
            for (int i = 0; i < persons.Length; i++)
            {
                persons[i] = new ZbPerson();
                persons[i].Date = Convert.ToDateTime(dt.Rows[i]["zbrq"]).ToString("yyyy-MM-dd");
                persons[i].Dept = Convert.ToString(dt.Rows[i]["bm"]).Trim();
                persons[i].DayNight = Convert.ToString(dt.Rows[i]["zblb"]);
                persons[i].Mobile = Convert.ToString(dt.Rows[i]["sjh"]).Trim();
                persons[i].Phone = Convert.ToString(dt.Rows[i]["zbdh"]).Trim();
                persons[i].signTime = Convert.ToString(dt.Rows[i]["qdsj"]);
                persons[i].Workno = Convert.ToString(dt.Rows[i]["zbrgh"]).Trim();
                persons[i].Name = Convert.ToString(dt.Rows[i]["zbrxm"]).Trim();
            }
            return persons;
        }
        public static string getZbLdps(DateTime dateTime)
        {
            return DAL.getZbLdps(dateTime);
        }

        internal static UserGw[] getLeaderList()
        {
            DataTable dt = DAL.getLeaderList();
            if (dt.TableName.Equals("leaderList") && dt.Rows.Count > 0)
            {
                UserGw[] users = new UserGw[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    users[i] = new UserGw();
                    users[i].Yhbh = Convert.ToInt32(dt.Rows[i]["yhbh"]);
                    users[i].Yhmc = Convert.ToString(dt.Rows[i]["yhmc"]);
                    users[i].Yhsm = Convert.ToString(dt.Rows[i]["yhsm"]);
                    users[i].Ssbm = Convert.ToInt32(dt.Rows[i]["ssbm"]);
                    users[i].Yhnc = Convert.ToString(dt.Rows[i]["yhnc"]);
                    users[i].Yhqx = Convert.ToInt32(dt.Rows[i]["yhqx"]);
                    users[i].ShuangQian = Convert.ToInt32(dt.Rows[i]["shuangqian"]);
                    //u.Wjxz = Convert.ToInt32(dt.Rows[0]["wjxz"]);
                    users[i].Ssxzbm = DAL.getSsxzbmBySsbm(users[i].Ssbm);

                }
                return users;
            }
            else
            {
                return null;
            }
        }

        internal static BuMenGw[] getBmList(int lbid)
        {
            DataTable dt = DAL.getGwbmList(lbid);
            if (dt.TableName.Equals("bmList") && dt.Rows.Count > 0)
            {
                BuMenGw[] bumen = new BuMenGw[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bumen[i] = new BuMenGw();
                    bumen[i].Bmbh = Convert.ToInt32(dt.Rows[i]["bmbh"]);
                    bumen[i].Bmlbmc = Convert.ToString(dt.Rows[i]["lbmc"]);
                    bumen[i].Bmmc = Convert.ToString(dt.Rows[i]["bmmc"]);
                    bumen[i].Bmlb = Convert.ToInt32(dt.Rows[i]["lbid"]);
                    try
                    {
                        bumen[i].Bmsqbh = Convert.ToInt32(dt.Rows[i]["ssxzbm"]);
                    }
                    catch
                    {
                        bumen[i].Bmsqbh = 0;
                    }

                }
                return bumen;
            }
            else
            {
                return null;
            }
        }

        internal static BuMenLeiBie[] getBmlbList()
        {
            DataTable dt = DAL.getGwbmlbList();
            if (dt.TableName.Equals("bmlbList") && dt.Rows.Count > 0)
            {
                BuMenLeiBie[] bmlb = new BuMenLeiBie[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bmlb[i] = new BuMenLeiBie();
                    bmlb[i].Lbmc = Convert.ToString(dt.Rows[i]["lbmc"]);
                    bmlb[i].ID = Convert.ToInt32(dt.Rows[i]["id"]);

                }
                return bmlb;
            }
            else
            {
                return null;
            }
        }

        internal static string[] getJianBaoBuMen()
        {
            DataTable dt = DAL.getJianBaoBuMen();
            if (dt.TableName.Equals("jbbm") && dt.Rows.Count > 0)
            {
                string[] bms = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bms[i] = Convert.ToString(dt.Rows[i]["bmmc"]);

                }
                return bms;
            }
            else
            {
                return null;
            }
        }

        internal static JianBao[] getJianBao(string dept, DateTime dateTime)
        {
            DataTable dt = DAL.getJianBao(dept,dateTime);
            if (dt.TableName.Equals("JianBao") && dt.Rows.Count > 0)
            {
                JianBao[] jbs = new JianBao[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jbs[i] = new JianBao();
                    jbs[i].BuMen = Convert.ToString(dt.Rows[i]["bm"]);
                    jbs[i].Date = Convert.ToDateTime(dt.Rows[i]["jbrq"]).ToString("yyyy-MM-dd");
                    jbs[i].NeiRong = Convert.ToString(dt.Rows[i]["jbnr"]);
                    jbs[i].XiangMu = Convert.ToString(dt.Rows[i]["jbxm"]);
                }
                return jbs;
            }
            else
            {
                return null;
            }
        }

        internal static JianBao[] getAllJianBao(DateTime dateTime)
        {
            DataTable dt = DAL.getJianBao("all" ,dateTime);
            if (dt.TableName.Equals("JianBao") && dt.Rows.Count > 0)
            {
                JianBao[] jbs = new JianBao[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jbs[i] = new JianBao();
                    jbs[i].BuMen = Convert.ToString(dt.Rows[i]["bm"]);
                    jbs[i].Date = Convert.ToDateTime(dt.Rows[i]["jbrq"]).ToString("yyyy-MM-dd");
                    jbs[i].NeiRong = Convert.ToString(dt.Rows[i]["jbnr"]);
                    jbs[i].XiangMu = Convert.ToString(dt.Rows[i]["jbxm"]);
                    jbs[i].BmOrders = Convert.ToInt32(dt.Rows[i]["orders"]);
                }
                return jbs;
            }
            else
            {
                return null;
            }
        }



        internal static ApkInfo getApkInfo()
        {
            return new ApkInfo(SjbgConfig.ApkVerCode, SjbgConfig.ApkVerName, SjbgConfig.ApkFilePath, SjbgConfig.ApkFileName, SjbgConfig.ApkUpdateContent);
        }


        public static INT getTqUidByWorkNo(int workno)
        {
            string work_no = workno.ToString().PadLeft(4, '0');
            return DAL.getTqUidByWorkNo(work_no);
        }

        public static INT getTqUtypeByUid(int uid)
        {
            return DAL.getTqUtypeByUid(uid);
        }
        public static INT getTqUDeptByUid(int uid)
        {
            return DAL.getTqUDeptByUid(uid);
        }

        internal static TeQing[] getTeQingByWorkNo(int workno,int ksxh,int count)
        {
            int uid = getTqUidByWorkNo(workno).Number;
            DataTable dt = DAL.getTeQingByUid(uid);
			if (  dt == null || dt.Rows.Count == 0 || !dt.TableName.Equals("getTeQingByUid") ) return null;
            TeQing[] tqs = new TeQing[count < dt.Rows.Count ? count : dt.Rows.Count];
            for (int i = 0; i < tqs.Length; i++)
            {
                tqs[i] = new TeQing();
				tqs[i].ID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["mid"]);
				tqs[i].SenderDept = Convert.ToString(dt.Rows[i + ksxh - 1]["deptName"]);
				tqs[i].SenderName = Convert.ToString(dt.Rows[i + ksxh - 1]["sName"]);
				tqs[i].Title = Convert.ToString(dt.Rows[i + ksxh - 1]["mtitle"]);
				tqs[i].Content = Convert.ToString(dt.Rows[i + ksxh - 1]["mtext"]);
				tqs[i].ReplyTime = Convert.ToString(dt.Rows[i + ksxh - 1]["readTime"]);
				tqs[i].SendTime = Convert.ToString(dt.Rows[i + ksxh - 1]["sendTime"]);
				tqs[i].NeedReply = Convert.ToInt32(dt.Rows[i + ksxh - 1]["needReply"]);
            }
            return tqs;
        }



        internal static BOOLEAN replyTq(int workno,int tid, string replayContent)
        {
            int uid = getTqUidByWorkNo(workno).Number;
            return DAL.replyTq(uid, tid, replayContent);   
        }

        internal static TeQing[] checkReply(int senderno ,int ksxh,int count)
        {
            int uid = getTqUidByWorkNo(senderno).Number;
            DataTable dt = DAL.checkReply(uid);
            if (dt.TableName != "checkReply" || dt == null || dt.Rows.Count == 0) return null;
			TeQing[] tqs = new TeQing[count < dt.Rows.Count ? count : dt.Rows.Count];
            for (int i = 0; i < tqs.Length; i++)
            {
				tqs[i] = new TeQing();
				tqs[i].ID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["mid"]);
				tqs[i].SenderDept = Convert.ToString(dt.Rows[i + ksxh - 1]["deptName"]);
				tqs[i].SenderName = Convert.ToString(dt.Rows[i + ksxh - 1]["sName"]);
				tqs[i].Title = Convert.ToString(dt.Rows[i + ksxh - 1]["mtitle"]);
				tqs[i].Content = Convert.ToString(dt.Rows[i + ksxh - 1]["mtext"]);
				tqs[i].SendTime = Convert.ToString(dt.Rows[i + ksxh - 1]["sendTime"]);
				tqs[i].ReplyCount = Convert.ToInt32(dt.Rows[i + ksxh - 1]["readCount"]);
				tqs[i].SendCount = Convert.ToInt32(dt.Rows[i + ksxh - 1]["sendCountl"]);
            }
            return tqs;
        }

        internal static TeQing[] checkReplyDetails(int tid,int ksxh,int count)
        {
            DataTable dt = DAL.checkReplyDetails(tid);
            if (dt.TableName != "checkReplyDetails" || dt == null || dt.Rows.Count == 0) return null;
			TeQing[] tqs = new TeQing[count < dt.Rows.Count ? count : dt.Rows.Count];
            for (int i = 0; i < tqs.Length; i++)
            {
                tqs[i] = new TeQing();
				tqs[i].ReceiverDept = Convert.ToString(dt.Rows[i + ksxh - 1]["rdeptname"]);
				tqs[i].ReceiverName = Convert.ToString(dt.Rows[i + ksxh - 1]["rName"]);
				tqs[i].ReplyTime = Convert.ToString(dt.Rows[i + ksxh - 1]["readTime"]);
				tqs[i].NeedReply = Convert.ToInt32(dt.Rows[i + ksxh - 1]["needReply"]);
				tqs[i].ReplyContent = Convert.ToString(dt.Rows[i + ksxh - 1]["txt"]);
            }
            return tqs;
        }


        internal static INT getTqLevel(int workno)
        {
            int tquid = getTqUidByWorkNo(workno).Number;
            return getTqUtypeByUid(tquid);
        }

		internal static INT getGwLevel(int workno)
		{
			int uid;
			try
			{
				uid = Convert.ToInt32(DAL.getProductUserIdByBaseNum(workno.ToString().PadLeft(4, '0'), 2));
			}
			catch (Exception ex)
			{
				return new INT (-1,ex.Message);
			}
			return new INT( getUserGwByUid(uid).Yhqx,"");
		}



		internal static INT registerDevice(RegisterInfo ri)
		{
            INT registerCode = DAL.checkUserMobile(ri.WorkNo, ri.Mobile, ri.UniqueCode);
            if (registerCode.Number == 0)//判断该设备是否已经注册
			{
                return DAL.registerDevice(ri.WorkNo, ri.Mobile, ri.UniqueCode, ri.RegisterCode, ri.SecurityQuestion, ri.SecurityAnswer, ri.EmailAddress);
			}
            else if (registerCode.Number == 1) //设备已注册
            {
                return new INT(-99,"该设备已经注册");
            }
            else 
			{
				return registerCode;
			}
		}


		internal static INT requestRegisterCode(RegisterInfo ri)
		{
			string code = generateRegisterCode(ri.WorkNo,ri.Mobile);
            INT i = DAL.checkUserMobile(ri.WorkNo, ri.Mobile, ri.UniqueCode);
            if (i.Number < 0 ) return i;
            i = DAL.insertRegisterCode(ri.WorkNo, ri.Mobile, code, ri.UniqueCode); 
			if( i.Number == 1)
			{
                return DAL.sendMobileMessage(ri.WorkNo, makeRegisterMobileMessageContent(code));

			}
			else
			{
				return i;
			}
		}
		static string makeRegisterMobileMessageContent(string code)
		{
			string str = "您的手机办公系统注册验证码是：" + code + "，请在2分钟内完成注册，如非本人操作请忽略。";
			return str;
		}

		static string generateRegisterCode(int work_no, string mobile)
		{
			string strSeed = work_no.ToString() + mobile +  Convert.ToString(DateTime.Now.Ticks);
			strSeed = ToMD5String(strSeed);
			strSeed = strSeed.Substring(0, 2) + strSeed.Substring(8, 2) + strSeed.Substring(16, 2) + strSeed.Substring(24 , 2);
			uint uSeed = uint.Parse(strSeed,System.Globalization.NumberStyles.HexNumber);
			int iSeed;
			iSeed = Convert.ToInt32(uSeed / 2);
			Random r = new Random(iSeed);
			strSeed = r.Next(1, 999999).ToString().PadLeft(6,'0');
			return strSeed;
		}

        internal static GpsData[] getGpsByNum(string gps_data)
        {
            DataTable dt = DAL.getGpsByNum(gps_data);
            if (dt.TableName.Equals("gpsRead") && dt.Rows.Count > 0)
            {
                GpsData[] gps = new GpsData[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gps[i] = new GpsData();
                    gps[i].work_no = Convert.ToString(dt.Rows[i]["work_no"]);
                    gps[i].work_name = Convert.ToString(dt.Rows[i]["work_name"]);
                    gps[i].Jingdu = Convert.ToString(dt.Rows[i]["JingDu"]);
                    gps[i].Weidu = Convert.ToString(dt.Rows[i]["WeiDu"]);
                    gps[i].Weizhi = Convert.ToString(dt.Rows[i]["WeiZhi"]);
                    gps[i].Shijian = Convert.ToString(dt.Rows[i]["ShiJian"]);

                }
                return gps;
            }
            else
            {
                return null;
            }


        }

        internal static UserRole[] getUserRoleByNum(int uid)
        {
            string work_no = uid.ToString().PadLeft(4, '0');
            DataTable dt = DAL.getUserRole(work_no);
            if (dt.TableName.Equals("getUserRole") && dt.Rows.Count > 0)
            {
                UserRole[] ur = new UserRole[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ur[i] = new UserRole();
                    ur[i].ID = Convert.ToInt32(dt.Rows[i]["rid"]);
                    ur[i].Name = Convert.ToString(dt.Rows[i]["rname"]);
                    ur[i].Description = Convert.ToString(dt.Rows[i]["descr"]);
               

                }
                return ur;
            }
            else
            {
                return null;
            }
        }

        internal static MenuItem[] getUserMenuByNum(int uid)
        {
            string work_no = uid.ToString().PadLeft(4, '0');
            DataTable dt = DAL.getUserMenu(work_no);
            if (dt.TableName.Equals("getUserMenu") && dt.Rows.Count > 0)
            {
                MenuItem[] mi = new MenuItem[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mi[i] = new MenuItem();
                    mi[i].M1Id = Convert.ToInt32(dt.Rows[i]["M1Id"]);
                    mi[i].M1Name = Convert.ToString(dt.Rows[i]["M1Name"]);
                    mi[i].M2Id = Convert.ToInt32(dt.Rows[i]["M2Id"]);
                    mi[i].M2Name = Convert.ToString(dt.Rows[i]["M2Name"]);
                    mi[i].ImageRes = Convert.ToString(dt.Rows[i]["ImageRes"]);
                    mi[i].Enabled = Convert.ToInt32(dt.Rows[i]["Enabled"]);
                    mi[i].ActivityName = Convert.ToString(dt.Rows[i]["ActivityName"]);
                    mi[i].Params = Convert.ToString(dt.Rows[i]["Params"]);
                }
                return mi;
            }
            else
            {
                return null;
            }
        }

        //20140918 add by zhh for 运用
        //机车计划处理数据
        internal static JcjhData[] getJcjhByNum(string jcjh_data)
        {
            DataTable dt = DAL.getJcjhByNum(jcjh_data);
            if (dt.TableName.Equals("jcjhRead") && dt.Rows.Count > 0)
            {
                JcjhData[] jcjh = new JcjhData[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jcjh[i] = new JcjhData();
                    jcjh[i].plan_date = Convert.ToString(dt.Rows[i]["plan_date"]);
                    jcjh[i].open_time = Convert.ToString(dt.Rows[i]["open_time"]);
                    jcjh[i].locus = Convert.ToString(dt.Rows[i]["locus"]);
                    jcjh[i].engi_brand = Convert.ToString(dt.Rows[i]["engi_brand"]);
                    jcjh[i].engi_no = Convert.ToString(dt.Rows[i]["engi_no"]);
                    //jcjh[i].Shijian = Convert.ToString(dt.Rows[i]["ShiJian"]);

                }
                return jcjh;
            }
            else
            {
                return null;
            }


        }
        //

        //人员计划处理数据
        //人员计划处理数据
        internal static RyjhData[] getRyjhByNum(string ryjh_data)
        {
            DataTable dt = DAL.getRyjhByNum(ryjh_data);
            if (dt.TableName.Equals("ryjhRead") && dt.Rows.Count > 0)
            {
                RyjhData[] ryjh = new RyjhData[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ryjh[i] = new RyjhData();
                    ryjh[i].plan_date = Convert.ToString(dt.Rows[i]["plan_date"]);
                    //ryjh[i].open_time = Convert.ToString(dt.Rows[i]["open_time"]);
                    ryjh[i].locus = Convert.ToString(dt.Rows[i]["locus"]);
                    ryjh[i].engi_no = Convert.ToString(dt.Rows[i]["engi_no"]);
                    ryjh[i].Roadway = Convert.ToString(dt.Rows[i]["Roadway"]);
                    ryjh[i].ZunDian_time = Convert.ToString(dt.Rows[i]["ZunDian_time"]);
                    ryjh[i].driver_1no = Convert.ToString(dt.Rows[i]["driver_1no"]);
                    //ryjh[i].driver_1name = Convert.ToString(dt.Rows[i]["driver_1name"]);
                    ryjh[i].driver_2no = Convert.ToString(dt.Rows[i]["driver_2no"]);
                    //ryjh[i].driver_2name = Convert.ToString(dt.Rows[i]["driver_2name"]);
                    ryjh[i].driver_3no = Convert.ToString(dt.Rows[i]["driver_3no"]);
                    //ryjh[i].driver_3name = Convert.ToString(dt.Rows[i]["driver_3name"]);
                }
                return ryjh;
            }
            else
            {
                return null;
            }


        }
        //

        //待乘计划处理数据
        internal static DcjhData[] getDcjhByNum(string dcjh_data)
        {
            DataTable dt = DAL.getDcjhByNum(dcjh_data);
            if (dt.TableName.Equals("dcjhRead") && dt.Rows.Count > 0)
            {
                DcjhData[] dcjh = new DcjhData[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dcjh[i] = new DcjhData();
                    dcjh[i].plan_date = Convert.ToString(dt.Rows[i]["plan_date"]);
                    //ryjh[i].open_time = Convert.ToString(dt.Rows[i]["open_time"]);
                    dcjh[i].dri_Room_no = Convert.ToString(dt.Rows[i]["dri_Room_no"]);
                    dcjh[i].Drive_name = Convert.ToString(dt.Rows[i]["Drive_name"]);
                    dcjh[i].Dri_time = Convert.ToString(dt.Rows[i]["Dri_time"]);
                    dcjh[i].Ass_name = Convert.ToString(dt.Rows[i]["Ass_name"]);
                    dcjh[i].Ass_time = Convert.ToString(dt.Rows[i]["Ass_time"]);
                    dcjh[i].Student_name = Convert.ToString(dt.Rows[i]["Student_name"]);
                    dcjh[i].Stu_time = Convert.ToString(dt.Rows[i]["Stu_time"]);

                }
                return dcjh;
            }
            else
            {
                return null;
            }


        }
        //
        //测酒结果查询处理数据
        internal static CjcxData[] getCjcxByNum(string cjcx_data)
        {
            DataTable dt = DAL.getCjcxByNum(cjcx_data);
            if (dt.TableName.Equals("cjcxRead") && dt.Rows.Count > 0)
            {
                CjcxData[] cjcx = new CjcxData[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cjcx[i] = new CjcxData();
                    cjcx[i].plan_date = Convert.ToString(dt.Rows[i]["plan_date"]);
                    //ryjh[i].open_time = Convert.ToString(dt.Rows[i]["open_time"]);
                    cjcx[i].dd = Convert.ToString(dt.Rows[i]["dd"]);
                    cjcx[i].work_no = Convert.ToString(dt.Rows[i]["work_no"]);
                    //cjcx[i].work_name = Convert.ToString(dt.Rows[i]["work_name"]);
                    cjcx[i].cjjg = Convert.ToString(dt.Rows[i]["cjjg"]);

                }
                return cjcx;
            }
            else
            {
                return null;
            }
        }
        //20140918 add by zhh for运用

        internal static BOOLEAN setNewPass(int uid, string opass, string npass)
        {
            string work_no = uid.ToString().PadLeft(4, '0');
            return DAL.setNewPass(work_no, opass, npass);
                
        }

        internal static MingPai[] getMingPaiByXianBie(int database, string line_mode,int type)
        {
            DataTable dt = new DataTable();
            dt = DAL.getMingPaiByXianBie(database, line_mode,type);
            if (dt.TableName.Equals("MingPai") && dt.Rows.Count > 0)
            {
                MingPai[] mps = new MingPai[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mps[i] = new MingPai();
                    mps[i].GongHao = Convert.ToString(dt.Rows[i]["work_no"]);
                    mps[i].XingMing = Convert.ToString(dt.Rows[i]["work_name"]);
                    mps[i].XianBieID = Convert.ToString(dt.Rows[i]["line_mode"]);
                    mps[i].XianBie = Convert.ToString(dt.Rows[i]["qduan"]);
                    mps[i].BanCi = Convert.ToInt32(dt.Rows[i]["banci"]);
                    mps[i].WeiZhi = Convert.ToInt32(dt.Rows[i]["weizhi"]);
                    mps[i].ZhuangTai = Convert.ToString(dt.Rows[i]["state_name"]);
                }
                return mps;
            }
            else
            {
                return null;
            }
        }

        internal static MingPai[] getMingPaiByUid(int database ,int uid)
        {
            DataTable dt = new DataTable();
            string work_no = uid.ToString().PadLeft(4, '0');
            dt = DAL.getMingPaiByGongHao(database, work_no);
            if (dt.TableName.Equals("MingPai") && dt.Rows.Count > 0)
            {
                MingPai[] mps = new MingPai[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mps[i] = new MingPai();
                    mps[i].GongHao = Convert.ToString(dt.Rows[i]["work_no"]);
                    mps[i].XingMing = Convert.ToString(dt.Rows[i]["work_name"]);
                    mps[i].XianBieID = Convert.ToString(dt.Rows[i]["line_mode"]);
                    mps[i].XianBie = Convert.ToString(dt.Rows[i]["qduan"]);
                    mps[i].BanCi = Convert.ToInt32(dt.Rows[i]["banci"]);
                    mps[i].WeiZhi = Convert.ToInt32(dt.Rows[i]["weizhi"]);
                    mps[i].ZhuangTai = Convert.ToString(dt.Rows[i]["state_name"]);
                }
                return mps;
            }
            else
            {
                return null;
            }
        }

        internal static XianBie[] getXianBie(int database)
        {
            DataTable dt = new DataTable();
            dt = DAL.getXianBie(database);
            if (dt.TableName.Equals("getXianBie") && dt.Rows.Count > 0)
            {
                XianBie[] xbs = new XianBie[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    xbs[i] = new XianBie();
                    xbs[i].LeiXing = 1;//轮乘
                    xbs[i].XianBieID = Convert.ToString(dt.Rows[i]["line_mode"]);
                    xbs[i].XianBieMingCheng = Convert.ToString(dt.Rows[i]["qduan"]);
                    

                }
                return xbs;
            }
            else
            {
                return null;
            }
        }

        internal static DaMingPai[] getDaMingPaiByXianBie(int database, string line, int type, string filter, int ksxh, int count)
        {
            DataTable dt = new DataTable();

            dt = DAL.getDaMingPai(database, line, type,filter);

            if (!dt.TableName.Equals("getDaMingPai") || (ksxh > dt.Rows.Count)) return null;
            DaMingPai[] dmps = new DaMingPai[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < dmps.Length; i++)
            {
                dmps[i] = new DaMingPai();
                dmps[i].XianBieID = Convert.ToString(dt.Rows[i + ksxh - 1]["line_mode"]);
                dmps[i].XianBie = Convert.ToString(dt.Rows[i + ksxh - 1]["qduan"]);
                dmps[i].BanCi = Convert.ToInt32(dt.Rows[i + ksxh - 1]["banci"]);
                dmps[i].GongHao1 = Convert.ToString(dt.Rows[i + ksxh - 1]["gh1"]);
                dmps[i].XingMing1 = Convert.ToString(dt.Rows[i + ksxh - 1]["xm1"]);
                dmps[i].ZhuangTai1 = Convert.ToString(dt.Rows[i + ksxh - 1]["zt1"]);
                dmps[i].GongHao2 = Convert.ToString(dt.Rows[i + ksxh - 1]["gh2"]);
                dmps[i].XingMing2 = Convert.ToString(dt.Rows[i + ksxh - 1]["xm2"]);
                dmps[i].ZhuangTai2 = Convert.ToString(dt.Rows[i + ksxh - 1]["zt2"]);
                dmps[i].GongHao3 = Convert.ToString(dt.Rows[i + ksxh - 1]["gh3"]);
                dmps[i].XingMing3 = Convert.ToString(dt.Rows[i + ksxh - 1]["xm3"]);
                dmps[i].ZhuangTai3 = Convert.ToString(dt.Rows[i + ksxh - 1]["zt3"]);
                dmps[i].GongHao4 = Convert.ToString(dt.Rows[i + ksxh - 1]["gh4"]);
                dmps[i].XingMing4 = Convert.ToString(dt.Rows[i + ksxh - 1]["xm4"]);
                dmps[i].ZhuangTai4 = Convert.ToString(dt.Rows[i + ksxh - 1]["zt4"]);
            }
            return dmps;
        }

        internal static CanBu[] getCanBu(int uid, string month)
        {
            string work_no = uid.ToString().PadLeft(4, '0');
            string strJssj = month + "-25 18:00:00";
            DateTime jssj = Convert.ToDateTime(strJssj);
            DateTime kssj = jssj.AddMonths(-1);
            DataTable dt = DAL.getCanBu(work_no, kssj, jssj);
            if (dt.TableName.Equals("getCanBu") && dt.Rows.Count > 0)
            {
                CanBu[] cbs = new CanBu[dt.Rows.Count];
                for (int i = 0; i < cbs.Length; i++)
                {
                    cbs[i] = new CanBu();
                    cbs[i].GongHao = Convert.ToString(dt.Rows[i]["rybh"]);
                    cbs[i].XingMing = Convert.ToString(dt.Rows[i]["ryxm"]);
                    cbs[i].JinE = Convert.ToString(dt.Rows[i]["je"]);
                    cbs[i].LingQV = Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("MM-dd HH:mm");
                    cbs[i].CheCi = Convert.ToString(dt.Rows[i]["roadway"]);
                    cbs[i].KaiChe = Convert.ToDateTime(dt.Rows[i]["open_time"]).ToString("MM-dd HH:mm");
                    cbs[i].DaoDa = Convert.ToDateTime(dt.Rows[i]["arrive_time"]).ToString("MM-dd HH:mm");
                    cbs[i].ShiChang = Math.Round( Convert.ToDouble(dt.Rows[i]["shichang"]) ,2).ToString();
                }
                return cbs;
            }
            else
            {
                return null;
            }
        }

        static string fyySjzh(string sj)
        {
            string xsj = "";
            if (sj == null || sj == "") return "";
            if (sj.Length != 12 ) return "";
            xsj = sj.Substring(0, 4);
            xsj += "-" + sj.Substring(4, 2);
            xsj += "-" + sj.Substring(6, 2);
            xsj += " " + sj.Substring(8, 2);
            xsj += ":" + sj.Substring(10, 2);
            return xsj;
        }

        internal static FeiYunYongJiChe[] getFyyjc(string jczt)
        {

            DataTable dt = DAL.getFyyjc(jczt);
            if (dt.TableName.Equals("fyyjc") && dt.Rows.Count > 0)
            {
                FeiYunYongJiChe[] fyys = new FeiYunYongJiChe[dt.Rows.Count];
                for (int i = 0; i < fyys.Length; i++)
                {
                    fyys[i] = new FeiYunYongJiChe();
                    fyys[i].JiCheLeiXing = Convert.ToString(dt.Rows[i]["jclx"]);
                    fyys[i].JiCheBianHao = Convert.ToString(dt.Rows[i]["jcbh"]);
                    fyys[i].ZhuangTai = Convert.ToString(dt.Rows[i]["jczt"]);
                    fyys[i].DiDian = Convert.ToString(dt.Rows[i]["dd"]);
                    fyys[i].ZhuanRuShiJian = fyySjzh(Convert.ToString(dt.Rows[i]["zrsj"]));
                    fyys[i].ZhuanChuShiJian = fyySjzh(Convert.ToString(dt.Rows[i]["zcsj"]));
                    fyys[i].GongZuoShiJian = Convert.ToString(dt.Rows[i]["gzsj"]);
                    
                }
                return fyys;
            }
            else
            {
                return null;
            }
        }

        internal static string[] getFyyjcZt()
        {
            DataTable dt = DAL.getFyyjcZt();
            if (dt.TableName.Equals("fyyjcZt") && dt.Rows.Count > 0)
            {
                string[] zts = new string[dt.Rows.Count];
                for (int i = 0; i < zts.Length; i++)
                {

                    zts[i] = Convert.ToString(dt.Rows[i]["zryy"]);
                    

                }
                return zts;
            }
            else
            {
                return null;
            } 
        }


        ////电子书名查询处理数据
        //internal static BookNameData[] getBookName(string book_Name)
        //{
        //    DataTable dt = DAL.getBookName(book_Name);
        //    if (dt.TableName.Equals("BookNameRead") && dt.Rows.Count > 0)
        //    {
        //        BookNameData[] BookName = new BookNameData[dt.Rows.Count];
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            BookName[i] = new BookNameData();
        //            BookName[i].id = Convert.ToString(dt.Rows[i]["id"]);
        //            BookName[i].Name = Convert.ToString(dt.Rows[i]["Name"]);

        //        }
        //        return BookName;
        //    }
        //    else
        //    {
        //        return null;
        //    }


        //}

        //电子书名查询处理数据
        internal static BookNameData[] getBookName(string bookname_data)
        {
            DataTable dt = DAL.getBookName(bookname_data);
            if (dt.TableName.Equals("BookNameRead") && dt.Rows.Count > 0)
            {
                BookNameData[] BookName = new BookNameData[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    BookName[i] = new BookNameData();
                    BookName[i].id = Convert.ToString(dt.Rows[i]["id"]);
                    BookName[i].Name = Convert.ToString(dt.Rows[i]["Name"]);
                    BookName[i].Address = Convert.ToString(dt.Rows[i]["Address"]);
                }
                return BookName;
            }
            else
            {
                return null;
            }


        }

        //电子书内容查询处理数据
        internal static BookNrData getBookNr(string book_Nr_id)
        {
            DataTable dt = DAL.getBookNr(book_Nr_id);
            if (dt.TableName.Equals("BookNrRead"))
            {
                BookNrData BookNr = new BookNrData();


                BookNr.id = Convert.ToString(dt.Rows[0]["id"]);
                BookNr.Txt = Convert.ToString(dt.Rows[0]["Txt"]);


                return BookNr;
            }
            else
            {
                return null;
            }


        }

        internal static SentFileList[] getSentFiles(int uid)
        {
            string work_no = uid.ToString().PadLeft(4, '0');
            DataTable dt = DAL.getSentFiles(work_no);
            if (!dt.TableName.Equals("getSentFiles"))
            {
                return null;
            }
            else
            {
                SentFileList[] sfls = new SentFileList[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sfls[i] = new SentFileList();
                    sfls[i].FID = Convert.ToInt32(dt.Rows[i]["fid"]);
                    sfls[i].AllCount = Convert.ToInt32(dt.Rows[i]["allCount"]);
                    sfls[i].ReceiveCount = Convert.ToInt32(dt.Rows[i]["receiveCount"]);
                    sfls[i].FileName = Convert.ToString(dt.Rows[i]["fileName"]);
                    sfls[i].Sender = Convert.ToString(dt.Rows[i]["sender"]);
                    sfls[i].SendTime = Convert.ToString(dt.Rows[i]["sendTime"]);
                }
                return sfls;
            }
        }


        internal static SentFileDetail[] getSentFileDetails(int fid)
        {
            DataTable dt = DAL.getSentFileDetails(fid);
            if (!dt.TableName.Equals("getSentFileDetails"))
            {
                return null;
            }
            else
            {
                SentFileDetail[] sfls = new SentFileDetail[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sfls[i] = new SentFileDetail();
                    sfls[i].DutyRoom = Convert.ToString(dt.Rows[i]["dutyroom"]);
                    sfls[i].Receiver = Convert.ToString(dt.Rows[i]["receiverName"]);
                    sfls[i].ReceiveTime = Convert.ToString(dt.Rows[i]["receive_time"]);
                    sfls[i].FileName = Convert.ToString(dt.Rows[i]["fileName"]);
                    sfls[i].SendTime = Convert.ToString(dt.Rows[i]["sendTime"]);
                }
                return sfls;
            }
        }

        internal static ReceiveFile[] getFilesToReceive(int uid, int type ,int ksxh ,int count)
        {
            string work_no = uid.ToString().PadLeft(4, '0');
            int drid = DAL.getDutyRoomIdByWork_no(work_no);
            if (drid == -1 ) return null;
            DataTable dt = DAL.getFilesToReceive(drid, type);
            if (!dt.TableName.Equals("getFilesToReceive") || (ksxh > dt.Rows.Count)) return null;
            else
            {
                ReceiveFile[] rfs = new ReceiveFile[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
                for (int i = 0; i < rfs.Length; i++)
                {
                    rfs[i] = new ReceiveFile();
                    rfs[i].ID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["fid"]);
                    rfs[i].FileDrId = Convert.ToInt32(dt.Rows[i + ksxh - 1]["id"]);
                    rfs[i].DutyRoomID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["drid"]);
                    rfs[i].DutyRoomDeptId = Convert.ToInt32(dt.Rows[i + ksxh - 1]["dutyroom_bmid"]);
                    rfs[i].DutyRoom = Convert.ToString(dt.Rows[i + ksxh - 1]["dutyroom"]);
                    rfs[i].DutyRoomDept = Convert.ToString(dt.Rows[i + ksxh - 1]["dutyroom_bmmc"]);
                    rfs[i].ExtName = Convert.ToString(dt.Rows[i + ksxh - 1]["extName"]);
                    rfs[i].FileDesc = Convert.ToString(dt.Rows[i + ksxh - 1]["filedesc"]);
                    rfs[i].FileName = Convert.ToString(dt.Rows[i + ksxh - 1]["fileName"]);
                    rfs[i].Receiver = Convert.ToString(dt.Rows[i + ksxh - 1]["receiver"]);
                    rfs[i].ReceiverName = Convert.ToString(dt.Rows[i + ksxh - 1]["receiverName"]);
                    rfs[i].ReceiveTime = Convert.ToString(dt.Rows[i + ksxh - 1]["receive_time"]);
                    rfs[i].Sender = Convert.ToString(dt.Rows[i + ksxh - 1]["sender"]);
                    rfs[i].SenderDept = Convert.ToString(dt.Rows[i + ksxh - 1]["senderDeptName"]);
                    rfs[i].SenderName = Convert.ToString(dt.Rows[i + ksxh - 1]["senderName"]);
                    rfs[i].SendTime = Convert.ToString(dt.Rows[i + ksxh - 1]["sendTime"]);
                    
                }
                return rfs;
            }
          
        }


        internal static INT receiveFile(int fdrid, int uid)
        {
            string work_no = uid.ToString().PadLeft(4, '0');
            return  DAL.receiveFile(fdrid, work_no);

        }

        internal static Department[] getDeptsByDeptId(int did)
        {
            switch (did)
            {
                case 17://新乡
                case 18://新南
                case 19://长北
                case 20://月山
                case 21://安阳
                    return getSendFileDept(did);
                case 10:
                case 11:
                case 12:
                    return getSendFileDept(0);
                default:
                    return null;
            }

        }


        public static Department[] getSendFileDept(int did)
        {
            DataTable dt = DAL.getSendFileDept(did);
            if (dt.TableName.Equals("getSendFileDept") && dt.Rows.Count > 0)
            {
                Department[] depts = new Department[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    depts[i] = new Department();
                    depts[i].ID = Convert.ToInt32(dt.Rows[i]["bm_id"]);
                    depts[i].Name = Convert.ToString(dt.Rows[i]["bm_mc"]);

                }
                return depts;
            }
            else
            {
                return null;
            }

        }

        internal static DutyRoom[] getDutyRoomByDeptId(int did)
        {
            switch (did)
            {
                case 17://新乡
                case 18://新南
                case 19://长北
                case 20://月山
                case 21://安阳
                    return getDutyRooms(did);
                case 10:
                case 11:
                case 12:
                    return getDutyRooms(0);
                default:
                    return null;
            }

        }

        internal static DutyRoom[] getDutyRooms(int did)
        {
            DataTable dt = DAL.getDutyRoomByDeptId(did);
            if (dt.TableName.Equals("getDutyRoomByDeptId") && dt.Rows.Count > 0)
            {
                DutyRoom[] drs = new DutyRoom[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    drs[i] = new DutyRoom();
                    drs[i].ID = Convert.ToInt32(dt.Rows[i]["id"]);
                    drs[i].WeiZhi = Convert.ToString(dt.Rows[i]["position"]);
                    drs[i].CheJianID = Convert.ToInt32(dt.Rows[i]["bm_id"]);
                    drs[i].CheJian = Convert.ToString(dt.Rows[i]["bm_mc"]);

                }
                return drs;
            }
            else
            {
                return null;
            }
        }



        internal static INT SendFile(int senderId, string fileFullName, string fileDesc, string fileContent, string receivers)
        {
            User sender = getUserByNum(senderId.ToString().PadLeft(4, '0'));
            if (sender == null) return new INT(-1, "发送人不存在");
            byte[] file;
            try
            {
                file = Convert.FromBase64String(fileContent);
            }
            catch
            {
                return new INT(-1, "文件内容错误");
            }
            string fileName, extName;
            int dotPos = fileFullName.LastIndexOf('.');
            if (dotPos <= 1) return new INT(-1, "文件名格式错误");
            fileName = fileFullName.Substring(0, dotPos);
            extName = fileFullName.Substring(dotPos +1, fileFullName.Length - dotPos -1 );
            return DAL.SendFile(sender.UserNo, fileName, extName, fileDesc, fileContent, receivers);
            
        }

        internal static INT SendFeedBack(int uid, string txt)
        {
            string work_no = uid.ToString().PadLeft(4, '0');
            return DAL.AddFeedBack(work_no, txt);
        }

        internal static LoginInfo[] GetLoginRecord(int uid, int ksxh, int count)
        {
            DataTable dt = new DataTable();
            string work_no = uid.ToString().PadLeft(4, '0');
            dt = DAL.GetLoginRecord(work_no);

            if (!dt.TableName.Equals("GetLoginRecord") || (ksxh > dt.Rows.Count)) return null;
            LoginInfo[] lis = new LoginInfo[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < lis.Length; i++)
            {
                lis[i] = new LoginInfo();
                lis[i].ID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["id"]);
                lis[i].UserNo = Convert.ToString(dt.Rows[i + ksxh - 1]["uid"]);
                lis[i].DeviceID = Convert.ToString(dt.Rows[i + ksxh - 1]["deviceId"]);
                lis[i].DeviceVersion = Convert.ToString(dt.Rows[i + ksxh - 1]["deviceVer"]);
                lis[i].DeviceInfo = Convert.ToString(dt.Rows[i + ksxh - 1]["deviceInfo"]);
                lis[i].IpAddress = Convert.ToString(dt.Rows[i + ksxh - 1]["IpAddress"]);
                lis[i].LoginTime = Convert.ToString(dt.Rows[i + ksxh - 1]["loginTime"]);
              
            }
            return lis;
        }



        internal static MqttTopic[] getUnsubTopics(int uid)
        {
            DataTable dt = new DataTable();
            string work_no = uid.ToString().PadLeft(4, '0');
            dt = DAL.getUnsubTopics(work_no);

            if (!dt.TableName.Equals("getUnsubTopics")) return null;
            MqttTopic[] lis = new MqttTopic[dt.Rows.Count];
            for (int i = 0; i < lis.Length; i++)
            {
                lis[i] = new MqttTopic();
                lis[i].ID = Convert.ToInt32(dt.Rows[i]["id"]);
                lis[i].Topic = Convert.ToString(dt.Rows[i]["topic"]);
             

            }
            return lis;
        }

        internal static INT setTopicsSubed(string tids)
        {
            return DAL.setTopicsSubed(tids);
        }

        internal static INT setMqttStaus(int uid,int type,string clientId)
        {
            string work_no = uid.toWorkNo();
            return DAL.setMqttStatus(work_no, type, clientId);
        }

        internal static INT getMqttStaus(int uid)
        {
            string work_no = uid.toWorkNo();
            return DAL.getMqttStatus(work_no);
        }

        internal static SystemMessage[] getSystemMessage(int uid, int type,int ksxh,int count)
        {
            DataTable dt = new DataTable();
            string work_no = uid.toWorkNo();
            dt = DAL.getSystemMessage(work_no,type);

            if (!dt.TableName.Equals("getSystemMessage")  || (ksxh > dt.Rows.Count)) return null;
            SystemMessage[] sms = new SystemMessage[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < sms.Length; i++)
            {
                sms[i] = new SystemMessage();
                sms[i].ID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["id"]);
                sms[i].Command = Convert.ToString(dt.Rows[i + ksxh - 1]["Command"]);
                sms[i].Content = Convert.ToString(dt.Rows[i + ksxh - 1]["Content"]);
                sms[i].SendTime = Convert.ToString(dt.Rows[i + ksxh - 1]["createTime"]);
                sms[i].ToUser = Convert.ToString(dt.Rows[i + ksxh - 1]["toUser"]);
                sms[i].HasRead = Convert.ToInt32(dt.Rows[i + ksxh - 1]["hasRead"]);
                sms[i].ReadTime = Convert.ToString(dt.Rows[i + ksxh - 1]["readTime"]);
                sms[i].Type = Convert.ToInt32(dt.Rows[i + ksxh - 1]["type"]);
            }
            return sms;
        }

        internal static INT insertSystemMessage(int uid, int type)
        {
            throw new NotImplementedException();
        }

        internal static INT readSystemMessage(int mid)
        {
            return DAL.readSystemMessage(mid);
        }

        internal static INT readMqttMessage(int uid, int mid)
        {
            string work_no = uid.toWorkNo();
            return DAL.readMqttMessage(work_no, mid);
        }

        internal static string[] getUnReadMqttMessage(int uid)
        {
            string work_no = uid.toWorkNo();
            DataTable dt = DAL.getUnReadMqttMessage(work_no);
            if (dt.TableName != "getUnReadMqttMessage" || dt.Rows.Count == 0) return null;
            string[] json = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                json[i] = dt.Rows[i]["json"].ToString();
            }
            return json;
        }

        #region 安全信息平台
        internal static INT ApplyAqxx(string sender, string auditor, string title, string content, string buMens,string setTime,string lingDaos)
        {
            DateTime time;
            try
            {
                time = Convert.ToDateTime(setTime);
            }
            catch
            {
                return new INT(-1, "时间格式不正确");
            }
            return DAL.ApplyAqxx(sender, auditor, title, content, buMens, time,lingDaos);
        }

        internal static INT AuditAqxx(int xxid, string auditor, int result, string title,string txt)
        {
            return DAL.AuditAqxx(xxid, auditor, result,title,txt);
        }

        internal static AQXX[] getAqxxToAudit(int uid ,int xxid)
        {
            string auditor = uid.toWorkNo();
            DataTable dt = new DataTable();
            dt = DAL.getAqxxToAudit(auditor ,xxid);

            if (!dt.TableName.Equals("getAqxxToAudit")) return null;
            AQXX[] aqxxs = new AQXX[dt.Rows.Count];
            for (int i = 0; i < aqxxs.Length; i++)
            {
                aqxxs[i] = new AQXX();
                aqxxs[i].ID = Convert.ToInt32(dt.Rows[i]["xxid"]);
                aqxxs[i].Sender = Convert.ToString(dt.Rows[i]["sender"]);
                aqxxs[i].Title = Convert.ToString(dt.Rows[i]["title"]);
                aqxxs[i].Content = Convert.ToString(dt.Rows[i]["txt"]);
                aqxxs[i].SendTime = Convert.ToString(dt.Rows[i]["sendTime"]);
                aqxxs[i].SetTime = Convert.ToString(dt.Rows[i]["setTime"]);
                aqxxs[i].SenderNo = Convert.ToString(dt.Rows[i]["sender"]);
                aqxxs[i].SenderName = Convert.ToString(dt.Rows[i]["sender"]);
            }
            return aqxxs;
        }

        internal static User[] getAqxxptShenHe()
        {
            DataTable dt = new DataTable();
            dt = DAL.getAqxxptShenHe();

            if (!dt.TableName.Equals("getAqxxptShenHe")) return null;
            User[] users = new User[dt.Rows.Count];
            for (int i = 0; i < users.Length; i++)
            {
                users[i] = new User();
                users[i].UserNo = Convert.ToString(dt.Rows[i]["user_no"]);
                users[i].UserName = Convert.ToString(dt.Rows[i]["user_name"]);


            }
            return users;
        }

        internal static Department[] getAqxxptBm(int xxid)
        {
            DataTable dt = new DataTable();
            if (xxid == 0)
            {
                dt = DAL.getAqxxptBm();
            }
            else
            {
                dt = DAL.getAqxxptBm(xxid);
            }
            if (!dt.TableName.Equals("getAqxxptBm")) return null;
            Department[] depts = new Department[dt.Rows.Count];
            for (int i = 0; i < depts.Length; i++)
            {
                depts[i] = new Department();
                depts[i].ID = Convert.ToInt32(dt.Rows[i]["bmid"]);
                depts[i].Name = Convert.ToString(dt.Rows[i]["bmmc"]);


            }
            return depts;
        }



        internal static AqxxInfo[] getAqxxInfo(int uid,int xxid)
        {
            User user = getUserByNum(uid.toWorkNo());
            if (user == null)
            {
                AqxxInfo ai = new AqxxInfo();
                ai.Title = "hei";
                return new AqxxInfo[] { ai };
            }

            DataTable dt = new DataTable();
            dt = DAL.getAqxxInfo(user.UserDept, xxid);

            if (!dt.TableName.Equals("getAqxxInfo")) return null;
            AqxxInfo[] ais = new AqxxInfo[dt.Rows.Count];
            for (int i = 0; i < ais.Length; i++)
            {
                ais[i] = new AqxxInfo();
                ais[i].XXID = Convert.ToInt32(dt.Rows[i]["xxid"]);
                ais[i].Title = Convert.ToString(dt.Rows[i]["title"]);
                ais[i].Sender = Convert.ToString(dt.Rows[i]["sender"]);
                ais[i].SendTime = Convert.ToString(dt.Rows[i]["sendtime"]);
                //ais[i].Content = Convert.ToString(dt.Rows[i]["txt"]);
                ais[i].ReadCount = Convert.ToInt32(dt.Rows[i]["readCount"]);
                ais[i].SendCount = Convert.ToInt32(dt.Rows[i]["sendCount"]);
                ais[i].Status = Convert.ToString(dt.Rows[i]["Status"]);
                ais[i].Auditor = Convert.ToString(dt.Rows[i]["Auditor"]);
                ais[i].AuditTime = Convert.ToString(dt.Rows[i]["AuditTime"]);
            }
            return ais;
        }

        internal static int getAqxxCount(int uid)
        {
            User user = getUserByNum(uid.toWorkNo());
            if (user == null)
            {
                return -1;
            }

            DataTable dt = new DataTable();
            dt = DAL.getAqxxInfo(user.UserDept, 0);

            return dt.Rows.Count;
        }

        internal static AqxxInfo[] getAqxxInfos(int uid, int ksxh,int count)
        {
            User user = getUserByNum(uid.toWorkNo());
            if (user == null)
            {
                AqxxInfo ai = new AqxxInfo();
                ai.Title = "hei";
                return new AqxxInfo[] { ai };
            }

            DataTable dt = new DataTable();
            dt = DAL.getAqxxInfo(user.UserDept, 0);

            if (!dt.TableName.Equals("getAqxxInfo")) return null;
            AqxxInfo[] ais = new AqxxInfo[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < ais.Length; i++)
            {
                ais[i] = new AqxxInfo();
                ais[i].XXID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["xxid"]);
                ais[i].Title = Convert.ToString(dt.Rows[i + ksxh - 1]["title"]);
                ais[i].Sender = Convert.ToString(dt.Rows[i + ksxh - 1]["sender"]);
                ais[i].SendTime = Convert.ToString(dt.Rows[i + ksxh - 1]["sendtime"]);
                //ais[i].Content = Convert.ToString(dt.Rows[i+ ksxh - 1]["txt"]);
                ais[i].ReadCount = Convert.ToInt32(dt.Rows[i + ksxh - 1]["readCount"]);
                ais[i].SendCount = Convert.ToInt32(dt.Rows[i + ksxh - 1]["sendCount"]);
                ais[i].Status = Convert.ToString(dt.Rows[i + ksxh - 1]["Status"]);
                ais[i].Auditor = Convert.ToString(dt.Rows[i + ksxh - 1]["Auditor"]);
                ais[i].AuditTime = Convert.ToString(dt.Rows[i + ksxh - 1]["AuditTime"]);
            }
            return ais;
        }

        internal static AqxxDetail[] getAqxxDetail(int xxid ,int ksxh ,int count)
        {
            DataTable dt = new DataTable();
            dt = DAL.getAqxxDetail(xxid);

            if (!dt.TableName.Equals("getAqxxDetail") || (ksxh > dt.Rows.Count)) return null;
            AqxxDetail[] depts = new AqxxDetail[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < depts.Length; i++)
            {
                depts[i] = new AqxxDetail();
                depts[i].Title = Convert.ToString(dt.Rows[i + ksxh - 1]["title"]);
                depts[i].Sender = Convert.ToString(dt.Rows[i + ksxh - 1]["sender"]);
                depts[i].Receiver = Convert.ToString(dt.Rows[i + ksxh - 1]["receiver"]);
                depts[i].ReceiveTime = Convert.ToString(dt.Rows[i + ksxh - 1]["receiveTime"]);
                depts[i].SendTime = Convert.ToString(dt.Rows[i + ksxh - 1]["sendTime"]);
                depts[i].ReceiverDept = Convert.ToString(dt.Rows[i + ksxh - 1]["receiverDept"]);
            }
            return depts;
        }


        internal static AQXX[] getAqxxContent(int xxid)
        {

            DataTable dt = new DataTable();
            dt = DAL.getAqxxContent(xxid);

            if (!dt.TableName.Equals("getAqxxContent")) return null;
            AQXX[] aqxxs = new AQXX[dt.Rows.Count];
            for (int i = 0; i < aqxxs.Length; i++)
            {
                aqxxs[i] = new AQXX();
                aqxxs[i].ID = Convert.ToInt32(dt.Rows[i]["xxid"]);
                aqxxs[i].Sender = Convert.ToString(dt.Rows[i]["sender"]);
                aqxxs[i].Title = Convert.ToString(dt.Rows[i]["title"]);
                aqxxs[i].Content = Convert.ToString(dt.Rows[i]["txt"]);
                aqxxs[i].SendTime = Convert.ToString(dt.Rows[i]["sendTime"]);
                aqxxs[i].SetTime = Convert.ToString(dt.Rows[i]["setTime"]);
                aqxxs[i].SenderNo = Convert.ToString(dt.Rows[i]["sender"]);
                aqxxs[i].SenderName = Convert.ToString(dt.Rows[i]["sender"]);
            }
            return aqxxs;
        }
        #endregion


    }
}