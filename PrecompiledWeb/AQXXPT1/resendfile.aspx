<%@ page language="C#" autoeventwireup="true" inherits="resendfile, App_Web_6tvq6jyo" %>

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
        BorderStyle="Solid" BorderWidth="2px" Caption="�ѷ�����Ϣ��" CaptionAlign="Right" CellPadding="4"
        ForeColor="#333333" Width="80%" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting"
        PageSize="15" OnPageIndexChanging="GridView1_PageIndexChanging" >
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerTemplate>
            <table width="100%">
                <tr>
                    <td align="left" width="30%">
                        ��<asp:Label ID="lblcurPage" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1      %>'></asp:Label>ҳ/��<asp:Label
                            ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>'></asp:Label>ҳ
                    </td>
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
                            Width="32px"></asp:TextBox><asp:Button ID="Button1" runat="server" OnClick="Button1_Click"
                                Text="ҳ" />
                    </td>
                </tr>
            </table>
        </PagerTemplate>
        <PagerStyle HorizontalAlign="Center" />
        <EmptyDataTemplate>
            ��������
        </EmptyDataTemplate>
        <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
        <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
        <EditRowStyle BackColor="Azure" />
        <Columns>
            <asp:BoundField DataField="sender" HeaderText="������" SortExpression="sender" />
            <asp:BoundField DataField="sendTime" HeaderText="����ʱ��" SortExpression="sendTime" />
            <asp:BoundField DataField="Title" HeaderText="��Ϣ����" SortExpression="Title" />
            <asp:BoundField DataField="SendCount" HeaderText="��������" SortExpression="SendCount" />
            <asp:BoundField DataField="readCount" HeaderText="��������" SortExpression="readCount" />

                    <asp:HyperLinkField DataNavigateUrlFields="XXID,SendCount" DataNavigateUrlFormatString="editfile_detail.aspx?xxid={0}&count={1}" Text="�鿴����"/>

        </Columns>
    </asp:GridView>
    </div>
    </form>
</body>
</html>
