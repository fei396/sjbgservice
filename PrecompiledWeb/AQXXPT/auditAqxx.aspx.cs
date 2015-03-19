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

public partial class auditAqxx : System.Web.UI.Page
{
    aqxxptService.aqxxptService s = new aqxxptService.aqxxptService();
    int xxid,uid;
    protected void Page_Load(object sender, EventArgs e)
    {

        string s = Session["user"] as string;
        uid = Convert.ToInt32(s);
        xxid = Convert.ToInt32(Request["xxid"]);
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
        Department[] depts = s.getAqxxptBm(xxid);
        for (int i = 0; i < depts.Length ; i++)
        {

            txtDept.Text += depts[i].Name + ";";
        }
        txtDept.Text = txtDept.Text.Substring(0, txtDept.Text.Length - 1);
        AQXX[] aqxx = s.getAqxxToAudit(uid, xxid);

        txtTitle.Text = aqxx[0].Title;
        txtContent.Text = aqxx[0].Content;

    }


  

    protected void AddButton_Click(object sender, EventArgs e)
    {
        string title = txtTitle.Text.Trim();
        string content = txtContent.Text.Trim();


        if (title.Equals("") || pbModule.hasForbiddenChar(title))
        {
            Response.Write(" <script> alert( '信息标题格式不正确！ ') </script> ");
            return;
        }
        string auditor = Session["user"] as string;
        auditor = "3974";
        if (auditor == null || auditor == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }

   
        INT result = s.auditAqxx(xxid,auditor, 1, title, content );
        if (result.Number==1)
        {
            Response.Write(" <script> alert( '信息审核成功，该信息已发送给：" + txtDept.Text +" ') </script> ");
        }
        else
        {
            Response.Write(" <script> alert( '信息审核失败: " + result.Message +  "');location='auditAqxxList.aspx' </script> ");
        }
    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        int blzbid = Convert.ToInt32(Request["fid"]);
        Response.Redirect("sendAqxx.aspx");
    }
}


  