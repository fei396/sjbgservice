using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class adminMain : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string s = Session["user"] as string;
        if (s == null || s == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        Response.Redirect("editfile.aspx");

    }
}
