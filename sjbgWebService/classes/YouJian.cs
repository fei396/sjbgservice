using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AE.Net.Mail.Imap;
using AE.Net.Mail;
using System.Net.Mail;

namespace sjbgWebService
{


    public class YouJianDiZhi
    {
        public string Address { get; set; }
        public string DisplayName { get; set; }


        public YouJianDiZhi()
        {
            Address = "";
            DisplayName = "";

        }
        public YouJianDiZhi(string a, string d, string h, string u)
        {
            Address = a;
            DisplayName = d;

        }

        public void LoadFrom(MailAddress ma)
        {
            if (ma == null) return;
            Address = ma.Address;
            DisplayName = ma.DisplayName;

        }

        public void LoadFrom(string strYj)
        {

            if (strYj.IndexOf(SjbgConfig.FuHaoKaiShi) != 0)
            {
                Address = "";
                DisplayName = "";
            }
            else
            {
                strYj = strYj.Substring(strYj.IndexOf(SjbgConfig.FuHaoKaiShi) + SjbgConfig.FuHaoKaiShi.Length, strYj.Length - strYj.IndexOf(SjbgConfig.FuHaoKaiShi) - SjbgConfig.FuHaoKaiShi.Length);
                string[] str = strYj.Split(new string[] { SjbgConfig.FuHaoFenGe }, 2, StringSplitOptions.None);
                Address = str[0];
                DisplayName = str[1];
            }
        }

        public MailAddress ToMailAddress()
        {
            if (Address == null || Address == string.Empty) return null;
            MailAddress ma = new MailAddress(Address, DisplayName);
            return ma;
        }

        public override string ToString()
        {
            string strYj = SjbgConfig.FuHaoKaiShi + Address + SjbgConfig.FuHaoFenGe + DisplayName;

            return strYj;
        }

    }

    public class YouJianFuJian
    {
        public string FileName { get; set; }
        public string Base64Code { get; set; }

        public int FileSize { get; set; }

        public YouJianFuJian()
        {
            FileName = string.Empty;
            Base64Code = string.Empty;
            FileSize = -1;
        }

        public YouJianFuJian(string f, string b)
        {
            FileName = f;
            Base64Code = b;
        }

        public void LoadFrom(AE.Net.Mail.Attachment att)
        {
            if (att == null)
            {
                FileName = "";
                FileSize = -1;
                Base64Code = "";
            }
            else
            {
                FileName = att.Filename;
                Base64Code = Convert.ToBase64String(att.GetData());

                FileSize = att.GetData().Length;
            }
        }

        public void LoadFrom(string strYj)
        {

            if (strYj.IndexOf(SjbgConfig.FuHaoKaiShi) != 0)
            {
                FileName = "";
                Base64Code = "";
                FileSize = -1;
            }
            else
            {
                strYj = strYj.Substring(strYj.IndexOf(SjbgConfig.FuHaoKaiShi) + SjbgConfig.FuHaoKaiShi.Length, strYj.Length - strYj.IndexOf(SjbgConfig.FuHaoKaiShi) - SjbgConfig.FuHaoKaiShi.Length);
                string[] str = strYj.Split(new string[] { SjbgConfig.FuHaoFenGe }, 3, StringSplitOptions.None);
                FileSize = Convert.ToInt32(str[0]);
                FileName = str[1];
                if (str.Length > 2)
                {
                    Base64Code = str[2];
                }
                else
                {
                    Base64Code = "";
                }
            }
        }

        public override string ToString()
        {
            return ToString(true);
        }


        public string ToString(bool infoOnly)
        {
            string strYj = SjbgConfig.FuHaoKaiShi + FileSize.ToString() + SjbgConfig.FuHaoFenGe + FileName;
            if (infoOnly == false) strYj += SjbgConfig.FuHaoFenGe + Base64Code;

            return strYj;
        }
    }

    public class YouJianSimple
    {
        string from;
        public int Size { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public bool HasAttachements { get; set; }
        public string From
        {
            get { return from; }
            set
            {
                if (value.IndexOf(SjbgConfig.FuHaoKaiShi) == 0)
                //格式正确
                {
                    int indexOfDisplayName = value.IndexOf(SjbgConfig.FuHaoFenGe) + SjbgConfig.FuHaoFenGe.Length;

                    if (indexOfDisplayName < value.Length)
                    //有displayname
                    {
                        from = value.Substring(indexOfDisplayName, value.Length - indexOfDisplayName);
                        from += "(" + value.Substring(SjbgConfig.FuHaoKaiShi.Length, indexOfDisplayName - SjbgConfig.FuHaoFenGe.Length - 2) + ")";
                    }
                    else
                    //没有displayname
                    {
                        from = value.Substring(SjbgConfig.FuHaoKaiShi.Length, value.Length - SjbgConfig.FuHaoKaiShi.Length - 2);
                    }
                }
                else
                {
                    from = value;
                }
            }
        }
        public string Uid { get; set; }
        public int Importance { get; set; }
    }

    public class YouJian
    {
        public int Size { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Date { get; set; }
        public bool IsBodyHtml { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string ReplyTo { get; set; }
        public string Attachments { get; set; }
        public string From { get; set; }
        public string Sender { get; set; }
        public string MessageID { get; set; }
        public string Uid { get; set; }
        public int Importance { get; set; }

        public YouJian()
        {
            Size = -1;
            Subject = "";
            IsBodyHtml = false;
            To = "";
            Cc = "";
            Bcc = "";
            ReplyTo = "";
            Attachments = "";
            From = "";
            Sender = "";
            MessageID = "";
            Uid = "";

            Importance = 3;
        }

        public YouJianSimple ToSimple()
        {
            YouJianSimple simple = new YouJianSimple();
            simple.Size = Size;
            simple.Subject = Subject;
            simple.From = From;
            simple.Importance = Importance;
            simple.Uid = Uid;
            simple.Date = Date;
            simple.HasAttachements = (Attachments.Length > 0);
            return simple;
        }

        string AddressToString(YouJianDiZhi[] yjdz)
        {
            string strYjdz = "";
            if (yjdz == null) return strYjdz;
            for (int i = 0; i < yjdz.Length; i++)
            {
                strYjdz += yjdz[i].ToString();
                if (i < yjdz.Length - 1) strYjdz += SjbgConfig.FuHaoYouJianDiZhi;
            }
            return strYjdz;
        }

        string AddressToString(List<MailAddress> ma)
        {
            string strYjdz = "";
            if (ma == null) return strYjdz;
            YouJianDiZhi[] yjdz = new YouJianDiZhi[ma.Count];
            for (int i = 0; i < ma.Count; i++)
            {
                yjdz[i] = new YouJianDiZhi();
                yjdz[i].LoadFrom(ma[i]);
            }
            strYjdz = AddressToString(yjdz);
            return strYjdz;
        }

        YouJianDiZhi[] StringToAddress(string strYjdz)
        {
            if (strYjdz.IndexOf(SjbgConfig.FuHaoYouJianDiZhi) <= 0) return null;
            string[] strYjdzs = strYjdz.Split(new string[] { SjbgConfig.FuHaoYouJianDiZhi }, StringSplitOptions.None);
            YouJianDiZhi[] yjdz = new YouJianDiZhi[strYjdzs.Length];
            for (int i = 0; i < strYjdzs.Length; i++)
            {
                yjdz[i] = new YouJianDiZhi();
                yjdz[i].LoadFrom(strYjdzs[i]);
            }
            return yjdz;
        }

        YouJianFuJian[] StringToAttachment(string strYjfj)
        {
            if (strYjfj.IndexOf(SjbgConfig.FuHaoYouJianDiZhi) <= 0) return null;
            string[] strYjfjs = strYjfj.Split(new string[] { SjbgConfig.FuHaoYouJianDiZhi }, StringSplitOptions.None);
            YouJianFuJian[] yjfj = new YouJianFuJian[strYjfjs.Length];
            for (int i = 0; i < strYjfjs.Length; i++)
            {
                yjfj[i] = new YouJianFuJian();
                yjfj[i].LoadFrom(strYjfjs[i]);
            }
            return yjfj;
        }


        string AttachmentToString(YouJianFuJian[] yjfj)
        {
            return AttachmentToString(yjfj, false);
        }

        public string AttachmentToString(YouJianFuJian[] yjfj, bool infoOnly)
        {
            string strYjfj = "";
            if (yjfj == null) return strYjfj;
            for (int i = 0; i < yjfj.Length; i++)
            {
                strYjfj += yjfj[i].ToString(infoOnly);
                if (i < yjfj.Length - 1) strYjfj += SjbgConfig.FuHaoYouJianDiZhi;
            }
            return strYjfj;
        }

        string AttachmentToString(List<AE.Net.Mail.Attachment> yjfj)
        {
            return AttachmentToString(yjfj, true);
        }

        string AttachmentToString(List<AE.Net.Mail.Attachment> atts, bool infoOnly)
        {
            string strYjfj = "";
            if (atts == null) return strYjfj;
            YouJianFuJian[] yjfj = new YouJianFuJian[atts.Count];
            for (int i = 0; i < atts.Count; i++)
            {
                yjfj[i] = new YouJianFuJian();
                yjfj[i].LoadFrom(atts[i]);
                strYjfj += yjfj[i].ToString(infoOnly);
                if (i < yjfj.Length - 1) strYjfj += SjbgConfig.FuHaoYouJianDiZhi;
            }
            return strYjfj;
        }

        public YouJian(int size, string sub, string body, bool isBodyHtml, YouJianDiZhi[] to, YouJianDiZhi[] cc, YouJianDiZhi[] bcc, YouJianDiZhi[] replyto, YouJianFuJian[] att, YouJianDiZhi from, YouJianDiZhi sender, int mp, string mid, string uid)
        {
            Size = size;
            Subject = sub;
            Body = body;
            IsBodyHtml = isBodyHtml;

            To = AddressToString(to);
            Cc = AddressToString(cc);
            Bcc = AddressToString(bcc);
            ReplyTo = replyto.ToString();
            Attachments = AttachmentToString(att);
            From = from.ToString();
            Sender = sender.ToString();
            MessageID = mid;
            Uid = uid;
            Importance = mp;
        }

        public System.Net.Mail.MailMessage ToSystemMail()
        {
            System.Net.Mail.MailMessage mm = new AE.Net.Mail.MailMessage();
            mm.Subject = Subject;
            mm.IsBodyHtml = IsBodyHtml;
            mm.Body = Body;
            switch (Importance)
            {
                case 1:
                    mm.Priority = System.Net.Mail.MailPriority.Low;
                    break;
                case 5:
                    mm.Priority = System.Net.Mail.MailPriority.High;
                    break;
                default:
                    mm.Priority = System.Net.Mail.MailPriority.Normal;
                    break;
            }
            YouJianDiZhi yjdz = new YouJianDiZhi();
            yjdz.LoadFrom(From);
            mm.From = yjdz.ToMailAddress();
            YouJianDiZhi[] yjdzs = StringToAddress(To);
            foreach (YouJianDiZhi dj in yjdzs)
            {
                mm.To.Add(dj.ToMailAddress());
            }
            YouJianFuJian[] yjfjs = StringToAttachment(Attachments);
            foreach (YouJianFuJian fj in yjfjs)
            {
                if (fj.FileName != string.Empty)
                {
                    mm.Attachments.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(System.Convert.FromBase64String(fj.Base64Code)), fj.FileName));
                }
            }
            return mm;
        }

        public void loadFrom(AE.Net.Mail.MailMessage mm)
        {
            if (mm == null) return;
            Size = mm.Size;
            Subject = mm.Subject;
            Body = mm.Body;
            YouJianDiZhi yjdz = new YouJianDiZhi();
            yjdz.LoadFrom(mm.From);
            From = yjdz.ToString();
            yjdz.LoadFrom(mm.Sender);
            Sender = yjdz.ToString();

            Uid = mm.Uid;
            Date = mm.Date.ToString("yyyy-MM-dd HH:mm:ss");
            MessageID = mm.MessageID;
            switch (mm.Importance)
            {
                case AE.Net.Mail.MailPriority.High:
                    Importance = 5;
                    break;
                case AE.Net.Mail.MailPriority.Low:
                    Importance = 1;
                    break;
                default:
                    Importance = 3;
                    break;
            }



            To = AddressToString(mm.To.ToList());
            Cc = AddressToString(mm.Cc.ToList());
            Bcc = AddressToString(mm.Bcc.ToList());

            ReplyTo = AddressToString(mm.ReplyTo.ToList());

            Attachments = AttachmentToString(mm.Attachments.ToList());

        }
    }

    public class YouJianSend
    {
        public string Body { get; set; }
        public string Subject { get; set; }
    }

    public class WenJianJia
    {
        public string Name { get; set; }
        public int NumNewMsg { get; set; }
        public int NumMsg { get; set; }
        public int NumUnSeen { get; set; }
        public int UIDValidity { get; set; }
        public int UIDNext { get; set; }
        private string TrueName;

        public static string NameToTrueName(string strName)
        {
            string strTrueName;
            switch (strName)
            {
                case "收件箱":
                    strTrueName = "INBOX";
                    break;
                case "已发送":
                    strTrueName = "Sent";
                    break;
                case "草稿箱":
                    strTrueName = "Drafts";
                    break;
                case "已删除":
                    strTrueName = "Trash";
                    break;
                case "垃圾邮件":
                    strTrueName = "Spam";
                    break;
                default:
                    strTrueName = strName;
                    break;

            }
            return strTrueName;
        }

        public void TrueNameToName(string strTrueName)
        {
            switch (strTrueName)
            {
                case "INBOX":
                    Name = "收件箱";
                    break;
                case "Sent":
                    Name = "已发送";
                    break;
                case "Drafts":
                    Name = "草稿箱";
                    break;
                case "Trash":
                    Name = "已删除";
                    break;
                case "Spam":
                    Name = "垃圾邮件";
                    break;
                default:
                    Name = strTrueName;
                    break;

            }

        }

        public WenJianJia()
        {
            TrueName = "";
            Name = "";
            NumNewMsg = -1;
            NumMsg = -1;
            NumUnSeen = -1;
            UIDValidity = -1;
            UIDNext = -1;
        }
        public WenJianJia(string name, int numNew, int num, int unseen, int uidV, int uidN)
        {
            TrueName = name;
            TrueNameToName(name);
            NumNewMsg = numNew;
            NumMsg = num;
            NumUnSeen = unseen;
            UIDValidity = uidV;
            UIDNext = uidN;
        }

        public void loadFrom(Mailbox mb)
        {
            TrueName = mb.Name;
            TrueNameToName(mb.Name);
            NumNewMsg = mb.NumNewMsg;
            NumMsg = mb.NumMsg;
            NumUnSeen = mb.NumUnSeen;
            UIDValidity = mb.UIDValidity;
            UIDNext = mb.UIDNext;
        }

    }


    //public class YouJianTKMP
    //{
    //    string serverIp = "10.99.81.68";
    //    string domain = "xxjwd.com";
    //    int pop3Port = 110;
    //    int smtpPort = 25;
    //    string username;
    //    string password;
    //    System.Net.IPAddress address;
    //    TKMP.Net.IPopLogon logon;
    //    TKMP.Net.PopClient pop;
    //    public YouJianTKMP(string username, string password)
    //    {
    //        this.username = username;
    //        this.password = password;
    //        address = System.Net.Dns.GetHostByName(serverIp).AddressList[0];
    //        logon = new TKMP.Net.BasicPopLogon(username, password);
    //        pop = new TKMP.Net.PopClient(logon, address, pop3Port);
    //    }

    //    /// <summary>
    //    /// 获取邮件列表基本信息
    //    /// </summary>
    //    /// <param name="start">开始序号</param>
    //    /// <param name="count">数量</param>
    //    /// <param name="asc">是否升序排列</param>
    //    /// <param name="flag">阅读标记:1只选未读，2只选已读,3所有邮件（赞不启用）</param>
    //    /// <returns></returns>
    //    public YouJianSimple[] getMailList(int start, int count, bool asc, int flag)
    //    {
    //        if (!pop.Connect())
    //        {
    //            return null;
    //        }

    //        int mailcount = pop.MailDatas.Length;
    //        if (start > mailcount) return null;
    //        YouJianSimple[] yjss = new YouJianSimple[mailcount - start + 1 > count ? count : mailcount - start + 1];

    //        int shuliang = yjss.Length;
    //        int jishu = asc ? start : mailcount - start + 1;
    //        int bujin = asc ? 1 : -1;
    //        for (int i = 0; i < shuliang; i++)
    //        {
    //            yjss[i] = new YouJianSimple();
    //            yjss[i].Uid = jishu.ToString();
    //            TKMP.Net.MailData Mail = pop.MailDatas[jishu - 1];
    //            jishu = jishu + bujin;
    //            yjss[i].Size = Mail.Length;
    //            if (!Mail.ReadHeader())
    //            {
    //                return null;
    //            }
    //            else
    //            {
    //                System.IO.Stream Header = Mail.HeaderStream;
    //                TKMP.Reader.MailReader reader = new TKMP.Reader.MailReader(Header, true);
    //                yjss[i].Subject = reader.HeaderCollection["Subject"];
    //                string from = reader.HeaderCollection["From"];

    //                int pos1 = from.LastIndexOf("<");
    //                int pos2 = from.LastIndexOf(">");
    //                if (pos1 != -1 && pos2 != -1)
    //                {
    //                    from = from.Substring(pos1 + 1, pos2 - pos1 - 1);
    //                }
    //                yjss[i].From = from;
    //                string date = reader.HeaderCollection["Date"];
    //                try
    //                {
    //                    date = Convert.ToDateTime(date).ToString("yyyy-MM-dd HH:mm:ss");
    //                }
    //                catch
    //                {

    //                }
    //                yjss[i].Date = date;
    //                if (reader.HeaderCollection["Importance"] == null) yjss[i].Importance = 3;
    //                else
    //                {
    //                    switch (reader.HeaderCollection["Importance"].ToLower())
    //                    {
    //                        case "low":
    //                            yjss[i].Importance = 1;
    //                            break;
    //                        case "normal":
    //                            yjss[i].Importance = 3;
    //                            break;
    //                        case "high":
    //                            yjss[i].Importance = 5;
    //                            break;
    //                        default:
    //                            yjss[i].Importance = 5;
    //                            break;
    //                    }

    //                }
    //                if (!Mail.ReadBody()) return null;
    //                System.IO.Stream body = Mail.DataStream;
    //                TKMP.Reader.MailReader bodyreader = new TKMP.Reader.MailReader(body, false);
    //                yjss[i].HasAttachements = bodyreader.FileCount > 0;

    //            }
    //        }
    //        pop.Close();
    //        return yjss;
    //    }


    //    public YouJian getMail(int muid)
    //    {
    //        if (!pop.Connect())
    //        {
    //            return null;
    //        }
    //        int mailcount = pop.MailDatas.Length;
    //        if (muid > mailcount) return null;
    //        TKMP.Net.MailData Mail = pop.MailDatas[muid-1];
    //        if (!Mail.ReadHeader())
    //        {
    //            return null;
    //        }
    //        else if (!Mail.ReadBody())
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            System.IO.Stream body = Mail.DataStream;
    //            TKMP.Reader.MailReader bodyreader = new TKMP.Reader.MailReader(body, false);
    //            YouJian yj = new YouJian();
    //            yj.Uid = muid.ToString();
    //            yj.Size = Mail.Length;
    //            string date = bodyreader.HeaderCollection["Date"];
    //            if (date == null)
    //            {
    //                date = "未知时间";
    //            }
    //            else
    //            {
    //                date = Convert.ToDateTime(date).ToString("yyyy-MM-dd HH:mm:ss");
    //            }
    //            yj.Date = date;
    //            yj.Body = bodyreader.MainText;
    //            string from = bodyreader.HeaderCollection["From"];
    //            int pos1 = from.LastIndexOf("<");
    //            int pos2 = from.LastIndexOf(">");
    //            if (pos1 != -1 && pos2 != -1)
    //            {
    //                from = from.Substring(pos1 + 1, pos2 - pos1 - 1);
    //            }
    //            yj.From = from;
    //            yj.To = bodyreader.HeaderCollection["To"];
    //            yj.Cc = bodyreader.HeaderCollection["Cc"];
    //            if (yj.Cc == null) yj.Cc = "";
    //            yj.Subject = bodyreader.HeaderCollection["Subject"];
    //            if (bodyreader.HeaderCollection["Importance"] == null) yj.Importance = 3;
    //            else
    //            {
    //                switch (bodyreader.HeaderCollection["Importance"].ToLower())
    //                {
    //                    case "low":
    //                        yj.Importance = 1;
    //                        break;
    //                    case "normal":
    //                        yj.Importance = 3;
    //                        break;
    //                    case "high":
    //                        yj.Importance = 5;
    //                        break;
    //                    default:
    //                        yj.Importance = 5;
    //                        break;
    //                }

    //            }

    //            if (bodyreader.FileCount == 0) yj.Attachments = "";
    //            else
    //            {
    //                YouJianFuJian[] yjfjs = new YouJianFuJian[bodyreader.FileCount];
    //                for (int i = 0; i < bodyreader.FileCount; i++)
    //                {
    //                    yjfjs[i] = new YouJianFuJian();
    //                    yjfjs[i].FileName = bodyreader.FileCollection[i].FileName;
    //                    yjfjs[i].FileSize = bodyreader.FileCollection[i].FileSize;
    //                    //byte[] bytes = bodyreader.FileCollection[i].GetData(1,bodyreader.FileCollection[i].FileSize);
    //                    //string base64 = Convert.ToBase64String(bytes);
    //                    //yjfjs[i].Base64Code = base64;
    //                }
    //                yj.Attachments = yj.AttachmentToString(yjfjs, true);
    //            }
    //            return yj;
    //        }
            
    //    }
    


    //    public INT sendMail(string subject,string body,string to,string cc,string bcc,string attachment)
    //    {
    //        return new INT(1);
    //    }


    //}


    #region 2016自制邮件系统

    public class YouJian2016
    {
        public int ID { get; set; }
        public int Size { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Date { get; set; }
        public bool IsBodyHtml { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Attachments { get; set; }
        public bool IsRead { get; set; }
    }

    public class YouJianList2016
    {
        public int ID { get; set; }
        public string Size { get; set; }
        public string Subject { get; set; }
        public string Date { get; set; }
        public string From { get; set; }
        public bool HasAttachment { get; set; }
        public bool IsRead { get; set; }
    }

    #endregion
}