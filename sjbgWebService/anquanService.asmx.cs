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
    public class anquanService : System.Web.Services.WebService
    {
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();

        //测酒查询处理数据
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public CjcxData[] getCjcx(string cjcx_data)
        {
            //if (!sjbgHeader.checkValid()) return null;
            return BLL.GetCjcxByNum(cjcx_data);
        }
        //

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BookNrData getBookNR(string id)
        {

            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetBookNr(id);

        }

        ////
        ////电子书名查询处理数据
        //[SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        //[SoapRpcMethod, WebMethod]
        //public BookNameData[] getBookName(string book_Name)
        //{
        //    if (!sjbgHeader.checkValid()) return null;
        //    return BLL.getBookName(book_Name);
        //}
        ////

        //电子书名查询处理数据
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public BookNameData[] getBookName(string bookname_data)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.GetBookName(bookname_data);
        }
    }


}
