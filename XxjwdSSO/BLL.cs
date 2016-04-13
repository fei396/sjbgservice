using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace XxjwdSSO
{
    static class  BLL
    {
        /// <summary>
        /// 过期时间，单位秒
        /// </summary>
        const int expiryTime = 600;
        static string  ToGuidString(string base64)
        {
            byte[] b = Convert.FromBase64String(base64);
            Guid g1 = new Guid(b);
            return g1.ToString().ToUpper();
        }



        internal static int insertSSO(string guid, string mac, string ip, string userAgent, string workno,int yyxtId)
        {
            return DAL.insertSSO(guid, mac, ip, userAgent, workno, yyxtId);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="mac"></param>
        /// <param name="ip"></param>
        /// <param name="userAgent"></param>
        /// <returns>0:该用户在该系统没有权限;-1:Guid不存在;-2:已过期;-3验证不通过;-4:该验证码已经使用;-5:应用系统不对应;-6:数据库错误;1:验证通过</returns>
        internal static VerifyResult verifyRedirect(string base64, string mac, string ip, string userAgent,int yyxtId)
        {
            string guid = ToGuidString(base64);
            DataTable dt = DAL.getSSOByGuid(guid);
            if (dt == null || !dt.TableName.Equals("getSSOByGuid"))
            {
                return new VerifyResult(-1,dt.TableName);
            }
            int isVerified = Convert.ToInt32(dt.Rows[0]["isVerified"]);
            if (isVerified == 1)
            {
                return new VerifyResult(-4); 
            }
            SSO sso = new SSO();
            sso.CreateTime = Convert.ToDateTime(dt.Rows[0]["createTime"]);
            sso.Guid = Convert.ToString(dt.Rows[0]["guid"]);
            sso.IpAddress = Convert.ToString(dt.Rows[0]["ip"]);
            sso.Mac = Convert.ToString(dt.Rows[0]["mac"]);
            sso.UserAgent = Convert.ToString(dt.Rows[0]["userAgent"]);
            sso.WorkNo = Convert.ToString(dt.Rows[0]["workno"]);
            sso.YyxtID = Convert.ToInt32(dt.Rows[0]["yyxtid"]);
            TimeSpan ts =  DAL.getServerTime() - sso.CreateTime;
            if (ts.TotalSeconds > expiryTime)
            {
                return new VerifyResult(-2);
            }
            if (yyxtId != sso.YyxtID)
            {
                return new VerifyResult(-5);
            }
            //if (mac.Equals(sso.Mac) && ip.Equals(sso.IpAddress) && userAgent.Equals(sso.UserAgent))
            if (ip.Equals(sso.IpAddress) && userAgent.Equals(sso.UserAgent))
            {
                int updateStatus = DAL.updateVerifiedStatus(sso.Guid);
                if (updateStatus != 1)
                {
                    return new VerifyResult(-6);
                }
                else
                {
                    string user = DAL.getYyxtUserNo(sso.YyxtID, sso.WorkNo);
                    if (user == null) return new VerifyResult(-6);
                    else if (user.Equals("")) return new VerifyResult(0);
                    else return new VerifyResult(1, sso.WorkNo);
                }
            }
            else
            {
                return new VerifyResult (-3);
            }
        }
    }
}
