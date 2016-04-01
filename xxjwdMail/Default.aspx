<%@ Page Title="主页" Language="C#"  AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AjaxMail._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>新乡机务段邮件管理系统</title>
</head>
<frameset rows="127,*" cols="*" frameborder="no" border="0" framespacing="0">
  <frame src="header.aspx" name="topFrame" scrolling="No" noresize="noresize" id="topFrame" title="topFrame" />
  <frameset cols="147,*" frameborder="no" border="0" framespacing="0">
    <frame src="mailtree.aspx" name="MenuTree" scrolling="No" noresize="noresize" id="leftFrame" title="leftFrame" height="100%"/>
    <frame src="Mailbox.aspx" name="Mailbox" id="Mailbox" title="mainFrame" height="100%" />
  </frameset>
</frameset>
<noframes><body>
</body>
</noframes></html>

