<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GroupList.aspx.cs" Inherits="AjaxMail.GroupList" StylesheetTheme="Web2ASPNET2"%>

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
					<td>收件箱</td>
				</tr>	
				<tr>
					<td>
						<asp:GridView ID="gvGroupView" runat="server" AutoGenerateColumns="False" 
                            Font-Names="Tahoma" Width="100%" SkinID="gvSkin" DataKeyNames="ID" 
                            EmptyDataText="邮件为空" OnRowCommand="gvMailbox_RowCommand" AllowPaging="True"
                            OnPageIndexChanging="gvMailbox_PageIndexChanging" PageSize="20" 
                            onrowdatabound="gvMailbox_RowDataBound" 
                            onrowediting="gvGroupView_RowEditing">
							<HeaderStyle Font-Names="Tahoma" HorizontalAlign="Center" />
							<EmptyDataRowStyle ForeColor="Blue" />
							<RowStyle BorderColor="#DAEEEE" BorderStyle="Ridge" BorderWidth="1px" HorizontalAlign="Center" />
							<Columns>

								<asp:TemplateField ItemStyle-Width="10%" ItemStyle-HorizontalAlign="Left" 
                                    HeaderText="群名称">
									<ItemTemplate>
										<%# Eval("GroupName")%> 
									</ItemTemplate>
								    <ItemStyle HorizontalAlign="Left" Width="10%" />
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-Width="80%" ItemStyle-HorizontalAlign="center" 
                                    HeaderText="群成员">
									<ItemTemplate>
										<%# Eval("GroupMember")%> 
									</ItemTemplate>
								    <ItemStyle HorizontalAlign="Center" Width="80%" />
								</asp:TemplateField>
								<asp:TemplateField ItemStyle-Width="5%" ItemStyle-HorizontalAlign="center" 
                                    HeaderText="编辑">
									<ItemTemplate>
										<a href='AddGroup.aspx?GroupID=<%#Eval("ID") %>'>编辑</a> 
									</ItemTemplate>
								    <ItemStyle HorizontalAlign="Center" Width="5%" />
								</asp:TemplateField>
    
                                <asp:TemplateField ItemStyle-Width="5%" HeaderText="删除">
                                <ItemTemplate>
                                <asp:ImageButton ID="imgDelete" runat="server" CommandArgument='<%# Eval("ID")  %>' ImageUrl="~/Images/delete.gif" CommandName="del" />
                                </ItemTemplate>                                
                                    <ItemStyle Width="5%" />
                                </asp:TemplateField>	

							</Columns>
						</asp:GridView>
					</td>
				</tr>
				<tr>
					<td><asp:Button ID="btnTag" runat="server" Text="新增群" 
                            SkinID="btnSkin" OnClick="btnTag_Click" /></td>
				</tr>
			</table>    
		</ContentTemplate> 
    </asp:UpdatePanel>
    </form>
</body>
</html>
