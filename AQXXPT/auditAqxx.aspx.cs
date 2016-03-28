using System;
using System.Web.UI;
using aqxxptWebService;

public partial class auditAqxx : Page
{
    private readonly aqxxptService s = new aqxxptService();
    private int xxid;
    private int uid;

    protected void Page_Load(object sender, EventArgs e)
    {
        var s = Session["user"] as string;
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

    private void initPage()
    {
        s.SjbgSoapHeaderValue = Security.GetSoapHeader();
        Security.SetCertificatePolicy();
        Department[] depts = s.GetAqxxptBm(xxid);
        for (var i = 0; i < depts.Length; i++)
        {
            txtDept.Text += depts[i].Name + ";";
        }
        txtDept.Text = txtDept.Text.Substring(0, txtDept.Text.Length - 1);
        AQXX[] aqxx = s.GetAqxxToAudit(uid, xxid);

        txtTitle.Text = aqxx[0].Title;
        txtContent.Text = aqxx[0].Content;
    }


    protected void AddButton_Click(object sender, EventArgs e)
    {
        var title = txtTitle.Text.Trim();
        var content = txtContent.Text.Trim();


        if (title.Equals("") || pbModule.hasForbiddenChar(title))
        {
            Response.Write(" <script> alert( '信息标题格式不正确！ ') </script> ");
            return;
        }
        var auditor = Session["user"] as string;
        auditor = "3974";
        if (auditor == null || auditor == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }


        INT result = s.AuditAqxx(xxid, auditor, 1, title, content);
        if (result.Number == 1)
        {
            Response.Write(" <script> alert( '信息审核成功，该信息已发送给：" + txtDept.Text + " ') </script> ");
        }
        else
        {
            Response.Write(" <script> alert( '信息审核失败: " + result.Message + "');location='auditAqxxList.aspx' </script> ");
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        var blzbid = Convert.ToInt32(Request["fid"]);
        Response.Redirect("sendAqxx.aspx");
    }
}