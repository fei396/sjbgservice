<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>新乡机务段公文流转系统</title>

<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<link href="../css/style01.css" type="text/css" rel="stylesheet" />
<link href="../css/common.css" type="text/css" rel="stylesheet" />
<script type="text/javascript">
		function check(){
			if(document.forms[0].unameTextbox.value==""){
				alert("请输入您的用户名");
				document.forms[0].unameTextbox.focus();
				return false;
			}
			if(document.forms[0].upassTextbox.value==""){
				alert("请输入您的密码");
				document.forms[0].upassTextbox.focus();
				return false;
			}
			
			return true;
		}		
		


		</script>

<meta content="MSHTML 6.00.2900.5897" name="GENERATOR" /></head>

	<body bgcolor="#ffffff">
	<table>
	<tr height="150">
	<td></td></tr>
	</table>
		<table  width="706" border="0" valign="middle" align="center" cellpadding="0" background="images/login_bg1.jpg"
			cellspacing="0">
			<tr>
				<td height="111">
					<img src="images\cqkklogo.jpg" width="700" height="111" />
				</td>
			</tr>
			<tr>
				<td >
					<form action="" method="post" runat="server">
						<table width="100%" border="0" cellspacing="0" cellpadding="0">
							<tr>
								<td align="center">
									<asp:Label ID="errors" runat="server" Text="" ForeColor="red" CssClass="logintxt"></asp:Label>
								</td>
							</tr>
							<tr>
								<td align="center">
									<table width="360" border="0" cellspacing="0" cellpadding="0" id="TABLE1" >
										<tr>
											<td width="90" align="right" class="logintxt" style="height: 31px">
                                                用户名&nbsp; ：
											</td>
											<td colspan="2" align="left" style="width: 194px; height: 31px;">
												<asp:TextBox CssClass="text_input" ID="txtUcode"  runat="server" Width="140px"></asp:TextBox>
											</td>
											<td rowspan="4" align="center" valign="middle">
												&nbsp;<asp:Button ID="loginButton" OnClientClick="return check();" runat="server" Text="登  录" OnClick="loginButton_Click" TabIndex="4"></asp:Button></td>
										</tr>
										<tr>
											<td align="right" class="logintxt" style="height: 31px">
                                                密&nbsp; 码&nbsp; ：

											</td>
											<td colspan="2" align="left" style="width: 194px; height: 31px;">
												<asp:TextBox  CssClass="text_input" ID="txtUpass" runat="server" TextMode="Password" Width="140px"></asp:TextBox>
											</td>
										</tr>
									<%--	<tr>
											<td align="right" class="logintxt" style="height: 31px">
                                                验证码&nbsp; ：</td>
											<td align="left" valign="middle" style="width: 134px; height: 31px;">
                                                <asp:TextBox ID="TextBox1" runat="server" Width="140px" ></asp:TextBox>
												</td>
												<td width="60px"><asp:Image Runat="server" Height="23px"  ID="ImageCheck" ImageUrl="ValidateCode.aspx"></asp:Image></td>
										</tr>--%>
<%--										<tr>
											<td height="31" align="right" class="logintxt">
												身份：

											</td>
											<td align="left">
												<html:select property="usertype">
													<logic:present
														name="<%=Global.APPLICATION_ATTRIB_USER_TYPE_LIST%>">
														<html:options
															collection="<%=Global.APPLICATION_ATTRIB_USER_TYPE_LIST%>"
															property="dictId" labelProperty="dictName" />
													</logic:present>
												</html:select>
											</td>
										</tr>
--%>									</table>
                                    <a href="sysc.doc"> 系统使用手册下载</a>
                                    <br />
                                    <a href="导入数据模板.xls"> 导入数据模板下载</a></td>
							</tr>
							<tr>
								<td valign="bottom" style="height: 35px">
									<div align="center">
										<hr>
										Copyright &copy; 2011-07,新乡机务段网络中心 All Rights Reserved
									</div>
								</td>
							</tr>
						</table>
					</form>
				</td>
			</tr>
		</table>
		</body>
</html>
