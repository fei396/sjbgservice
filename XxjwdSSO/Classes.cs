using System;
using System.Collections.Generic;
using System.Text;

namespace XxjwdSSO
{
    class SSO
    {
        public string Guid{get;set;}
        public string Mac { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreateTime { get; set; }
        public string WorkNo { get; set; }
        public int YyxtID { get; set; }
    }

    public class VerifyResult
    {
        /// <summary>
        /// -1:Guid不存在;-2:已过期;-3验证不通过;1:验证通过
        /// </summary>
        public int Result { get; set; }
        public string WorkNo { get; set; }
        public string Message { get; set; }
        public int Uid { get; set; }
        public VerifyResult(int r, string w,int u,string m)
        {
            Result = r;
            WorkNo = w;
            Message = m;
            Uid = u;
        }
        public VerifyResult(int r)
        {
            Result = r;
            WorkNo = string.Empty;
            Message = string.Empty;
            Uid = -1;
        }
    }
}
