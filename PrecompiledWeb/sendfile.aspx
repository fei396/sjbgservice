<%@ page language="C#" autoeventwireup="true" inherits="sendfile, App_Web_10opxpjw" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body align="center">
    <form id="form1" runat="server">
        <div height="100">
         <br />
            <br />
            <br />
        </div>
        <div align="center" valign="middle" >
            <table width="700" BORDER=1 cellspacing="0"  rules="all">
                <tr height="50px">
                    <td align="right" width="25%" style="height: 50px">
                        <asp:Label ID="Label1" runat="server" Text="选择要发送的文件："></asp:Label></td>
                    <td colspan="3" style="height: 50px" align="left">
                        <asp:FileUpload ID="FileUpload1" runat="server" Width="300px" />
                        <asp:TextBox ID="TextBox1" runat="server" ReadOnly="True" Visible="False" 
                            Width="300px"></asp:TextBox>
                        <asp:Button ID="btnUpload" runat="server" onclick="btnUpload_Click" Text="上传" />
                    </td>

                </tr>
                <tr style="height: 50px">
                    <td align="right" width="25%" style="height: 50px">
                        <asp:Label ID="Label2" runat="server" Text="文件说明："></asp:Label></td>
                    <td colspan="3" style="height: 50px" align="left">
                        <asp:TextBox ID="wjsmTextBox" runat="server" width="300px"></asp:TextBox></td>

                </tr>

                 <tr valign="middle" style="height: 200px">
                    <td align="right" width="25%">
                        <asp:Label ID="Label3" runat="server" Text="选择要发送的指导室："></asp:Label></td>
                    <td >
                        未选定：<br />
                        <asp:ListBox ID="lbDRn" runat="server" Rows="10" width="180px" SelectionMode="Multiple">
                        </asp:ListBox>
                        </td>
                    <td align="center"><asp:Button ID="zu1AddButton" runat="server" Text="==>" OnClick="zu1AddButton_Click"  /><br />
                        <asp:Button ID="zu1DelButton" runat="server" Text="<==" OnClick="zu1DelButton_Click"   /></td>
                    <td>
                        已选定：<br />
                        <asp:ListBox ID="lbDR" runat="server" Rows="10" width="180px" SelectionMode="Multiple"></asp:ListBox></td>
                </tr>
        
            
                 <tr>
                    <td style="height: 26px" >&nbsp
                    </td>
                    <td colspan="2" style="height: 26px">
                    <asp:Button ID="AddButton" runat="server" Text="确 定" OnClick="AddButton_Click" />
                    </td>
                    <td style="height: 26px">
                        
                        <asp:Button ID="CancelButton" runat="server" Text="取 消" OnClick="CancelButton_Click" /></td>

                </tr>
            </table>
</div>
    </form>
</body>
</html>
