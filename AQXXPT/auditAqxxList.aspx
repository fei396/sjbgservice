<%@ Page Language="C#" AutoEventWireup="true" CodeFile="auditAqxxList.aspx.cs" Inherits="auditAqxxList" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312"/>
    <title></title>

    <link href="css/reset.css" rel="stylesheet" type="text/css"/>
    <link href="css/common.css" rel="stylesheet" type="text/css"/>
</head>
<body style="background-color: #F7Feff" align="center">
<form id="form1" runat="server" align="center">

    <hr align="center" color="#00cc00" noshade="noshade" width="96%"/>
    <div align="center">
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
                      BorderStyle="Solid" BorderWidth="2px" Caption="待审核申请：" CaptionAlign="Right" CellPadding="4"
                      ForeColor="#333333" Width="80%" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting"
                      PageSize="15" OnPageIndexChanging="GridView1_PageIndexChanging">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"/>

            <PagerStyle HorizontalAlign="Center"/>
            <EmptyDataTemplate>
                暂无数据
            </EmptyDataTemplate>
            <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333"/>
            <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White"/>
            <EditRowStyle BackColor="Azure"/>
            <Columns>
                <asp:BoundField DataField="sender" HeaderText="申请人" SortExpression="sender">
                    <ControlStyle Width="100px"/>
                    <ItemStyle Width="100px"/>
                </asp:BoundField>
                <asp:BoundField DataField="sendTime" HeaderText="申请时间"
                                SortExpression="sendTime">
                    <ControlStyle Width="120px"/>
                    <ItemStyle Width="120px"/>
                </asp:BoundField>
                <asp:BoundField DataField="title" HeaderText="信息标题" SortExpression="title"/>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%# Eval("id", "auditAqxx.aspx?xxid={0}") %>' Text="查看详情"></asp:HyperLink>
                    </ItemTemplate>
                    <ControlStyle Width="60px"/>
                    <ItemStyle Width="60px"/>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</form>
</body>
</html>