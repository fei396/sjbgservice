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
        GongWenYongHu gwyh = Session["user"] as GongWenYongHu;
        if (gwyh.Equals(null))
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        if (gwyh.RoleID == 20)
        {
            Response.Redirect("ListGongWenGuiDang.aspx?type=0");
        }
        else
        {
            Response.Redirect("ListGongWen.aspx?type=0");
        }
    }
}
