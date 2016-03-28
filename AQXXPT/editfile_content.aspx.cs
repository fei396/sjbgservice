using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using aqxxptWebService;
public partial class editfile_content : Page
{
    private readonly aqxxptService s = new aqxxptService();
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
        var xxid = Convert.ToInt32(Request["xxid"]);
        s.SjbgSoapHeaderValue = Security.GetSoapHeader();
        Security.SetCertificatePolicy();
        Department[] depts = s.GetAqxxptBm(xxid);
        lbDR.Items.Clear();
        for (var i = 0; i < depts.Length; i++)
        {
            var li = new ListItem(depts[i].Name, depts[i].ID.ToString());
            lbDR.Items.Add(li);
        }

        AQXX aqxx = s.GetAqxxContent(xxid)[0];
        txtTitle.Text = aqxx.Title;
        txtContent.Text = aqxx.Content;
        txtSetTime.Text = aqxx.SetTime;
    }
}