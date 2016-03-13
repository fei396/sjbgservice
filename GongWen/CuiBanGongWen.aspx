<%@ Page Language="C#" MaintainScrollPositionOnPostback="true" AutoEventWireup="true" CodeFile="CuiBanGongWen.aspx.cs" Inherits="CuiBanGongWen" %>

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
            if (y.charAt(y.length - 1) == "��") {
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
                    <%--<td align="left" class="auto-style1">
                        <asp:Label ID="lblLeiXing" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>--%>
                    <td align="left" class="auto-style1">
                        <asp:Label ID="lblYiJian" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="lblFaWenDanWeiHeShiJian" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>
                    <%--<td align="lfet">
                        <asp:Label ID="lblFaWenShiJian" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>--%>
                </tr>
                <tr>
                    <td colspan="2">
                        <table width="100%">
                            <tr>
                                <td aligh="left" class="auto-style1">
                                    <asp:Label ID="Label1" runat="server" Text="������" Font-Size="Medium" Font-Bold="True"></asp:Label></td>
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
                            ForeColor="#333333" Width="100%" Caption="�ļ�ǩ�����" Font-Size="Medium" OnSelectedIndexChanged="gvList_SelectedIndexChanged">
                         
                            <EmptyDataTemplate>
                                ������ǩ��
                            </EmptyDataTemplate>

                            <Columns>
                                <asp:BoundField DataField="FaSongRenXM" HeaderText="������">
                                    <HeaderStyle Width="8%" />
                                    <ItemStyle Width="8%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="FaSongShiJian" HeaderText="����ʱ��">
                                    <HeaderStyle Width="9%" />
                                    <ItemStyle Width="9%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="JieShouRenXM" HeaderText="������">
                                    <HeaderStyle Width="8%" />
                                    <ItemStyle Width="8%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="JieShouRenBM" HeaderText="�����˲���">
                                    <HeaderStyle Width="9%" />
                                    <ItemStyle Width="9%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="QianShouShiJian" HeaderText="ǩ��ʱ��">
                                    <HeaderStyle Width="9%" />
                                    <ItemStyle Width="9%" />
                                </asp:BoundField>
                                <asp:BoundField DataField="QianShouNeiRong" HeaderText="ǩ������" >
                                    
                                     <HeaderStyle Width="20%" />
                                    <ItemStyle Width="20%" />
                                  </asp:BoundField>
                                <asp:TemplateField HeaderText="ת������">

                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%#  Convert.ToInt32( Eval("LiuZhuanShu")) >0?Eval("LiuZhuanShu"):"��" %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle Width="7%" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="ǩ������">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server"  Text='<%#  Convert.ToInt32( Eval("WanChengShu")) >0 ?Eval("WanChengShu"): Convert.ToInt32( Eval("LiuZhuanShu")) >0?"0":  "��" %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle Width="7%" />
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False">
                                    <HeaderTemplate>
                                        <asp:LinkButton ID="hylSuoYouWeiQian" OnClick="hylSuoYouWeiQian_Click"  runat="server">����δǩ��</asp:LinkButton>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="�鿴����" Visible='<%#Convert.ToInt32( Eval("LiuZhuanShu")) >0?true:false %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle Width="7%" />
                                </asp:TemplateField>
                                <asp:TemplateField ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="HyperLink1" runat="server" CausesValidation="False" NavigateUrl='<%#"BuGongWen.aspx?lzid=" + Eval("LiuZhuanID") + "&gwid=" + Eval("GongWenID") + "&uid=" + Eval("JieShouRen")%>' Text="�ļ�����" ></asp:HyperLink>
                                    </ItemTemplate>
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle Width="7%" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>
                                <tr>
                    <td colspan="2" align="center">
                        <asp:Label ID="lblCuiBanDuiXiang" runat="server" Text="�߰����" Font-Size="Medium"></asp:Label>
                        <asp:DropDownList ID="ddlCuiBanDuiXiang" runat="server" Font-Size="Medium">
                            <asp:ListItem Value="21">�γ������</asp:ListItem>
                            <asp:ListItem Value="22">���ӳ�Ա</asp:ListItem>
                            <asp:ListItem Value="23">�����в�</asp:ListItem>
                            <asp:ListItem Value="24">�����в�</asp:ListItem>
                            <asp:ListItem Value="25">������Ա</asp:ListItem>
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <table runat="server" id="tableGuiDang" width="100%">
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btnGuiDang" runat="server" Text="��  ��" OnClick="btnGuiDang_Click" />
                                    <asp:Button ID="btnCuiBan" runat="server" Text="��  ��" OnClick="btnCuiBan_Click" />
                                </td>
                            </tr>
                        </table>
                
                    </td>
                </tr>

            </table>
    </form>
</body>
</html>
