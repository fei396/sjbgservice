using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sjbgWebService
{
    public class DutyRoom
    {
        public int ID { get; set; }
        public string WeiZhi { get; set; }
        public int CheJianID { get; set; }
        public string CheJian { get; set; }
    }

    public class SendFile
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string ExtName { get; set; }
        public string FileDesc { get; set; }
        public string Sender { get; set; }
        public string SendTime { get; set; }
    }

    public class SentFileList
    {
        public int FID { get; set; }
        public string FileName { get; set; }
        public string SendTime { get; set; }
        public string Sender { get; set; }
        public int AllCount { get; set; }
        public int ReceiveCount { get; set; }
    }

    public class SentFileDetail
    {
        
        public string FileName { get; set; }
        public string SendTime { get; set; }
        public string Receiver { get; set; }
        public string DutyRoom { get; set; }
        public string ReceiveTime { get; set; }
    }

    public class ReceiveFile
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string ExtName { get; set; }
        public string FileDesc { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string SendTime { get; set; }
        public string SenderDept { get; set; }
        public int DutyRoomID { get; set; }
        public string DutyRoom { get; set; }
        public int DutyRoomDeptId { get; set; }
        public string DutyRoomDept { get; set; }
        public string Receiver { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiveTime { get; set; }
        public int FileDrId { get; set; }
    }
}