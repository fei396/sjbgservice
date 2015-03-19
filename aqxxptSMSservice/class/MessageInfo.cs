using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ImApiDotNet;
namespace aqxxptSMSservice
{

    public class MessagesToSend
    {
        public  List<MessageToSend> Messages {get;set;}

        public MessagesToSend()
        {
            Messages = null;
        }

        public void getMessage()
        {
            DataTable dt = DAL.getMessageToSend();
            if (dt.TableName.Equals("getMessageToSend") && dt.Rows.Count > 0)
            {
                long lastSmId = -1;
                Messages = new List<MessageToSend>();
                MessageToSend message = new MessageToSend();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    long smId = Convert.ToInt64(dt.Rows[i]["smId"]);
                    if (smId != lastSmId)
                    {
                        message = new MessageToSend();
                        message.SmID = smId;
                        message.Content = Convert.ToString(dt.Rows[i]["txt"]);
                        message.Mobile.Add(Convert.ToString(dt.Rows[i]["mobile"]));
                        Messages.Add(message);
                    }
                    else
                    {
                        message.Mobile.Add(Convert.ToString(dt.Rows[i]["mobile"]));
                    }
                    lastSmId = smId;
                }
            }
            else
            {
                Messages = null;
            }
        }
    }

	public class MessageToSend
	{
        public List<string> Mobile { get; set; }
        public string Content { get; set; }
        public long SmID { get; set; }

        public MessageToSend()
        {
            Mobile = new List<string>();
            Content = "";
            SmID = -1;
        }
	}

    public class MessageRPT
    {
        public List<RPTItem> Items { get; set; }

        public MessageRPT()
        {
            Items = null;
        }

        internal void doReceive()
        {
            if (Items == null) return;
            foreach (RPTItem item in Items)
            {
                long smId = item.getSmID();
                string mobile = item.getMobile();
                string rptTime = item.getRptTime();
                DAL.ReceiveRPT(smId, mobile, rptTime);
            }
        }
    }
    public class MessageRecieve
    {
        public List<MOItem> Items { get; set; }

        public MessageRecieve()
        {
            Items = null;
        }

        internal void doReceive()
        {
            if (Items == null) return;
            foreach (MOItem item in Items)
            {
                long smId = item.getSmID();
                string mobile = item.getMobile();
                string rptTime = item.getMoTime();
                string content = item.getContent();
                if (content.Length > 50) content = content.Substring(0, 47) + "...";
                DAL.ReceiveReply(smId,content, mobile, rptTime);
            }
        }
    }
}
