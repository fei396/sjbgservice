using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace sjbgWebService
{
    /// <summary>
    /// yunyongService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://sjbg.xxjwd.org/")]

    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [SoapRpcService]
    public class yunyongService : System.Web.Services.WebService
    {
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();

        //机车计划处理数据
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public JcjhData[] getJcjh(string jcjh_data)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetJcjhByNum(jcjh_data);
        }
        //

        //人员计划处理数据
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public RyjhData[] getRyjh(string ryjh_data)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetRyjhByNum(ryjh_data);
        }
        //

        //待乘计划处理数据
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public DcjhData[] getDcjh(string dcjh_data)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetDcjhByNum(dcjh_data);
        }
        //


        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public MingPai[] getMingPaiByXianBie(int database ,string line_mode ,int type)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetMingPaiByXianBie(database ,line_mode ,type);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public MingPai[] getMingPaiByGongHao(int database ,int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetMingPaiByUid(database,uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public DaMingPai[] getDaMingPai(int database, string line,int type , string filter, int ksxh,int count)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetDaMingPaiByXianBie(database, line ,type, filter ,ksxh,count);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public XianBie[] getXianBie(int database)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetXianBie(database);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public CanBu[] getCanBu(int uid,string month)
        {
            if (!sjbgHeader.checkValid()) return null;
            return Bll.GetCanBu(uid, month);
        }
    }
}
