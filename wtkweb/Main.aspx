<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Main.aspx.cs" Inherits="adminMain" %>



<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>派班安全卡控系统</title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
<link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">

        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AutoManageConnectionString %>" SelectCommand="SELECT [fsrxm], [fsrid], [id], [bfsrid], [fssj], [fslx], [fsnr], [wjdz], [fsip], [sfck] FROM [V_CQKK_XXFSB] WHERE (([bfsrid] = @bfsrid) AND ([sfck] = @sfck)) order by fssj"
        DeleteCommand="update T_CQKK_XXFSB set sfck=1 where id=@id" >
            <SelectParameters>
                <asp:SessionParameter Name="bfsrid" SessionField="usercode" Type="String" />
                <asp:Parameter DefaultValue="0" Name="sfck" Type="Int32" />
            </SelectParameters>
            <DeleteParameters>
                <asp:Parameter Name="id" Type="int32" />
            </DeleteParameters>
        </asp:SqlDataSource>
		    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label><br />
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
            BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4"
            DataKeyNames="ID" DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None"
            Width="100%">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#EFF3FB" />
            <Columns>
                <asp:BoundField DataField="fsrxm" HeaderText="消息发送人" SortExpression="BianHao">
                    <ItemStyle Width="100px" Wrap="False" />
                    <ControlStyle Width="100px" />
                </asp:BoundField>
                <asp:BoundField DataField="fsnr" HeaderText="消息内容" SortExpression="fsnr" />
                <asp:BoundField DataField="fssj" HeaderText="发送时间" SortExpression="fssj" >
                    <ControlStyle Width="120px" />
                    <ItemStyle Width="120px" />
                </asp:BoundField>
              <asp:HyperLinkField DataNavigateUrlFields="wjdz" Target="_blank" DataNavigateUrlFormatString="upexcel/{0}" DataTextField="wjdz" HeaderText="附件地址"
                    >
                    <ControlStyle Width="300px" />
                    <ItemStyle Width="300px" />
                </asp:HyperLinkField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete"
                            Text="确认"></asp:LinkButton>
                    </ItemTemplate>
                    <ControlStyle Width="60px" />
                    <ItemStyle Width="60px" />
                </asp:TemplateField>
            </Columns>
            <PagerStyle HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="Azure" />
            <AlternatingRowStyle BackColor="White" />
            <EmptyDataTemplate>
                无新消息
            </EmptyDataTemplate>
     
        </asp:GridView>

    </form>
</body>
</html>
