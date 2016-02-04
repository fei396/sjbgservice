<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ListGongWenGuiDang.aspx.cs" Inherits="ListGongWenGuiDang" %>

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

                <td align="left" valign="middle" width="10%">�ؼ��֣�</td><td width="20%"><asp:TextBox ID="txtBiaoTi" runat="server" width="90%"></asp:TextBox>
                </td>
                <td align="left" valign="middle" width="10%">��ʼʱ�䣺</td><td width="20%"><asp:TextBox ID="txtStart" runat="server" width="90%" onClick="WdatePicker()"></asp:TextBox>
                </td>
                <td align="left" valign="middle" width="10%">����ʱ�䣺</td><td width="20%"><asp:TextBox ID="txtEnd" runat="server" width="90%" onClick="WdatePicker()"></asp:TextBox>
                </td>
                <td align="center" width="20%">
                    <asp:Button ID="btnChaXun" runat="server" Text="��  ѯ" OnClick="btnChaXun_Click" /></td>
            </tr>
  
        </table>
        
        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        
        <div align="center">
            <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False"
                BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4"
                ForeColor="#333333" Width="80%" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting"
                PageSize="15" OnPageIndexChanging="GridView1_PageIndexChanging">
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerTemplate>
                    <%--  <table width="100%">
                <tr>
                    <td align="left" width="30%">
                        ��<asp:Label ID="lblcurPage" runat="server"></asp:Label>ҳ/��<asp:Label
                            ID="lblPageCount" runat="server" ></asp:Label>ҳ
                    </td>
                    <td align="center" width="40%">
                        <asp:LinkButton ID="cmdFirstPage" runat="server" OnClick="First_Click"
                            >��ҳ</asp:LinkButton>
                        <asp:LinkButton ID="cmdPreview" runat="server" OnClick="Pre_Click"
                            >ǰҳ</asp:LinkButton>
                        <asp:LinkButton ID="cmdNext" runat="server" OnClick="Next_Click"
                            >��ҳ</asp:LinkButton>
                        <asp:LinkButton ID="cmdLastPage" runat="server" OnClick="Last_Click"
                            >βҳ</asp:LinkButton>
                        ת��<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>'
                            Width="32px"></asp:TextBox><asp:Button ID="Button1" runat="server" OnClick="Custom_Click"
                                Text="ҳ" />
                    </td>
                </tr>
            </table>--%>
                </PagerTemplate>
                <PagerStyle HorizontalAlign="Center" />
                <EmptyDataTemplate>
                    ��ǰû��δ��ɹ��ģ�������<a href="ListGongWenGuiDang.aspx?type=1">�鿴ȫ������</a>
                </EmptyDataTemplate>
                <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="Azure" />
                <Columns>
                    <asp:BoundField DataField="XuHao" HeaderText="���" />
                    <asp:BoundField DataField="JieShouRenXM" HeaderText="������" />
                    <asp:BoundField DataField="WenHao" HeaderText="�ĺ�" />
                    <asp:BoundField DataField="BiaoTi" HeaderText="����" />
                    <asp:BoundField DataField="FaBuShiJian" HeaderText="����ʱ��" />

                    <asp:TemplateField>
                        <HeaderTemplate>
                            ������
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:Label ID="label" runat="server" ForeColor='<%#Eval("ShiFouLiuZhuanWanCheng").Equals(true)?System.Drawing.Color.Green:System.Drawing.Color.Red %>'
                                Text='<%#Eval("ShiFouLiuZhuanWanCheng").Equals(true)?"�����":"δ���" %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            ���ò���
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink1" runat="server"
                                NavigateUrl='<%#"ViewGongWen.aspx?lzid=" + Eval("LiuZhuanID") + "&gwid=" + Eval("GongWenID")+ "&type=" + (Eval("ShiFouLiuZhuanWanCheng").Equals(true)?"1":"0") %>'
                                Text="�鿴����"></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
            <table width="80%">
                <tr>
                    <td align="right" width="30%">��<asp:Label ID="lblcurPage" runat="server"></asp:Label>ҳ/��<asp:Label
                        ID="lblPageCount" runat="server"></asp:Label>ҳ
                    </td>
                    <td align="center" width="40%">
                        <asp:LinkButton ID="cmdFirstPage" runat="server" OnClick="First_Click">��ҳ</asp:LinkButton>
                        <asp:LinkButton ID="cmdPreview" runat="server" OnClick="Pre_Click">ǰҳ</asp:LinkButton>
                        <asp:LinkButton ID="cmdNext" runat="server" OnClick="Next_Click">��ҳ</asp:LinkButton>
                        <asp:LinkButton ID="cmdLastPage" runat="server" OnClick="Last_Click">βҳ</asp:LinkButton>
                        ת��<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>'
                            Width="32px"></asp:TextBox><asp:Button ID="Button1" runat="server" OnClick="Custom_Click"
                                Text="ҳ" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
