using System;
using System.Web.UI;

public partial class logout : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var s = new Security();
        s.SetSecurity(-1);
        s.SetUserName("");
        s.SetUserCode("");
        Session["sec"] = s;
        Session["usec"] = -1;
        Session["usercode"] = "";
        Session["udept"] = "";
        Response.Redirect("login.aspx");
    }
}