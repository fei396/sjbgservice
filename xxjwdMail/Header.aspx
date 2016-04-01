<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Header.aspx.cs" Inherits="AjaxMail.Header" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1">
    <title>无标题页</title>
    
 <style type="text/css">
<!--
body {
	margin-left: 0px;
	margin-top: 0px;
	margin-right: 0px;
	margin-bottom: 0px;
}
.STYLE1 {
	font-size: 12px;
	color: #000000;
}
.STYLE5 {font-size: 12}
.STYLE7 {font-size: 12px; color: #FFFFFF; }
     .style2
     {
         font-size: 12px;
         color: #FFFFFF;
         width: 47px;
     }
     A:link {
	COLOR: #444444; TEXT-DECORATION: none
}
A:visited {
	COLOR: #444444; TEXT-DECORATION: none
}
A:active {
	COLOR: #ff0000; TEXT-DECORATION: none
}
A:hover {
	COLOR: #ff0000; TEXT-DECORATION: underline
}
-->
</style>
<body>
    <form id="form1" runat="server">
<table width="100%" border="0" cellspacing="0" cellpadding="0">
  <tr>
    <td height="57" background="images/main_03.gif"><table width="100%" border="0" cellspacing="0" cellpadding="0">
      <tr>
        <td width="378" height="57" background="images/main_01.gif">&nbsp;</td>
        <td>&nbsp;</td>
        <td width="281" valign="bottom"><table width="100%" border="0" cellspacing="0" cellpadding="0">
          <tr>
            <td width="33" height="27"><img src="images/main_05.gif" width="33" height="27" /></td>
            <td width="248" background="images/main_06.gif"><table width="225" border="0" align="center" cellpadding="0" cellspacing="0">
              <tr>
                <td height="17"><div align="right"> <a href="yhxx.aspx" target="rightframe"> 
                            </a><a href="editpass.aspx" target="Mailbox"><img src="images/pass.gif" width="69" height="17" border="0" /></a> 
                          </div></td>
                <td><div align="right">

                    </div></td>
                <td><div align="right">
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="~/images/quit.gif" 
                        onclick="ImageButton1_Click" />
                    </div></td>
              </tr>
            </table></td>
          </tr>
        </table></td>
      </tr>
    </table></td>
  </tr>
  <tr>
    <td height="40" background="images/main_10.gif"><table width="100%" border="0" cellspacing="0" cellpadding="0">
      <tr>
        <td width="194" height="40" background="images/main_07.gif">&nbsp;</td>
        <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
          <tr>
            <td width="21"><img src="images/main_13.gif" width="19" height="14" /></td>
            <td class="style2"><div align="center"><a href="http://www.xxjwd.com" target="_blank"><font color="#FFFFFF">段主页</font></a></div></td>
            <td width="21" class="STYLE7"><img src="images/main_21.gif" width="19" height="14" /></td>
            <td width="35" class="STYLE7"><div align="center">帮助</div></td>
            <td width="21" class="STYLE7">&nbsp;</td>
            <td width="35" class="STYLE7"><div align="center"></div></td>
            <td width="21" class="STYLE7">&nbsp;</td>
            <td width="35" class="STYLE7"><div align="center"></div></td>
            <td width="21" class="STYLE7">&nbsp;</td>
            <td width="35" class="STYLE7"><div align="center"></div></td>
            <td>&nbsp;</td>
          </tr>
        </table></td>
        <td width="248" background="images/main_11.gif"><table width="100%" border="0" cellspacing="0" cellpadding="0">
          <tr>
            <td width="16%"><span class="STYLE5"></span></td>
            <td width="75%"><div align="center"><span class="STYLE7">时间：
            <SCRIPT language=JavaScript>
                today = new Date();
                function initArray() {
                    this.length = initArray.arguments.length
                    for (var i = 0; i < this.length; i++)
                        this[i + 1] = initArray.arguments[i]
                }
                var d = new initArray(
     "星期日",
     "星期一",
     "星期二",
     "星期三",
     "星期四",
     "星期五",
     "星期六");
                document.write(
     "<font color=#ffffff style='font-size:9pt;font-family: 宋体'> ",
     today.getYear(), "年",
     today.getMonth() + 1, "月",
     today.getDate(), "日",
     d[today.getDay() + 1],
     "</font>"); 
</SCRIPT>
            </span></div></td>
            <td width="9%">&nbsp;</td>
          </tr>
        </table></td>
      </tr>
    </table></td>
  </tr>
  <tr>
    <td height="30" background="images/main_31.gif"><table width="100%" border="0" cellspacing="0" cellpadding="0">
      <tr>
        <td width="8" height="30"><img src="images/main_28.gif" width="8" height="30" /></td>
        <td width="147" background="images/main_29.gif"><table width="100%" border="0" cellspacing="0" cellpadding="0">
          <tr>
            <td width="24%">&nbsp;</td>
            <td width="43%" height="20" valign="bottom" class="STYLE1">管理菜单</td>
            <td width="33%">&nbsp;</td>
          </tr>
        </table></td>
        <td width="39"><img src="images/main_30.gif" width="39" height="30" /></td>
        <td><table width="100%" border="0" cellspacing="0" cellpadding="0">
          <tr>
            <td height="20" valign="bottom"><span class="STYLE1">当前登录用户：<% Response.Write(Session["yhxm"].ToString()); %> &nbsp;用户工号：<% Response.Write(Session["yhgh"].ToString()); %> &nbsp;用户部门：<% Response.Write(Session["yhbm"].ToString()); %> &nbsp;&nbsp;&nbsp;您的操作时间为60分钟，请尽快发送。</span></td>
            <td valign="bottom" class="STYLE1"><div align="right">&nbsp;&nbsp; &nbsp;&nbsp;</div></td>
          </tr>
        </table></td>
        <td width="17"><img src="images/main_32.gif" width="22" height="30" /></td>
      </tr>
    </table></td>
  </tr>
</table>
    </form>
</body>
</html>
