using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using aqxxptWebService;
public partial class EditfileDetail : Page
{
    private const int PageCount = 20;
    private static int _cpage;
    private readonly aqxxptService _s = new aqxxptService();
    private int xxid;
    private int count;
    private int maxPage;

    protected void Page_Load(object sender, EventArgs e)
    {
        var s = Session["user"] as string;
        xxid = Convert.ToInt32(Request["xxid"]);

        count = Convert.ToInt32(Request["count"]);
        maxPage = count/PageCount + 1;
        if (string.IsNullOrEmpty(s))
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }


        if (!IsPostBack)
        {

            _s.SjbgSoapHeaderValue = Security.GetSoapHeader();
            _cpage = 1;
            GetData(_cpage ,0);
        }
    }


    private void GetData(int page ,int type)
    {
        Security.SetCertificatePolicy();

        if ((page - 1)*PageCount >= count) page = maxPage;
        _cpage = page;
        AqxxDetail[] ads = _s.GetAqxxDetail(xxid, type,PageCount*(page - 1) + 1, PageCount);
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
        if (_cpage == 1)
        {
            cmdFirstPage.Enabled = false;
            cmdPreview.Enabled = false;
            cmdNext.Enabled = true;
            cmdLastPage.Enabled = true;
        }
        else if (_cpage >= maxPage)
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
        int type = Convert.ToInt32(ddlStatus.SelectedValue);
        GetData(1,type);
    }

    protected void Last_Click(object sender, EventArgs e)
    {
        int type = Convert.ToInt32(ddlStatus.SelectedValue);
        GetData(maxPage,type);
    }

    protected void Pre_Click(object sender, EventArgs e)
    {
        int type = Convert.ToInt32(ddlStatus.SelectedValue);
        GetData(_cpage - 1,type);
    }

    protected void Next_Click(object sender, EventArgs e)
    {
        int type = Convert.ToInt32(ddlStatus.SelectedValue);
        GetData(_cpage + 1,type);
    }

    protected void Custom_Click(object sender, EventArgs e)
    {
        var tbGopage = (TextBox) GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");
        int type = Convert.ToInt32(ddlStatus.SelectedValue);
        GetData(Convert.ToInt32(tbGopage.Text),type);
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
        TextBox tbGopage = (TextBox) GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");
        try
        {
            GridView1.PageIndex = Convert.ToInt32(tbGopage.Text) - 1;
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

    protected void btnChaXun_Click(object sender, EventArgs e)
    {
        int type = Convert.ToInt32(ddlStatus.SelectedValue);
        GetData(1, type);
    }
}