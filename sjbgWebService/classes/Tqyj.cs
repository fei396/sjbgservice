using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sjbgWebService
{
    public class TeQing
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SendTime { get; set; }
        public int NeedReply { get; set; }
        public string SenderName { get; set; }
        public string SenderDept { get; set; }
        public string ReceiverNo { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverDept { get; set; }
        public string ReplyTime { get; set; }
        public string ReplyContent { get; set; }
        public int SendCount { get; set; }
        public int ReplyCount { get; set; }
    }
}