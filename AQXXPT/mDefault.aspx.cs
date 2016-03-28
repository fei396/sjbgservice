using System;
using System.Web.UI;

public partial class mDefault : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var s = Session["user"] as string;
        if (s == null || s == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }


        Response.Write(
            "<script language='javascript' type='text/javascript' src='js/madminMain.js' charset='GB2312'></script>");
        Response.Write(
            "<script language='javascript' type='text/javascript'>window.onload = function() { init();}</script>");
    }
}