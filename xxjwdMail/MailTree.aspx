<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MailTree.aspx.cs" Inherits="AjaxMail.MailTree"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
    <style type="text/css">
TD {
	FONT-SIZE: 13px;
	COLOR: #666666;
	LINE-HEIGHT: 160%;
	font-weight: bold;
}
A:link {
	COLOR: #333333; TEXT-DECORATION: none
}
A:visited {
	COLOR: #333333; TEXT-DECORATION: none
}
A:active {
	COLOR: #ff0000; TEXT-DECORATION: none
}
A:hover {
	COLOR: #000000; TEXT-DECORATION: underline
}

BODY {
	background-color:#ADD2DA;
	margin-left: 0px;
	margin-top: 0px;
}

        .style1
        {
            cursor: hand;
            height: 23px;
        }

    </style>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" /></head>
<body style="margin-bottom:0;margin-left:0;margin-right:0;margin-top:0">
    <form id="form1" runat="server">
    <div height="100%">
    <table  height="1000" border="0" cellpadding="0" cellspacing="0">
    <tr>
    <td width="8" bgcolor="#353c44" height="100%">&nbsp;</td>
    <td height="100%" valign="top">




<table height="260px">

	          <tr>
          <td height="38"><table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
              <td width="33" height="28"><img src="Images/outfolder.gif" width="19" height="20"></td>
              <td width="99"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr> 
                          <td height="23" class="STYLE4" style="cursor:hand" onMouseOver="this.style.backgroundImage='url(images/tab_bg.gif)';this.style.borderStyle='solid';this.style.borderWidth='1';borderColor='#adb9c2'; "onmouseout="this.style.backgroundImage='url()';this.style.borderStyle='none'"><a href="sendmail.aspx" target="Mailbox">发送新邮件</a></td>
                  </tr>
              </table></td>
            </tr>
          </table></td>
        </tr>
        	          <tr>
          <td height="38"><table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
              <td width="33" height="28"><img src="images/infolder.gif" width="19" height="20"></td>
              <td width="99"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr> 
                          <td height="23" class="STYLE4" style="cursor:hand" onMouseOver="this.style.backgroundImage='url(images/tab_bg.gif)';this.style.borderStyle='solid';this.style.borderWidth='1';borderColor='#adb9c2'; "onmouseout="this.style.backgroundImage='url()';this.style.borderStyle='none'"><a href="mailbox.aspx" target="Mailbox">接收新邮件</a></td>
                  </tr>
              </table></td>
            </tr>
          </table></td>
        </tr>
        <tr>
          <td height="38"><table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
              <td width="33" height="28"><img src="images/navdot.gif" width="12" height="14"></td>
              <td width="99"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr> 
                        <td height="23" class="STYLE4" style="cursor:hand" onMouseOver="this.style.backgroundImage='url(images/tab_bg.gif)';this.style.borderStyle='solid';this.style.borderWidth='1';borderColor='#adb9c2'; "onmouseout="this.style.backgroundImage='url()';this.style.borderStyle='none'"><a href="dustibnmail.aspx" target="Mailbox">垃圾箱</a></td>
                  </tr>
              </table></td>
            </tr>
          </table></td>
        </tr>
                <tr>
          <td height="38"><table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
              <td width="33" height="28"><img src="images/addr.gif" width="19" height="20"></td>
              <td width="99"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr> 
                        <td class="style1" 
                              onMouseOver="this.style.backgroundImage='url(images/tab_bg.gif)';this.style.borderStyle='solid';this.style.borderWidth='1';borderColor='#adb9c2'; "
                              onmouseout="this.style.backgroundImage='url()';this.style.borderStyle='none'"><a href="grouplist.aspx" target="Mailbox">群发设置</a></td>
                  </tr>
              </table></td>
            </tr>
          </table></td>
        </tr>
        <tr>
          <td height="38"><table width="100%" border="0" cellspacing="0" cellpadding="0">
            <tr>
              <td width="33" height="28"><img src="images/openfolder.gif" width="19" height="20"></td>
              <td width="99"><table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr> 
                        <td height="23"  style="cursor:hand" onMouseOver="this.style.backgroundImage='url(images/tab_bg.gif)';this.style.borderStyle='solid';this.style.borderWidth='1';borderColor='#adb9c2'; "onmouseout="this.style.backgroundImage='url()';this.style.borderStyle='none'"><a href="yfssendmail.aspx" target="Mailbox">已发送邮件</a></td>
                  </tr>
              </table></td>
            </tr>
          </table></td>
        </tr>
      </table>




    </td>
    <td></td>
    </tr>
    </table>
    </div>
    </form>
</body>
</html>
