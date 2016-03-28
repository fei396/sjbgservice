using System;
using System.Web.UI;

public partial class adminMain : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var s = Session["user"] as string;
        if (s == null || s == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        Response.Redirect("editfile.aspx");
    }
}