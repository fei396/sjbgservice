﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class error : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		string errString = Request["errcode"];
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
