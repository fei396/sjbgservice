<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ListGongWen.aspx.cs" Inherits="ListGongWen" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
<script language="javascript" type="text/javascript" src="js/WdatePicker.js"  charset="gb2312"></script>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body style="background-color: #F7Feff" align="center">
    <form id="form1" runat="server" align="center">

        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        <table runat="server" id="tableChaXun" align="center" width="80%">
            <tr>

                <td align="left" valign="middle" width="10%">关键字：</td><td width="15%"><asp:TextBox ID="txtBiaoTi" runat="server" width="90%"></asp:TextBox>
                </td>
                <td align="left" valign="middle" width="15%">开始日期：</td><td width="15%"><asp:TextBox ID="txtStart" runat="server" width="90%" onClick="WdatePicker()"></asp:TextBox>
                </td>
                <td align="left" valign="middle" width="15%">截至日期：</td><td width="15%"><asp:TextBox ID="txtEnd" runat="server" width="90%" onClick="WdatePicker()"></asp:TextBox>
                </td>
                <td align="center">
                    <asp:Button ID="btnChaXun" runat="server" Text="查  询" OnClick="btnChaXun_Click" /></td>
            </tr>
  
        </table>
        
        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        
        <div align="center">
            <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False"
                BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4"
                ForeColor="#333333" Width="90%" 
                PageSize="15" OnRowDeleting="gvList_RowDeleting"  DataKeyNames="LiuZhuanID">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"  />
                <PagerTemplate>
                    <%--  <table width="100%">
                <tr>
                    <td align="left" width="30%">
                        第<asp:Label ID="lblcurPage" runat="server"></asp:Label>页/共<asp:Label
                            ID="lblPageCount" runat="server" ></asp:Label>页
                    </td>
                    <td align="center" width="40%">
                        <asp:LinkButton ID="cmdFirstPage" runat="server" OnClick="First_Click"
                            >首页</asp:LinkButton>
                        <asp:LinkButton ID="cmdPreview" runat="server" OnClick="Pre_Click"
                            >前页</asp:LinkButton>
                        <asp:LinkButton ID="cmdNext" runat="server" OnClick="Next_Click"
                            >后页</asp:LinkButton>
                        <asp:LinkButton ID="cmdLastPage" runat="server" OnClick="Last_Click"
                            >尾页</asp:LinkButton>
                        转第<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>'
                            Width="32px"></asp:TextBox><asp:Button ID="Button1" runat="server" OnClick="Custom_Click"
                                Text="页" />
                    </td>
                </tr>
            </table>--%>
                </PagerTemplate>
                <PagerStyle HorizontalAlign="Center" />
                <EmptyDataTemplate>
         
                    当前没有新的待办公文，您可以<a href="ListGongWen.aspx?type=1">查看全部公文</a>
         

                </EmptyDataTemplate>
                <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="Azure" />
                <Columns>
                    <asp:BoundField DataField="XuHao" HeaderText="序号" />
                    <asp:BoundField DataField="FaSongRen" HeaderText="发送人" />
                    <asp:BoundField DataField="WenHao" HeaderText="文号" />
                    <asp:BoundField DataField="BiaoTi" HeaderText="标题" />
                    <asp:BoundField DataField="FaSongShiJian" HeaderText="时间" />
                    <asp:BoundField DataField="JinJi" HeaderText="紧急程度" />
                    <asp:TemplateField>
                        <HeaderTemplate>
                            签收情况
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblQianShouQingKuang" runat="server" ForeColor='<%#Eval("ShiFouQianShou").Equals(1)?System.Drawing.Color.Green:System.Drawing.Color.Red %>'
                                Text='<%#Eval("QianShouQingKuang") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            可用操作
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink1" runat="server"
                                NavigateUrl='<%#"ViewGongWen.aspx?lzid=" + Eval("LiuZhuanID") + "&gwid=" + Eval("GongWenID") + "&type=" + Eval("ShiFouQianShou") %>'
                                Text='<%#Convert.ToInt32(Eval("ShiFouQianShou"))==0?"签收文件":"查看详情" %>'></asp:HyperLink>
                            &nbsp;&nbsp
                            <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" OnClientClick="return confirm('撤销签阅你确定要撤销吗？');" CommandName="Delete" Text="撤销批阅" Visible='<%#Convert.ToInt32(Eval("ShiFouCheXiao"))==1?true:false%>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
            <table width="80%">
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
            </table>
        </div>
    </form>
</body>
</html>
