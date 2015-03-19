using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class mDefault : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		
		Security s = Session["sec"] as Security;
		if (s == null)
		{
			Response.Redirect("error.aspx");
		}

		int admin = Convert.ToInt32(Session["usec"]);
		Response.Write("<script language='javascript' type='text/javascript' src='js/madminMain.js' charset='GB2312'></script>");
		Response.Write("<script language='javascript' type='text/javascript'>window.onload = function() { init(" + admin.ToString() + ");}</script>");

    }
}
