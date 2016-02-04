<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BuMen.aspx.cs" Inherits="BuMen" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <script language="javascript" type="text/javascript" src="js/WdatePicker.js" charset="gb2312"></script>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body style="background-color: #F7Feff" align="center">
    <form id="form1" runat="server" align="center">

        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        <table align="center" width="50%">
            <tr>

                <td align="left" valign="middle" width="20%">部门：</td>
                <td width="30%">
                    <asp:TextBox ID="txtBuMen" runat="server" Width="90%"></asp:TextBox>
                </td>

                <td align="center">
                    <asp:Button ID="btnAdd" runat="server" Text="添  加" OnClick="btnAdd_Click" /></td>
            </tr>

        </table>

        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />

        <div align="center">
            <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
                BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4"
                ForeColor="#333333" Width="50%" OnRowCancelingEdit="gvList_RowCancelingEdit" OnRowEditing="gvList_RowEditing" OnRowUpdating="gvList_RowUpdating" OnRowDeleting="gvList_RowDeleting">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />

                <PagerStyle HorizontalAlign="Center" />
                <EmptyDataTemplate>
                    当前没有自定义部门，请在上方添加
                </EmptyDataTemplate>
                <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="Azure" />
                <Columns>
                    <asp:TemplateField HeaderText="序号" InsertVisible="False">
                        <ItemStyle HorizontalAlign="Center" />
                        <HeaderStyle HorizontalAlign="Center" Width="10%" />
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# this.gvList.PageIndex * this.gvList.PageSize + this.gvList.Rows.Count + 1%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="MingCheng" HeaderText="名称" />



                    <asp:CommandField ShowEditButton="True" />
                    <asp:CommandField ShowDeleteButton="True" />
                    <asp:TemplateField>
                        <HeaderTemplate>
                            可用操作
                        </HeaderTemplate>
                    <ItemTemplate>
                            <asp:HyperLink ID="HyperLink1" runat="server"
                                NavigateUrl='<%#"BuMenDetail.aspx?id=" + Eval("ID") + "&mc=" + Eval("MingCheng") %>'
                                Text="查看详情"></asp:HyperLink>
                        </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <%--<table width="80%">
                <tr>
                    <td align="right" width="30%">第<asp:Label ID="lblcurPage" runat="server"></asp:Label>页/共<asp:Label
                        ID="lblPageCount" runat="server"></asp:Label>页
                    </td>
                    <td align="center" width="40%">
                        <asp:LinkButton ID="cmdFirstPage" runat="server" OnClick="First_Click">首页</asp:LinkButton>
                        <asp:LinkButton ID="cmdPreview" runat="server" OnClick="Pre_Click">前页</asp:LinkButton>
                        <asp:LinkButton ID="cmdNext" runat="server" OnClick="Next_Click">后页</asp:LinkButton>
                        <asp:LinkButton ID="cmdLastPage" runat="server" OnClick="Last_Click">尾页</asp:LinkButton>
                        转第<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>'
                            Width="32px"></asp:TextBox><asp:Button ID="Button1" runat="server" OnClick="Custom_Click"
                                Text="页" />
                    </td>
                </tr>
            </table>--%>
        </div>
    </form>
</body>
</html>
