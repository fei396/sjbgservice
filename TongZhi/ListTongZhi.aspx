<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ListTongZhi.aspx.cs" Inherits="ListTongZhi" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">

* {
font-family: "Microsoft YaHei"
}

* { word-break: break-all; word-wrap: break-word; }
td { font:12px  Tahoma, Arial, Helvetica, snas-serif; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        <table id="tableChaXun" runat="server" align="center" width="80%">
            <tr>
                <td id="tdLeiXing1" runat="server" align="left" valign="middle" width="10%">通知类型：</td>
                <td id="tdLeiXing2" runat="server" width="15%">
                    <asp:DropDownList ID="ddlLeiXing" runat="server" >
                    </asp:DropDownList>
                </td>
                <td align="left" valign="middle" width="15%">标题关键字：</td>
                <td width="15%">
                    <asp:TextBox ID="txtBiaoTi" runat="server" Width="90%"></asp:TextBox>
                </td>
                <td align="left" valign="middle" width="10%">开始日期：</td>
                <td width="15%">
                    <asp:TextBox ID="txtStart" runat="server" onClick="WdatePicker()" Width="90%"></asp:TextBox>
                </td>
                <td align="left" valign="middle" width="10%">截至日期：</td>
                <td width="15%">
                    <asp:TextBox ID="txtEnd" runat="server" onClick="WdatePicker()" Width="90%"></asp:TextBox>
                </td>
                <td align="center">
                    <asp:Button ID="btnChaXun" runat="server" OnClick="btnChaXun_Click" Text="查  询" />
                </td>
            </tr>
        </table>
        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        <div align="center">
            <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False" BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4" DataKeyNames="LiuZhuanID" ForeColor="#333333"  PageSize="15" Width="90%">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />

                <PagerStyle HorizontalAlign="Center" />
                <EmptyDataTemplate>
                    没有该类型通知
                </EmptyDataTemplate>
                <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="Azure" />
                <Columns>
                    <asp:BoundField DataField="XuHao" HeaderText="序号" />
                    <asp:BoundField DataField="FaSongRen" HeaderText="发布人" />
                    <asp:BoundField DataField="TongZhiLeiXing" HeaderText="通知类型" />
                    <asp:BoundField DataField="BiaoTi" HeaderText="标题" />
                    <asp:BoundField DataField="FaSongShiJian" HeaderText="时间" />
                    <asp:TemplateField>
                        <HeaderTemplate>
                            签收情况
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblQianShouQingKuang" runat="server" ForeColor='<%#Eval("ShiFouQianShou").Equals(1)?System.Drawing.Color.Green:System.Drawing.Color.Red %>' Text='<%#Eval("QianShouQingKuang") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            可用操作
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl='<%#"ViewTongZhi.aspx?lzid=" + Eval("LiuZhuanID") + "&tzid=" + Eval("TongZhiID") + "&type=" + Eval("ShiFouQianShou") %>' Text='<%#Convert.ToInt32(Eval("ShiFouQianShou"))==0?"签收文件":"查看详情" %>'></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <table width="80%">
                <tr>
                    <td align="right" width="30%">第<asp:Label ID="lblcurPage" runat="server"></asp:Label>
                        页/共<asp:Label ID="lblPageCount" runat="server"></asp:Label>
                        页 </td>
                    <td align="center" width="40%">
                        <asp:LinkButton ID="cmdFirstPage" runat="server" OnClick="First_Click">首页</asp:LinkButton>
                        <asp:LinkButton ID="cmdPreview" runat="server" OnClick="Pre_Click">前页</asp:LinkButton>
                        <asp:LinkButton ID="cmdNext" runat="server" OnClick="Next_Click">后页</asp:LinkButton>
                        <asp:LinkButton ID="cmdLastPage" runat="server" OnClick="Last_Click">尾页</asp:LinkButton>
                        转第<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text="<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>" Width="32px"></asp:TextBox>
                        <asp:Button ID="Button1" runat="server" OnClick="Custom_Click" Text="页" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
