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

public partial class kkxmbj : System.Web.UI.Page

{


    protected void Page_Load(object sender, EventArgs e)
    {


		Security s = Session["sec"] as Security;
		if (s == null)
		{
			Response.Redirect("error.aspx");
		}

        string sxtj = Session["sxtj"] as string;
        dbModule dm = new dbModule();
		string sscj = Session["udept"] as string;

		int admin = s.getSecurity();
		string where = "";
		if (admin != 1)
		{
			where += " and cj='" + sscj + "' and tjr='" + s.getUserCode() + "'";
		}
        if (sxtj == "" || sxtj == null)
        {
			SqlDataSource1.SelectCommand = "SELECT [id], [xmmc], [kklx], [mdlx], [cj], [kssj], [jzsj], [sfyx],sfddtj FROM [T_CQKK_KKXMB] where sfyx>=0  " + where + " ORDER BY [sfyx] DESC, [tjsj]";
        }
        else
        {
			SqlDataSource1.SelectCommand = "SELECT [id], [xmmc], [kklx], [mdlx], [cj],[kssj], [jzsj], [sfyx],sfddtj FROM [T_CQKK_KKXMB]  WHERE sfyx>=0 " + where + " and (xmmc like '%" + sxtj + "%' or jzsj like '%" + sxtj + "%' or cj like '%" + sxtj + "%') ORDER BY [sfyx] DESC, [tjsj]";
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

	public string getKKLX(int lx)
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

	public int getAdmin()
	{
		Security s = Session["sec"] as Security;
		return s.getSecurity();
	}
    
    protected void GridView1_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
		dbModule dm = new dbModule();
		Security s = Session["sec"] as Security;
		dm.addLog(3, s.getUserCode(), Convert.ToInt32(e.Keys["ID"]), "", "删除卡控项目" + e.Values["xmmc"].ToString());
        Response.Write(" <script> alert( '删除成功！ ') </script> ");
    }


    protected void sxButton_Click(object sender, EventArgs e)
    {
        pbModule pm = new pbModule();
        string sxtj = sxtjTextBox.Text.Trim();
        if (pm.hasForbiddenChar(sxtj))
        {
            messageLabel.Text = "筛选条件格式不正确，请重新输入！";
            return;
        }
        else
        {
            Session["sxtj"] = sxtj;
            Response.Redirect("kkxmbj.aspx");
        }

    }
    protected void czButton_Click(object sender, EventArgs e)
    {
        Session["sxtj"] = "";
        Response.Redirect("kkxmbj.aspx");
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
    protected void SqlDataSource1_Selected(object sender, SqlDataSourceStatusEventArgs e)
    {
		//TextBox tb_gopage = (TextBox)GridView1.BottomPagerRow.Cells[0].FindControl("txtGoPage");
		GridView1.Caption = "本次共查询到记录" + e.AffectedRows.ToString() + "条";
		//Label lb = (Label)GridView1.BottomPagerRow.Cells[0].FindControl("lballcount");
		//lb.Text = e.AffectedRows.ToString();
    }
    protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
		//int zbid = Convert.ToInt32(e.Keys["ID"]);
		//dbModule dm = new dbModule();

		////dm.deleteRYbyKKID(zbid,e.Values["sscj"].ToString());
        
    }
	protected void GridView1_RowUpdated(object sender, GridViewUpdatedEventArgs e)
	{
		dbModule dm = new dbModule();
		Security s = Session["sec"] as Security;
		string str1 = "";
		string str2 = "";
		for (int i = 0; i < e.NewValues.Count; i++)
		{
			str1 += e.NewValues[i].ToString() + ",";
			str2 += e.OldValues[i].ToString() + ",";
		}
		dm.addLog(2, s.getUserCode(), Convert.ToInt32(e.Keys["ID"]), "", "编辑卡控项目：" + str2 + "变更为：" + str1);
	}
}
