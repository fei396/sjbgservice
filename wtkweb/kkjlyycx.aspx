<%@ Page Language="C#" AutoEventWireup="true" CodeFile="kkjlyycx.aspx.cs" Inherits="kkjlyycx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body style=" background-color :#F7Feff ">
    <form id="form1" runat="server">
        &nbsp;<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AutoManageConnectionString %>" SelectCommand="SELECT xmmc, [Work_no], [Work_name], [Department], [kklx], [czr] + [czrxm] AS czr, [czsj], [Team_mode], [Coach_team], [Duty], xmmc,kkxmlx,mdlx FROM [V_CQKK_KKJLYYB] WHERE kkjlid = @kkjlid">
            <SelectParameters>
                <asp:QueryStringParameter DefaultValue="0" Name="kkjlid" QueryStringField="kkjlid" />
            </SelectParameters>
         
        </asp:SqlDataSource>
                            <hr align="center" color="#ff0000" noshade="noshade" width="96%" />
        &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
                                <table align="center" border="0" cellpadding="0" cellspacing="0" width="200">
                                    <tr>
                                        <td align="center" valign="bottom" style="height: 30px">
                                            <input onclick="window.print();" style="border-top-width: 1px; padding-right: 1px;
                                                padding-left: 1px; border-left-width: 1px; font-size: 9pt; border-left-color: rgb(204,204,204);
                                                border-bottom-width: 1px; border-bottom-color: rgb(204,204,204); padding-bottom: 1px;
                                                border-top-color: rgb(204,204,204); padding-top: 4px; height: 23px; border-right-width: 1px;
                                                border-right-color: rgb(204,204,204)" type="button" value=" �� ӡ " />
                                            &nbsp;<asp:Button runat="server" ID="saveBtn"  style="border-top-width: 1px;
                                                padding-right: 1px; padding-left: 1px; border-left-width: 1px; font-size: 9pt;
                                                border-left-color: rgb(204,204,204); border-bottom-width: 1px; border-bottom-color: rgb(204,204,204);
                                                padding-bottom: 1px; border-top-color: rgb(204,204,204); padding-top: 4px; height: 23px;
                                                border-right-width: 1px; border-right-color: rgb(204,204,204)"
                                                Text="����Excel" OnClick="saveBtn_Click" /></td>
                                    </tr>
                                </table>
                            <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        &nbsp;&nbsp;<asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
            BorderStyle="Solid" BorderWidth="2px" Caption="����Ϣ��" CaptionAlign="Right" CellPadding="4"
             ForeColor="#333333"
            Width="100%" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting" PageSize="15" OnPageIndexChanging="GridView1_PageIndexChanging" DataSourceID="SqlDataSource1">
     
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerTemplate>
                <table width="100%">
                    <tr>
                        <td align="left" width="30%">
                            ��<asp:Label ID="lblcurPage" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1      %>'></asp:Label>ҳ/��<asp:Label
                                ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>'></asp:Label>ҳ</td>
                        <td align="center" width="40%">
                            <asp:LinkButton ID="cmdFirstPage" runat="server" CommandArgument="First" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=0 %>">��ҳ</asp:LinkButton>
                            <asp:LinkButton ID="cmdPreview" runat="server" CommandArgument="Prev" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=0 %>">ǰҳ</asp:LinkButton>
                            <asp:LinkButton ID="cmdNext" runat="server" CommandArgument="Next" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=((GridView)Container.Parent.Parent).PageCount-1 %>">��ҳ</asp:LinkButton>
                            <asp:LinkButton ID="cmdLastPage" runat="server" CommandArgument="Last" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=((GridView)Container.Parent.Parent).PageCount-1 %>">βҳ</asp:LinkButton>
                            ת��<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>'
                                Width="32px"></asp:TextBox><asp:Button ID="Button1" runat="server" 
                                OnClick="Button1_Click"     Text="ҳ" /></td>
                    </tr>
                </table>
            </PagerTemplate>
            <PagerStyle HorizontalAlign="Center" />
            <EmptyDataTemplate>
               δ�ҵ�����Ա������
            </EmptyDataTemplate>
            <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="Azure" />
            <Columns>
                <asp:BoundField DataField="xmmc" HeaderText="��Ŀ����" />
            <asp:BoundField DataField="Work_no" HeaderText="��Ա����" SortExpression="Work_no" />
                <asp:BoundField DataField="work_name" HeaderText="��Ա����" SortExpression="Work_name" />
                <asp:BoundField DataField="Department" HeaderText="����" SortExpression="Department" />
                <asp:TemplateField HeaderText="��Ŀ����" SortExpression="kklx">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("kklx") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# getKKXMLX(Convert.ToInt32(Eval("kkxmlx"))) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                                <asp:TemplateField HeaderText="��������" SortExpression="kklx">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1243" runat="server" Text='<%# Bind("kklx") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label15123" runat="server" Text='<%# getMDLX(Convert.ToInt32(Eval("mdlx"))) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="czsj" HeaderText="����ʱ��" SortExpression="czsj" />
                <asp:BoundField DataField="czr" HeaderText="����Ա" SortExpression="czr" />
            </Columns>

        </asp:GridView>
                     </form>
</body>
</html>
