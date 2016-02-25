<%@ Page Language="C#" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="ViewGongWen.aspx.cs" Inherits="ViewGongWen" %>

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
                        <asp:Label ID="lblHongTou" runat="server" Text="Label" Width="100%" Font-Bold="True" Font-Size="Large" ForeColor="Red"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Label ID="lblWenHao" runat="server" Text="Label" Font-Size="Small"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:Label ID="lblBiaoti" runat="server" Text="Label" Font-Bold="True" Font-Size="Medium"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:TextBox valign="middle" ID="txtZhengWen" runat="server" Height="200px" ReadOnly="True" TextMode="MultiLine" Width="100%" Font-Size="Medium"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="lblLeiXing" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>
                    <td align="left">
                        <asp:Label ID="lblYiJian" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="lblFaWenDanWei" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>
                    <td align="lfet">
                        <asp:Label ID="lblFaWenShiJian" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%">
                            <tr>
                                <td aligh="left" class="auto-style1">
                                    <asp:Label ID="Label1" runat="server" Text="附件：" Font-Size="Medium" Font-Bold="True"></asp:Label></td>
                            </tr>
                            <tr>
                                <td runat="server" id="tdFuJian" align="center">
                                    <a style="font-size: large"></a></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />

                        <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False"
                            BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4" DataKeyNames="LiuZhuanID"
                            ForeColor="#333333" Width="100%" Caption="文件签收情况" Font-Size="Medium" OnSelectedIndexChanged="gvList_SelectedIndexChanged">
                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />

                            <PagerStyle HorizontalAlign="Center" />
                            <EmptyDataTemplate>
                                尚无人签收
                            </EmptyDataTemplate>

                            <Columns>
                                <asp:BoundField DataField="FaSongRenXM" HeaderText="发送人">
                                    <HeaderStyle Width="50px" />
                                    <ItemStyle Width="50px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="FaSongShiJian" HeaderText="发送时间">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle Width="120px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="JieShouRenXM" HeaderText="接收人">
                                    <HeaderStyle Width="50px" />
                                    <ItemStyle Width="50px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="JieShouRenBM" HeaderText="接收人部门">
                                    <HeaderStyle Width="70px" />
                                    <ItemStyle Width="70px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="QianShouShiJian" HeaderText="签收时间">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle Width="120px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="QianShouNeiRong" HeaderText="签收内容" />
                                <asp:TemplateField HeaderText="转发人数">

                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%#  Convert.ToInt32( Eval("LiuZhuanShu")) >0?Eval("LiuZhuanShu"):"N/A" %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" />
                                    <ItemStyle Width="50px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="签收人数">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server"  Text='<%#  Convert.ToInt32( Eval("WanChengShu")) >0 ?Eval("WanChengShu"): Convert.ToInt32( Eval("LiuZhuanShu")) >0?"0":  "N/A" %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="50px" />
                                    <ItemStyle Width="50px" />
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="查看详情" Visible='<%#Convert.ToInt32( Eval("LiuZhuanShu")) >0?true:false %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle Width="60px" />
                                    <ItemStyle Width="60px" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>

                <tr>
                    <td colspan="2">
                        <table runat="server" id="tableGuiDang" width="100%">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btnGuiDang" runat="server" Text="归  档" OnClick="btnGuiDang_Click" />
                                    <asp:Button ID="btnCuiBan" runat="server" Text="催  办" OnClick="btnCuiBan_Click" />
                                </td>
                            </tr>
                        </table>
                        <table runat="server" id="tableQianShou" width="100%">
                            <tr>
                                <td>
                                    <br />
                                    <asp:Label ID="Label2" runat="server" Text="签阅内容：" Font-Size="Medium" Font-Bold="True"></asp:Label>

                                    <asp:TextBox ID="txtQianShouNeiRong" runat="server" TextMode="MultiLine" Height="100px" Width="100%" Font-Size="Medium"></asp:TextBox>

                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <%=duanyu()%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <table runat="server" id="tableZdybm" width="100%">
                                        <tr>
                                            <td width="20%">
                                                <asp:Label ID="Label3" runat="server" Text="自定义部门：" Font-Size="Medium"></asp:Label></td>
                                            <td width="20%">
                                                <asp:CheckBox ID="cbAll" runat="server" AutoPostBack="True" OnCheckedChanged="cbAll_CheckedChanged" Text="各科室、各车间" />
                                            </td>
                                            <td width="80%" align="left">

                                                <asp:CheckBoxList Font-Size="Medium" ID="cblZdybm" runat="server" AutoPostBack="True" OnSelectedIndexChanged="cblZdybm_SelectedIndexChanged" RepeatDirection="Horizontal">
                                                </asp:CheckBoxList>

                                            </td>
                                        </tr>

                                    </table>
                                </td>
                            </tr>
                            <%--                <tr>
                    <td colspan="2">
                        <asp:Table ID="tableBm" runat="server" Width="100%">
                        </asp:Table>
                    </td>
                </tr>--%>
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
                                                    <asp:CheckBox ID="chb" AutoPostBack="true" OnCheckedChanged="chb_CheckedChanged" runat="server" Text='<%# Eval("FenLeiZongCheng") %>' />
                                                </ItemTemplate>
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle Width="10%" />
                                            </asp:TemplateField>
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
                                    <asp:Button ID="btnQianShou" runat="server" OnClick="btnQianShou_Click" Text=" 签  阅" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>
    </form>
</body>
</html>
