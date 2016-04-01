<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="yfssendmail.aspx.cs" Inherits="AjaxMail.yfssendmail" StylesheetTheme="Web2ASPNET2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>无标题页</title>
</head>
<body style="margin:0;border:0" class="Body">
    <form id="form1" runat="server">
    <asp:ScriptManager ID="sm" runat="server"/>
    <asp:UpdatePanel ID="upForm" runat="server" UpdateMode="Always" RenderMode="Block">
		<ContentTemplate>
			<table class="Table" width="100%" cellpadding="2" cellspacing="0" border="1" bordercolor="#daeeee">
				<tr>
					<td><span style="color: rgb(0, 0, 0); font-family: Arial, Helvetica, sans-serif; font-size: 12px; font-style: normal; font-variant: normal; font-weight: normal; letter-spacing: normal; line-height: normal; orphans: auto; text-align: start; text-indent: 0px; text-transform: none; white-space: normal; widows: auto; word-spacing: 0px; -webkit-text-stroke-width: 0px; display: inline !important; float: none; background-color: rgb(255, 255, 247);">
                        已发送邮件&nbsp;&nbsp;&nbsp; 共<asp:Label ID="yjzslbl" runat="server" Text="Label"></asp:Label>
                        封邮件&nbsp;&nbsp;&nbsp;</span></td>
				</tr>	
				<tr>
					<td>
						<asp:GridView ID="gvMailbox" runat="server" AutoGenerateColumns="False" 
                            Font-Names="Tahoma" Width="100%" SkinID="gvSkin" DataKeyNames="ID" 
                            EmptyDataText="邮件为空" OnRowCommand="gvMailbox_RowCommand" AllowPaging="True"
                            OnPageIndexChanging="gvMailbox_PageIndexChanging" PageSize="20" 
                            onrowdatabound="gvMailbox_RowDataBound">
							<HeaderStyle Font-Names="Tahoma" HorizontalAlign="Center" />
							<EmptyDataRowStyle ForeColor="Blue" />
							<RowStyle BorderColor="#DAEEEE" BorderStyle="Ridge" BorderWidth="1px" HorizontalAlign="Center" />
							<Columns>
								<asp:TemplateField ItemStyle-Width="2%" ItemStyle-HorizontalAlign="center">
									<ItemTemplate>
										<asp:CheckBox ID="cbMail" runat="server" />
									</ItemTemplate>
								    <ItemStyle HorizontalAlign="Center" Width="2%" />
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-Width="35%" ItemStyle-HorizontalAlign="Left" HeaderText="收件人">
									<ItemTemplate>
										<%# Eval("fromaddress")%> 
									</ItemTemplate>
								    <ItemStyle HorizontalAlign="Left" Width="15%" />
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-Width="35%" ItemStyle-HorizontalAlign="center" HeaderText="主题">
									<ItemTemplate>
										<a href='ReadMail.aspx?fromym=yfsmail&MailID=<%#Eval("ID") %>'><%# Eval("title")%></a> 
									</ItemTemplate>
								    <ItemStyle HorizontalAlign="Center" Width="50%" />
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-Width="16%" ItemStyle-HorizontalAlign="center" HeaderText="发送时间">
									<ItemTemplate>
										<%# Eval("CreateDate")%>
									</ItemTemplate>
								    <ItemStyle HorizontalAlign="Center" Width="15%" />
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-Width="12%" ItemStyle-HorizontalAlign="center" HeaderText="邮件大小">
									<ItemTemplate>
										<%# (int)Eval("Size") / 1024 %>KB
									</ItemTemplate>
								    <ItemStyle HorizontalAlign="Center" Width="10%" />
								</asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="10%" HeaderText="操作">
                                <ItemTemplate>
                                <asp:ImageButton ID="imgDelete" runat="server" CommandArgument='<%# Eval("ID")  %>' ImageUrl="~/Images/delete.gif" CommandName="del" />
                                </ItemTemplate>                                
                                    <ItemStyle Width="10%" />
                                </asp:TemplateField>	

							    <asp:TemplateField Visible="False">
                                	<ItemTemplate>
										<%# Eval("readflag")%> 
									</ItemTemplate>
                                </asp:TemplateField>

							</Columns>
						</asp:GridView>
					</td>
				</tr>
				<tr>
					<td><b style="color: rgb(0, 0, 0); font-family: Arial, Helvetica, sans-serif; font-size: 12px; font-style: normal; font-variant: normal; letter-spacing: normal; line-height: normal; orphans: auto; text-align: -webkit-left; text-indent: 0px; text-transform: none; white-space: normal; widows: auto; word-spacing: 0px; -webkit-text-stroke-width: 0px; background-color: rgb(255, 255, 247);">
                        <asp:CheckBox ID="qxckb" runat="server" ForeColor="#CC3300" 
                            oncheckedchanged="qxckb_CheckedChanged" Text="全选" AutoPostBack="True" />
                        </b><span 
                            style="color: rgb(0, 0, 0); font-family: Arial, Helvetica, sans-serif; font-size: 12px; font-style: normal; font-variant: normal; font-weight: normal; letter-spacing: normal; line-height: normal; orphans: auto; text-align: -webkit-left; text-indent: 0px; text-transform: none; white-space: normal; widows: auto; word-spacing: 0px; -webkit-text-stroke-width: 0px; display: inline !important; float: none; background-color: rgb(255, 255, 247);"><span 
                            class="Apple-converted-space">&nbsp;</span>&nbsp;<span class="Apple-converted-space">&nbsp;<asp:Button ID="btnTag1" 
                            runat="server" OnClick="btnTag1_Click" SkinID="btnSkin" Text="删除邮件" />
                        &nbsp;&nbsp;</span></span></td>
				</tr>
			</table>    
		</ContentTemplate> 
    </asp:UpdatePanel>
    </form>
</body>
</html>
