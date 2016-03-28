using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using aqxxptWebService;
public partial class auditAqxxList : Page
{
    private readonly aqxxptService _s = new aqxxptService();

    protected void Page_Load(object sender, EventArgs e)
    {
        var s = Session["user"] as string;
        if (s == null || s == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }


        if (!IsPostBack)
        {
            _s.SjbgSoapHeaderValue = Security.GetSoapHeader();
            Security.SetCertificatePolicy();
            AQXX[] aqxxs = _s.GetAqxxToAudit(Convert.ToInt32(s), 0);
            GridView1.DataSource = aqxxs;
            GridView1.DataBind();
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