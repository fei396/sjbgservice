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
public partial class ListGongWenGuiDang : System.Web.UI.Page
{
    gwxxService.gwxxWebService s = new gwxxService.gwxxWebService();
    static int maxPage;
    const int pageCount = 15;
    static int cpage;
    static int allCount;
    protected void Page_Load(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
		{
			Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
		}
        int type = Convert.ToInt32(Request["type"]);
        if (type ==0)
        {
            tableChaXun.Visible = false;
        }
        
		if (!IsPostBack)
		{
			//string cj = Session["udept"] as string;
			//SqlDataSource1.SelectCommand = "SELECT    xmmc, cj , rygh, work_name , sfhg , kkid FROM V_CQKK_RYCJB  ";
			//if (cj != "_所有") SqlDataSource1.SelectCommand += " where cj='" + cj + "'";

			//SqlDataSource1.SelectCommand += " order by  xmmc,cj,rygh";
            
            s.SjbgSoapHeaderValue = Security.getSoapHeader();
            Security.SetCertificatePolicy();

            cpage = 1;
            allCount = 0;
            getData(cpage);
		}
    }

    private void getData(int page)
    {
        getData(page, "", "", "");
    }
    private void getData(int page,string key,string sTime,string ETime)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        int type ;
        try
        {
            type = Convert.ToInt32(Request["type"]);
        }
        catch
        {
            type = 0;
        }
        int uid = Convert.ToInt32(user.GongHao);
        Security.SetCertificatePolicy();
        if (allCount == 0) allCount = s.getGongWenGuiDangCount(uid,type, key, sTime, ETime);
        GongWenGuiDangList[] gwlist = s.getGongWenGuiDangList(uid,type, key, sTime, ETime, (page - 1) * pageCount + 1, pageCount);
        maxPage =  (int)((allCount -0.1) / pageCount) + 1;
        if (page < 1) page = 1;  
        if ((page - 1) * pageCount >= allCount) page = maxPage;
        cpage = page;
        gvList.DataSource = gwlist;

        gvList.DataBind();
       
        //Label lblcurPage = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lblcurPage");

        //Label lblPageCount = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lblPageCount");
        lblcurPage.Text = page.ToString();
        lblPageCount.Text = maxPage.ToString();
        //LinkButton cmdFirstPage = (LinkButton)GridView1.BottomPagerRow.Cells[0].FindControl("cmdFirstPage");
        //LinkButton cmdPreview = (LinkButton)GridView1.BottomPagerRow.Cells[0].FindControl("cmdPreview");
        //LinkButton cmdNext = (LinkButton)GridView1.BottomPagerRow.Cells[0].FindControl("cmdNext");
        //LinkButton cmdLastPage = (LinkButton)GridView1.BottomPagerRow.Cells[0].FindControl("cmdLastPage");
        if (page == 1)
        {
            cmdFirstPage.Enabled = false;
            cmdPreview.Enabled = false;
            if (maxPage > 1)
            {
                cmdNext.Enabled = true;
                cmdLastPage.Enabled = true;
            }
            else
            {
                cmdNext.Enabled = false;
                cmdLastPage.Enabled = false;
            }
        }
        else if (page >= maxPage)
        {
            cmdFirstPage.Enabled = true;
            cmdPreview.Enabled = true;
            cmdNext.Enabled = false;
            cmdLastPage.Enabled = false;
        }
        else
        {
            cmdFirstPage.Enabled = true;
            cmdPreview.Enabled = true;
            cmdNext.Enabled = true;
            cmdLastPage.Enabled = true;
        }
    }
	





    protected void First_Click(object sender, EventArgs e)
    {
        getData(1);
    }

    protected void Last_Click(object sender, EventArgs e)
    {
        getData(maxPage);
    }
    protected void Pre_Click(object sender, EventArgs e)
    {
        getData(cpage - 1);
    }
    protected void Next_Click(object sender, EventArgs e)
    {
        getData(cpage + 1);
    }
    protected void Custom_Click(object sender, EventArgs e)
    {
        int pcount = Convert.ToInt32(txtGoPage.Text);
        if (pcount > maxPage) pcount = maxPage;
        if (pcount < 1) pcount = 1;
        txtGoPage.Text = pcount.ToString();
        getData(pcount);
    }


	protected void GridView1_DataBound(object sender, EventArgs e)
	{

	}
	protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
	{

	}
	protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
	{
		
	}
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
    protected void btnChaXun_Click(object sender, EventArgs e)
    {
        getData(1 ,txtBiaoTi.Text ,txtStart.Text ,txtEnd.Text);
    }
    protected void gvList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int gwid = Convert.ToInt32(e.Keys[0].ToString());
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        int uid = Convert.ToInt32(user.GongHao);
        INT r = s.deleteGongWen2016(uid, gwid);
        if (r.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "delete", "alert('删除公文成功')", true);
            getData(cpage);
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "delete", "alert('删除公文失败：" + r.Message + "')", true);
        }
    }
}
