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
public partial class editfile : System.Web.UI.Page
{
    aqxxptService.aqxxptService ss = new aqxxptService.aqxxptService();
    int uid, maxPage;
    const int pageCount = 10;
    static int cpage;
    static int allCount;
    protected void Page_Load(object sender, EventArgs e)
    {
        string s = Session["user"] as string;
        uid = Convert.ToInt32(s);
		if (s == null || s == "")
		{
			//Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
		}
		
        
		if (!IsPostBack)
		{
			//string cj = Session["udept"] as string;
			//SqlDataSource1.SelectCommand = "SELECT    xmmc, cj , rygh, work_name , sfhg , kkid FROM V_CQKK_RYCJB  ";
			//if (cj != "_所有") SqlDataSource1.SelectCommand += " where cj='" + cj + "'";

			//SqlDataSource1.SelectCommand += " order by  xmmc,cj,rygh";
            
            ss.SjbgSoapHeaderValue = Security.getSoapHeader();
            Security.SetCertificatePolicy();
            allCount = ss.getAqxxCount(uid);
            if (allCount < 0) return;
            //aqxxptService.AqxxInfo[] aqxxs = ss.getAqxxInfo(uid,0);
            //GridView1.DataSource = aqxxs;
            //GridView1.DataBind();
            cpage = 1;
            getData(cpage);
		}
    }

    private void getData(int page)
    {
        Security.SetCertificatePolicy();

      
        maxPage = (allCount / pageCount) + 1;
        if (page < 1) page = 1;  
        if ((page - 1) * pageCount >= allCount) page = maxPage;
        cpage = page;
        aqxxptService.AqxxInfo[] aqxxs = ss.getAqxxInfos(uid, pageCount * (page - 1) + 1, pageCount);
        GridView1.DataSource = aqxxs;
        GridView1.DataBind();
       
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
            cmdNext.Enabled = true;
            cmdLastPage.Enabled = true;
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
	



    protected void saveBtn_Click(object sender, EventArgs e)
    {
		string fileName = "export.xls";

		StringBuilder sb = new StringBuilder();
		StringWriter sw = new StringWriter(sb);
		HtmlTextWriter htw = new HtmlTextWriter(sw);

		Page page = new Page();
		HtmlForm form = new HtmlForm();

		GridView1.EnableViewState = false;
		GridView1.AllowPaging = false;
		GridView1.DataBind();
		// Deshabilitar la validación de eventos, sólo asp.net 2
		page.EnableEventValidation = false;

		// Realiza las inicializaciones de la instancia de la clase Page que requieran los diseñadores RAD.
		page.DesignerInitialize();

		page.Controls.Add(form);
		form.Controls.Add(GridView1);

		page.RenderControl(htw);

		Response.Clear();
		Response.Buffer = true;
		Response.ContentType = "application/vnd.ms-excel";
		Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
		Response.Charset = "UTF-8";
		Response.ContentEncoding = Encoding.UTF8;
		Response.Write(sb.ToString());
		Response.End();
		GridView1.AllowPaging = true;
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

        getData(Convert.ToInt32(txtGoPage.Text));
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
		GridView1.PageIndex = e.NewPageIndex;
		
	}
	protected void Button1_Click(object sender, EventArgs e)
	{

		try
		{
            GridView1.PageIndex = Convert.ToInt32(txtGoPage.Text) - 1;
		}
		catch
		{
			Response.Write(" <script> alert( '请输入正确的数字！ ') </script> ");
		}
	}

	protected void SqlDataSource1_Selected(object sender, SqlDataSourceStatusEventArgs e)
	{
		//TextBox tb_gopage = (TextBox)GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");
		
		GridView1.Caption = "本次共查询到记录" + e.AffectedRows.ToString() + "条";
		//Label lb = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lballcount");
		//lb.Text = e.AffectedRows.ToString();
	}
}
