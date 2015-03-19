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

public partial class sadm_blzbgl_add : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Security s = Session["sec"] as Security;
        if (s == null)
        {
            Response.Redirect("error.aspx");
        }
        if (IsPostBack) return;
        //int blzbid = Convert.ToInt32(Request["id"]);
        dbModule dm = new dbModule();
        //blzbmcTextBox.Text = dm.getBlzbMc(blzbid);
        //jssTextBox.Text = dm.getBlzbMs(blzbid);


        DataTable zu1ndt = dm.getDWLB();

        for (int i = 0; i < zu1ndt.Rows.Count; i++)
        {
            zu1nListBox.Items.Add(new ListItem(zu1ndt.Rows[i]["topic"].ToString(), zu1ndt.Rows[i]["bianma"].ToString()));
        }
        string zu2 = "";
        DataTable zu2ndt = dm.getDWLB();

        for (int i = 0; i < zu2ndt.Rows.Count; i++)
        {
            zu2nListBox.Items.Add(new ListItem(zu2ndt.Rows[i]["topic"].ToString(), zu2ndt.Rows[i]["bianma"].ToString()));
        }
        //string zhi1 = dm.getBlzbZhi(blzbid, 1);
        //string zhi2 = dm.getBlzbZhi(blzbid, 2);
        //int LeiBie1 = dm.getZhiBiaoLeiBieByCode(zhi1);
        //int LeiBie2 = dm.getZhiBiaoLeiBieByCode(zhi2);
        //zhibiaozu1DDL.SelectedValue = LeiBie1.ToString();
        //zhibiaozu2DDL.SelectedValue = LeiBie2.ToString();
        //typeDDL.SelectedValue = dm.getBlzbType(blzbid).ToString();
        
    }





    protected void zhibiaozu1DDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        zhi1DDL.DataBind();
    }
    protected void zhibiaozu2DDL_SelectedIndexChanged(object sender, EventArgs e)
    {
        zhi2DDL.DataBind();
    }
    protected void zhibiaozu1DDL_DataBound(object sender, EventArgs e)
    {
        //zhi1DDL.DataBind();
    }
    protected void zu1AddButton_Click(object sender, EventArgs e)
    {
        ArrayList arr = new ArrayList();
        foreach (ListItem li in zu1nListBox.Items)
        {
            if (li.Selected == true)
            {
                arr.Add(li);
            }
        }
        foreach (ListItem li in arr)
        {
            zu1nListBox.Items.Remove(li);
            zu1ListBox.Items.Add(li);
        }
    }
    protected void zu1DelButton_Click(object sender, EventArgs e)
    {
        ArrayList arr = new ArrayList();
        foreach (ListItem li in zu1ListBox.Items)
        {
            if (li.Selected == true)
            {
                arr.Add(li);
            }
        }
        foreach (ListItem li in arr)
        {
            zu1ListBox.Items.Remove(li);
            zu1nListBox.Items.Add(li);
        }
    }
    protected void zu2AddButton_Click(object sender, EventArgs e)
    {
        ArrayList arr = new ArrayList();
        foreach (ListItem li in zu2nListBox.Items)
        {
            if (li.Selected == true)
            {
                arr.Add(li);
            }
        }
        foreach (ListItem li in arr)
        {
            zu2nListBox.Items.Remove(li);
            zu2ListBox.Items.Add(li);
        }
    }
    protected void zu2DelButton_Click(object sender, EventArgs e)
    {
        ArrayList arr = new ArrayList();
        foreach (ListItem li in zu2ListBox.Items)
        {
            if (li.Selected == true)
            {
                arr.Add(li);
            }
        }
        foreach (ListItem li in arr)
        {
            zu2ListBox.Items.Remove(li);
            zu2nListBox.Items.Add(li);
        }
    }
    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (zu1ListBox.Items.Count == 0 || zu2ListBox.Items.Count == 0)
        {
            Response.Write(" <script> alert( '请选择好分组！ ') </script> ");
            return;
        }
        
        string topic = blzbmcTextBox.Text;
        string miaoshu = jssTextBox.Text;
        string zhi1 = zhi1DDL.SelectedValue;
        string zhi2 = zhi2DDL.SelectedValue;
        int type = Convert.ToInt32(typeDDL.SelectedValue);
        string group1 = "";
        string group2 = "";
        foreach (ListItem li in zu1ListBox.Items)
        {
            group1 += li.Value + ",";
        }

            group1 = group1.Substring(0, group1.Length - 1);

        foreach (ListItem li in zu2ListBox.Items)
        {
            group2 += li.Value + ",";
        }

            group2 = group2.Substring(0, group2.Length - 1);

        dbModule dm = new dbModule();
        int result = dm.addBlzb(topic, miaoshu, zhi1, group1, zhi2, group2, type);
        if (result == 1)
        {
            Response.Write(" <script> alert( '添加比率指标成功！ ') </script> ");
            //Response.Redirect("sadm_blzbgl_edit.aspx?id=" + id.ToString());
        }
        else
        {
            Response.Write(" <script> alert( '添加比率指标失败！ ') </script> ");
        }
    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        
        Response.Redirect("sadm_blzbgl_add.aspx");
    }
    protected void zhi1DDL_DataBound(object sender, EventArgs e)
    {
        //int blzbid = Convert.ToInt32(Request["id"]);
        //dbModule dm = new dbModule();
        //string zhi1 = zhibiaozu1DDL.SelectedValue;
        ////Response.Write(" <script> alert( '" + zhi1 + "！ ') </script> ");
        //if (zhi1DDL.Items.Count != 0)
        //{
        //    zhi1DDL.SelectedValue = zhi1;
        //}
    }
    protected void zhi2DDL_DataBound(object sender, EventArgs e)
    {
        //int blzbid = Convert.ToInt32(Request["id"]);
        //dbModule dm = new dbModule();
        //string zhi2 = zhibiaozu2DDL.SelectedValue;
        //if (zhi2DDL.Items.Count != 0)
        //{
        //    zhi2DDL.SelectedValue = zhi2;
        //}
    }
}
