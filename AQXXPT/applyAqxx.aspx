<%@ Page Language="C#" AutoEventWireup="true" CodeFile="applyAqxx.aspx.cs" Inherits="applyAqxx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="js/WebCalendar.js"></script>
</head>
<body align="center">
    <form id="form1" runat="server">
    <div height="100">
        <br />
        <br />
        <br />
    </div>
    <div  align="center" valign="middle">
        <table width="700" border="1" cellspacing="0" rules="all">
                    <tr style="height: 50px">
                <td align="right" width="25%" style="height: 50px">
                    <asp:Label ID="Label4" runat="server" Text="����ˣ�"></asp:Label>
                </td>
                <td colspan="3" style="height: 50px" align="left">
                    <asp:DropDownList ID="ddlAuditor" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr style="height: 50px">
                <td align="right" width="25%" style="height: 50px">
                    <asp:Label ID="Label2" runat="server" Text="��Ϣ���⣺"></asp:Label>
                </td>
                <td colspan="3" style="height: 50px" align="left">
                    <asp:TextBox ID="txtTitle" runat="server" Width="300px"></asp:TextBox>
                </td>
            </tr>
                                    <tr height="300"style="height: 50px">
                <td align="right" width="25%" >
                    <asp:Label ID="Label5" runat="server" Text="����ʱ�䣺"></asp:Label>
                </td>
                <td colspan="3"  align="left">
                     <asp:TextBox ID="txtSetTime" runat="server" Width="300px"
                      onclick="SelectDate(this,'yyyy-MM-dd hh:mm')"></asp:TextBox>
                </td>
            </tr>
            <tr height="300">
                <td align="right" width="25%" >
                    <asp:Label ID="Label1" runat="server" Text="��Ϣ���ݣ�"></asp:Label>
                </td>
                <td colspan="3"  align="left">
                    <asp:TextBox ID="txtContent" runat="server" Height="200px" Width="80%" 
                        TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>

            <tr valign="middle" style="height: 200px">
                <td align="right" width="25%">
                    <asp:Label ID="Label3" runat="server" Text="ѡ��Ҫ���͵Ĳ��ţ�"></asp:Label>
                </td>
                <td>
                    δѡ����<br />
                    <asp:ListBox ID="lbDRn" runat="server" Rows="10" Width="180px" SelectionMode="Multiple">
                    </asp:ListBox>
                </td>
                <td align="center">
                    <asp:Button ID="zu1AddButton" runat="server" Text="==>" OnClick="zu1AddButton_Click" /><br />
                    <asp:Button ID="zu1DelButton" runat="server" Text="<==" OnClick="zu1DelButton_Click" />
                </td>
                <td>
                    ��ѡ����<br />
                    <asp:ListBox ID="lbDR" runat="server" Rows="10" Width="180px" SelectionMode="Multiple">
                    </asp:ListBox>
                </td>
            </tr>
            <tr>
                <td style="height: 26px">
                   ���Ͷ��쵼��
                </td>
                <td colspan="4"  id="tdLeader" runat="server" style="height: 26px" 
                    align="justify">
                    <asp:CheckBox ID="cbLeaderAll" runat="server" Text="ȫѡ" AutoPostBack="True" 
                        oncheckedchanged="cbLeaderAll_CheckedChanged" />&nbsp;&nbsp;&nbsp; <asp:CheckBox ID="cbLeader0001" runat="server" Text="��˼��" />&nbsp;&nbsp;&nbsp; <asp:CheckBox
                        ID="cbLeader0002" Text="��ΰ��" runat="server" />&nbsp;&nbsp;&nbsp; <asp:CheckBox ID="cbLeader0008" Text="������" runat="server" />&nbsp;&nbsp;&nbsp; <asp:CheckBox
                            ID="cbLeader0007" Text="��ϲ��" runat="server" />&nbsp;&nbsp;&nbsp; <asp:CheckBox
                            ID="cbLeader7034" Text="����" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="height: 26px">
                    &nbsp
                </td>
                <td colspan="2" style="height: 26px">
                    <asp:Button ID="AddButton" runat="server" Text="ȷ ��" OnClick="AddButton_Click" />
                </td>
                <td style="height: 26px">
                    <asp:Button ID="CancelButton" runat="server" Text="ȡ ��" OnClick="CancelButton_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
