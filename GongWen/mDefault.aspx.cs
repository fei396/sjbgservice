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

        gwxxService.GongWenYongHu gwyh = Session["user"] as gwxxService.GongWenYongHu;
        if (gwyh == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        switch (gwyh.RoleID)
        {
            case 20:
                //公文处理员
                break;
            case 21:
                //段长、书记
                break;
            case 22:
                //段领导
                break;
            case 23:
                //科室中层干部
                break;
            case 24:
                //车间中层干部
                break;
            case 25:
            case 26:
                //基层管理人员
                break;
            default:
                Response.Redirect("error.aspx?errcode=登录失败：用户没有权限");
                return;

        }

        lblWelcome.Text = "当前用户：" + gwyh.XingMing;
		Response.Write("<script language='javascript' type='text/javascript' src='js/madminMain.js' charset='GB2312'></script>");
		Response.Write("<script language='javascript' type='text/javascript'>window.onload = function() { init("+ gwyh.RoleID +");}</script>");

    }
}
