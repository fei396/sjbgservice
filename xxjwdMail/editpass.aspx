<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editpass.aspx.cs" Inherits="AjaxMail.editpass" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>无标题页</title>

</head>
<body>
    <form id="form1" runat="server">

    <asp:Panel ID="Panel1" runat="server" BackColor="#EAF4FF" Height="211px" 
        style="margin-bottom: 83px">
    <div style="height: 211px" align="center">
    
        <table width="90%" border="0" cellspacing="0" cellpadding="0">
          <tr>
            <td height="35" colspan="2" align="center"><asp:Label ID="Label1" runat="server" 
                    Text="修改密码" Font-Bold="True" Font-Size="18pt"></asp:Label>            </td>
          </tr>
          <tr>
            <td width="47%" height="35" align="right"><asp:Label ID="Label2" runat="server" 
                    Text="请输入原密码：" Enabled="False" Visible="False"></asp:Label></td>
            <td width="53%" height="35" align="left">
                <asp:TextBox ID="ymm_Txt" runat="server" Width="90px" 
            ontextchanged="ymm_Txt_TextChanged" Enabled="False" Visible="False"></asp:TextBox></td>
          </tr>
          <tr>
            <td height="35" align="right"><asp:Label ID="Label3" runat="server" Text="    输入新密码："></asp:Label></td>
            <td height="35" align="left"><asp:TextBox ID="xmm_txt" runat="server" Width="90px"></asp:TextBox></td>
          </tr>
          <tr>
            <td height="35" align="right"><asp:Label ID="Label4" runat="server" Text="确    认    密    码："></asp:Label></td>
            <td height="35" align="left"><asp:TextBox ID="qrmm_txt" runat="server" Width="90px"></asp:TextBox></td>
          </tr>
          <tr>
            <td height="35" colspan="2" align="center"><asp:Button ID="Button1" runat="server" Text="确认" onclick="Button1_Click" />
              <div align="center"></div></td>
          </tr>
        </table>
        <p>&nbsp;        </p>
        <p><br />
          <br />
          <br />
          <br />
  &nbsp;&nbsp;&nbsp;<br />
          <br />
  &nbsp;&nbsp;<br />
          <br />
          <br />
          
            </p>
    </div>
    </asp:Panel>


<br />


    </form>
</body>
</html>
