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
using System.Data.SqlClient;
using System.Text;
using System.IO;
using gwxxService;
public partial class DuanYu : System.Web.UI.Page
{
    gwxxWebService s = new gwxxWebService();

    protected void Page_Load(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }


        if (!IsPostBack)
        {
            //string cj = Session["udept"] as string;
            //SqlDataSource1.SelectCommand = "SELECT    xmmc, cj , rygh, work_name , sfhg , kkid FROM V_CQKK_RYCJB  ";
            //if (cj != "_所有") SqlDataSource1.SelectCommand += " where cj='" + cj + "'";

            //SqlDataSource1.SelectCommand += " order by  xmmc,cj,rygh";

            s.SjbgSoapHeaderValue = Security.getSoapHeader();
            Security.SetCertificatePolicy();


            getData();
        }
    }


    private void getData()
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        Security.SetCertificatePolicy();
        int uid = 0;
        try
        {
            uid = Convert.ToInt32(user.GongHao);
        }
        catch
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        GongWenZiDingYiDuanYu[] dyList = s.getZiDingYiDuanYu(uid, true);
        
        gvList.DataSource = dyList;

        gvList.DataBind();

        //Label lblcurPage = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lblcurPage");

        //Label lblPageCount = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lblPageCount");
        //lblcurPage.Text = page.ToString();
        //lblPageCount.Text = maxPage.ToString();
        //LinkButton cmdFirstPage = (LinkButton)GridView1.BottomPagerRow.Cells[0].FindControl("cmdFirstPage");
        //LinkButton cmdPreview = (LinkButton)GridView1.BottomPagerRow.Cells[0].FindControl("cmdPreview");
        //LinkButton cmdNext = (LinkButton)GridView1.BottomPagerRow.Cells[0].FindControl("cmdNext");
        //LinkButton cmdLastPage = (LinkButton)GridView1.BottomPagerRow.Cells[0].FindControl("cmdLastPage");
        //if (page == 1)
        //{
        //    cmdFirstPage.Enabled = false;
        //    cmdPreview.Enabled = false;
        //    if (maxPage > 1)
        //    {
        //        cmdNext.Enabled = true;
        //        cmdLastPage.Enabled = true;
        //    }
        //    else
        //    {
        //        cmdNext.Enabled = false;
        //        cmdLastPage.Enabled = false;
        //    }
        //}
        //else if (page >= maxPage)
        //{
        //    cmdFirstPage.Enabled = true;
        //    cmdPreview.Enabled = true;
        //    cmdNext.Enabled = false;
        //    cmdLastPage.Enabled = false;
        //}
        //else
        //{
        //    cmdFirstPage.Enabled = true;
        //    cmdPreview.Enabled = true;
        //    cmdNext.Enabled = true;
        //    cmdLastPage.Enabled = true;
        //}
    }






    //protected void First_Click(object sender, EventArgs e)
    //{
    //    getData(1);
    //}

    //protected void Last_Click(object sender, EventArgs e)
    //{
    //    getData(maxPage);
    //}
    //protected void Pre_Click(object sender, EventArgs e)
    //{
    //    getData(cpage - 1);
    //}
    //protected void Next_Click(object sender, EventArgs e)
    //{
    //    getData(cpage + 1);
    //}
    //protected void Custom_Click(object sender, EventArgs e)
    //{
    //    int pcount = Convert.ToInt32(txtGoPage.Text);
    //    if (pcount > maxPage) pcount = maxPage;
    //    if (pcount < 1) pcount = 1;
    //    txtGoPage.Text = pcount.ToString();
    //    getData(pcount);
    //}


    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        //GridView1.PageIndex = e.NewPageIndex;

    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        try
        {
            //GridView1.PageIndex = Convert.ToInt32(txtGoPage.Text) - 1;
        }
        catch
        {
            Response.Write(" <script> alert( '请输入正确的数字！ ') </script> ");
        }
    }

    protected void SqlDataSource1_Selected(object sender, SqlDataSourceStatusEventArgs e)
    {
        //TextBox tb_gopage = (TextBox)GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");

        //GridView1.Caption = "本次共查询到记录" + e.AffectedRows.ToString() + "条";
        //Label lb = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lballcount");
        //lb.Text = e.AffectedRows.ToString();
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string dynr = txtDuanYu.Text.Trim();
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        Security.SetCertificatePolicy();
        int uid = 0;
        try
        {
            uid = Convert.ToInt32(user.GongHao);
        }
        catch
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        INT result = s.addDuanYu(uid, dynr);
        if (result.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "添加成功", "alert('添加自定义短语成功')", true);
            getData();

        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "添加失败", "alert('添加自定义短语失败：" + result.Message +"')" , true);
        }
    }
    protected void gvList_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvList.EditIndex = e.NewEditIndex;
        getData();

    }
    protected void gvList_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int id=0;
        try
        {
            id = Convert.ToInt32(e.Keys["ID"]);
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "更新失败", "alert('更新自定义短语失败：" + ex.Message + "')", true);
            getData();
            return;
        }
        string newTxt = e.NewValues[0].ToString();
        INT result = s.updateDuanYu(id, newTxt);
        if (result.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "更新成功", "alert('更新自定义短语成功')", true);
            gvList.EditIndex = -1;

        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "更新失败", "alert('更新自定义短语失败：" + result.Message + "')", true);
        }
        getData();
    }
    protected void gvList_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvList.EditIndex = -1;
        getData();
    }
    protected void gvList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int id = 0;
        try
        {
            id = Convert.ToInt32(e.Keys["ID"]);
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "删除失败", "alert('删除自定义短语失败：" + ex.Message + "')", true);
            getData();
            return;
        }
        INT result = s.deleteDuanYu(id);
        if (result.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "删除成功", "alert('删除自定义短语成功')", true);


        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "删除失败", "alert('删除自定义短语失败：" + result.Message + "')", true);
        }
        getData();
    }
}
