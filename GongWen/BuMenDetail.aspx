<%@ Page Language="C#" AutoEventWireup="true" CodeFile="BuMenDetail.aspx.cs" Inherits="BuMenDetail" %>

<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <script language="javascript" type="text/javascript" src="js/WdatePicker.js" charset="gb2312"></script>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body style="background-color: #F7Feff" align="center">
    <form id="form1" runat="server" align="center">

        <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        <div align="center">
        <table width="70%"><tr><td align="center">

            <asp:Label ID="lblBuMen" runat="server" Text="Label" Font-Size="Medium"></asp:Label>

                   </td></tr></table>

         <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        
            <table width="70%" >
                
                <tr valign="middle" style="height: 200px">
                    <td align="right" width="25%">
                        <asp:Label ID="Label3" runat="server" Text="选择人员："></asp:Label>
                    </td>
                    <td>未选定：<br />
                        <asp:ListBox ID="lbDRn" runat="server" Rows="10" Width="180px" SelectionMode="Multiple"></asp:ListBox>
                    </td>
                    <td align="center">
                        <asp:Button ID="zu1AddButton" runat="server" Text="==>" OnClick="zu1AddButton_Click" /><br />
                        <asp:Button ID="zu1DelButton" runat="server" Text="<==" OnClick="zu1DelButton_Click" />
                    </td>
                    <td>已选定：<br />
                        <asp:ListBox ID="lbDR" runat="server" Rows="10" Width="180px" SelectionMode="Multiple"></asp:ListBox>
                    </td>
                </tr>
                <tr>
                    
                    <td colspan="4" style="height: 26px" align="center">
                        <asp:Button ID="btnAdd" runat="server" Text="确 定" OnClick="btnAdd_Click"  />
                    </td>
                    
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
