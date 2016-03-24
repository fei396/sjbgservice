<%@ Page Language="C#" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="BuGongWen.aspx.cs" Inherits="BuGongWen" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function qing(x) {
            var y = document.getElementById("txtQianShouNeiRong").value;
            //document.getElementById("txt_pishi").innerText=q+x;     
            if (y.charAt(y.length - 1) == "、") {
                document.getElementById("txtQianShouNeiRong").innerHTML = y.substring(y, y.length - 1) + x;
            }
            else {
                document.getElementById("txtQianShouNeiRong").innerHTML = y.substring(y, y.length) + x;
            }
        }


    </script>
    <style type="text/css">
        .auto-style1 {
            height: 28px;
        }
    </style>
</head>
<body style="background-color: #F7Feff" align="center">
    <form id="form1" runat="server" align="center">

        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        <div align="center">
            <table width="80%">

                <tr>

                    <td colspan="2" align="center">
                        <asp:Label ID="lblBiaoti" runat="server" Text="Label" Font-Bold="True" Font-Size="Medium"></asp:Label></td>
                </tr>
               
                                <tr>

                    <td colspan="2" align="center">
                        <asp:Label ID="lblLingDao" runat="server" Text="Label" Font-Bold="True" Font-Size="Medium"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2">

                        <table runat="server" id="tableQianShou" width="100%">
                            <tr>
                                <td>
                                    <br />
                                    <asp:Label ID="Label2" runat="server" Text="批示内容：" Font-Size="Medium" Font-Bold="True"></asp:Label>

                                    <asp:TextBox ID="txtQianShouNeiRong" runat="server" TextMode="MultiLine" ReadOnly="true" Height="100px" Width="100%" Font-Size="Medium"></asp:TextBox>

                                </td>
                            </tr>
                           
                           
                            <tr>
                                <td>

                                    <asp:GridView ID="gvListBuMen" runat="server" AutoGenerateColumns="False"
                                        BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4"
                                        ForeColor="#333333" Width="100%" OnRowDataBound="gvListBuMen_RowDataBound">

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
                                                    <asp:CheckBoxList ID="cbl" runat="server" OnSelectedIndexChanged="cbl_SelectedIndexChanged" RepeatColumns="5" RepeatDirection="Horizontal" DataTextField="XianShiMingCheng" DataValueField="GongHao" DataSource='<%# GetCKBLDataSource(Container.DataItemIndex)%>' AutoPostBack="True" CellPadding="5" CellSpacing="5">
                                                    </asp:CheckBoxList>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btnQianShou" runat="server" OnClick="btnQianShou_Click" Text=" 补  阅" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>
    </form>
</body>
</html>
