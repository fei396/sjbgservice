using gwxxService;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SuoYouWeiQian : System.Web.UI.Page
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

        GongWenLiuZhuan[] gwlz = s.getSuoYouWeiQian(uid, gwid);
        gvList.DataSource = gwlz;
        gvList.DataBind();
    }
    protected void cbSelectAll_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox cbAll = (CheckBox)sender;
        foreach(GridViewRow row in gvList.Rows)
        {
            CheckBox cb = row.FindControl("cbSelect") as CheckBox;
            if (cb != null)
            {
                cb.Checked = cbAll.Checked;
            }
        }
    }
    protected void btnCuiBan_Click(object sender, EventArgs e)
    {
        string[] beiCuiBanRen;
        List<string> bcbrList = new List<string>();
        foreach (GridViewRow row in gvList.Rows)
        {
            CheckBox cb = row.FindControl("cbSelect") as CheckBox;
            if (cb != null)
            {
                if ( cb.Checked )
                {
                    bcbrList.Add(row.Cells[0].Text);
                }
            }
        }
        beiCuiBanRen = bcbrList.ToArray();
        gwxxWebService s = new gwxxWebService();
        int gwid = Convert.ToInt32(Request["gwid"]);
        INT i= s.makeCuiBanByRenYuan(gwid, beiCuiBanRen);
        if (i.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "催办成功", "alert('" + i.Message + "');", true);
            //Response.Redirect("ListGongWen.aspx?type=0");
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "催办失败", "alert('催办失败：" + i.Message + "')", true);
        }
    }
}