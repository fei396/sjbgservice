<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AjaxMail.Login" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>新乡机务段邮箱管理系统</title>
<style type="text/css">
<!--
body {
	margin-left: 0px;
	margin-top: 0px;
	margin-right: 0px;
	margin-bottom: 0px;
	overflow:hidden;
}
.STYLE3 {font-size: 12px; color: #adc9d9; }
-->
</style></head>

<body>
    <form id="form1" runat="server">
    <table width="100%"  height="100%" border="0" cellspacing="0" cellpadding="0">
  <tr>
    <td bgcolor="#1075b1">&nbsp;</td>
  </tr>
  
  
  
  <tr>
    <td height="608" background="Images/login_03.gif">
	    <table width="847" border="0" align="center" cellpadding="0" cellspacing="0">
      
	       <tr>
              <td height="318" background="Images/login_04.gif">&nbsp;</td>
           </tr>  
	  
           <tr>
              <td height="84">
		          <table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr>
                          <td width="381" height="84" background="Images/login_06.gif">&nbsp;</td>
                          <td width="162" valign="middle" background="Images/login_07.gif">
						       <table width="100%" border="0" cellspacing="0" cellpadding="0">

                                         <tr>
                                            <td width="44" height="24" valign="bottom"><div align="right"><span class="STYLE3">用户</span></div></td>
                                            <td width="10" valign="bottom">&nbsp;</td>
                                            <td height="24" valign="bottom">
                                               <div align="left">
                                               <asp:TextBox ID="user_txt" runat="server" Width="90px" Height="20px"></asp:TextBox>
                                                 </div></td>
                                         </tr>
                                        <tr>
                                             <td height="24" valign="bottom"><div align="right"><span class="STYLE3">密码</span></div></td>
                                             <td width="10" valign="bottom">&nbsp;</td>
                                             <td height="24" valign="bottom">
                                                   <asp:TextBox ID="pass_txt" runat="server" Width="90px" Height="20px" 
                                                    TextMode="Password"></asp:TextBox>
                                              </td>
                                        </tr>

                                    </table>
							</td>
                          <td width="26"><img src="Images/login_08.gif" width="26" height="84"></td>
                          <td width="67" background="Images/login_09.gif" valign="center" align="center">
						         <table width="100%" border="0" cellspacing="0" cellpadding="0">
              
                                    <tr>
                                        <td height="24"><div align="center">
                                              <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="Images/dl.gif" 
                                                   onclick="ImageButton1_Click" />
                                        </div></td>
                                   </tr>
                                   <tr>
                                        <td height="24"><div align="center">
                                              <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="Images/cz.gif" 
                                              onclick="ImageButton2_Click" />
                                        </div></td>
                                   </tr>
                                 </table>
						 </td>
                         <td width="211" background="Images/login_10.gif">&nbsp;</td>
                     </tr>
                </table>
		
		     </td>
          </tr>
          <tr>
                <td height="206" background="Images/login_11.gif">&nbsp;</td>
          </tr>
      </table>	
	</td>
  </tr>
  <tr>
    <td bgcolor="#152753">&nbsp;</td>
  </tr>
</table>
</form>
</body>
</html>

