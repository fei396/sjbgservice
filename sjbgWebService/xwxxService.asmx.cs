using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace sjbgWebService.xwxx
{
	/// <summary>
	/// xwxxService 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://sjbg.xxjwd.org/")]

	[System.ComponentModel.ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	// [System.Web.Script.Services.ScriptService]
	[WebServiceBinding(ConformsTo = WsiProfiles.None)]
	[SoapRpcService]
	public class xwxxService : System.Web.Services.WebService
	{
		[SoapRpcMethod, WebMethod]
		public XinWen[] getXinWen(int xwlx, int ksxh, int count)
		{
			return BLL.GetXinWen(xwlx, ksxh, count);
		}


		[SoapRpcMethod, WebMethod]
		public XinWenLeiXing[] getXinWenLeiXing()
		{
			return BLL.GetXinWenLeiXing();
		}
	}
}
