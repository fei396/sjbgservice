<%@ page language="C#" autoeventwireup="true" inherits="editfile_detail, App_Web_gjajr-s1" %>

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
        BorderStyle="Solid" BorderWidth="2px" Caption="�ļ��������飺" CaptionAlign="Right" CellPadding="4"
        ForeColor="#333333" Width="80%" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting"
        PageSize="15" OnPageIndexChanging="GridView1_PageIndexChanging" >
        <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerTemplate>
            <table width="100%">
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
            
            
            <asp:BoundField DataField="ReceiverDept" HeaderText="�����˲���" SortExpression="ReceiverDept" />
            <asp:BoundField DataField="Receiver" HeaderText="������" SortExpression="Receiver" />
            <asp:BoundField DataField="ReceiveTime" HeaderText="����ʱ��" SortExpression="ReceiveTime" />
        </Columns>
    </asp:GridView>
    </div>
    </form>
</body>
</html>
