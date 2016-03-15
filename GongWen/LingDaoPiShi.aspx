<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LingDaoPiShi.aspx.cs" Inherits="LingDaoPiShi" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>段领导批示</title>
<script language="javascript" type="text/javascript">
    window.onload = function () {

        var iWidth = 500; //模态窗口宽度
        var iHeight = 400;//模态窗口高度
        var iTop = (window.screen.height - iHeight) / 2;
        var iLeft = (window.screen.width - iWidth) / 2;
        window.resizeTo(iWidth, iHeight);//调整大小
        window.moveTo(iLeft,iTop);//移动位置
    }
</script>
</head>
<body style="background-color: #F7Feff" >
    <form id="form1" runat="server">
    <div>
    <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="False"
                            BorderStyle="Solid" BorderWidth="2px" CaptionAlign="Right" CellPadding="4"
                            ForeColor="#333333" Width="100%" Caption="领导批示" Font-Size="Medium">


                            <Columns>

                                <asp:BoundField DataField="JieShouRenXM" HeaderText="批示人">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="QianShouShiJian" HeaderText="批示时间">
                                    <HeaderStyle Width="100px" />
                                    <ItemStyle Width="100px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="QianShouNeiRong" HeaderText="批示内容" >
                                    
        
                                  </asp:BoundField>
                                </Columns>
                        </asp:GridView>
    </div>
    </form>
</body>
</html>
