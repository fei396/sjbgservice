using System;
using System.Web.UI;

public partial class error : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var errString = Request["errcode"];
        if (errString != null)
        {
            errorLabel.Text = errString;
        }
        else
        {
            errorLabel.Text = "未登录";
        }
    }
}