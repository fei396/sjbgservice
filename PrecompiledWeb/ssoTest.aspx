﻿<%@ Page Language="C#" %>
<%@ import Namespace="System.Diagnostics" %>
<%@ import Namespace="XxjwdSSO" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        
        Guid g = Guid.NewGuid();
        XxjwdSSO sso = new XxjwdSSO(Request);
        string code = sso.getNewVerifyCode("3974");
        Response.Redirect("login.aspx?code=" + code);
    }

    public string GetCustomerMac(string IP)
    {
        string dirResults = "";
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        psi.FileName = "nbtstat";
        psi.RedirectStandardInput = false;
        psi.RedirectStandardOutput = true;
        psi.Arguments = "-a " + IP;
        psi.UseShellExecute = false;
        proc = System.Diagnostics.Process.Start(psi);
        dirResults = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        //匹配mac地址
        Match m = Regex.Match(dirResults, "\\w+\\-\\w+\\-\\w+\\-\\w+\\-\\w+\\-\\w\\w");

        //若匹配成功则返回mac，否则返回找不到主机信息
        if (m.ToString() != "")
        {
            return m.ToString();
        }
        else
        {
            return dirResults;
        }

    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
        <br />
        <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
        <br />
        <asp:Label ID="Label4"
            runat="server" Text="Label"></asp:Label>
    </div>
    </form>
</body>
</html>
