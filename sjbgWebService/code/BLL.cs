﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using sjbgWebService.gwxx;
using sjbgWebService.xwxx;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Web.Script.Serialization;
using sjbgWebService.youjian;

namespace sjbgWebService
{
    public static class BLL
    {

        public static MqttMessageType ToMqttMessageType(this int type)
        {
            switch (type)
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

        public static string ToListString(this int[] i)
        {
            if (i == null || i.Length == 0) return string.Empty;
            string str = "";
            for (int j = 0; j < i.Length; j++)
            {
                str += i[j].ToString() + ",";
            }
            str = str.Substring(0, str.Length - 1);
            return str;
        }

        public static string ToListString(this string[] i)
        {
            if (i == null || i.Length == 0) return string.Empty;
            string str = "";
            for (int j = 0; j < i.Length; j++)
            {
                str += i[j] + ",";
            }
            str = str.Substring(0, str.Length - 1);
            return str;
        }

        public static string[] ToStringList(this string str, string[] separator)
        {
            if (str.IndexOf(",", StringComparison.Ordinal) > 0)
            {
                return str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                return new string[0];
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
            string md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5");
            return md5?.ToUpper();
        }

        public static string ToMD5String(this int i)
        {
            string s = i.ToString();
            return s.ToMD5String();
        }
        public static bool IsValidUserNo(this string str)
        {
            return true;
        }
        public static bool IsValidPass(this string str)
        {
            return true;
        }
        public static bool IsValidEmail(this string str)
        {
            return true;
        }
        public static bool IsValidMobile(this string str)
        {
            return true;
        }
        public static bool IsValidString(this string str)
        {
            return true;
        }

        /// <summary>
        /// 设置用户密码
        /// </summary>
        /// <param name="gh">工号</param>
        /// <param name="pass">密码</param>
        /// <returns></returns>
        public static string SetEncryptPass(string gh, string pass)
        {
            //以工号为salt，简单的用(工号+密码)的md5值存入数据库
            return ToMD5String(gh + pass);
        }

        public static string ToWorkNo(this int uid)
        {
            if (uid > 9999 && uid < 100000)
            {
                return uid.ToString();
            }
            else
            {
                int length = 4;
                if (uid <= 0 || uid >= Math.Pow(10, length)) throw new ArgumentOutOfRangeException("uid", "工号只能在0001-9999中间");
                return uid.ToString().PadLeft(length, '0');
            }
        }


        public static List<System.Net.Mail.MailAddress> ToSysMailAddress(string str)
        {
            if (str.Contains(SjbgConfig.FuHaoKaiShi) == false) return null;
            string[] strYjdzs = str.Split(new string[] { SjbgConfig.FuHaoYouJianDiZhi }, StringSplitOptions.RemoveEmptyEntries);
            List<System.Net.Mail.MailAddress> ma = new List<System.Net.Mail.MailAddress>();
            for (int i = 0; i < strYjdzs.Length; i++)
            {
                string address = strYjdzs[i].Substring(strYjdzs[i].IndexOf(SjbgConfig.FuHaoKaiShi) + SjbgConfig.FuHaoKaiShi.Length, strYjdzs[i].IndexOf(SjbgConfig.FuHaoFenGe) - strYjdzs[i].IndexOf(SjbgConfig.FuHaoKaiShi) - SjbgConfig.FuHaoKaiShi.Length);
                string displayname = strYjdzs[i].Substring(strYjdzs[i].IndexOf(SjbgConfig.FuHaoFenGe) + SjbgConfig.FuHaoFenGe.Length, strYjdzs[i].Length - SjbgConfig.FuHaoFenGe.Length - strYjdzs[i].IndexOf(SjbgConfig.FuHaoFenGe));
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
                    atts.Add(new System.Net.Mail.Attachment(new MemoryStream(Convert.FromBase64String(fj.Base64Code)), fj.FileName));
                }
            }
            return atts;
        }



        public static GongWen GetGongWenByWh(string wh)
        {

            GongWen gw = new GongWen();
            DataTable dt = DAL.GetGwxxByWh(wh);
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

        public static UserGw GetUserGwByUid(int uid)
        {
            DataTable dt = DAL.GetUserGwByUid(uid);
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
                u.Ssxzbm = DAL.GetSsxzbmBySsbm(u.Ssbm);

            }
            return u;
        }
        public static UserGw GetUserGwByUserName(string userName)
        {

            DataTable dt = DAL.GetUserGwByUserName(userName);
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
                u.Ssxzbm = DAL.GetSsxzbmBySsbm(u.Ssbm);
            }
            return u;
        }

        public static BOOLEAN SignGw(string wh, int gh, string insStr, string nextUids)
        {
            int uid;
            try
            {
                uid = Convert.ToInt32(DAL.GetProductUserIdByBaseNum(gh.ToString().PadLeft(4, '0'), 2));
            }
            catch (Exception ex)
            {
                return new BOOLEAN(false, ex.Message);
            }
            GongWen gw = GetGongWenByWh(wh);
            if (gw.Id == 0) return new BOOLEAN(false, "");
            UserGw user = GetUserGwByUid(uid);
            if (user.Yhmc.Equals("")) return new BOOLEAN(false, "");
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
                return DAL.LeaderSign(gw, user, ins, nextUser);
            }
            else//中层签收
            {
                return DAL.Sign(gw, user);
            }

        }

        public static BOOLEAN IsSigned(string wh, int gh)
        {
            int uid;
            try
            {
                uid = Convert.ToInt32(DAL.GetProductUserIdByBaseNum(gh.ToString().PadLeft(4, '0'), 2));
            }
            catch
            {
                return new BOOLEAN(true, "");
            }
            GongWen gw = GetGongWenByWh(wh);
            UserGw user = GetUserGwByUid(uid);
            return DAL.IsSigned(gw, user);
        }

        public static Instruction[] GetLdps(string wh)
        {
            GongWen gw = GetGongWenByWh(wh);
            DataTable dt = DAL.GetLdps(gw);

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



        public static GongWen[] GetGwlb(int gh, int lblx, gwlx gwlx, dwlx dwlx, int ksxh, int count)
        {
            int uid;
            try
            {
                uid = Convert.ToInt32(DAL.GetProductUserIdByBaseNum(gh.ToString().PadLeft(4, '0'), 2));
            }
            catch
            {
                return null;
            }
            UserGw user = GetUserGwByUid(uid);
            DataTable dt = new DataTable();
            if (lblx == 1)//所有文件列表
            {
                dt = DAL.GetAllGwlb(user, gwlx, dwlx);
            }
            else//未批阅或签收文件列表
            {
                dt = DAL.GetUnfinishedGwlb(user, dwlx);
            }
            if (ksxh > dt.Rows.Count) return null;
            GongWen[] gws = new GongWen[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
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

        public static string MakeGwString(GongWen gw)
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

        public static User GetUserByNum(string userNo)
        {
            DataTable dt = DAL.GetUserByNum(userNo);
            User u = new User();
            if (dt.TableName.Equals("user"))
            {
                u.Uid = Convert.ToInt32(dt.Rows[0]["user_no"]);
                u.UserNo = Convert.ToString(dt.Rows[0]["user_no"]);
                u.UserName = Convert.ToString(dt.Rows[0]["user_name"]);
                u.UserDept = Convert.ToInt32(dt.Rows[0]["bumen_id"]);
                u.GwLevel = GetGwLevel(u.Uid).Number;
                u.TqLevel = GetTqLevel(u.Uid).Number;
                GongWenYongHu gwyh = GetGongWenYongHuByUid(u.Uid);
                if (gwyh == null)
                {
                    u.GwRoleID = 0;
                }
                else
                {
                    u.GwRoleID = gwyh.RoleID;
                }
            }
            return u;

        }



        public static INT Login(string userNo, string userPass, string code, string ip, string deviceInfo, string deviceVersion)
        {
            if (!IsValidPass(userPass)) return new INT(0, "工号或密码不正确");
            if (!IsValidUserNo(userNo)) return new INT(0, "工号或密码不正确");
            return DAL.Login(userNo, userPass, code, ip, deviceInfo, deviceVersion);
        }

        public static INT LoginDirect(string userNo, string userPass, string code, string ip, string deviceInfo, string deviceVersion)
        {
            if (!IsValidPass(userPass)) return new INT(0, "工号或密码不正确");
            if (!IsValidUserNo(userNo)) return new INT(0, "工号或密码不正确");
            return DAL.LoginDirect(userNo, userPass, code, ip, deviceInfo, deviceVersion);
        }

        public static XinWen[] GetXinWen(int xwlx, int ksxh, int count)
        {
            DataTable dt = DAL.GetXwlb(xwlx);
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

        public static XinWenLeiXing[] GetXinWenLeiXing()
        {
            DataTable dt = DAL.GetXwlx();
            XinWenLeiXing[] xwlx = new XinWenLeiXing[dt.Rows.Count];
            for (int i = 0; i < xwlx.Length; i++)
            {
                xwlx[i] = new XinWenLeiXing();
                xwlx[i].ID = Convert.ToInt32(dt.Rows[i]["id"]);
                xwlx[i].Type = Convert.ToString(dt.Rows[i]["sclb"]);
            }
            return xwlx;
        }

        public static Product GetProductByPname(string pname)
        {
            DataTable dt = DAL.GetProductByPid(pname);
            Product p = new Product();
            if (dt.TableName.Equals("product"))
            {
                p.Pid = Convert.ToInt32(dt.Rows[0]["xt_id"]);
                p.PName = Convert.ToString(dt.Rows[0]["xt_mc"]);
                p.ServicePage = Convert.ToString(dt.Rows[0]["webServiceURL"]);
            }
            return p;
        }

        public static ZbPerson[] GetZbPersons(DateTime dateTime, int isNight)
        {
            DataTable dt = DAL.GetZbPerson(dateTime, isNight);
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
        public static string GetZbLdps(DateTime dateTime)
        {
            return DAL.GetZbLdps(dateTime);
        }

        internal static UserGw[] GetLeaderList()
        {
            DataTable dt = DAL.GetLeaderList();
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
                    users[i].Ssxzbm = DAL.GetSsxzbmBySsbm(users[i].Ssbm);

                }
                return users;
            }
            else
            {
                return null;
            }
        }

        internal static BuMenGw[] GetBmList(int lbid)
        {
            DataTable dt = DAL.GetGwbmList(lbid);
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

        internal static BuMenLeiBie[] GetBmlbList()
        {
            DataTable dt = DAL.GetGwbmlbList();
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



        internal static string[] GetJianBaoBuMen()
        {
            DataTable dt = DAL.GetJianBaoBuMen();
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

        internal static JianBao[] GetJianBao(string dept, DateTime dateTime)
        {
            DataTable dt = DAL.GetJianBao(dept, dateTime);
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



        internal static JianBao[] GetAllJianBao(DateTime dateTime)
        {
            DataTable dt = DAL.GetJianBao("all", dateTime);
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



        internal static ApkInfo GetApkInfo()
        {
            return new ApkInfo(SjbgConfig.ApkVerCode, SjbgConfig.ApkVerName, SjbgConfig.ApkFilePath, SjbgConfig.ApkFileName, SjbgConfig.ApkUpdateContent);
        }



        public static INT GetTqUidByWorkNo(int workno)
        {
            string workNo = workno.ToString().PadLeft(4, '0');
            return DAL.GetTqUidByWorkNo(workNo);
        }

        public static INT GetTqUtypeByUid(int uid)
        {
            return DAL.GetTqUtypeByUid(uid);
        }
        public static INT GetTqUDeptByUid(int uid)
        {
            return DAL.GetTqUDeptByUid(uid);
        }

        internal static TeQing[] GetTeQingByWorkNo(int workno, int ksxh, int count)
        {
            int uid = GetTqUidByWorkNo(workno).Number;
            DataTable dt = DAL.GetTeQingByUid(uid);
            if (dt == null || dt.Rows.Count == 0 || !dt.TableName.Equals("getTeQingByUid")) return null;
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



        internal static BOOLEAN ReplyTq(int workno, int tid, string replayContent)
        {
            int uid = GetTqUidByWorkNo(workno).Number;
            return DAL.ReplyTq(uid, tid, replayContent);
        }

        internal static TeQing[] CheckReply(int senderno, int ksxh, int count)
        {
            int uid = GetTqUidByWorkNo(senderno).Number;
            DataTable dt = DAL.CheckReply(uid);
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

        internal static TeQing[] CheckReplyDetails(int tid, int ksxh, int count)
        {
            DataTable dt = DAL.CheckReplyDetails(tid);
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


        internal static INT GetTqLevel(int workno)
        {
            int tquid = GetTqUidByWorkNo(workno).Number;
            return GetTqUtypeByUid(tquid);
        }

        internal static INT GetGwLevel(int workno)
        {
            int uid;
            try
            {
                uid = Convert.ToInt32(DAL.GetProductUserIdByBaseNum(workno.ToString().PadLeft(4, '0'), 2));
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            return new INT(GetUserGwByUid(uid).Yhqx, "");
        }



        internal static INT RegisterDevice(RegisterInfo ri)
        {
            INT registerCode = DAL.CheckUserMobile(ri.WorkNo, ri.Mobile, ri.UniqueCode);
            if (registerCode.Number == 0)//判断该设备是否已经注册
            {
                return DAL.RegisterDevice(ri.WorkNo, ri.Mobile, ri.UniqueCode, ri.RegisterCode, ri.SecurityQuestion, ri.SecurityAnswer, ri.EmailAddress);
            }
            else if (registerCode.Number == 1) //设备已注册
            {
                return new INT(-99, "该设备已经注册");
            }
            else
            {
                return registerCode;
            }
        }


        internal static INT RequestRegisterCode(RegisterInfo ri)
        {
            string code = GenerateRegisterCode(ri.WorkNo, ri.Mobile);
            INT i = DAL.CheckUserMobile(ri.WorkNo, ri.Mobile, ri.UniqueCode);
            if (i.Number < 0) return i;
            i = DAL.InsertRegisterCode(ri.WorkNo, ri.Mobile, code, ri.UniqueCode);
            if (i.Number == 1)
            {
                return DAL.SendMobileMessage(ri.WorkNo, MakeRegisterMobileMessageContent(code));

            }
            else
            {
                return i;
            }
        }
        static string MakeRegisterMobileMessageContent(string code)
        {
            string str = "您的手机办公系统注册验证码是：" + code + "，请在2分钟内完成注册，如非本人操作请忽略。";
            return str;
        }

        static string GenerateRegisterCode(int workNo, string mobile)
        {
            string strSeed = workNo.ToString() + mobile + Convert.ToString(DateTime.Now.Ticks);
            strSeed = ToMD5String(strSeed);
            strSeed = strSeed.Substring(0, 2) + strSeed.Substring(8, 2) + strSeed.Substring(16, 2) + strSeed.Substring(24, 2);
            uint uSeed = uint.Parse(strSeed, System.Globalization.NumberStyles.HexNumber);
            int iSeed;
            iSeed = Convert.ToInt32(uSeed / 2);
            Random r = new Random(iSeed);
            strSeed = r.Next(1, 999999).ToString().PadLeft(6, '0');
            return strSeed;
        }

        internal static GpsData[] GetGpsByNum(string gpsData)
        {
            DataTable dt = DAL.GetGpsByNum(gpsData);
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

        internal static UserRole[] GetUserRoleByNum(int uid)
        {
            string workNo = uid.ToString().PadLeft(4, '0');
            DataTable dt = DAL.GetUserRole(workNo);
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

        internal static MenuItem[] GetUserMenuByNum(int uid)
        {
            string workNo = uid.ToString().PadLeft(4, '0');
            DataTable dt = DAL.GetUserMenu(workNo);
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
        internal static JcjhData[] GetJcjhByNum(string jcjhData)
        {
            DataTable dt = DAL.GetJcjhByNum(jcjhData);
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
        internal static RyjhData[] GetRyjhByNum(string ryjhData)
        {
            DataTable dt = DAL.GetRyjhByNum(ryjhData);
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
        internal static DcjhData[] GetDcjhByNum(string dcjhData)
        {
            DataTable dt = DAL.GetDcjhByNum(dcjhData);
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
        internal static CjcxData[] GetCjcxByNum(string cjcxData)
        {
            DataTable dt = DAL.GetCjcxByNum(cjcxData);
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

        internal static BOOLEAN SetNewPass(int uid, string opass, string npass)
        {
            string workNo = uid.ToString().PadLeft(4, '0');
            return DAL.SetNewPass(workNo, opass, npass);

        }

        internal static MingPai[] GetMingPaiByXianBie(int database, string lineMode, int type)
        {
            DataTable dt = new DataTable();
            dt = DAL.GetMingPaiByXianBie(database, lineMode, type);
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

        internal static MingPai[] GetMingPaiByUid(int database, int uid)
        {
            DataTable dt = new DataTable();
            string workNo = uid.ToString().PadLeft(4, '0');
            dt = DAL.GetMingPaiByGongHao(database, workNo);
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

        internal static XianBie[] GetXianBie(int database)
        {
            DataTable dt = new DataTable();
            dt = DAL.GetXianBie(database);
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

        internal static DaMingPai[] GetDaMingPaiByXianBie(int database, string line, int type, string filter, int ksxh, int count)
        {
            DataTable dt = new DataTable();

            dt = DAL.GetDaMingPai(database, line, type, filter);

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

        internal static CanBu[] GetCanBu(int uid, string month)
        {
            string workNo = uid.ToString().PadLeft(4, '0');
            string strJssj = month + "-25 18:00:00";
            DateTime jssj = Convert.ToDateTime(strJssj);
            DateTime kssj = jssj.AddMonths(-1);
            DataTable dt = DAL.GetCanBu(workNo, kssj, jssj);
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
                    cbs[i].ShiChang = Math.Round(Convert.ToDouble(dt.Rows[i]["shichang"]), 2).ToString();
                }
                return cbs;
            }
            else
            {
                return null;
            }
        }

        static string FyySjzh(string sj)
        {
            string xsj = "";
            if (sj == null || sj == "") return "";
            if (sj.Length != 12) return "";
            xsj = sj.Substring(0, 4);
            xsj += "-" + sj.Substring(4, 2);
            xsj += "-" + sj.Substring(6, 2);
            xsj += " " + sj.Substring(8, 2);
            xsj += ":" + sj.Substring(10, 2);
            return xsj;
        }

        internal static FeiYunYongJiChe[] GetFyyjc(string jczt)
        {

            DataTable dt = DAL.GetFyyjc(jczt);
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
                    fyys[i].ZhuanRuShiJian = FyySjzh(Convert.ToString(dt.Rows[i]["zrsj"]));
                    fyys[i].ZhuanChuShiJian = FyySjzh(Convert.ToString(dt.Rows[i]["zcsj"]));
                    fyys[i].GongZuoShiJian = Convert.ToString(dt.Rows[i]["gzsj"]);

                }
                return fyys;
            }
            else
            {
                return null;
            }
        }

        internal static string[] GetFyyjcZt()
        {
            DataTable dt = DAL.GetFyyjcZt();
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
        internal static BookNameData[] GetBookName(string booknameData)
        {
            DataTable dt = DAL.GetBookName(booknameData);
            if (dt.TableName.Equals("BookNameRead") && dt.Rows.Count > 0)
            {
                BookNameData[] bookName = new BookNameData[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bookName[i] = new BookNameData();
                    bookName[i].id = Convert.ToString(dt.Rows[i]["id"]);
                    bookName[i].Name = Convert.ToString(dt.Rows[i]["Name"]);
                    bookName[i].Address = Convert.ToString(dt.Rows[i]["Address"]);
                }
                return bookName;
            }
            else
            {
                return null;
            }


        }

        //电子书内容查询处理数据
        internal static BookNrData GetBookNr(string bookNrId)
        {
            DataTable dt = DAL.GetBookNr(bookNrId);
            if (dt.TableName.Equals("BookNrRead"))
            {
                BookNrData bookNr = new BookNrData();


                bookNr.id = Convert.ToString(dt.Rows[0]["id"]);
                bookNr.Txt = Convert.ToString(dt.Rows[0]["Txt"]);


                return bookNr;
            }
            else
            {
                return null;
            }


        }

        internal static SentFileList[] GetSentFiles(int uid)
        {
            string workNo = uid.ToString().PadLeft(4, '0');
            DataTable dt = DAL.GetSentFiles(workNo);
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


        internal static SentFileDetail[] GetSentFileDetails(int fid)
        {
            DataTable dt = DAL.GetSentFileDetails(fid);
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

        internal static ReceiveFile[] GetFilesToReceive(int uid, int type, int ksxh, int count)
        {
            string workNo = uid.ToString().PadLeft(4, '0');
            int drid = DAL.getDutyRoomIdByWork_no(workNo);
            if (drid == -1) return null;
            DataTable dt = DAL.GetFilesToReceive(drid, type);
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


        internal static INT ReceiveFile(int fdrid, int uid)
        {
            string workNo = uid.ToString().PadLeft(4, '0');
            return DAL.ReceiveFile(fdrid, workNo);

        }

        internal static Department[] GetDeptsByDeptId(int did)
        {
            switch (did)
            {
                case 17://新乡
                case 18://新南
                case 19://长北
                case 20://月山
                case 21://安阳
                    return GetSendFileDept(did);
                case 10:
                case 11:
                case 12:
                    return GetSendFileDept(0);
                default:
                    return null;
            }

        }


        public static Department[] GetSendFileDept(int did)
        {
            DataTable dt = DAL.GetSendFileDept(did);
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

        internal static DutyRoom[] GetDutyRoomByDeptId(int did)
        {
            switch (did)
            {
                case 17://新乡
                case 18://新南
                case 19://长北
                case 20://月山
                case 21://安阳
                    return GetDutyRooms(did);
                case 10:
                case 11:
                case 12:
                    return GetDutyRooms(0);
                default:
                    return null;
            }

        }

        internal static DutyRoom[] GetDutyRooms(int did)
        {
            DataTable dt = DAL.GetDutyRoomByDeptId(did);
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
            User sender = GetUserByNum(senderId.ToString().PadLeft(4, '0'));
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
            extName = fileFullName.Substring(dotPos + 1, fileFullName.Length - dotPos - 1);
            return DAL.SendFile(sender.UserNo, fileName, extName, fileDesc, fileContent, receivers);

        }

        internal static INT SendFeedBack(int uid, string txt)
        {
            string workNo = uid.ToString().PadLeft(4, '0');
            return DAL.AddFeedBack(workNo, txt);
        }

        internal static LoginInfo[] GetLoginRecord(int uid, int ksxh, int count)
        {
            DataTable dt = new DataTable();
            string workNo = uid.ToString().PadLeft(4, '0');
            dt = DAL.GetLoginRecord(workNo);

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



        internal static MqttTopic[] GetUnsubTopics(int uid)
        {
            DataTable dt = new DataTable();
            string workNo = uid.ToString().PadLeft(4, '0');
            dt = DAL.GetUnsubTopics(workNo);

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

        internal static INT SetTopicsSubed(string tids)
        {
            return DAL.SetTopicsSubed(tids);
        }

        internal static INT SetMqttStaus(int uid, int type, string clientId)
        {
            string workNo = uid.ToWorkNo();
            return DAL.SetMqttStatus(workNo, type, clientId);
        }

        internal static INT GetMqttStaus(int uid)
        {
            string workNo = uid.ToWorkNo();
            return DAL.GetMqttStatus(workNo);
        }

        internal static SystemMessage[] GetSystemMessage(int uid, int type, int ksxh, int count)
        {
            DataTable dt = new DataTable();
            string workNo = uid.ToWorkNo();
            dt = DAL.GetSystemMessage(workNo, type);

            if (!dt.TableName.Equals("getSystemMessage") || (ksxh > dt.Rows.Count)) return null;
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

        internal static INT InsertSystemMessage(int uid, int type)
        {
            throw new NotImplementedException();
        }

        internal static INT ReadSystemMessage(int mid)
        {
            return DAL.ReadSystemMessage(mid);
        }

        internal static INT ReadMqttMessage(int uid, int mid)
        {
            string workNo = uid.ToWorkNo();
            return DAL.ReadMqttMessage(workNo, mid);
        }

        internal static string[] GetUnReadMqttMessage(int uid)
        {
            string workNo = uid.ToWorkNo();
            DataTable dt = DAL.GetUnReadMqttMessage(workNo);
            if (dt.TableName != "getUnReadMqttMessage" || dt.Rows.Count == 0) return null;
            string[] json = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                json[i] = dt.Rows[i]["json"].ToString();
            }
            return json;
        }

        #region 安全信息平台
        internal static INT ApplyAqxx(string sender, string auditor, string title, string content, string buMens, string setTime, string lingDaos)
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
            return DAL.ApplyAqxx(sender, auditor, title, content, buMens, time, lingDaos);
        }

        internal static INT AuditAqxx(int xxid, string auditor, int result, string title, string txt)
        {
            return DAL.AuditAqxx(xxid, auditor, result, title, txt);
        }

        internal static AQXX[] GetAqxxToAudit(int uid, int xxid)
        {
            string auditor = uid.ToWorkNo();
            DataTable dt = new DataTable();
            dt = DAL.GetAqxxToAudit(auditor, xxid);

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

        internal static User[] GetAqxxptShenHe()
        {
            DataTable dt = new DataTable();
            dt = DAL.GetAqxxptShenHe();

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

        internal static Department[] GetAqxxptBm(int xxid)
        {
            DataTable dt = new DataTable();
            if (xxid == 0)
            {
                dt = DAL.GetAqxxptBm();
            }
            else
            {
                dt = DAL.GetAqxxptBm(xxid);
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



        internal static AqxxInfo[] GetAqxxInfo(int uid, int xxid)
        {
            User user = GetUserByNum(uid.ToWorkNo());
            if (user == null)
            {
                AqxxInfo ai = new AqxxInfo();
                ai.Title = "hei";
                return new AqxxInfo[] { ai };
            }

            DataTable dt = new DataTable();
            dt = DAL.GetAqxxInfo(user.UserDept, xxid);

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

        internal static int GetAqxxCount(int uid)
        {
            User user = GetUserByNum(uid.ToWorkNo());
            if (user == null)
            {
                return -1;
            }

            DataTable dt = new DataTable();
            dt = DAL.GetAqxxInfo(user.UserDept, 0);

            return dt.Rows.Count;
        }

        internal static AqxxInfo[] GetAqxxInfos(int uid, int ksxh, int count)
        {
            User user = GetUserByNum(uid.ToWorkNo());
            if (user == null)
            {
                AqxxInfo ai = new AqxxInfo();
                ai.Title = "hei";
                return new AqxxInfo[] { ai };
            }

            DataTable dt = DAL.GetAqxxInfo(user.UserDept, 0);

            if (!dt.TableName.Equals("getAqxxInfo")) return null;
            AqxxInfo[] ais = new AqxxInfo[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < ais.Length; i++)
            {
                ais[i] = new AqxxInfo
                {

                    XXID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["xxid"]),
                    Title = Convert.ToString(dt.Rows[i + ksxh - 1]["title"]),
                    Sender = Convert.ToString(dt.Rows[i + ksxh - 1]["sender"]),
                    SendTime = Convert.ToString(dt.Rows[i + ksxh - 1]["sendtime"]),
                    ReadCount = Convert.ToInt32(dt.Rows[i + ksxh - 1]["readCount"]),
                    SendCount = Convert.ToInt32(dt.Rows[i + ksxh - 1]["sendCount"]),
                    Status = Convert.ToString(dt.Rows[i + ksxh - 1]["Status"]),
                    Auditor = Convert.ToString(dt.Rows[i + ksxh - 1]["Auditor"]),
                    AuditTime = Convert.ToString(dt.Rows[i + ksxh - 1]["AuditTime"]),
                    SendingCount = Convert.ToInt32(dt.Rows[i + ksxh - 1]["sendingCount"]),
                    FailCount = Convert.ToInt32(dt.Rows[i + ksxh - 1]["failCount"])
                };
                //ais[i].Content = Convert.ToString(dt.Rows[i+ ksxh - 1]["txt"]);
            }
            return ais;
        }

        internal static AqxxDetail[] GetAqxxDetail(int xxid, int type, int ksxh, int count)
        {
            DataTable dt = DAL.GetAqxxDetail(xxid, type);

            if (!dt.TableName.Equals("getAqxxDetail") || (ksxh > dt.Rows.Count)) return null;
            AqxxDetail[] ads = new AqxxDetail[ksxh + count < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < ads.Length; i++)
            {
                ads[i] = new AqxxDetail
                {
                    Title = Convert.ToString(dt.Rows[i + ksxh - 1]["title"]),
                    Sender = Convert.ToString(dt.Rows[i + ksxh - 1]["sender"]),
                    Receiver = Convert.ToString(dt.Rows[i + ksxh - 1]["receiver"]),
                    ReceiveTime = Convert.ToString(dt.Rows[i + ksxh - 1]["receiveTime"]),
                    SendTime = Convert.ToString(dt.Rows[i + ksxh - 1]["sendTime"]),
                    ReceiverDept = Convert.ToString(dt.Rows[i + ksxh - 1]["receiverDept"]),
                    Status = Convert.ToString(dt.Rows[i + ksxh - 1]["status"])
                };
            }
            return ads;
        }


        internal static AQXX[] GetAqxxContent(int xxid)
        {

            DataTable dt = new DataTable();
            dt = DAL.GetAqxxContent(xxid);

            if (!dt.TableName.Equals("getAqxxContent")) return null;
            AQXX[] aqxxs = new AQXX[dt.Rows.Count];
            for (int i = 0; i < aqxxs.Length; i++)
            {
                aqxxs[i] = new AQXX
                {
                    ID = Convert.ToInt32(dt.Rows[i]["xxid"]),
                    Sender = Convert.ToString(dt.Rows[i]["sender"]),
                    Title = Convert.ToString(dt.Rows[i]["title"]),
                    Content = Convert.ToString(dt.Rows[i]["txt"]),
                    SendTime = Convert.ToString(dt.Rows[i]["sendTime"]),
                    SetTime = Convert.ToString(dt.Rows[i]["setTime"]),
                    SenderNo = Convert.ToString(dt.Rows[i]["sender"]),
                    SenderName = Convert.ToString(dt.Rows[i]["sender"])
                };
            }
            return aqxxs;
        }
        #endregion


        #region 2016新版公文流转系统

        internal static bool IsBanZiChengYuanFinished(int gwid)
        {
            int count = DAL.UnfinishedBanZiChengYuanRenShu(gwid);
            if (count > 0) return false;
            else return true;
        }
        internal static GongWenList[] GetGongWenList(int uid, string fsr, int xzid, int lxid, string keyWord, string sTime, string eTime, int gwtype, int ksxh, int count)
        {

            //调用数据操作层的函数获取公文列表DataTable
            string jsr = uid.ToWorkNo();
            DataTable dt = DAL.GetGongWenList(jsr, fsr, xzid, lxid, keyWord, sTime, eTime, gwtype);

            //如果获取数据过程错误，返回null
            if (dt.TableName.Equals("error!"))
            {
                return null;
            }
            else
            {
                //将DataTable转换为GongWenList
                GongWenList[] gwlist = new GongWenList[ksxh + count - 1 < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
                for (int i = 0; i < gwlist.Length; i++)
                {
                    gwlist[i] = new GongWenList();
                    gwlist[i].GongWenID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["gwid"]);
                    gwlist[i].LiuZhuanID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["lzID"]);
                    gwlist[i].XuHao = i + 1;
                    gwlist[i].BiaoTi = Convert.ToString(dt.Rows[i + ksxh - 1]["bt"]);
                    gwlist[i].WenHao = Convert.ToString(dt.Rows[i + ksxh - 1]["wh"]);
                    gwlist[i].FaSongRen = Convert.ToString(dt.Rows[i + ksxh - 1]["fsrxm"]);
                    gwlist[i].FaSongShiJian = Convert.ToDateTime(dt.Rows[i + ksxh - 1]["fssj"]).ToString("yyyy-MM-dd HH:mm:ss");
                    gwlist[i].JinJi = Convert.ToString(dt.Rows[i + ksxh - 1]["jinji"]);
                    gwlist[i].ShiFouCheXiao = Convert.ToInt32(dt.Rows[i + ksxh - 1]["chexiao"]);
                    gwlist[i].WenJianLeiXing = Convert.ToString(dt.Rows[i + ksxh - 1]["wjlx"]);
                    string qssj = Convert.ToString(dt.Rows[i + ksxh - 1]["qssj"]);
                    int fsrRid = Convert.ToInt32(dt.Rows[i + ksxh - 1]["fsr_rid"]);
                    int jsrRid = Convert.ToInt32(dt.Rows[i + ksxh - 1]["jsr_rid"]);

                    if (string.IsNullOrEmpty(qssj))//签收时间为空
                    {
                        gwlist[i].ShiFouQianShou = 0;
                        gwlist[i].QianShouQingKuang = "未签收";
                        //if (jsrRid ==23 || jsrRid == 24)//中层能否签收首先看班子成员是否都已经完成签收
                        ////if (fsr_rid == 21 && (jsr_rid == 23 || jsr_rid == 24))//并且是段长书记直接发给中层的
                        //{
                        //    if (IsBanZiChengYuanFinished(gwlist[i].GongWenID)) //判断班子成员是否都已经完成签收
                        //    {
                        //        gwlist[i].ShiFouQianShou = 0;//如果班子成员都签完了，标记为未签收
                        //        gwlist[i].QianShouQingKuang = "未签收";
                        //    }
                        //    else
                        //    {
                        //        gwlist[i].ShiFouQianShou = 2;//否则标记为只读只读
                        //        gwlist[i].QianShouQingKuang = "只读";
                        //    }
                        //}
                        //else //不是段长、书记直接发给中层的，标记为未签收
                        //{
                        //    gwlist[i].ShiFouQianShou = 0;
                        //    gwlist[i].QianShouQingKuang = "未签收";
                        //}

                    }
                    else //签收时间不为空，已签收
                    {
                        gwlist[i].ShiFouQianShou = 1;
                        gwlist[i].QianShouQingKuang = "已签收";
                    }
                }
                return gwlist;
            }
        }

        internal static GongWenList[] GetDuanWenList(int uid, string keyWord, string sTime, string eTime, int ksxh, int count)
        {

            //调用数据操作层的函数获取公文列表DataTable
            GongWenYongHu gwyh = GetGongWenYongHuByUid(uid);
            if (gwyh == null)
            {
                return null;
            }
            if (gwyh.RoleID != 21 && gwyh.RoleID != 22) //不是领导不能查看全部段文
            {
                return null;
            }
            DataTable dt = DAL.GetDuanWenList(keyWord, sTime, eTime);

            //如果获取数据过程错误，返回null
            if (dt.TableName.Equals("error!"))
            {
                return null;
            }
            else
            {
                //将DataTable转换为GongWenList
                GongWenList[] gwlist = new GongWenList[ksxh + count - 1 < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
                for (int i = 0; i < gwlist.Length; i++)
                {
                    gwlist[i] = new GongWenList();
                    gwlist[i].GongWenID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["gwid"]);
                    gwlist[i].LiuZhuanID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["lzID"]);
                    gwlist[i].XuHao = i + 1;
                    gwlist[i].BiaoTi = Convert.ToString(dt.Rows[i + ksxh - 1]["bt"]);
                    gwlist[i].WenHao = Convert.ToString(dt.Rows[i + ksxh - 1]["wh"]);
                    gwlist[i].FaSongRen = Convert.ToString(dt.Rows[i + ksxh - 1]["fbrxm"]);
                    gwlist[i].FaSongShiJian = Convert.ToDateTime(dt.Rows[i + ksxh - 1]["fbrq"]).ToString("yyyy-MM-dd HH:mm:ss");
                    gwlist[i].JinJi = Convert.ToString(dt.Rows[i + ksxh - 1]["jinji"]);

                }
                return gwlist;
            }
        }

        internal static int GetDuanWenCount(int uid, string keyWord, string sTime, string eTime)
        {
            GongWenYongHu gwyh = GetGongWenYongHuByUid(uid);
            if (gwyh == null)
            {
                return 0;
            }
            if (gwyh.RoleID != 21 && gwyh.RoleID != 22) //不是领导不能查看全部段文
            {
                return 0;
            }
            DataTable dt = DAL.GetDuanWenList(keyWord, sTime, eTime);
            if (dt.TableName.Equals("error!"))
            {
                return -1;
            }
            else
            {
                return dt.Rows.Count;
            }
        }

        internal static int GetGongWenGuiDangCount(int uid, int type, string keyWord, string sTime, string eTime)
        {
            string workNo = uid.ToWorkNo();
            DataTable dt = DAL.GetGongWenGuiDangList(workNo, keyWord, sTime, eTime, type);
            if (dt.TableName.Equals("error!"))
            {
                return -1;
            }
            else
            {
                return dt.Rows.Count;
            }
        }


        internal static GongWenGuiDangList[] GetGongWenGuiDangList(int uid, int type, string keyWord, string sTime, string eTime, int ksxh, int count)
        {
            string workNo = uid.ToWorkNo();
            DataTable dt = DAL.GetGongWenGuiDangList(workNo, keyWord, sTime, eTime, type);
            if (dt.TableName.Equals("error!"))
            {
                return null;
            }
            else
            {
                GongWenGuiDangList[] gwlist = new GongWenGuiDangList[ksxh + count - 1 < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
                for (int i = 0; i < gwlist.Length; i++)
                {
                    gwlist[i] = new GongWenGuiDangList();
                    gwlist[i].GongWenID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["gwid"]);
                    gwlist[i].LiuZhuanID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["lzid"]);
                    gwlist[i].XuHao = i + 1;
                    gwlist[i].BiaoTi = Convert.ToString(dt.Rows[i + ksxh - 1]["bt"]);
                    gwlist[i].WenHao = Convert.ToString(dt.Rows[i + ksxh - 1]["wh"]);
                    gwlist[i].FaBuRen = Convert.ToString(dt.Rows[i + ksxh - 1]["fbr"]);
                    gwlist[i].FaBuRenXM = Convert.ToString(dt.Rows[i + ksxh - 1]["fbrxm"]);
                    gwlist[i].JinJi = Convert.ToString(dt.Rows[i + ksxh - 1]["jinji"]);
                    gwlist[i].WenJianLeiXing = Convert.ToString(dt.Rows[i + ksxh - 1]["wjlx"]);
                    gwlist[i].FaBuShiJian = Convert.ToDateTime(dt.Rows[i + ksxh - 1]["fbrq"]).ToString("yyyy-MM-dd HH:mm:ss");
                    //gwlist[i].JieShouRen = Convert.ToString(dt.Rows[i + ksxh - 1]["jsr"]);
                    //gwlist[i].JieShouRenXM = Convert.ToString(dt.Rows[i + ksxh - 1]["jsrxm"]);
                    int qswc = Convert.ToInt32(dt.Rows[i + ksxh - 1]["ShiFouLiuZhuanWanCheng"]);
                    if (qswc == 1)
                    {
                        gwlist[i].ShiFouLiuZhuanWanCheng = true;
                    }
                    else
                    {
                        gwlist[i].ShiFouLiuZhuanWanCheng = false;
                    }
                }
                return gwlist;
            }
        }

        internal static int GetGongWenCount(int uid, string fsr, int xzid, int lxid, string keyWord, string sTime, string eTime, int gwtype)
        {
            string jsr = uid.ToWorkNo();
            DataTable dt = DAL.GetGongWenList(jsr, fsr, xzid, lxid, keyWord, sTime, eTime, gwtype);
            if (dt.TableName.Equals("error!"))
            {
                return -1;
            }
            else
            {
                return dt.Rows.Count;
            }
        }

        internal static INT AddNewGongWen2016(int uid, string ht, string dw, string wh, string bt, string zw, string yj, int xzid, int lxid, string jinji, string ip, string[] jsr, string[] gwfj)
        {
            //工号转换为字符
            string workNo = uid.ToWorkNo();

            //判断参数是否含有非法字符
            if (!ht.IsValidString() || !dw.IsValidString() || !wh.IsValidString() || !bt.IsValidString() || !zw.IsValidString() || !yj.IsValidString())
            {
                return new INT(-1, "文件信息中包含非法字符。");
            }

            //调用数据操作层函数添加公文
            return DAL.AddNewGongWen2016(ht, dw, wh, bt, zw, yj, xzid, lxid, workNo, jinji, ip, jsr, gwfj);
        }

        internal static INT SignGongWen2016(int gwid, int lzid, int fsr, string[] jsr, string qsnr, int[] zdybm, string device, string ip)
        {
            string workNo = fsr.ToWorkNo();
            GongWenYongHu gwyh = GetGongWenYongHuByUid(fsr);
            if (gwyh == null)
            {
                return new INT(-1, "尚未设置签阅公文权限");
            }
            GongWen2016 gw = GetGongWen2016ById(gwid);

            //if (!zdybm.Equals(null) && zdybm.Length > 0)
            //{
            //    DataTable dt = DAL.getZiDingYiBuMenRenYuan(zdybm);
            //    string[] zdyjsr = new string[dt.Rows.Count];
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        zdyjsr[i] = dt.Rows[i]["user_no"].ToString();
            //    }
            //    jsr = jsr.Concat(zdyjsr).ToArray();
            //}
            string throwJsr = "";
            List<string> jsrList = jsr.Distinct().ToList();
            jsr = jsrList.ToArray();
            BuMenFenLei[] bmfl = GetBuMenFenLei(fsr, gwyh.RoleID);
            if (bmfl == null)
            {
                if (jsrList.Count > 0)
                {
                    foreach (string gh in jsrList)
                    {
                        throwJsr += gh + ",";

                    }

                }
                jsrList.Clear();
            }
            else
            {
                List<string> allJsr = new List<string>();
                foreach (BuMenFenLei fl in bmfl)
                {
                    foreach (GongWenBuMenRenYuan ry in fl.RenYuan)
                    {
                        allJsr.Add(ry.GongHao);
                    }
                }
                foreach (string gh in jsr)
                {
                    if (allJsr.IndexOf(gh) < 0)
                    {
                        jsrList.Remove(gh);
                        throwJsr += gh + ",";
                    }
                }

            }
            jsr = jsrList.ToArray();
            if (!throwJsr.Equals(""))
            {
                throwJsr = "舍弃接收人： " + throwJsr;
                DAL.SignGongWen2016Log(gwid, lzid, workNo, throwJsr);
            }
            if (ip.Equals(("手机")))
            {
                DAL.SignGongWen2016Log(gwid, lzid, workNo, jsr.ToListString());

            }
            return DAL.SignGongWen2016(gwid, lzid, workNo, jsr, gw.BiaoTi, gwyh.XingMing, gwyh.RoleID, qsnr, device, ip);
        }

        internal static INT BuGongWen2016(int gwid, int lzid, int fsr, int buid, string[] jsr)
        {
            string workNo = fsr.ToWorkNo();
            string buyueren = buid.ToWorkNo();
            GongWenYongHu gwyh = GetGongWenYongHuByUid(fsr);
            if (gwyh == null)
            {
                return new INT(-1, "尚未设置签阅公文权限");
            }
            GongWen2016 gw = GetGongWen2016ById(gwid);
            //if (!zdybm.Equals(null) && zdybm.Length > 0)
            //{
            //    DataTable dt = DAL.getZiDingYiBuMenRenYuan(zdybm);
            //    string[] zdyjsr = new string[dt.Rows.Count];
            //    for (int i = 0; i < dt.Rows.Count; i++)
            //    {
            //        zdyjsr[i] = dt.Rows[i]["user_no"].ToString();
            //    }
            //    jsr = jsr.Concat(zdyjsr).ToArray();
            //}
            jsr = jsr.Distinct().ToArray();
            return DAL.BuGongWen2016(gwid, lzid, workNo, gwyh.XingMing, gw.BiaoTi, jsr, buyueren);
        }

        internal static INT SignGongWen2016Mobile(int gwid, int lzid, int fsr, string jsr, string qsnr)
        {
            string[] jsrs = jsr.ToStringList(new string[] { "," });

            return SignGongWen2016(gwid, lzid, fsr, jsrs, qsnr, new int[0], "手机", "手机");
        }


        internal static GongWenXingZhi[] GetGongWenXingZhi()
        {
            DataTable dt = DAL.GetGongWenXingZhi();
            if (dt.TableName.Equals("error!"))
            {
                return null;
            }
            else
            {
                GongWenXingZhi[] gwxz = new GongWenXingZhi[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gwxz[i] = new GongWenXingZhi();
                    gwxz[i].XZID = Convert.ToInt32(dt.Rows[i]["id"]);
                    gwxz[i].XZMC = Convert.ToString(dt.Rows[i]["wjxz"]);
                }
                return gwxz;
            }
        }

        internal static GongWenLeiXing[] GetGongWenLeiXing()
        {
            DataTable dt = DAL.GetGongWenLeiXing();
            if (dt.TableName.Equals("error!"))
            {
                return null;
            }
            else
            {
                GongWenLeiXing[] gwlx = new GongWenLeiXing[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gwlx[i] = new GongWenLeiXing();
                    gwlx[i].LXID = Convert.ToInt32(dt.Rows[i]["id"]);
                    gwlx[i].LXMC = Convert.ToString(dt.Rows[i]["wjlx"]);
                }
                return gwlx;
            }
        }


        internal static GongWenYongHu[] GetGongWenYongHu(int[] rid)
        {
            DataTable dt = DAL.getGongWenYongHu(rid);
            if (dt.TableName.Equals("error!"))
            {
                return null;
            }
            else
            {
                GongWenYongHu[] gwyh = new GongWenYongHu[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gwyh[i] = new GongWenYongHu();
                    gwyh[i].GongHao = Convert.ToString(dt.Rows[i]["user_no"]);
                    gwyh[i].XingMing = Convert.ToString(dt.Rows[i]["user_name"]);
                    gwyh[i].NiCheng = Convert.ToString(dt.Rows[i]["nc"]);
                    gwyh[i].BuMenID = Convert.ToInt32(dt.Rows[i]["bm_id"]);
                    gwyh[i].BuMen = Convert.ToString(dt.Rows[i]["bm_mc"]);
                }
                return gwyh;
            }
        }

        internal static GongWenYongHu GetGongWenYongHuByUid(int uid)
        {
            string workNo = uid.ToWorkNo();
            DataTable dt = DAL.getGongWenYongHu(workNo);
            if (dt.TableName.Equals("error!"))
            {
                return null;
            }
            else if (dt.Rows.Count == 0 || dt.Rows.Count > 1)
            {
                return null;
            }
            else
            {
                GongWenYongHu gwyh = new GongWenYongHu();
                gwyh.GongHao = Convert.ToString(dt.Rows[0]["user_no"]);
                gwyh.XingMing = Convert.ToString(dt.Rows[0]["user_name"]);
                gwyh.NiCheng = Convert.ToString(dt.Rows[0]["nc"]);
                gwyh.BuMenID = Convert.ToInt32(dt.Rows[0]["bm_id"]);
                gwyh.BuMen = Convert.ToString(dt.Rows[0]["bm_mc"]);
                gwyh.RoleID = Convert.ToInt32(dt.Rows[0]["rid"]);
                return gwyh;
            }
        }


        internal static GongWen2016 GetGongWen2016ById(int gwid)
        {

            DataTable dt = DAL.GetGongWen2016ById(gwid);
            if (!dt.TableName.Equals("error!"))
            {
                if (dt.Rows.Count == 1)
                {
                    GongWen2016 gw = new GongWen2016();
                    gw.BiaoTi = Convert.ToString(dt.Rows[0]["bt"]);
                    gw.ChengSongYiJian = Convert.ToString(dt.Rows[0]["csyj"]);
                    gw.FaBuRen = Convert.ToString(dt.Rows[0]["fbr"]);
                    gw.FaBuRenXM = Convert.ToString(dt.Rows[0]["fbrxm"]);
                    gw.FaBuShiJian = Convert.ToString(dt.Rows[0]["fbrq"]);
                    gw.FaWenDanWei = Convert.ToString(dt.Rows[0]["dw"]);
                    gw.GongWenID = Convert.ToInt32(dt.Rows[0]["gwid"]);
                    gw.HongTou = Convert.ToString(dt.Rows[0]["ht"]);
                    gw.LeiXingID = Convert.ToInt32(dt.Rows[0]["wjlxid"]);
                    gw.XingZhiID = Convert.ToInt32(dt.Rows[0]["wjxzid"]);
                    gw.WenHao = Convert.ToString(dt.Rows[0]["wh"]);
                    gw.WenJianLeiXing = Convert.ToString(dt.Rows[0]["wjlx"]);
                    gw.WenJianXingZhi = Convert.ToString(dt.Rows[0]["wjxz"]);
                    gw.ZhengWen = Convert.ToString(dt.Rows[0]["zw"]);
                    DataTable dtFj = DAL.GetGongWenFuJian2016ById(gwid);
                    if (!dtFj.TableName.Equals("error!"))
                    {
                        if (dtFj.Rows.Count > 0)
                        {
                            gw.FuJian = new string[dtFj.Rows.Count];
                            for (int j = 0; j < dtFj.Rows.Count; j++)
                            {
                                gw.FuJian[j] = dtFj.Rows[j]["fjmc"].ToString();
                            }
                            return gw;
                        }

                    }

                }
            }
            return null;
        }

        internal static GongWenLiuZhuan[] GetLiuZhuanXianByLzId(bool sfbr, int lzlvl, int lzid)
        {

            DataTable dt = DAL.GetLiuZhuanXianByLzId(sfbr, lzlvl, lzid);
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            else
            {
                GongWenLiuZhuan[] gwlz = new GongWenLiuZhuan[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gwlz[i] = new GongWenLiuZhuan();
                    gwlz[i].LiuZhuanID = Convert.ToInt32(dt.Rows[i]["lzid"]);
                    gwlz[i].GongWenID = Convert.ToInt32(dt.Rows[i]["gwid"]);
                    gwlz[i].FaSongRen = Convert.ToString(dt.Rows[i]["fsr"]);
                    gwlz[i].FaSongRenXM = Convert.ToString(dt.Rows[i]["fsrxm"]);
                    gwlz[i].FaSongShiJian = Convert.ToString(dt.Rows[i]["fssj"]);
                    gwlz[i].JieShouRen = Convert.ToString(dt.Rows[i]["jsr"]);
                    gwlz[i].JieShouRenXM = Convert.ToString(dt.Rows[i]["jsrxm"]);
                    gwlz[i].QianShouNeiRong = Convert.ToString(dt.Rows[i]["qsnr"]);
                    gwlz[i].QianShouShiJian = Convert.ToString(dt.Rows[i]["qssj"]);
                    gwlz[i].LiuZhuanShu = Convert.ToInt32(dt.Rows[i]["liuzhuan"]);
                    gwlz[i].WanChengShu = Convert.ToInt32(dt.Rows[i]["wancheng"]);
                    gwlz[i].JieShouRenBM = Convert.ToString(dt.Rows[i]["jsr_bm"]);
                }
                return gwlz;
            }

        }

        internal static GongWenLiuZhuan[] GetLingDaoPiShi(int uid, int gwid)
        {
            GongWenYongHu gwyh = GetGongWenYongHuByUid(uid);
            if (gwyh == null) return null;
            if (gwyh.RoleID != 23 && gwyh.RoleID != 24) return null;
            DataTable dt = DAL.GetLingDaoPiShi(gwid);
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            else
            {
                GongWenLiuZhuan[] gwlz = new GongWenLiuZhuan[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gwlz[i] = new GongWenLiuZhuan();
                    gwlz[i].JieShouRenXM = Convert.ToString(dt.Rows[i]["jsrxm"]);
                    gwlz[i].QianShouNeiRong = Convert.ToString(dt.Rows[i]["qsnr"]);
                    gwlz[i].QianShouShiJian = Convert.ToString(dt.Rows[i]["qssj"]);
                }
                return gwlz;
            }

        }

        internal static GongWenLiuZhuan[] GetSuoYouWeiQian(int uid, int gwid)
        {
            GongWenYongHu gwyh = GetGongWenYongHuByUid(uid);
            if (gwyh == null) return null;
            if (gwyh.RoleID != 20) return null;
            DataTable dt = DAL.GetSuoYouWeiQian(gwid);
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            else
            {
                GongWenLiuZhuan[] gwlz = new GongWenLiuZhuan[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gwlz[i] = new GongWenLiuZhuan();
                    gwlz[i].JieShouRenXM = Convert.ToString(dt.Rows[i]["jsrxm"]);
                    gwlz[i].JieShouRen = Convert.ToString(dt.Rows[i]["jsr"]);
                    gwlz[i].JieShouRenBM = Convert.ToString(dt.Rows[i]["jsr_bm"]);
                    gwlz[i].FaSongShiJian = Convert.ToString(dt.Rows[i]["fssj"]);

                }
                return gwlz;
            }

        }

        internal static INT UpdateDuanYu(int id, string newTxt)
        {
            return DAL.UpdateDuanYu(id, newTxt);
        }
        internal static INT DeleteDuanYu(int id)
        {
            return DAL.DeleteDuanYu(id);
        }
        internal static INT AddZdybm(int uid, string dynr)
        {
            string workNo = uid.ToWorkNo();
            return DAL.AddZdybm(workNo, dynr);
        }

        internal static INT UpdateZdybm(int id, string newTxt)
        {
            return DAL.UpdateZdybm(id, newTxt);
        }
        internal static INT DeleteZdybm(int id)
        {
            return DAL.DeleteZdybm(id);
        }
        internal static INT AddDuanYu(int uid, string dynr)
        {
            string workNo = uid.ToWorkNo();
            return DAL.AddDuanYu(workNo, dynr);
        }

        internal static GongWenZiDingYiDuanYu[] GetZiDingYiDuanYu(int uid, bool onlyPrivate)
        {
            string workNo = uid.ToWorkNo();
            DataTable dt = DAL.GetZiDingYiDuanYu(workNo, onlyPrivate);
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            else
            {
                GongWenZiDingYiDuanYu[] duanyu = new GongWenZiDingYiDuanYu[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    duanyu[i] = new GongWenZiDingYiDuanYu();
                    duanyu[i].ID = Convert.ToInt32(dt.Rows[i]["id"]);
                    duanyu[i].DuanYuNeiRong = dt.Rows[i]["dynr"].ToString();
                    if (dt.Rows[i]["uid"].ToString().Equals("0"))
                    {
                        duanyu[i].SiYou = false;
                    }
                    else
                    {
                        duanyu[i].SiYou = true;
                    }
                }
                return duanyu;
            }
        }
        internal static GongWenZiDingYiBuMen[] GetZiDingYiBuMen(int uid)
        {
            string workNo = uid.ToWorkNo();
            DataTable dt = DAL.GetZiDingYiBuMen(workNo);
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            else
            {
                GongWenZiDingYiBuMen[] bumen = new GongWenZiDingYiBuMen[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bumen[i] = new GongWenZiDingYiBuMen();
                    bumen[i].ID = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
                    bumen[i].MingCheng = Convert.ToString(dt.Rows[i]["bmnr"].ToString());
                }
                return bumen;
            }
        }




        internal static BuMenFenLei[] GetBuMenFenLei(int uid, int rid)
        {
            string workNo = uid.ToWorkNo();
            if (rid == 20 || rid == 21 || rid == 22)
            {
                return GetBuMenFenLeiLingDao(workNo, rid);
            }
            else if (rid == 23)
            {
                return GetBuMenFenLeiKeShi(workNo, rid);
            }
            else if (rid == 24)
            {
                return GetBuMenFenLeiZhongCeng(workNo, rid);
            }
            else return null;
        }


        internal static INT SetZiDingYiBuMenRenYuan(int zdybmid, string[] userNo)
        {
            return DAL.SetZiDingYiBuMenRenYuan(zdybmid, userNo);
        }

        internal static GongWenBuMenRenYuan[] GetZiDingYiBuMenRenYuan(int zdybmid, bool added)
        {
            DataTable dt = DAL.getZiDingYiBuMenRenYuan(zdybmid, added);
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            else
            {
                GongWenBuMenRenYuan[] bmry = new GongWenBuMenRenYuan[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bmry[i] = new GongWenBuMenRenYuan();
                    bmry[i].GongHao = Convert.ToString(dt.Rows[i]["user_no"].ToString());
                    bmry[i].XianShiMingCheng = Convert.ToString(dt.Rows[i]["xsmc"].ToString());
                    bmry[i].NiCheng = Convert.ToString(dt.Rows[i]["xsmc"].ToString());
                }
                return bmry;
            }
        }

        internal static GongWenBuMenRenYuan[] GetBuMenRenYuan(int bmid)
        {
            DataTable dt = DAL.GetBuMenRenYuan(bmid);
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            else
            {
                GongWenBuMenRenYuan[] bmry = new GongWenBuMenRenYuan[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bmry[i] = new GongWenBuMenRenYuan();
                    bmry[i].GongHao = Convert.ToString(dt.Rows[i]["user_no"].ToString());
                    bmry[i].XianShiMingCheng = Convert.ToString(dt.Rows[i]["user_name"].ToString());
                }
                return bmry;
            }
        }

        internal static BuMenFenLei[] GetBuMenFenLeiZhongCeng(string workNo, int rid)
        {

            DataTable dt = DAL.GetBenBuMenRenYuan(workNo);
            if (dt.TableName.Equals("error!"))
            {
                return null;
            }
            else if (dt.Rows.Count == 0)
            {
                return null;
            }
            else
            {
                BuMenFenLei[] bmfl = new BuMenFenLei[1];
                bmfl[0] = new BuMenFenLei();
                bmfl[0].FenLeiMingCheng = Convert.ToString(dt.Rows[dt.Rows.Count - 1]["bm_mc"]);
                bmfl[0].FenLeiZongCheng = "全体人员";
                bmfl[0].FenLeiID = 0;
                GongWenBuMenRenYuan[] ry = new GongWenBuMenRenYuan[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    ry[i] = new GongWenBuMenRenYuan();
                    ry[i].GongHao = Convert.ToString(dt.Rows[i]["user_no"].ToString());
                    if (rid == 24)
                    {
                        ry[i].XianShiMingCheng = Convert.ToString(dt.Rows[i]["ziwu"].ToString());
                    }
                    else
                    {
                        ry[i].XianShiMingCheng = Convert.ToString(dt.Rows[i]["user_name"].ToString());
                    }
                    ry[i].NiCheng = Convert.ToString(dt.Rows[i]["nc"].ToString());
                }
                bmfl[0].RenYuan = ry;
                return bmfl;
            }
        }

        internal static BuMenFenLei[] GetBuMenFenLeiKeShi(string workNo, int rid)
        {

            BuMenFenLei[] bmfl = GetBuMenFenLeiZhongCeng(workNo, rid);
            BuMenFenLei[] bmfl1 = GetBuMenFenLeiLingDao(workNo, rid);
            if (bmfl != null)
            {
                bmfl1 = bmfl.Concat(bmfl1.ToList()).ToArray();
            }
            return bmfl1;
        }

        internal static BuMenFenLei[] GetBuMenFenLeiLingDao(string workNo, int rid)
        {
            if (rid != 21 && rid != 22 && rid != 23 && rid != 20) return null;//只有领导有权限
            DataTable dt = DAL.GetBuMenFenLei(rid);
            if (dt == null) return null;
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            else
            {
                BuMenFenLei[] bumen = new BuMenFenLei[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bumen[i] = new BuMenFenLei();
                    bumen[i].FenLeiID = Convert.ToInt32(dt.Rows[i]["flID"].ToString());
                    bumen[i].FenLeiMingCheng = Convert.ToString(dt.Rows[i]["flmc"].ToString());
                    bumen[i].FenLeiZongCheng = Convert.ToString(dt.Rows[i]["flzc"].ToString());

                    DataTable dtyh = DAL.GetBuMenFenLeiYongHu(rid, workNo, bumen[i].FenLeiID);
                    GongWenBuMenRenYuan[] ry = new GongWenBuMenRenYuan[dtyh.Rows.Count];
                    for (int j = 0; j < dtyh.Rows.Count; j++)
                    {
                        ry[j] = new GongWenBuMenRenYuan();
                        ry[j].GongHao = Convert.ToString(dtyh.Rows[j]["user_no"].ToString());
                        ry[j].XianShiMingCheng = Convert.ToString(dtyh.Rows[j]["xsmc"].ToString());
                        ry[j].NiCheng = Convert.ToString(dtyh.Rows[j]["nc"].ToString());
                    }
                    bumen[i].RenYuan = ry;
                }
                return bumen;
            }
        }


        internal static INT makeCuiBan(int gwid, int rid)
        {
            GongWen2016 gw = GetGongWen2016ById(gwid);
            if (gw == null)
            {
                return new INT(-1, "无此公文");
            }
            return DAL.makeCuiBan(gwid, rid, gw.BiaoTi);
        }

        internal static INT makeCuiBan(int gwid, string[] jsr)
        {
            GongWen2016 gw = GetGongWen2016ById(gwid);
            if (gw == null)
            {
                return new INT(-1, "无此公文");
            }
            return DAL.makeCuiBan(jsr, gw.BiaoTi);
        }

        internal static INT AddGongWenRenYuan(int uid, string gh, int rid)
        {
            GongWenYongHu gwyh = GetGongWenYongHuByUid(uid);
            if (gwyh == null)
            {
                return new INT(-1, "无权限添加新公文用户。");
            }
            if (rid <= 20 || rid > 26)
            {
                return new INT(-1, "错误的角色ID。");
            }
            if (gwyh.RoleID == 21 || gwyh.RoleID == 22 || gwyh.RoleID == 25) //段领导和基层管理人员
            {
                return new INT(-1, "无权限添加新公文用户。");
            }

            else if (gwyh.RoleID == 20)//公文处理员
            {
                if (rid == 25 || rid == 26)//公文处理员不能直接添加基层用户
                {
                    return new INT(-1, "无权限添加新公文用户。");
                }
            }
            else //中层干部
            {
                if (rid != 25 && rid != 26)
                {
                    return new INT(-1, "无权限添加新公文用户。");
                }

            }
            if (DAL.IsGongWenYongHu(gh))
            {
                return new INT(-1, "已经是公文用户，不能再次添加");
            }
            if (GetYongHuXinXiByGh(uid, gh) == null)
            {
                return new INT(-1, "不是本部门人员，无法添加");
            }
            return DAL.AddGongWenRenYuan(gh, rid);
        }

        internal static INT DeleteGongWenRenYuan(int uid, string gh, int rid)
        {
            GongWenYongHu gwyh = GetGongWenYongHuByUid(uid);
            if (gwyh == null)
            {
                return new INT(-1, "无权限删除公文用户。");
            }
            if (rid <= 20 || rid > 26)
            {
                return new INT(-1, "错误的角色ID。");
            }
            if (gwyh.RoleID == 21 || gwyh.RoleID == 22 || gwyh.RoleID == 25 || gwyh.RoleID == 26) //段领导和基层管理人员无权删除用户
            {
                return new INT(-1, "无权限删除公文用户。");
            }

            else if (gwyh.RoleID == 20)//公文处理员
            {
                if (rid == 25 || rid == 26)//公文处理员不能直接删除基层用户
                {
                    return new INT(-1, "无权限删除公文用户。");
                }
            }
            else //中层干部
            {
                if (rid != 25 && rid != 26)
                {
                    return new INT(-1, "无权限删除公文用户。");
                }

            }
            if (!DAL.IsGongWenYongHu(gh))
            {
                return new INT(-1, "不是公文用户，不能删除");
            }
            return DAL.DeleteGongWenRenYuan(gh, rid);
        }

        internal static INT DeleteGongWen2016(int uid, int gwid)
        {
            GongWenYongHu gwyh = GetGongWenYongHuByUid(uid);
            if (gwyh == null)
            {
                return new INT(-1, "无权限删除公文。");
            }
            if (gwyh.RoleID != 20)
            {
                return new INT(-1, "无权限删除公文。");
            }
            return DAL.DeleteGongWen2016(uid, gwid);
        }

        internal static GongWenYongHu GetYongHuXinXiByGh(int uid, string gh)
        {
            GongWenYongHu gwyh = GetGongWenYongHuByUid(uid);
            if (gwyh == null)
            {
                return null;
            }
            if (gwyh.RoleID != 20 && gwyh.RoleID != 23 && gwyh.RoleID != 24)
            {
                return null;
            }
            DataTable dt = DAL.GetYongHuXinXiByGh(gh);
            if (dt == null)
            {
                return null;
            }
            else if (dt.Rows.Count != 1)
            {
                return null;
            }
            GongWenYongHu r = new GongWenYongHu();

            r.GongHao = Convert.ToString(dt.Rows[0]["user_no"]);
            r.XingMing = Convert.ToString(dt.Rows[0]["user_name"]);
            r.BuMen = Convert.ToString(dt.Rows[0]["bm_mc"]);
            r.BuMenID = Convert.ToInt32(dt.Rows[0]["bm_id"]);
            if ((gwyh.RoleID == 23 || gwyh.RoleID == 24) && gwyh.BuMenID != r.BuMenID)
            {
                return null;
            }
            return r;
        }

        internal static INT UndoSignGongWen2016(int uid, int lzid)
        {
            return DAL.UndoSignGongWen2016(uid, lzid);
        }
        #endregion


        #region 2016自制邮件系统

        internal static YouJian2016 GetMailByMid2016(int uid, int mid)
        {
            DataTable dt = DAL.GetMailById2016(mid);
            if (dt == null || dt.TableName.Equals("error!"))
            {
                return null;
            }
            if (dt.Rows.Count != 1)
            {
                return null;

            }
            YouJian2016 yj = new YouJian2016();
            yj.ID = Convert.ToInt32(dt.Rows[0]["id"]);
            yj.From = Convert.ToString(dt.Rows[0]["fromaddress"]);
            yj.To = Convert.ToString(dt.Rows[0]["toaddress"]);
            yj.Subject = Convert.ToString(dt.Rows[0]["title"]);
            yj.Body = Convert.ToString(dt.Rows[0]["body"]);
            yj.Date = Convert.ToString(dt.Rows[0]["createdate"]);
            yj.Attachments = Convert.ToString(dt.Rows[0]["filenamestring"]);
            yj.IsRead = Convert.ToInt32(dt.Rows[0]["readflag"]) == 1 ? true : false;
            yj.IsBodyHtml = Convert.ToInt32(dt.Rows[0]["ishtmlformat"]) == 1 ? true : false;
            yj.Size = Convert.ToInt32(dt.Rows[0]["size"]);
            return yj;
        }

        internal static YouJianList2016[] GetMailList2016(int uid, int type, int ksxh, int count)
        {
            string workNo = uid.ToWorkNo();


            DataTable dt = DAL.GetMailList2016(workNo, type);
            if (dt == null) return null;
            if (dt.TableName.Equals("error!"))
            {

                return null;
            }
            YouJianList2016[] yjlist = new YouJianList2016[ksxh + count - 1 < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
            for (int i = 0; i < yjlist.Length; i++)
            {
                yjlist[i] = new YouJianList2016();
                yjlist[i].ID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["id"]);
                yjlist[i].From = Convert.ToString(dt.Rows[i + ksxh - 1]["fromaddress"]);
                yjlist[i].Subject = Convert.ToString(dt.Rows[i + ksxh - 1]["title"]);
                yjlist[i].Date = Convert.ToString(dt.Rows[i + ksxh - 1]["createdate"]);
                int size = Convert.ToInt32(dt.Rows[i + ksxh - 1]["size"]);
                if (size < 1024)
                {
                    yjlist[i].Size = size.ToString() + "B";
                }
                else if (size < 1024 * 1024)
                {
                    yjlist[i].Size = Math.Round(size / 1024.0, 2).ToString() + "KB";
                }
                else
                {
                    yjlist[i].Size = Math.Round(size / 1024.0 / 1024.0, 2).ToString() + "MB";
                }
                yjlist[i].IsRead = Convert.ToInt32(dt.Rows[i + ksxh - 1]["readflag"]) == 1 ? true : false;
                yjlist[i].HasAttachment = string.IsNullOrEmpty(Convert.ToString(dt.Rows[i + ksxh - 1]["filenamestring"])) ? false : true;
            }
            return yjlist;
        }

        internal static YouJianFuJian GetYouJianFuJian(int uid, int mid, int pos)
        {
            string gh = uid.ToWorkNo();
            DataTable dt = DAL.GetYouJianFuJian(gh, mid);
            if (dt == null)
            {
                return null;
            }
            if (dt.Rows.Count != 1)
            {
                return null;
            }
            string[] urls = dt.Rows[0]["url"].ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] names = dt.Rows[0]["filenamestring"].ToString().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length < pos)
            {
                return null;
            }
            YouJianFuJian fj = new YouJianFuJian();
            fj.FileName = names[pos - 1];
            string fileFullPath = YoujianService.StrFilePath + urls[pos - 1];
            HttpWebRequest request;
            WebResponse response;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(fileFullPath);
                response = request.GetResponse();
            }
            catch
            {
                return null;
            }


            Stream inStream = response.GetResponseStream();
            if (inStream == null)
            {
                return null;
            }
            List<Byte> b = new List<Byte>();
            byte[] buffer = new byte[1];
            int i;
            do
            {

                i = inStream.Read(buffer, 0, 1);
                if (i > 0)
                {

                    b.Add(buffer[0]);
                }
            }
            while (i > 0);


            inStream.Close();
            fj.Base64Code = Convert.ToBase64String(b.ToArray());
            fj.FileSize = b.Count;
            return fj;
        }

        #endregion



        #region 2016段发通知
        internal static TongZhiLeiXing[] GetTongZhiLeiXing(int uid)
        {

            DataTable dt;
            if (uid == 0)
            {
                dt = DAL.GetAllTongZhiLeiXing();
            }
            else
            {

                dt = DAL.GetTongZhiLeiXing(uid);
            }

            if (dt == null) return null;

            TongZhiLeiXing[] tzlx = new TongZhiLeiXing[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tzlx[i] = new TongZhiLeiXing
                {
                    LXID = Convert.ToInt32(dt.Rows[i]["lxid"].ToString()),
                    LXMC = Convert.ToString(dt.Rows[i]["lxmc"].ToString())
                };
            }
            return tzlx;

        }


        public static BuMenFenLei[] GetTongZhiBuMenFenLei(int uid)
        {
            DataTable dt = DAL.GetTongZhiBuMenFenLei(uid);
            if (dt == null) return null;

            BuMenFenLei[] bumen;

            GongWenBuMenRenYuan[] bmry = GetTongZhiBuMenRenYuan(uid);
            if (bmry.Length > 0)
            {
                bumen = new BuMenFenLei[dt.Rows.Count + 1];
                bumen[0] = new BuMenFenLei();
                bumen[0].FenLeiID = 0;
                bumen[0].FenLeiMingCheng = "科室人员";
                bumen[0].FenLeiZongCheng = "全体人员";
                bumen[0].RenYuan = bmry;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bumen[i + 1] = new BuMenFenLei();
                    bumen[i + 1].FenLeiID = Convert.ToInt32(dt.Rows[i]["flID"].ToString());
                    bumen[i + 1].FenLeiMingCheng = Convert.ToString(dt.Rows[i]["flmc"].ToString());
                    bumen[i + 1].FenLeiZongCheng = Convert.ToString(dt.Rows[i]["flzc"].ToString());

                    DataTable dtyh = DAL.GetTongZhiBuMenFenLeiYongHu(uid, bumen[i + 1].FenLeiID);
                    GongWenBuMenRenYuan[] ry = new GongWenBuMenRenYuan[dtyh.Rows.Count];
                    for (int j = 0; j < dtyh.Rows.Count; j++)
                    {
                        ry[j] = new GongWenBuMenRenYuan();
                        ry[j].GongHao = Convert.ToString(dtyh.Rows[j]["user_no"].ToString());
                        ry[j].XianShiMingCheng = Convert.ToString(dtyh.Rows[j]["xsmc"].ToString());
                        ry[j].NiCheng = Convert.ToString(dtyh.Rows[j]["nc"].ToString());
                        ry[j].Uid = Convert.ToInt32(dtyh.Rows[j]["uid"].ToString());
                    }
                    bumen[i + 1].RenYuan = ry;
                }
            }
            else
            {
               bumen = new BuMenFenLei[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bumen[i ] = new BuMenFenLei();
                    bumen[i].FenLeiID = Convert.ToInt32(dt.Rows[i]["flID"].ToString());
                    bumen[i].FenLeiMingCheng = Convert.ToString(dt.Rows[i]["flmc"].ToString());
                    bumen[i].FenLeiZongCheng = Convert.ToString(dt.Rows[i]["flzc"].ToString());

                    DataTable dtyh = DAL.GetTongZhiBuMenFenLeiYongHu(uid, bumen[i].FenLeiID);
                    GongWenBuMenRenYuan[] ry = new GongWenBuMenRenYuan[dtyh.Rows.Count];
                    for (int j = 0; j < dtyh.Rows.Count; j++)
                    {
                        ry[j] = new GongWenBuMenRenYuan();
                        ry[j].GongHao = Convert.ToString(dtyh.Rows[j]["user_no"].ToString());
                        ry[j].XianShiMingCheng = Convert.ToString(dtyh.Rows[j]["xsmc"].ToString());
                        ry[j].NiCheng = Convert.ToString(dtyh.Rows[j]["nc"].ToString());
                        ry[j].Uid = Convert.ToInt32(dtyh.Rows[j]["uid"].ToString());
                    }
                    bumen[i].RenYuan = ry;
                }
            }
            
            return bumen;
        }

        public static INT AddNewTongZhi2016(string bt, string zw, int fbrid, int lxid, int[] jsrid, string[] files, string ip, int sfgk)
        {
            if (!bt.IsValidString())
            {
                return new INT(-1, "标题中存在非法字符");
            }

            if (!zw.IsValidString())
            {
                return new INT(-1, "正文中存在非法字符");
            }
            return DAL.AddNewTongZhi2016(bt, zw, fbrid, lxid, jsrid, files, ip, sfgk);
        }

        internal static int GetTongZhiCount(int uid, int lxid, int fsrid, string keys, string sTime, string eTime, int type)
        {
            DataTable dt = DAL.GetTongZhiList(uid, lxid, fsrid, keys, sTime, eTime, type);
            if (dt == null || dt.TableName.Equals("error!"))
            {
                return -1;
            }
            else
            {
                return dt.Rows.Count;
            }
        }

        internal static TongZhiList[] GetTongZhiList(int uid, int lxid, int fsrid, string keys, string sTime,
            string eTime, int type, int ksxh, int count)
        {

            DataTable dt;

            if (uid <= 0)
            {
                dt = DAL.GetTongZhiListChaXun(lxid, fsrid, keys, sTime, eTime, 0);//只显示公开
            }
            else
            {
                int role = BLL.GetTongZhiUserRoleType(uid);
                if (role <= 0)
                {
                    DAL.GetTongZhiListChaXun(lxid, fsrid, keys, sTime, eTime, 0);//只显示公开
                }
                if (role == 1)
                {
                    dt = DAL.GetTongZhiListChaXun(lxid, fsrid, keys, sTime, eTime, 1); //领导查询，显示所有
                }

                else
                {
                    dt = DAL.GetTongZhiList(uid, lxid, fsrid, keys, sTime, eTime, type);
                }
            }
           

            //如果获取数据过程错误，返回null
            if (dt == null || dt.TableName.Equals("error!"))
            {
                return null;
            }
            else
            {
                //将DataTable转换为GongWenList
                TongZhiList[] tzlist =
                    new TongZhiList[ksxh + count - 1 < dt.Rows.Count ? count : dt.Rows.Count - ksxh + 1];
                for (int i = 0; i < tzlist.Length; i++)
                {
                    tzlist[i] = new TongZhiList();
                    tzlist[i].XuHao = i + 1;
                    tzlist[i].TongZhiID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["tzid"]);
                    tzlist[i].LiuZhuanID = Convert.ToInt32(dt.Rows[i + ksxh - 1]["lzID"]);
                    tzlist[i].BiaoTi = Convert.ToString(dt.Rows[i + ksxh - 1]["bt"]);
                    tzlist[i].FaSongRen = Convert.ToString(dt.Rows[i + ksxh - 1]["fsrxm"]);
                    tzlist[i].FaSongShiJian =
                        Convert.ToDateTime(dt.Rows[i + ksxh - 1]["fssj"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string qssj = Convert.ToString(dt.Rows[i + ksxh - 1]["qssj"]);
                    if (uid <= 0)
                    {
                        tzlist[i].ShiFouQianShou = 2;
                        tzlist[i].QianShouQingKuang = "只读";
                    }
                    else if (string.IsNullOrEmpty(qssj)) //签收时间为空
                    {
                        tzlist[i].ShiFouQianShou = 0;
                        tzlist[i].QianShouQingKuang = "未签收";

                    }
                    else //签收时间不为空，已签收
                    {
                        tzlist[i].ShiFouQianShou = 1;
                        tzlist[i].QianShouQingKuang = "已签收";
                    }
                    tzlist[i].TongZhiLeiXing = Convert.ToString(dt.Rows[i + ksxh - 1]["lxmc"]);
                }
                return tzlist;
            }
        }


        internal static GongWenBuMenRenYuan[] GetTongZhiBuMenRenYuan(int uid)
        {
            DataTable dt = DAL.GetBuMenFenLeiRenYuanByUid(uid);
            GongWenBuMenRenYuan[] ry = new GongWenBuMenRenYuan[dt.Rows.Count];
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                ry[j] = new GongWenBuMenRenYuan();
                ry[j].GongHao = Convert.ToString(dt.Rows[j]["user_no"].ToString());
                ry[j].XianShiMingCheng = Convert.ToString(dt.Rows[j]["user_name"].ToString());
                ry[j].NiCheng = Convert.ToString(dt.Rows[j]["nc"].ToString());
                ry[j].Uid = Convert.ToInt32(dt.Rows[j]["uid"].ToString());
            }
            return ry;
        }

        internal static TongZhiLiuZhuan[] GetTongZhiLiuZhuanXian(bool sfbr, int lzlvl, int lzid)
        {
            DataTable dt = DAL.GetTongZhiLiuZhuanXianByLzId(sfbr, lzlvl, lzid);
            if (dt == null) return null;

            TongZhiLiuZhuan[] tzlz = new TongZhiLiuZhuan[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tzlz[i] = new TongZhiLiuZhuan();
                tzlz[i].LiuZhuanID = Convert.ToInt32(dt.Rows[i]["lzid"]);
                tzlz[i].TongZhiID = Convert.ToInt32(dt.Rows[i]["tzid"]);
                tzlz[i].FaSongRen = Convert.ToInt32(dt.Rows[i]["fsrid"]);
                tzlz[i].FaSongRenXM = Convert.ToString(dt.Rows[i]["fsrxm"]);
                tzlz[i].FaSongShiJian = Convert.ToString(dt.Rows[i]["fssj"]);
                tzlz[i].JieShouRen = Convert.ToInt32(dt.Rows[i]["jsrid"]);
                tzlz[i].JieShouRenXM = Convert.ToString(dt.Rows[i]["jsrxm"]);
                tzlz[i].QianShouNeiRong = Convert.ToString(dt.Rows[i]["qsnr"]);
                tzlz[i].QianShouShiJian = Convert.ToString(dt.Rows[i]["qssj"]);
                tzlz[i].LiuZhuanShu = Convert.ToInt32(dt.Rows[i]["liuzhuan"]);
                tzlz[i].WanChengShu = Convert.ToInt32(dt.Rows[i]["wancheng"]);
                tzlz[i].JieShouRenBM = Convert.ToString(dt.Rows[i]["jsrbmmc"]);
            }
            return tzlz;


        }

        internal static TongZhi2016 GetTongZhi2016ById(int tzid)
        {
            DataTable dt = DAL.GetTongZhi2016ById(tzid);
            if (dt == null) return null;
            if (dt.Rows.Count != 1) return null;

            TongZhi2016 tz = new TongZhi2016();
            tz.TongZhiID = Convert.ToInt32(dt.Rows[0]["tzid"]);
            tz.BiaoTi = Convert.ToString(dt.Rows[0]["bt"]);
            tz.FaBuRenID = Convert.ToInt32(dt.Rows[0]["fbrid"]);
            tz.FaBuRenXM = Convert.ToString(dt.Rows[0]["fbrxm"]);
            tz.FaBuShiJian = Convert.ToString(dt.Rows[0]["fbrq"]);
            tz.LeiXingID = Convert.ToInt32(dt.Rows[0]["tzlxid"]);
            tz.WenJianLeiXing = Convert.ToString(dt.Rows[0]["lxmc"]);

            tz.ZhengWen = Convert.ToString(dt.Rows[0]["zw"]);
            DataTable dtFj = DAL.GetTongZhiFuJian2016ById(tzid);
            if (dtFj?.Rows.Count > 0)
            {
                tz.FuJian = new string[dtFj.Rows.Count];
                for (int j = 0; j < dtFj.Rows.Count; j++)
                {
                    tz.FuJian[j] = dtFj.Rows[j]["fjmc"].ToString();
                }
            }
            return tz;


        }
        internal static INT SignTongZhi2016(int tzid, int lzid, int uid, int[] jsry, string pishi, string ip)
        {


            TongZhi2016 tz = GetTongZhi2016ById(tzid);

            return DAL.SignTongZhi2016(tzid, lzid, uid, jsry, tz.BiaoTi, pishi, ip);
        }



        internal static int GetTongZhiUserRoleType(int uid)
        {
            return DAL.GetTongZhiUserRoleType(uid);
        }

        internal static GongWenZiDingYiDuanYu[] GetTongZhiZiDingYiDuanYu(int uid)
        {
            string workno = DAL.GetWorkNoByUid(uid);
            return BLL.GetZiDingYiDuanYu(Convert.ToInt32(workno), false);
        }

        #endregion


    }




}