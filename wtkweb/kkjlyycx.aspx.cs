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
public partial class kkjlyycx : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{


		Security s = Session["sec"] as Security;
		if (s == null)
		{
			Response.Redirect("error.aspx");
		}

	

	}

	public string getKKLX(int lx)
	{
		if (lx == 1)
		{
			return "记入旷工";
		}
		else if (lx == 2)
		{
			return "提醒未派";
		}
		else if (lx == 3)
		{
			return "提醒出乘";
		}
		else
		{
			return "error!";
		}
	}

	public string getMDLX(int mdlx)
	{
		if (mdlx == 0)
		{
			return "白名单";
		}
		else if (mdlx == 1)
		{
			return "黑名单";
		}
		else
		{
			return "error!";
		}

	}

	public string getKKXMLX(int lx)
	{
		if (lx == 1)
		{
			return "提示警告型";
		}
		else if (lx == 2)
		{
			return "严格卡控型";
		}
		else
		{
			return "error!";
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

		TextBox tb_gopage = (TextBox)GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");
		try
		{
			GridView1.PageIndex = Convert.ToInt32(tb_gopage.Text) - 1;
		}
		catch
		{
			Response.Write(" <script> alert( '请输入正确的数字！ ') </script> ");
		}
	}

}
