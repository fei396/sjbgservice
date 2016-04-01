<%@ Page Language="C#" AutoEventWireup="true" CodeFile="editfile_detail.aspx.cs" Inherits="EditfileDetail" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312"/>
    <title></title>
    <script type="text/javascript" src="js/coolWindowsCalendar.js"></script>
    <link href="css/reset.css" rel="stylesheet" type="text/css"/>
    <link href="css/common.css" rel="stylesheet" type="text/css"/>
</head>
<body style="background-color: #F7Feff" align="center">
<form id="form1" runat="server" align="center">
    <hr align="center" color="#00cc00" noshade="noshade" width="96%"/>
        <div align="center">
        
        <asp:Label ID="Label1" runat="server" Text="选择状态："></asp:Label>&nbsp;&nbsp;&nbsp;
        <asp:DropDownList ID="ddlStatus" runat="server">
            <asp:ListItem Value="0">全部状态</asp:ListItem>
            <asp:ListItem Value="1">尚未发送</asp:ListItem>
            <asp:ListItem Value="2">暂无回执</asp:ListItem>
            <asp:ListItem Value="3">发送失败</asp:ListItem>
            <asp:ListItem Value="4">发送成功</asp:ListItem>
        </asp:DropDownList>
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnChaXun" runat="server" Text="查询" OnClick="btnChaXun_Click" />
        
        </div>
    <hr align="center" color="#00cc00" noshade="noshade" width="96%"/>
    <div align="center">
        <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
                      BorderStyle="Solid" BorderWidth="2px" Caption="文件接收详情：" CaptionAlign="Right" CellPadding="4"
                      ForeColor="#333333" Width="80%" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting"
                      PageSize="15" OnPageIndexChanging="GridView1_PageIndexChanging">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"/>
            <PagerTemplate>
                <table width="100%">
                    <tr>
                        <td align="left" width="30%">
                            第<asp:Label ID="lblcurPage" runat="server"></asp:Label>页/共<asp:Label
                                                                                          ID="lblPageCount" runat="server">
                            </asp:Label>页
                        </td>
                        <td align="center" width="40%">
                            <asp:LinkButton ID="cmdFirstPage" runat="server" OnClick="First_Click">首页</asp:LinkButton>
                            <asp:LinkButton ID="cmdPreview" runat="server" OnClick="Pre_Click">前页</asp:LinkButton>
                            <asp:LinkButton ID="cmdNext" runat="server" OnClick="Next_Click">后页</asp:LinkButton>
                            <asp:LinkButton ID="cmdLastPage" runat="server" OnClick="Last_Click">尾页</asp:LinkButton>
                            转第<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text="<%# ((GridView) Container.Parent.Parent).PageIndex + 1 %>"
                                           Width="32px">
                            </asp:TextBox><asp:Button ID="Button1" runat="server" OnClick="Custom_Click"
                                                      Text="页"/>
                        </td>
                    </tr>
                </table>
            </PagerTemplate>
            <PagerStyle HorizontalAlign="Center"/>
            <EmptyDataTemplate>
                暂无数据
            </EmptyDataTemplate>
            <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333"/>
            <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White"/>
            <EditRowStyle BackColor="Azure"/>
            <Columns>


                <asp:BoundField DataField="ReceiverDept" HeaderText="接收人部门" SortExpression="ReceiverDept"/>
                <asp:BoundField DataField="Receiver" HeaderText="接收人" SortExpression="Receiver"/>
                <asp:BoundField DataField="Status" HeaderText="状态" SortExpression="Status"/>
                <asp:BoundField DataField="ReceiveTime" HeaderText="接收时间" SortExpression="ReceiveTime"/>
            </Columns>
        </asp:GridView>
    </div>
</form>
</body>
</html>