<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendMail.aspx.cs" Inherits="AjaxMail.SendMail"  %>
<%@ Reference Page="~/OtherShouJian.aspx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>发送邮件</title>
     <link rel="stylesheet" rev="stylesheet" href="App_Themes/Stylebody.css" type="text/css" media="all" />
    <style type="text/css">
        .style1
        {
            width:90px;
        }
        .input
        {
            height: 21px;
        }
    </style>
</head>

<body style= "background-color:#eaf4ff">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="sm" runat="server"></asp:ScriptManager>
    <div>
    <p></p>
    <img src="images/sendmail.gif" alt="" name="kklk" width="20" height="16" id="kklk" />&nbsp;
        <asp:Label ID="Label1" runat="server" Text="写新邮件"></asp:Label>
        <br />
&nbsp;<table width="90%" border="1" cellspacing="0" bordercolor=gray  bordercolordark=white cellpadding="0">
        <tr>
          <td width="92" align="center" valign="middle" class="style1"><div align="center"><strong>收件人：</strong></div></td>
          <td colspan="2">
              <asp:TextBox ID="tbTo" runat="server" ReadOnly="True" Width="60%">请先增加收件人</asp:TextBox>
<asp:Button ID="Button4" runat="server" OnClick="Button4_Click"  CssClass="input" Text="选择收件人" ToolTip="单击此按钮就可以选择收件人" />
                           &nbsp;&nbsp;<asp:Label ID="xzq_label" runat="server" Text="选择群："></asp:Label>
              <asp:DropDownList ID="GroupList" runat="server" 
                  AutoPostBack="True" onselectedindexchanged="GroupList_SelectedIndexChanged">
              </asp:DropDownList>
&nbsp;<asp:Button ID="Button2" runat="server" CssClass="input" Text="清除" OnClick="Button2_Click1" ToolTip="清空收件人文本框" />
		  <asp:TextBox ID="tbCC" runat="server" SkinID="tbSkin" Width="30%" 
                  MaxLength="50" Visible="False"></asp:TextBox>		  </td>
        </tr>

        <tr>
          <td align="center" valign="middle" class="style1"><div align="center"><strong>主题：</strong></div></td>
          <td colspan="2">
              <asp:TextBox ID="tbTitle" runat="server" SkinID="tbSkin" Width="60%" MaxLength="50"></asp:TextBox>            </td>
        </tr>
        <tr>
          <td align="center" valign="middle" class="style1"><div align="center"><strong>邮件格式：</strong></div></td>
          <td colspan="2">
              <input ID="HtmlCB" type="checkbox" runat="server" class="GbText" />HTML格式</td>
        </tr>
        <tr>
          <td align="center" valign="middle" class="style1"><div align="center"><strong>内容：</strong></div></td>
          <td colspan="2">
              <asp:TextBox ID="tbBody" runat="server" SkinID="tbSkin" Width="90%" Height="300px" TextMode="MultiLine"></asp:TextBox>            </td>
        </tr>
                <tr>
          <td align="center" valign="middle" class="style1"><div align="center"></div></td>
          <td colspan="2">
              <input ID="CopyCB" type="checkbox" runat="server" class="GbText" 
                  checked="checked" />保存副本到发件箱</td>
        </tr>
        <tr>
          <td align="center" valign="middle" class="style1"><div align="center"><strong>邮件附件：</strong></div></td>
          <td colspan="2">
              <asp:GridView ID="gvAttachment" runat="server" SkinID="tbSkin" 
                  EmptyDataText="附件为空。" ShowHeader="False" AutoGenerateColumns="False" 
                  Width="169px" Visible="False">
              <Columns>
              <asp:TemplateField>
              <ItemTemplate>
              <a href='<%# Eval("urlstring") %>' target="_blank"><%# Eval("filenamestring") %></a>
              </ItemTemplate>
              </asp:TemplateField>
              </Columns>
              </asp:GridView>
              
              <asp:ListBox ID="fj_listbox" runat="server" Visible="False" Width="60%">
              </asp:ListBox>
              <asp:Button ID="del_btn" runat="server" onclick="del_btn_Click" Text="删除附件" 
                  Visible="False" />
              
              </td>
        </tr>
      </table>

<input id="Button1" type="button" value="增加附件" class="Button" onclick="addFile()" />
              <p id="FileList"><input ID="File1" runat="server" type="file" size="50" name="File" class="Button" /></p>
          <script language="javascript" type="text/javascript">
              function addFile() {
                  var filebutton = '<br><input type="file" size="50" name="File" class="Button" />';
                  document.getElementById('FileList').insertAdjacentHTML("beforeEnd", filebutton);
              }
              </script>  
              <asp:Button ID="BtnCommit" runat="server" Text="发送" SkinID="btnSkin" Width="100px" OnClick="btnCommit_Click" />
    </div>
    </form>
</body>
</html>
