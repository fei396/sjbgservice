<%@ page language="C#" autoeventwireup="true" inherits="auditAqxx, App_Web_6tvq6jyo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body align="center">
    <form id="form1" runat="server">
    <div height="100">
        <br />
        <br />
        <br />
    </div>
    <div  align="center" valign="middle">
        <table width="700" border="1" cellspacing="0" rules="all">
                  
            <tr style="height: 50px">
                <td align="right" width="25%" style="height: 50px">
                    <asp:Label ID="Label2" runat="server" Text="信息标题："></asp:Label>
                </td>
                <td style="height: 50px" align="left">
                    <asp:TextBox ID="txtTitle" runat="server" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr height="300">
                <td align="right" width="25%" >
                    <asp:Label ID="Label1" runat="server" Text="信息内容："></asp:Label>
                </td>
                <td  align="left">
                    <asp:TextBox ID="txtContent" runat="server" Height="200px" Width="80%" 
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
            <tr valign="middle" style="height: 200px">
                <td align="right" width="25%">
                    <asp:Label ID="Label3" runat="server" Text="发送部门："></asp:Label>
                </td>
                <td>
                   <asp:TextBox ID="txtDept" runat="server"   Width="100%"
                        ></asp:TextBox>
                    
                </td>
                
            </tr>
            <tr>
                <td style="height: 26px">
                    &nbsp
                </td>
                <td   style="height: 26px">
                    <asp:Button ID="AddButton" runat="server" Text="同意（直接发送）" OnClick="AddButton_Click" />
                    <asp:Button ID="CancelButton" runat="server" Text="退回(退回修改 )" OnClick="CancelButton_Click" />
                </td>

            </tr>
        </table>
    </div>
    </form>
</body>
</html>
