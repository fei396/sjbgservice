<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddGroup.aspx.cs" Inherits="AjaxMail.AddMailbox" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>收件人列表</title>
     <link rel="stylesheet" rev="stylesheet" href="App_Themes/Stylebody.css" type="text/css" media="all" />
    <style type="text/css">
        .style1
        {
            width: 63%;
        }
        .style2
        {
            height: 24px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>

       <table width="450px" cellpadding="0" cellspacing="0" align="center" class="table"> 
       <tr>
       
         <td align="center" bgcolor="#999999" class="style1" colspan="3" height="30px">选择人员</td>
   
       </tr>
       <tr>
       <td align="center" colspan="3">
           请输入群名称：<asp:TextBox ID="tbgn" runat="server"></asp:TextBox>
       </td>
       </tr>
          <tr>
            <td height="30px" align="center" colspan="3">
                <asp:Label ID="Label1" runat="server" Text="请选择："></asp:Label>
                <asp:DropDownList ID="DropDownList1" runat="server" Width="260px" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                    <asp:ListItem>请选择部门</asp:ListItem>
                </asp:DropDownList></td>
          </tr>
          <tr>
            <td colspan="3"><hr size="1" /></td>
          </tr>
          <tr>
            <td height="30px" align="center" colspan="3">
               输入工号：<asp:TextBox ID="txt_pymm" runat="server"></asp:TextBox><asp:Button ID="btn_pymmQuery" runat="server" Text="查询" OnClick="btn_pymmQuery_Click" />
            </td>
          </tr>
          <tr>
            <td align="center" height="300px" width="45%">
                <asp:ListBox ID="ListBox1" runat="server" Height="289px" Width="178px" 
                    SelectionMode="Multiple" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged"></asp:ListBox>
            </td>
     
             <td class="style1">
               <table>
                  <tr>
                    <td height="30px" align="center">
                        <asp:Button ID="Button1" runat="server" CssClass="input"  Text="全部>>" OnClick="Button1_Click" /></td>
                  </tr>
                  <tr>
                    <td height="30px" align="center">
                        <asp:Button ID="Button4" runat="server" CssClass="input" Text="<<移除" OnClick="Button4_Click" /></td>
                  </tr>
                  <tr>
                    <td height="30px" align="center">
                        <asp:Button ID="Button3" runat="server" CssClass="input" Text="添加>" OnClick="Button3_Click" /></td>
                  </tr>
                  <tr>
                    <td height="30px" align="center">
                        <asp:Button ID="Button2" runat="server" CssClass="input" Text="<移除" OnClick="Button2_Click" /></td>  
                  </tr>
               </table>
                 
               </td>  
                 
                  
            <td align="center" width="45%">
                &nbsp;<asp:ListBox ID="ListBox2" runat="server" Height="286px" Width="178px" SelectionMode="Multiple"></asp:ListBox>
            </td>
          </tr>
          <tr>
            <td align="right" colspan="3" class="style2">
                <asp:Label ID="lblsum" runat="server" Text=""></asp:Label>&nbsp;&nbsp;&nbsp;<asp:Label 
                    ID="qunidlb" runat="server" Text="Label" Visible="False"></asp:Label>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
          </tr>
          <tr>
            <td colspan="3"><asp:Label ID="LblName" runat="server"></asp:Label>
                <asp:Label ID="LblID" runat="server"></asp:Label></td>
          </tr>
          <tr>
            <td height="35px" align="center" colspan="3">
                &nbsp;<asp:Button ID="Button5" runat="server" Enabled="False" OnClick="Button5_Click" Visible="False" />
                <asp:Button ID="Button6" runat="server" Text="添加" OnClick="Button6_Click" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="fhbtn" runat="server" onclick="fhbtn_Click" Text="返回" />
              </td>
          </tr>
          <tr>
            <td height="30px" colspan="3" align="center">
                说明：用户只需要把左边系统列表收件人添加到右边的列表框，并确定就可以了。</td>
          </tr>
       </table>
    </div>
    </form>
</body>
</html>

