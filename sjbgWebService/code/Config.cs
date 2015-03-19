using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace sjbgWebService
{
    public class SjbgConfig
    {

        //预定义说明文字
        public static string StrInvlaidRequest = "该请求不合法，请下载使用正确的客户端程序。";
        public static string StrInvalidParameter = "请求的参数不合法，请下载使用正确的客户端程序";

        //字符串标识
        public static string FuHaoKaiShi = System.Configuration.ConfigurationManager.AppSettings["KaiShi"];
        public static string FuHaoFenGe = System.Configuration.ConfigurationManager.AppSettings["FenGe"];
        public static string FuHaoYouJianDiZhi = System.Configuration.ConfigurationManager.AppSettings["YouJianDiZhi"];
        public static string FuHaoLingDaoFenGe = System.Configuration.ConfigurationManager.AppSettings["LingDaoFenGe"];
        //apkinfo
        public static int ApkVerCode = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApkVersionCode"]);
        public static string ApkVerName = System.Configuration.ConfigurationManager.AppSettings["ApkVersionName"];
        public static string ApkFilePath = System.Configuration.ConfigurationManager.AppSettings["ApkFilePath"];
        public static string ApkFileName = System.Configuration.ConfigurationManager.AppSettings["ApkFileName"];
        public static string ApkUpdateContent = System.Configuration.ConfigurationManager.AppSettings["ApkUpdateContent"];

        //mail
        public static string MailPassword = System.Configuration.ConfigurationManager.AppSettings["MailPass"];
        public static Encoding MailEncoding = Encoding.GetEncoding(936);
        public static string MailDomain = System.Configuration.ConfigurationManager.AppSettings["MailDomain"];
        public static string MailHost = System.Configuration.ConfigurationManager.AppSettings["MailHost"];
        public static int MailSmtpPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MailSmtpPort"]);
        public static int MailImapPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MailImapPort"]);

        //Sendfile
        public static string SendFilePath = System.Configuration.ConfigurationManager.AppSettings["SendFile"];
    }
}