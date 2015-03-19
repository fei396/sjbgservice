using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sjbgWebService
{
    public class BookNrData
    {
        public string id { get; set; }
        public string Txt { get; set; }
    }
    //public class BookNameData
    //{
    //    public string id { get; set; }
    //    public string Name { get; set; }
    //}

    public class BookNameData
    {
        public string id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }


    public class AQXX
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string SendTime { get; set; }
        public string Sender { get; set; }
        public string SenderNo { get; set; }
        public string SenderName { get; set; }
        public string SetTime { get; set; }
    }

    public class AqxxInfo
    {
        public int XXID { get; set; }
        public string Sender{get;set;}
        public string Title { get; set; }
        public string Content { get; set; }
        public string SendTime { get; set; }
        public int SendCount { get; set; }
        public int ReadCount { get; set; }
        public string Status { get; set; }
        public string Auditor { get; set; }
        public string AuditTime { get; set; }
    }

    public class AqxxDetail
    {
        public string Sender { get; set; }
        public string Title { get; set; }
        public string SendTime { get; set; }
        public string Receiver { get; set; }
        public string ReceiveTime { get; set; }
        public string ReceiverDept { get; set; }
    }
}