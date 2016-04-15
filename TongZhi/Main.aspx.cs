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
using gwxxService;

public partial class adminMain : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int uid = Convert.ToInt32(Session["uid"]);
        int role = Convert.ToInt32(Session["role"]);
        if (uid <= 0)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }
        if (role == 1)
        {
            Response.Redirect("ListTongZhi.aspx?type=2");
        }
        else if (role == 4)
        {
            Response.Redirect("ListTongZhi.aspx?type=0");
        }
        else if (role == 2 || role == 3)
        {
            Response.Redirect("ListTongZhi.aspx?type=1");
        }
        else
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
    }
}
