using System;
using System.Web;

namespace XxjwdSSO
{
    public class XxjwdSSO
    {
        readonly string _ip;
        readonly string _mac;
        readonly string _userAgent;
        int _yyxtid;

        public XxjwdSSO(HttpRequest request)
        {
            _ip = request.UserHostAddress;
            _mac = GetCustomerMac(_ip);
            _userAgent = request.UserAgent;
            _yyxtid = 0;
            
        }

        public XxjwdSSO(HttpRequest request, int yyxtId)
        {
            _ip = request.UserHostAddress;
            _mac = GetCustomerMac(_ip);
            _userAgent = request.UserAgent;
            _yyxtid = yyxtId;
        }

        /// <summary>
        /// 不限应用系统，获取一个新的验证码
        /// </summary>
        /// <param name="workno">要传递的工号</param>
        /// <returns>生成的验证码，用于web页面传递的验证参数。如果返回null，说明在生成验证码时出错。如Response.Redirect("target.aspx?code=" + ....) </returns>
        public string GetNewVerifyCode(string workno)
        {
            return GetNewVerifyCode(workno, 0);
        }

        /// <summary>
        /// 不限应用系统，获取一个新的验证码
        /// </summary>
        /// <param name="workno">要传递的工号</param>
        /// <param name="yyxtId">应用系统ID号</param>
        /// <returns>生成的验证码，用于web页面传递的验证参数。如果返回null，说明在生成验证码时出错。如Response.Redirect("target.aspx?code=" + ....) </returns>
        public string GetNewVerifyCode(string workno,int yyxtId)
        {
            string guid,base64;
            do
            {
                var g = Guid.NewGuid();
                guid = g.ToString().ToUpper();
                base64 =  Convert.ToBase64String(g.ToByteArray());
            }
            while (base64.Contains("/") || base64.Contains("+"));
            if (BLL.InsertSso(guid, _mac, _ip, _userAgent, workno ,yyxtId) >= 1)
            {
                return base64;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 检验该验证码是否合法。
        /// </summary>
        /// <param name="code">待检验的验证码</param>
        /// <returns>如果合法，返回值的Result为1，此时WorkNo字段为工号。Result其它值说明-1:Guid不存在;-2:已过期;-3验证不通过;-4:该验证码已经使用;-5:应用系统不对应;-6:数据库错误;1:验证通过</returns>
        public VerifyResult VerifyCode(string code )
        {
            return BLL.VerifyRedirect(code, _mac, _ip, _userAgent, 0);
        }

        /// <summary>
        /// 检验该验证码是否合法。
        /// </summary>
        /// <param name="code">待检验的验证码</param>
        /// <param name="yyxtId">应用系统ID号</param>
        /// <returns>如果合法，返回值的Result为1，此时WorkNo字段为工号。Result其它值说明0:该用户在该系统没有权限;-1:Guid不存在;-2:已过期;-3验证不通过;-4:该验证码已经使用;-5:应用系统不对应;-6:数据库错误;1:验证通过</returns>
        public VerifyResult VerifyCode(string code, int yyxtId)
        {
            return BLL.VerifyRedirect(code, _mac, _ip, _userAgent ,yyxtId);
        }

        string GetCustomerMac(string ip)
        {
            return ip;
            //string dirResults = "";
            //System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            //System.Diagnostics.Process proc = new System.Diagnostics.Process();
            //psi.FileName = "nbtstat";
            //psi.RedirectStandardInput = false;
            //psi.RedirectStandardOutput = true;
            //psi.Arguments = "-a " + ip;
            //psi.UseShellExecute = false;
            //proc = System.Diagnostics.Process.Start(psi);
            //dirResults = proc.StandardOutput.ReadToEnd();
            //proc.WaitForExit();

            ////匹配mac地址
            //Match m = Regex.Match(dirResults, "\\w+\\-\\w+\\-\\w+\\-\\w+\\-\\w+\\-\\w\\w");

            ////若匹配成功则返回mac，否则返回找不到主机信息
            //if (m.ToString() != "")
            //{
            //    return m.ToString();
            //}
            //else
            //{
            //    return dirResults;
            //}
        }
    }
}