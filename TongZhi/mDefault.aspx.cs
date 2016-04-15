using System;

public partial class mDefault : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        int uid = Convert.ToInt32(Session["uid"]);
        int role = Convert.ToInt32(Session["role"]);
        if (uid <= 0)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        switch (role)
        {
            case 1:
                //段领导
                break;
            case 2:
                //能发不能收
                break;
            case 3:
                //能发能收
                break;
            case 4:
                //能收不能发                       
                break;
        
            default:
                Response.Redirect("error.aspx?errcode=登录失败：用户没有权限");
                return;

        }

        //lblWelcome.Text = "当前用户：" + gwyh.XingMing;
		Response.Write("<script language='javascript' type='text/javascript' src='js/madminMain.js' charset='GB2312'></script>");
		Response.Write("<script language='javascript' type='text/javascript'>window.onload = function() { init("+ role +");}</script>");

    }
}
