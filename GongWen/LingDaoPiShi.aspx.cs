using gwxxService;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class LingDaoPiShi : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        //uid = Convert.ToInt32(user);

        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        if (!IsPostBack)
        {
            getdata();
        }
    }

    private void getdata()
    {
        gwxxWebService s = new gwxxWebService();
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        int uid,gwid;
        uid = Convert.ToInt32(user.GongHao);
        gwid = Convert.ToInt32(Request["gwid"]);
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        GongWenLiuZhuan[] gwlz = s.getLingDaoPiShi(uid, gwid);
        gvList.DataSource = gwlz;
        gvList.DataBind();
    }
}