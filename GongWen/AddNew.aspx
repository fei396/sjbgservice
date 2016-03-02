<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AddNew.aspx.cs" Inherits="AddNew" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
    
    <style type="text/css">
        .auto-style1 {
            height: 50px;
        }
    </style>
    <script language="javascript" type="text/javascript">
        var i = 2;
        function addFile() {

            if (i <= 6) {

                var html = '<span style="font-size: larger">����' + i + ':</span><br/><input accept=".doc,.docx,.pdf" type="file" runat="server" style="font-size: larger; color: #FF0000;" /><br />';

                var o = document.getElementById('MyFile');
                o.insertAdjacentHTML("beforeEnd", html)

                i++;
            }
            else {
                alert("������ϴ�6���ļ�!");
            }
        };

        function fileError(error) {//�ϴ��ļ�����  
            alert(error);
        }
    </script>
</head>
<body align="center">
    <form id="form1" runat="server">
        <div height="100">
            <br />
            <br />
            <br />
        </div>
        <div align="center" valign="middle">
            <table width="700" border="1" cellspacing="0" rules="all">
                <tr style="height: 80px">
                    <td align="right" width="10%" >�ļ���ͷ��</td>
                    <td colspan="5" align="center" valign="middle">
                        <asp:TextBox ID="txtHt" runat="server" Width="90%" TextMode="MultiLine" Height="50"></asp:TextBox>
                    </td>
                </tr>
                <tr style="height: 50px">
                    <td align="right" width="10%" style="height: 50px">���ĵ�λ��
                    </td>
                    <td colspan="5" style="height: 50px" align="center" valign="middle">
                        <asp:TextBox ID="txtFwdw" runat="server" Width="90%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right" width="10%" class="auto-style1">��  �ţ�
                    </td>
                    <td colspan="5" align="center" valign="middle" class="auto-style1">
                        <asp:TextBox ID="txtWh" runat="server" Width="90%"></asp:TextBox>
                    </td>
                </tr>
                <tr height="300" style="height: 80px">
                    <td align="right" width="10%" >��  �⣺
                    </td>
                    <td colspan="5" align="center" valign="middle">
                        <asp:TextBox ID="txtBt" runat="server" Width="90%" TextMode="MultiLine" Height="50"
                            ></asp:TextBox>
                    </td>
                </tr>
                <tr height="300">
                    <td align="right" width="10%">��  �ģ�
                    </td>
                    <td colspan="5" align="center" valign="middle">
                        <asp:TextBox ID="txtZw" runat="server" Height="300" Width="90%"
                            TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>



                <tr valign="middle" style="height: 100px">
                    <td align="right" width="10%">������ 
                    </td>
                    <td colspan="5" align="center" valign="middle">
                        <p id="MyFile" width="80%">
                            <span style="font-size: larger">����1:</span><br />
                            <input accept=".doc,.docx,.pdf" type="file" runat="server" style="font-size: larger; color: #FF0000; width=100%;" /><br />
                        </p>

                        <br />
                        <input type="button" value="�������" onclick="addFile()">
                    </td>

                </tr>
                <tr style="height: 50px">
                    <td align="right" width="10%">���������
                    </td>
                    <td colspan="5" align="center" valign="middle">
                        <asp:TextBox ID="txtYj" runat="server" Width="90%"
                            ></asp:TextBox>
                    </td>
                    </tr>
               <tr style="height: 50px">
                    <td align="right" width="10%">�ļ����ͣ�
                    </td>
                    <td align="center" valign="middle" width="20%">
                        <asp:DropDownList ID="ddlLeiXing" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td align="right" width="10%">�ļ����ʣ�
                    </td>
                    <td align="center" valign="middle" width="20%">
                        <asp:DropDownList ID="ddlXingZhi" runat="server">
                        </asp:DropDownList>
                    </td>
                    
                </tr>
                <tr style="height: 50px">
                    <td align="right" width="10%">�����̶ȣ�
                    </td>
                    <td align="center" valign="middle">
                        <asp:DropDownList ID="ddlJinJi" runat="server">
                            <asp:ListItem>һ��</asp:ListItem>
                            <asp:ListItem>�ؼ�</asp:ListItem>
                            <asp:ListItem>�ؼ�</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="right" width="10%">�����쵼��
                    </td>
                    <td align="center" valign="middle">
                        <asp:DropDownList ID="ddlLingDao" runat="server">
                        </asp:DropDownList>
                    </td>
                    </tr>
                <tr>

                    <td style="height: 50" colspan="6" width="100%" align="center">
                        <asp:Button ID="btnAdd" runat="server" Text="ȷ  ��" OnClick="AddButton_Click" />
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnReset" runat="server" Text="ȡ  ��" OnClick="btnReset_Click" />
                    </td>

                </tr>
            </table>
        </div>
    </form>
</body>
</html>
