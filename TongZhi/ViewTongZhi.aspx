<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ViewTongZhi.aspx.cs" Inherits="ViewTongZhi" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
   <%-- <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />--%>
    <link href="css/test.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function qing(x) {
            var y = document.getElementById("txtQianShouNeiRong").value;
            //document.getElementById("txt_pishi").innerText=q+x;     
            if ("��" === y.charAt(y.length - 1)) {
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

        .auto-style2 {
            height: 30px;
        }

    </style>
</head>
<body style="background-color: #F7Feff" align="center">
    <form id="form1" runat="server" align="center">

        <hr align="center" color="#00cc00" noshade="noshade" width="96%"></hr>
        <div align="center">
            <table width="80%">
                <tr>
                    <td align="center">
                        <asp:Label ID="lblBiaoti" runat="server" Text="Label" Font-Bold="True" Font-Size="Medium"></asp:Label></td>
                </tr>
                <tr>
                    <td align="center">
                        <asp:TextBox CssClass="Text_Multi" valign="middle" ID="txtZhengWen" runat="server"   ReadOnly="True" TextMode="MultiLine" Height="261px" Width="1252px"  ></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:Label ID="lblFaWenDanWeiHeShiJian" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>
                    <%--<td align="lfet">
                        <asp:Label ID="lblFaWenShiJian" runat="server" Text="Label" Font-Size="Medium"></asp:Label></td>--%>
                </tr>
                <tr>
                    <td>
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
                    <td>
                        <br />

                        <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False"
                            BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4" DataKeyNames="LiuZhuanID" 
                            ForeColor="#333333" Width="100%" Caption="�ļ�ǩ�����" Font-Size="Medium">
                         
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
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Select" Text="�鿴����" Visible='<%#Convert.ToInt32( Eval("LiuZhuanShu")) >0?true:false %>'></asp:LinkButton>
                                    </ItemTemplate>
                                    <HeaderStyle Width="7%" />
                                    <ItemStyle Width="7%" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </td>
                </tr>

                <tr>
                    <td>

                        <table runat="server" id="tableQianShou" width="100%">
                            <tr>
                                <td>
                                    <br />
                                    <asp:Label ID="Label2" runat="server" Text="ǩ�����ݣ�" Font-Size="Medium" Font-Bold="True"></asp:Label>

                                    <br />

                                    <asp:TextBox ID="txtQianShouNeiRong" runat="server" TextMode="MultiLine"  CssClass="Text_Multi" Height="189px" Width="1248px" ></asp:TextBox>

                                </td>
                            </tr>


                            <tr>
                                <td class="auto-style2">

                                    <asp:CheckBoxList ID="CheckBoxList1" runat="server">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td align="center">
                                    <asp:Button ID="btnQianShou" runat="server" OnClick="btnQianShou_Click" Text="ǩ  ��" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>
    </form>
</body>
</html>
