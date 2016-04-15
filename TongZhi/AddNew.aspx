<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddNew.aspx.cs" Inherits="AddNew" %>

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
p { margin: 0; padding: 0; }
input { font:12px  Tahoma, Arial, Helvetica, snas-serif; }
        .auto-style2 {
            width: 10%;
        }
        .auto-style3 {
            height: 31px;
        }
        .auto-style4 {
            height: 40px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        var i = 2;
        function addFile() {

            if (i <= 6) {

                var html = '<span style="font-size: larger">附件' + i + ':</span><br/><input accept=".doc,.docx,.pdf" type="file" runat="server" style="font-size: larger; color: #FF0000;" /><br />';

                var o = document.getElementById('MyFile');
                o.insertAdjacentHTML("beforeEnd", html);

                i++;
            }
            else {
                alert("最多能上传6个文件!");
            }
        };

        function fileError(error) {//上传文件出错  
            alert(error);
        }
    </script>
</head>
<body>
    <form id="form2" runat="server">
        <div class="auto-style3">
            <br />
            <br />
            <br />
        </div>
        <div align="center" valign="middle">
            <table border="1" cellspacing="0" rules="all" width="700">
                <tr height="300" style="height: 80px">
                    <td align="right" width="10%">标 题： </td>
                    <td align="center" colspan="3" valign="middle">
                        <asp:TextBox ID="txtBt" runat="server" Height="50" TextMode="MultiLine" Width="90%"></asp:TextBox>
                    </td>
                </tr>
                <tr height="300">
                    <td align="right" width="10%">正 文： </td>
                    <td align="center" colspan="3" valign="middle">
                        <asp:TextBox ID="txtZw" runat="server" Height="300" TextMode="MultiLine" Width="90%"></asp:TextBox>
                    </td>
                </tr>
                <tr style="height: 100px" valign="middle">
                    <td align="right" width="10%">附件： </td>
                    <td align="center" colspan="3" valign="middle">
                        <p id="MyFile" width="80%">
                            <span style="font-size: larger">附件1:</span><br />
                            <input runat="server" accept=".doc,.docx,.pdf" style="font-size: larger; color: #FF0000; width=100%;" type="file" /><br />
                        </p>
                        <br />
                        <input onclick="addFile()" type="button" value="继续添加" /> </td>
                </tr>
                <tr style="height: 50px">
                    <td align="right" width="10%">文件类型： </td>
                    <td align="center" valign="middle" width="20%">
                        <asp:DropDownList ID="ddlLeiXing" runat="server" AutoPostBack="True" >
                        </asp:DropDownList>
                    </td>
                    <td align="right" class="auto-style2">是否公开： </td>
                    <td align="center" valign="middle" width="20%">
                        <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                            <asp:ListItem>是</asp:ListItem>
                            <asp:ListItem>否</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td align="center" class="auto-style4" colspan="4">
                        <asp:Label ID="lbBM" runat="server" Text="Label"></asp:Label>
                    </td>
                </tr>
                <tr style="height: 150px">
                    <td align="right" colspan="4">
                        <asp:GridView ID="gvListBuMen" runat="server" AutoGenerateColumns="False" BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4" ForeColor="#333333" Height="261px" OnRowDataBound="gvListBuMen_RowDataBound" Width="100%">
                            <HeaderStyle Height="0px" />
                            <PagerStyle HorizontalAlign="Center" />
                            <Columns>
                                <asp:BoundField DataField="FenLeiMingCheng">
                                <ControlStyle Font-Size="Medium" />
                                <HeaderStyle Width="10%" />
                                <ItemStyle Width="10%" />
                                </asp:BoundField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chb" runat="server" AutoPostBack="true" OnCheckedChanged="chb_CheckedChanged" Text='<%# Eval("FenLeiZongCheng") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle Width="13%" />
                                    <ItemStyle Width="13%" />
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBoxList ID="cbl" runat="server" AutoPostBack="True" CellPadding="5" CellSpacing="5" DataSource="<%# GetCKBLDataSource(Container.DataItemIndex)%>" DataTextField="XianShiMingCheng" DataValueField="Uid" OnSelectedIndexChanged="cbl_SelectedIndexChanged" RepeatColumns="5" RepeatDirection="Horizontal">
                                        </asp:CheckBoxList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                <%--  --%>
                <tr>
                    <td align="center" colspan="4" style="height: 50px" width="100%">
                        <asp:Button ID="btnAdd" runat="server" OnClick="AddButton_Click" OnClientClick="return confirm('你确定发布该公文吗？');" Text="确  定" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnReset" runat="server" OnClick="btnReset_Click" Text="取  消" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
 

</body>
</html>
