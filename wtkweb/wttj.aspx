<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wttj.aspx.cs" Inherits="sadm_blzbgl_add" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">

        <div >
            <table width="700" BORDER=1 cellspacing="0"  rules="all">
                <tr height="50px">
                    <td align="right" width="15%" style="height: 50px">
                        ��ˣ�</td>
                    <td colspan="3" style="height: 50px">
                        <asp:TextBox ID="txtTbr" runat="server" ReadOnly="true" width="300px" ></asp:TextBox></td>

                </tr>
                <tr style="height: 50px">
                    <td align="right" width="15%" style="height: 50px">
                        ������⣺</td>
                    <td colspan="3" style="height: 50px">
                        <asp:TextBox ID="txtBt" runat="server" width="300px"></asp:TextBox></td>

                </tr>
                                <tr style="height: 200px">
                    <td align="right" width="15%" >
                        �������ݣ�</td>
                    <td colspan="3" style="height: 50px">
                        <asp:TextBox Height="100%" ID="txtNr" runat="server" width="300px" TextMode="MultiLine"></asp:TextBox></td>

                </tr>
                         <tr valign="middle" style="height: 200px">
                    <td align="right" width="15%">
                        ���β��ţ�</td>
                    <td >
                        δѡ����<br />
                        <asp:ListBox ID="lbZrwx" runat="server" Rows="10" width="180px" SelectionMode="Multiple">
                        </asp:ListBox>
                        </td>
                    <td align="center"><asp:Button ID="btnZrAdd" runat="server" Text="==>" OnClick="zu1AddButton_Click"  /><br />
                        <asp:Button ID="btnZrDel" runat="server" Text="<==" OnClick="zu1DelButton_Click"   /></td>
                    <td>
                        ��ѡ����<br />
                        <asp:ListBox ID="lbZryx" runat="server" Rows="10" width="180px" SelectionMode="Multiple"></asp:ListBox></td>
                </tr>

                 <tr style="height: 50px">
                <td align="right" width="15%">���ʱ�ޣ�</td>
                    <td colspan="3">
                        <asp:TextBox ID="txtWcsj" runat="server" width="300px"></asp:TextBox>
                    </td>
                    
                </tr>

                <tr style="height: 50px">
                <td align="right" width="15%">�������ţ�</td>
                    <td colspan="3">
                        <asp:DropDownList ID="ddlSp" runat="server" width="200px">
                            <asp:ListItem Value="1">��ȫ��</asp:ListItem>
                            <asp:ListItem Value="2">���ÿ�</asp:ListItem>
                            <asp:ListItem Value="3">������</asp:ListItem>
                            <asp:ListItem Value="4">������</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    
                </tr>
                 <tr>
                    <td style="height: 26px" >&nbsp
                    </td>
                    <td colspan="2" style="height: 26px">
                    <asp:Button ID="AddButton" runat="server" Text="ȷ ��" OnClick="AddButton_Click" />
                    </td>
                    <td style="height: 26px">
                        
                        <asp:Button ID="CancelButton" runat="server" Text="ȡ ��" OnClick="CancelButton_Click" /></td>

                </tr>
            </table>
</div>
    </form>
</body>
</html>
