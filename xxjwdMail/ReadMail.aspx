<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReadMail.aspx.cs" Inherits="AjaxMail.ReadMail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>阅读邮件</title>
    <style type="text/css">
        .style1
        {
            width: 7%;
        }
    </style>
</head>
<body style= "background-color:#eaf4ff">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="sm" runat="server"></asp:ScriptManager>
    <div>
      <table width="90%" border="0" cellspacing="0" cellpadding="0" border="1" bordercolor=gray  bordercolordark=white>
        <tr>
          <td class="style1">
              <asp:Label ID="fjrlbl" runat="server" Text="Label"></asp:Label>
              ：</td>
          <td width="80%" height="25px"><asp:TextBox ID="tbTo" runat="Server" SkinID="tbSkin" Width="60%" 
                  MaxLength="255" ReadOnly="True"></asp:TextBox>

		  
		  </td>
        </tr>

        <tr>
          <td class="style1">主题：</td>
          <td height="25px">
              <asp:TextBox ID="tbTitle" runat="server" SkinID="tbSkin" Width="60%" 
                  MaxLength="50" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
        <tr>
          <td class="style1">内容：</td>
          <td>
              <asp:TextBox ID="tbBody" runat="server" SkinID="tbSkin" Width="90%" 
                  Height="300px" TextMode="MultiLine" ReadOnly="True"></asp:TextBox>
            </td>
        </tr>
        <tr>
          <td class="style1">邮件附件：</td>
          <td>
              <asp:GridView ID="gvAttachment" runat="server" SkinID="tbSkin" 
                  EmptyDataText="附件为空。" ShowHeader="False" AutoGenerateColumns="False" 
                  Width="444px">
              <Columns>
              <asp:TemplateField>
              <ItemTemplate>
              <a href='<%# Eval("urlstring") %>' target="_blank"><%# Eval("filenamestring") %></a>
              </ItemTemplate>
              </asp:TemplateField>
              </Columns>
              </asp:GridView>
            </td>
        </tr>
        <tr>
          <td class="style1">&nbsp;</td>
          <td>
              <p id="FileList">
                  <asp:Label ID="Label1" runat="server" Text="Label" Visible="False"></asp:Label>
              </p>
              <script language="javascript" type="text/javascript">
                  function addFile() {
                      var filebutton = '<br><input type="file" size="50" name="File" class="Button" />';
                      document.getElementById('FileList').insertAdjacentHTML("beforeEnd", filebutton);
                  }
              </script>
            </td>
        </tr>
        <tr>
          <td class="style1">&nbsp;</td>
          <td>
              &nbsp;
              <asp:Button ID="BtnCommit" runat="server" Text="返回" SkinID="btnSkin" 
                  Width="90px" OnClick="btnCommit_Click" />
              &nbsp;
              <asp:Button ID="btnReply" runat="server" Text="回复该邮件" 
                  onclick="btnReply_Click" Width="90px" />
            &nbsp;
              <asp:Button ID="zf_btn" runat="server" onclick="zf_btn_Click" Text="转发" 
                  Width="90px" />
            </td>
        </tr>
      </table>
    </div>
    </form>
</body>
</html>
