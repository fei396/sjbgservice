<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SuoYouWeiQian.aspx.cs" Inherits="SuoYouWeiQian" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script language="javascript" type="text/javascript">
        window.onload = function () {

            var iWidth = 600; //模态窗口宽度
            var iHeight = 400;//模态窗口高度
            var iTop = (window.screen.height - iHeight) / 2;
            var iLeft = (window.screen.width - iWidth) / 2;
            window.resizeTo(iWidth, iHeight);//调整大小
            window.moveTo(iLeft, iTop);//移动位置
        }
    </script>
</head>
<body style="background-color: #F7Feff">
    <form id="form1" runat="server">
        <div align="center">

            <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False"
                BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4"
                ForeColor="#333333" Width="100%" Caption="所有未签收人员" Font-Size="Medium">


                <Columns>

                    <asp:BoundField DataField="JieShouRen" HeaderText="接收人工号">
                        <HeaderStyle Width="100px" />
                        <ItemStyle Width="100px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="JieShouRenXM" HeaderText="接收人姓名">
                        <HeaderStyle Width="100px" />
                        <ItemStyle Width="100px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="JieShouRenBM" HeaderText="部门">
                        <HeaderStyle Width="100px" />
                        <ItemStyle Width="100px" />
                    </asp:BoundField>
                    <asp:BoundField DataField="FaSongShiJian" HeaderText="接收时间">
                        <HeaderStyle Width="200px" />
                        <ItemStyle Width="200px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="选择">
                        <HeaderStyle Width="100px" />
                        <ItemStyle Width="100px" />
                        <HeaderTemplate>
                            <asp:CheckBox ID="cbSelectAll" Text="全选" AutoPostBack="true" OnCheckedChanged="cbSelectAll_CheckedChanged" runat="server" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="cbSelect" runat="server" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <asp:Button ID="btnCuiBan" runat="server" Text="催  办" OnClick="btnCuiBan_Click" />

        </div>
    </form>
</body>
</html>
