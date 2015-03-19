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
using System.Diagnostics;
using System.IO;
using aqxxptService;

public partial class applyAqxx : System.Web.UI.Page
{
    aqxxptService.aqxxptService s = new aqxxptService.aqxxptService();
    int uid;
    protected void Page_Load(object sender, EventArgs e)
    {

        string s = Session["user"] as string;
        try
        {
            uid = Convert.ToInt32(s);
        }
        catch
        {
            Response.Redirect("error.aspx?errCode=请通过正确方式登录本站");
        }
        if (s == null || s == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        if (!IsPostBack)
        {
            initPage();
        }
    }
    void initPage()
    {


        s.SjbgSoapHeaderValue = Security.getSoapHeader();
        Security.SetCertificatePolicy();
        Department[] depts = s.getAqxxptBm(0);
        lbDRn.Items.Clear();
        lbDR.Items.Clear();
        for (int i = 0; i < depts.Length ; i++)
        {
            ListItem li = new ListItem(depts[i].Name, depts[i].ID.ToString());
            lbDRn.Items.Add(li);
        }


        txtTitle.Text = "";
        txtContent.Text = "";

        User[] autiors = s.getAqxxptShenHe();
        ddlAuditor.DataSource = autiors;
        ddlAuditor.DataValueField = "userno";
        ddlAuditor.DataTextField = "username";
        ddlAuditor.DataBind();
    }


    protected void zhibiaozu1DDL_DataBound(object sender, EventArgs e)
    {
        //zhi1DDL.DataBind();
    }
    protected void zu1AddButton_Click(object sender, EventArgs e)
    {
        ArrayList arr = new ArrayList();
        foreach (ListItem li in lbDRn.Items)
        {
            if (li.Selected == true)
            {
                arr.Add(li);
            }
        }
        foreach (ListItem li in arr)
        {
            lbDRn.Items.Remove(li);
            lbDR.Items.Add(li);
        }
    }
    protected void zu1DelButton_Click(object sender, EventArgs e)
    {
        ArrayList arr = new ArrayList();
        foreach (ListItem li in lbDR.Items)
        {
            if (li.Selected == true)
            {
                arr.Add(li);
            }
        }
        foreach (ListItem li in arr)
        {
            lbDR.Items.Remove(li);
            lbDRn.Items.Add(li);
        }
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        string title = txtTitle.Text.Trim();
        string content = txtContent.Text.Trim();
        if (lbDR.Items.Count == 0)
        {
            Response.Write(" <script> alert( '请选择要发送的部门！ ') </script> ");
            return;
        }

        if (title.Equals("") || pbModule.hasForbiddenChar(title))
        {
            Response.Write(" <script> alert( '信息标题格式不正确！ ') </script> ");
            return;
        }

        string buMens = "";
        foreach (ListItem li in lbDR.Items)
        {
            buMens += li.Value + ",";
        }

        buMens = buMens.Substring(0, buMens.Length - 1);
        string fileSender = Session["user"] as string;
        if (fileSender == null || fileSender == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }

        string auditor = ddlAuditor.SelectedItem.Value;
        string setTime = txtSetTime.Text;
        if (auditor.Equals("0000"))
        {
            auditor = "3974";
            INT result = s.applyAqxx(fileSender, auditor, title, content, buMens, setTime);
            if (result.Number == 1)
            {
                int xxid = Convert.ToInt32(result.Message);
                INT result1 = s.auditAqxx(xxid, auditor, 1, title, content);
                if (result.Number == 1)
                {
                    Response.Write(" <script> alert( '提交信息成功，将于" + setTime + "发送信息。 ') </script> ");
                    //Response.Redirect("sendfile.aspx");
                    Session["sent"] = "sent";
                    initPage();
                }
                else
                {
                    Response.Write(" <script> alert( '审核信息失败: " + result.Message + "') </script> ");
                }
            }
            else
            {
                Response.Write(" <script> alert( '提交信息失败: " + result.Message + "') </script> ");
            }
        }
        else
        {
            INT result = s.applyAqxx(fileSender, auditor, title, content, buMens, setTime);
            if (result.Number == 1)
            {
                Response.Write(" <script> alert( '提交信息成功，请等待领导审核。 ') </script> ");
                //Response.Redirect("sendfile.aspx");
                Session["sent"] = "sent";
                initPage();
            }
            else
            {
                Response.Write(" <script> alert( '提交信息失败: " + result.Message + "') </script> ");
            }
        }
    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        int blzbid = Convert.ToInt32(Request["fid"]);
        Response.Redirect("sendAqxx.aspx");
    }
}


  