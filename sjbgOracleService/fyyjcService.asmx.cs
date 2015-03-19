using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;

namespace sjbgOracleService
{
    /// <summary>
    /// fyyjcService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class fyyjcService : System.Web.Services.WebService
    {
        [WebMethod]
        public DataTable getFyyjc(string jczt)
        {
            DataTable dt = DAL.getFyyjc(jczt);
            if (dt.TableName.Equals("error")) return null;
            else return dt;
        }

        [WebMethod]
        public DataTable getFyyjcZt()
        {
            DataTable dt = DAL.getAllFyyjcZt();
            if (dt.TableName.Equals("error")) return null;
            else return dt;
        }
    }
}
