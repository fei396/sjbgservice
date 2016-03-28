using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using aqxxptWebService;
public partial class editfile_detail : Page
{
    private const int pageCount = 20;
    private static int cpage;
    private readonly aqxxptService ss = new aqxxptService();
    private int xxid;
    private int count;
    private int maxPage;

    protected void Page_Load(object sender, EventArgs e)
    {
        var s = Session["user"] as string;
        xxid = Convert.ToInt32(Request["xxid"]);

        count = Convert.ToInt32(Request["count"]);
        maxPage = count/pageCount + 1;
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


            ss.SjbgSoapHeaderValue = Security.GetSoapHeader();
            cpage = 1;
            getData(cpage);
        }
    }


    private void getData(int page)
    {
        Security.SetCertificatePolicy();

        if ((page - 1)*pageCount >= count) page = maxPage;
        cpage = page;
        AqxxDetail[] ads = ss.GetAqxxDetail(xxid, pageCount*(page - 1) + 1, pageCount);
        GridView1.DataSource = ads;
        GridView1.DataBind();

        var lblcurPage = (Label) GridView1.BottomPagerRow.Cells[0].FindControl("lblcurPage");

        var lblPageCount = (Label) GridView1.BottomPagerRow.Cells[0].FindControl("lblPageCount");
        lblcurPage.Text = page.ToString();
        lblPageCount.Text = maxPage.ToString();
        var cmdFirstPage = (LinkButton) GridView1.BottomPagerRow.Cells[0].FindControl("cmdFirstPage");
        var cmdPreview = (LinkButton) GridView1.BottomPagerRow.Cells[0].FindControl("cmdPreview");
        var cmdNext = (LinkButton) GridView1.BottomPagerRow.Cells[0].FindControl("cmdNext");
        var cmdLastPage = (LinkButton) GridView1.BottomPagerRow.Cells[0].FindControl("cmdLastPage");
        if (cpage == 1)
        {
            cmdFirstPage.Enabled = false;
            cmdPreview.Enabled = false;
            cmdNext.Enabled = true;
            cmdLastPage.Enabled = true;
        }
        else if (cpage >= maxPage)
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
        var tb_gopage = (TextBox) GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");
        getData(Convert.ToInt32(tb_gopage.Text));
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
        var tb_gopage = (TextBox) GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");
        try
        {
            GridView1.PageIndex = Convert.ToInt32(tb_gopage.Text) - 1;
        }
        catch
        {
            Response.Write(" <script> alert( '请输入正确的数字！ ') </script> ");
        }
    }

    protected void SqlDataSource1_Selected(object sender, SqlDataSourceStatusEventArgs e)
    {
        //TextBox tb_gopage = (TextBox)GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");

        GridView1.Caption = "本次共查询到记录" + e.AffectedRows + "条";
        //Label lb = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lballcount");
        //lb.Text = e.AffectedRows.ToString();
    }
}