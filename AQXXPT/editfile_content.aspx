<%@ Page Language="C#" AutoEventWireup="true" CodeFile="editfile_content.aspx.cs" Inherits="editfile_content" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="js/WebCalendar.js"></script>
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
                    <asp:Label ID="Label4" runat="server" Text="审核人："></asp:Label>
                </td>
                <td colspan="3" style="height: 50px" align="left">
                    <asp:TextBox ID="txtAuditor" runat="server" Width="300px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr style="height: 50px">
                <td align="right" width="25%" style="height: 50px">
                    <asp:Label ID="Label2" runat="server" Text="信息标题："></asp:Label>
                </td>
                <td colspan="3" style="height: 50px" align="left">
                    <asp:TextBox ID="txtTitle" runat="server" Width="300px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
                                    <tr height="300"style="height: 50px">
                <td align="right" width="25%" >
                    <asp:Label ID="Label5" runat="server" Text="发送时间："></asp:Label>
                </td>
                <td colspan="3"  align="left">
                     <asp:TextBox ID="txtSetTime" runat="server" Width="300px"
                      onclick="SelectDate(this,'yyyy-MM-dd hh:mm')" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr height="300">
                <td align="right" width="25%" >
                    <asp:Label ID="Label1" runat="server" Text="信息内容："></asp:Label>
                </td>
                <td colspan="3"  align="left">
                    <asp:TextBox ID="txtContent" runat="server" Height="200px" Width="80%" 
                        TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>

            <tr valign="middle" style="height: 200px">
                <td align="right" width="25%">
                    <asp:Label ID="Label3" runat="server" Text="发送部门："></asp:Label>
                </td>
               
                <td>
                    
                    <asp:ListBox ID="lbDR" runat="server" Rows="10" Width="180px" SelectionMode="Multiple">
                    </asp:ListBox>
                </td>
            </tr>
           
        </table>
    </div>
    </form>
</body>
</html>
