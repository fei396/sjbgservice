using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using aqxxptWebService;
public partial class applyAqxx : Page
{
    private readonly aqxxptService _s = new aqxxptService();
    private int uid;

    protected void Page_Load(object sender, EventArgs e)
    {
        var s = Session["user"] as string;
        try
        {
            uid = Convert.ToInt32(s);
        }
        catch
        {
            Response.Redirect("error.aspx?errCode=请通过正确方式登录本站");
        }
        if (string.IsNullOrEmpty(s))
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        if (!IsPostBack)
        {
            InitPage();
        }
    }

    private void InitPage()
    {
        _s.SjbgSoapHeaderValue = Security.GetSoapHeader();
        Security.SetCertificatePolicy();
        Department[] depts = _s.GetAqxxptBm(0);
        lbDRn.Items.Clear();
        lbDR.Items.Clear();
        for (var i = 0; i < depts.Length; i++)
        {
            var li = new ListItem(depts[i].Name, depts[i].ID.ToString());
            lbDRn.Items.Add(li);
        }


        txtTitle.Text = "";
        txtContent.Text = "";
        txtSetTime.Text = "";
        User[] autiors = _s.GetAqxxptShenHe();
        ddlAuditor.DataSource = autiors;
        ddlAuditor.DataValueField = "userno";
        ddlAuditor.DataTextField = "username";
        ddlAuditor.DataBind();
        cbLeaderAll.Checked = false;
        cbLeader0001.Checked = false;
        cbLeader0002.Checked = false;
        cbLeader0007.Checked = false;
        cbLeader0008.Checked = false;
    }


    protected void zhibiaozu1DDL_DataBound(object sender, EventArgs e)
    {
        //zhi1DDL.DataBind();
    }

    protected void zu1AddButton_Click(object sender, EventArgs e)
    {
        var arr = new ArrayList();
        foreach (ListItem li in lbDRn.Items)
        {
            if (li.Selected)
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
        var arr = new ArrayList();
        foreach (ListItem li in lbDR.Items)
        {
            if (li.Selected)
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
        var title = txtTitle.Text.Trim();
        var content = txtContent.Text.Trim();
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

        var buMens = "";
        foreach (ListItem li in lbDR.Items)
        {
            buMens += li.Value + ",";
        }

        buMens = buMens.Substring(0, buMens.Length - 1);
        var fileSender = Session["user"] as string;
        if (string.IsNullOrEmpty(fileSender))
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }

        var auditor = ddlAuditor.SelectedItem.Value;
        var setTime = txtSetTime.Text;
        var lingDaos = "";
        //if (cbLeader0001.Checked) lingDaos += "0001,";
        //if (cbLeader0002.Checked) lingDaos += "0002,";
        //if (cbLeader0007.Checked) lingDaos += "0007,";
        //if (cbLeader0008.Checked) lingDaos += "0008,";
        //if (cbLeader0006.Checked) lingDaos += "0006,";
        //if (!lingDaos.Equals("")) lingDaos = lingDaos.Substring(0, lingDaos.Length - 1);
        if (auditor.Equals("0000"))
        {
            auditor = "3974";
            INT result = _s.ApplyAqxx(fileSender, auditor, title, content, buMens, setTime, lingDaos);
            if (result.Number == 1)
            {
                int xxid = Convert.ToInt32(result.Message);
                INT result1 = _s.AuditAqxx(xxid, auditor, 1, title, content);
                if (result.Number == 1)
                {
                    Response.Write(" <script> alert( '提交信息成功，将于" + setTime + "发送信息。 ') </script> ");
                    //Response.Redirect("sendfile.aspx");
                    Session["sent"] = "sent";
                    InitPage();
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
            INT result = _s.ApplyAqxx(fileSender, auditor, title, content, buMens, setTime, lingDaos);
            if (result.Number == 1)
            {
                Response.Write(" <script> alert( '提交信息成功，请等待领导审核。 ') </script> ");
                //Response.Redirect("sendfile.aspx");
                Session["sent"] = "sent";
                InitPage();
            }
            else
            {
                Response.Write(" <script> alert( '提交信息失败: " + result.Message + "') </script> ");
            }
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        var blzbid = Convert.ToInt32(Request["fid"]);
        Response.Redirect("applyAqxx.aspx");
    }

    protected void cbLeaderAll_CheckedChanged(object sender, EventArgs e)
    {
        cbLeader0001.Checked = cbLeaderAll.Checked;
        cbLeader0002.Checked = cbLeaderAll.Checked;
        cbLeader0007.Checked = cbLeaderAll.Checked;
        cbLeader0008.Checked = cbLeaderAll.Checked;
        cbLeader0006.Checked = cbLeaderAll.Checked;
    }
}