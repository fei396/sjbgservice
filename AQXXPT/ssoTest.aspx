﻿<%@ Page Language="C#" %>
<%@ Import Namespace="System.Diagnostics" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        var g = Guid.NewGuid();
        XxjwdSSO.XxjwdSSO sso = new XxjwdSSO.XxjwdSSO(Request);
        string code = sso.getNewVerifyCode("3770");
        Response.Redirect("login.aspx?code=" + code);
    }

    public string GetCustomerMac(string IP)
    {
        var dirResults = "";
        var psi = new ProcessStartInfo();
        var proc = new Process();
        psi.FileName = "nbtstat";
        psi.RedirectStandardInput = false;
        psi.RedirectStandardOutput = true;
        psi.Arguments = "-a " + IP;
        psi.UseShellExecute = false;
        proc = Process.Start(psi);
        dirResults = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();

        //匹配mac地址
        var m = Regex.Match(dirResults, "\\w+\\-\\w+\\-\\w+\\-\\w+\\-\\w+\\-\\w\\w");

        //若匹配成功则返回mac，否则返回找不到主机信息
        if (m.ToString() != "")
        {
            return m.ToString();
        }
        return dirResults;
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
        <br/>
        <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
        <br/>
        <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
        <br/>
        <asp:Label ID="Label4"
                   runat="server" Text="Label">
        </asp:Label>
    </div>
</form>
</body>
</html>