<%@ Page Language="C#" AutoEventWireup="true" CodeFile="kkmxcx.aspx.cs" Inherits="kkmxcx" %>


<html >
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=gb2312" />
    <title></title>
    <script type="text/javascript" src="js/coolWindowsCalendar.js"></script>
    <link href="css/reset.css" rel="stylesheet" type="text/css" />
    <link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body style=" background-color :#F7Feff ">
    <form id="form1" runat="server">
        &nbsp;<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AutoManageConnectionString %>" SelectCommand="SELECT    work_no,work_name,department,kklx,czr+czrxm as czr,czsj from v_cqkk_kkjlb where (department=@cj or @cjself='_所有' or department=@cjself) and (kklx=@kklx or @kklx=0) and czsj > @kssj and czsj < 1+ cast(@jzsj as datetime)" OnSelected="SqlDataSource1_Selected">
            <SelectParameters>
            <asp:SessionParameter DefaultValue="_所有" Name="cjself" SessionField="udept" />
                <asp:QueryStringParameter DefaultValue="_所有" Name="cj" QueryStringField="cj" />
                <asp:QueryStringParameter DefaultValue="0" Name="kklx" QueryStringField="kklx" />
                <asp:QueryStringParameter DefaultValue="2000-1-1" Name="kssj" QueryStringField="kssj" />
                <asp:QueryStringParameter DefaultValue="2099-12-31" Name="jzsj" QueryStringField="jzsj" />
               
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:AutoManageConnectionString %>"
            SelectCommand="select distinct userdept as deptvalue ,userdept + '车间' as deptname from t_cqkk_glyxxb where isvalid=1 and (@udept='_所有' or @udept=userdept) order by userdept">
            <SelectParameters>
                <asp:SessionParameter DefaultValue="_所有" Name="udept" SessionField="udept" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:AutoManageConnectionString %>" SelectCommand="(SELECT [id],  '(' + cj + '车间)' + [xmmc]  as xmmc , xmmc as od FROM [V_CQKK_YXKKXM] where (cj=@cj or cj='_所有' or @cj='_所有') union select 0 as id,'__所有项目' as xmmc,'_' as od) order by od">
            <SelectParameters>
                <asp:SessionParameter DefaultValue="_所有" Name="cj" SessionField="udept" />
            </SelectParameters>
        </asp:SqlDataSource>
        <table align="center" border="0" cellpadding="0" cellspacing="0" height="35" width="400">
                                <tr>
                                    <td align="left" class="f23" valign="middle" style="height: 34px; width: 249px;">
                                        <br />
                                        选择车间：<asp:DropDownList ID="ddlCJ" runat="server" DataSourceID="SqlDataSource2" DataTextField="deptname"
                                            DataValueField="deptvalue">
                                        </asp:DropDownList><br />
                                        <br />
                                        卡控类型：<asp:DropDownList ID="ddlKKLX" runat="server">
                                            <asp:ListItem Value="0">所有类型</asp:ListItem>
                                            <asp:ListItem Value="1">记入旷工</asp:ListItem>
                                            <asp:ListItem Value="2">提醒未派</asp:ListItem>
                                            <asp:ListItem Value="3">提醒出乘</asp:ListItem>
                                        </asp:DropDownList><br />
                                        <br />
                                         开始日期：<asp:TextBox ID="txtKSRQ" runat="server" CssClass="text_input" onclick='javascript: setday(this);'></asp:TextBox><br />
                                        <br />
                    截止日期：<asp:TextBox ID="txtJZRQ" runat="server" CssClass="text_input" onclick='javascript: setday(this);'></asp:TextBox><br />
                                     
                                             </td>
                                             <td align="center">
                                                 <asp:Button ID="Button2" runat="server" Text="确  定" OnClick="Button2_Click" /></td>
                                </tr>
                            </table>

        <br />
                            <hr align="center" color="#ff0000" noshade="noshade" width="96%" />
        &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp;&nbsp;
                                <table align="center" border="0" cellpadding="0" cellspacing="0" width="200">
                                    <tr>
                                        <td align="center" valign="bottom" style="height: 30px">
                                            <input onclick="window.print();" style="border-top-width: 1px; padding-right: 1px;
                                                padding-left: 1px; border-left-width: 1px; font-size: 9pt; border-left-color: rgb(204,204,204);
                                                border-bottom-width: 1px; border-bottom-color: rgb(204,204,204); padding-bottom: 1px;
                                                border-top-color: rgb(204,204,204); padding-top: 4px; height: 23px; border-right-width: 1px;
                                                border-right-color: rgb(204,204,204)" type="button" value=" 打 印 " />
                                            &nbsp;<asp:Button runat="server" ID="saveBtn"  style="border-top-width: 1px;
                                                padding-right: 1px; padding-left: 1px; border-left-width: 1px; font-size: 9pt;
                                                border-left-color: rgb(204,204,204); border-bottom-width: 1px; border-bottom-color: rgb(204,204,204);
                                                padding-bottom: 1px; border-top-color: rgb(204,204,204); padding-top: 4px; height: 23px;
                                                border-right-width: 1px; border-right-color: rgb(204,204,204)"
                                                Text="导出Excel" OnClick="saveBtn_Click" /></td>
                                    </tr>
                                </table>
                            <hr align="center" color="#00cc00" noshade="noshade" width="96%" />
        &nbsp;&nbsp;<asp:GridView ID="GridView1" runat="server" AllowPaging="True" AutoGenerateColumns="False"
            BorderStyle="Solid" BorderWidth="2px" Caption="表单信息：" CaptionAlign="Right" CellPadding="4"
             ForeColor="#333333"
            Width="100%" OnRowDataBound="GridView1_RowDataBound" OnSorting="GridView1_Sorting" PageSize="15" OnPageIndexChanging="GridView1_PageIndexChanging" DataSourceID="SqlDataSource1">
     
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerTemplate>
                <table width="100%">
                    <tr>
                        <td align="left" width="30%">
                            第<asp:Label ID="lblcurPage" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1      %>'></asp:Label>页/共<asp:Label
                                ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>'></asp:Label>页</td>
                        <td align="center" width="40%">
                            <asp:LinkButton ID="cmdFirstPage" runat="server" CommandArgument="First" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=0 %>">首页</asp:LinkButton>
                            <asp:LinkButton ID="cmdPreview" runat="server" CommandArgument="Prev" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=0 %>">前页</asp:LinkButton>
                            <asp:LinkButton ID="cmdNext" runat="server" CommandArgument="Next" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=((GridView)Container.Parent.Parent).PageCount-1 %>">后页</asp:LinkButton>
                            <asp:LinkButton ID="cmdLastPage" runat="server" CommandArgument="Last" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=((GridView)Container.Parent.Parent).PageCount-1 %>">尾页</asp:LinkButton>
                            转第<asp:TextBox ID="txtGoPage" runat="server" CssClass="inputmini" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>'
                                Width="32px"></asp:TextBox><asp:Button ID="Button1" runat="server" 
                                OnClick="Button1_Click"     Text="页" /></td>
                    </tr>
                </table>
            </PagerTemplate>
            <PagerStyle HorizontalAlign="Center" />
            <EmptyDataTemplate>
                暂无数据
            </EmptyDataTemplate>
            <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="Azure" />
            <Columns>
                <asp:BoundField DataField="work_no" HeaderText="人员工号" SortExpression="work_no" />
                <asp:BoundField DataField="work_name" HeaderText="人员姓名" SortExpression="work_name" />
                <asp:BoundField DataField="department" HeaderText="车间" SortExpression="department" />
                 <asp:TemplateField HeaderText="卡控类型" SortExpression="kklx">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("kklx") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# getKKLX(Convert.ToInt32(Eval("kklx"))) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField> 
                <asp:BoundField DataField="czsj" HeaderText="被卡控时间" SortExpression="czsj" />
                <asp:BoundField DataField="czr" HeaderText="调度员" SortExpression="czr" />
               
            </Columns>

        </asp:GridView>
                     </form>
</body>
</html>
