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

public partial class editfile_content : System.Web.UI.Page
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

        int xxid = Convert.ToInt32(Request["xxid"]);
        s.SjbgSoapHeaderValue = Security.getSoapHeader();
        Security.SetCertificatePolicy();
        Department[] depts = s.getAqxxptBm(xxid);
        lbDR.Items.Clear();
        for (int i = 0; i < depts.Length ; i++)
        {
            ListItem li = new ListItem(depts[i].Name, depts[i].ID.ToString());
            lbDR.Items.Add(li);
        }

        AQXX aqxx = s.getAqxxContent(xxid)[0];
        txtTitle.Text = aqxx.Title;
        txtContent.Text = aqxx.Content;
        txtSetTime.Text = aqxx.SetTime;
        
    }


}


  