<%@ Page Language="C#" AutoEventWireup="true" CodeFile="editfile.aspx.cs" Inherits="editfile" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <script type="text/javascript" src="js/coolWindowsCalendar.js"></script>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body style="background-color: #F7Feff" align="center">
    <form id="form1" runat="server" align="center">

    <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
    <div align="center">
   <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
        BorderStyle="Solid" BorderWidth="2px" Caption="已发送文件：" CaptionAlign="Right" CellPadding="4"
        ForeColor="#333333" Width="80%" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting"
        PageSize="15" OnPageIndexChanging="GridView1_PageIndexChanging" >
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerTemplate>
            <table width="100%">
                <tr>
                    <td align="left" width="30%">
                        第<asp:Label ID="lblcurPage" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1      %>'></asp:Label>页/共<asp:Label
                            ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>'></asp:Label>页
                    </td>
                    <td align="center" width="40%">
                        <asp:LinkButton ID="cmdFirstPage" runat="server" CommandArgument="First" CommandName="Page"
                            Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=0 %>">首页</asp:LinkButton>
                        <asp:LinkButton ID="cmdPreview" runat="server" CommandArgument="Prev" CommandName="Page"
                            Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=0 %>">前页</asp:LinkButton>
                        <asp:LinkButton ID="cmdNext" runat="server" CommandArgument="Next" CommandName="Page"
                            Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=((GridView)Container.Parent.Parent).PageCount-1 %>">后页</asp:LinkButton>
                        <asp:LinkButton ID="cmdLastPage" runat="server" CommandArgument="Last" CommandName="Page"
                            Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=((GridView)Container.Parent.Parent).PageCount-1 %>">尾页</asp:LinkButton>
                        转第<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>'
                            Width="32px"></asp:TextBox><asp:Button ID="Button1" runat="server" OnClick="Button1_Click"
                                Text="页" />
                    </td>
                </tr>
            </table>
        </PagerTemplate>
        <PagerStyle HorizontalAlign="Center" />
        <EmptyDataTemplate>
            暂无数据
        </EmptyDataTemplate>
        <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="Azure" />
        <Columns>
            <asp:BoundField DataField="fileName" HeaderText="文件名" SortExpression="fileName" />
            <asp:BoundField DataField="sender" HeaderText="发送人" SortExpression="sender" />
            <asp:BoundField DataField="sendTime" HeaderText="发送时间" SortExpression="sendTime" />
            <asp:BoundField DataField="allCount" HeaderText="发送人数" SortExpression="allCount" />
            <asp:BoundField DataField="ReceiveCount" HeaderText="接收人数" SortExpression="ReceiveCount" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("FID", "editfile_detail.aspx?fid={0}") %>' Text="查看详情"></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </div>
    </form>
</body>
</html>
