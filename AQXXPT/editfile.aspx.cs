using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using aqxxptWebService;
public partial class Editfile : Page
{
    private const int pageCount = 10;
    private static int cpage;
    private static int allCount;
    private readonly aqxxptService _s = new aqxxptService();
    private int uid;
    private int maxPage;

    protected void Page_Load(object sender, EventArgs e)
    {
        var s = Session["user"] as string;
        uid = Convert.ToInt32(s);
        if (string.IsNullOrEmpty(s))
        {
            //Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }


        if (!IsPostBack)
        {
            _s.SjbgSoapHeaderValue = Security.GetSoapHeader();
            Security.SetCertificatePolicy();
            allCount = _s.GetAqxxCount(uid);
            if (allCount < 0) return;
            cpage = 1;
            GetData(cpage);
        }
    }

    private void GetData(int page)
    {
        Security.SetCertificatePolicy();


        maxPage = allCount/pageCount + 1;
        if (page < 1) page = 1;
        if ((page - 1)*pageCount >= allCount) page = maxPage;
        cpage = page;
        AqxxInfo[] aqxxs = _s.GetAqxxInfos(uid, pageCount*(page - 1) + 1, pageCount);
        GridView1.DataSource = aqxxs;
        GridView1.DataBind();


        lblcurPage.Text = page.ToString();
        lblPageCount.Text = maxPage.ToString();
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
        var fileName = "export.xls";

        var sb = new StringBuilder();
        var sw = new StringWriter(sb);
        var htw = new HtmlTextWriter(sw);

        var page = new Page();
        var form = new HtmlForm();

        GridView1.EnableViewState = false;
        GridView1.AllowPaging = false;
        GridView1.DataBind();

        page.EnableEventValidation = false;
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
        GetData(1);
    }

    protected void Last_Click(object sender, EventArgs e)
    {
        GetData(maxPage);
    }

    protected void Pre_Click(object sender, EventArgs e)
    {
        GetData(cpage - 1);
    }

    protected void Next_Click(object sender, EventArgs e)
    {
        GetData(cpage + 1);
    }

    protected void Custom_Click(object sender, EventArgs e)
    {
        GetData(Convert.ToInt32(txtGoPage.Text));
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
}