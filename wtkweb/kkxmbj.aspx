<%@ Page Language="C#" AutoEventWireup="true" CodeFile="kkxmbj.aspx.cs" Inherits="kkxmbj" %>

<html >
<head>
 <script type="text/javascript" src="js/coolWindowsCalendar.js"></script>
<title></title>
<link href="css/reset.css" rel="stylesheet" type="text/css" />
<link href="css/common.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AutoManageConnectionString %>"
            SelectCommand="SELECT [id], [xmmc], [kklx], [mdlx], [cj], [jzsj], [sfyx] FROM [T_CQKK_KKXMB] ORDER BY [sfyx] DESC, [tjsj]" OnSelected="SqlDataSource1_Selected"
            DeleteCommand="update t_cqkk_kkxmb set sfyx=-1 where id=@id"
            updateCommand="update t_cqkk_kkxmb set xmmc=@xmmc,kklx=@kklx,mdlx=@mdlx,cj=@cj,kssj=@kssj,jzsj=@jzsj,sfddtj=@sfddtj where id=@id"
            >
            <DeleteParameters>
                <asp:Parameter Name="id" Type="int32" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:Parameter Name="id" Type="int32" />
                <asp:Parameter Name="xmmc" Type="string" />
                <asp:Parameter Name="kklx" Type="int32" />
                <asp:Parameter Name="mdlx" Type="int32" />
                <asp:Parameter Name="kssj" Type="string" />
                <asp:Parameter Name="jzsj" Type="string" />
                <asp:Parameter Name="sfddtj" Type="int32" />

            </UpdateParameters>
        </asp:SqlDataSource>
        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:AutoManageConnectionString %>"
            SelectCommand="select distinct userdept as deptvalue ,userdept + '车间' as deptname from t_cqkk_glyxxb where isvalid=1 and (@udept='_所有' or @udept=userdept) order by userdept">
            <SelectParameters>
                <asp:SessionParameter DefaultValue="_所有" Name="udept" SessionField="udept" />
            </SelectParameters>
        </asp:SqlDataSource>
    
    </div>
            <div style="padding:5px 15px">
                &nbsp;&nbsp;<br />
                &nbsp;<table width="100%">
                    <tr>
                        <td align="center">
                            <asp:Label ID="messageLabel" runat="server" ForeColor="red" Text=""></asp:Label></td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label ID="label1" runat="server" Text="筛选条件："></asp:Label>
                            <asp:TextBox ID="sxtjTextBox" runat="server" Text=""></asp:TextBox>
                            <asp:Button ID="sxButton" runat="server" OnClick="sxButton_Click" Text="确  定" />
                            <asp:Button ID="czButton" runat="server" OnClick="czButton_Click" Text="显示全部" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
                &nbsp;</div>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
            DataKeyNames="ID" DataSourceID="SqlDataSource1" ForeColor="#333333" GridLines="None"
            Width="100%" BorderStyle="Solid" BorderWidth="2px" Caption="指标信息：" CaptionAlign="Right" OnRowDeleted="GridView1_RowDeleted"  AllowPaging="True" OnRowDeleting="GridView1_RowDeleting" OnRowUpdated="GridView1_RowUpdated">
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#EFF3FB" />
            <Columns>
                <asp:BoundField DataField="xmmc" HeaderText="项目名称" SortExpression="BianHao" >
                    <ItemStyle Wrap="False" Width="200px" />
                    <ControlStyle Width="200px" />
                  
                </asp:BoundField>
                <asp:TemplateField HeaderText="项目类型" SortExpression="ShuoMing">
                    <EditItemTemplate>
                        &nbsp;<asp:DropDownList ID="ddlKKLX" runat="server" SelectedValue='<%# Bind("kklx") %>'
                    Width="110px">
                            <asp:ListItem Value="1">提示警告型</asp:ListItem>
                            <asp:ListItem Value="2">严格卡控型</asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Text='<%# getKKLX(Convert.ToInt32(Eval("kklx"))) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Wrap="False" />
                    <ControlStyle Width="70px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="名单类型" SortExpression="LeiXing">
                    <EditItemTemplate>
                        &nbsp;<asp:DropDownList ID="ddlMDLX" runat="server" SelectedValue='<%# Bind("mdlx") %>'
                    Width="110px">
                    <asp:ListItem Value="0">白名单</asp:ListItem>
                    <asp:ListItem Value="1">黑名单</asp:ListItem>
                </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# getMDLX(Convert.ToInt32(Eval("mdlx"))) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Wrap="True" />
                    <ControlStyle Width="60px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="所属车间" SortExpression="LeiBie">
                    <EditItemTemplate>
                        <asp:DropDownList  Enabled='<%# getAdmin()==1?true:false%>' ID="ddlSSCJ" runat="server"  SelectedValue='<%# Bind("cj") %>' DataSourceID="SqlDataSource2" DataTextField="deptname" DataValueField="deptvalue">
                            <asp:ListItem Value="_所有">所有车间</asp:ListItem> 
                            <asp:ListItem Value="新南">新南车间</asp:ListItem>
                            <asp:ListItem Value="运用">运用车间</asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("cj")+"车间" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Wrap="False" />
                    <ControlStyle Width="60px" />
                    
                </asp:TemplateField>
                                <asp:TemplateField HeaderText="开始时间" SortExpression="oldzb">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox222"  onclick='javascript: setday(this);' runat="server" Text='<%# Bind("kssj") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" Text='<%# Convert.ToDateTime(Eval("kssj")).ToString("yyyy-MM-dd") %>'></asp:Label>
                    </ItemTemplate>
                    <ControlStyle Width="70px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="截止时间" SortExpression="oldzb">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2"  onclick='javascript: setday(this);' runat="server" Text='<%# Bind("jzsj") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label123" runat="server" Text='<%# Convert.ToDateTime(Eval("jzsj")).ToString("yyyy-MM-dd") %>'></asp:Label>
                    </ItemTemplate>
                    <ControlStyle Width="70px" />
                </asp:TemplateField>
                       <asp:TemplateField HeaderText="单独录入成绩" SortExpression="sfddtj">
                    <EditItemTemplate>
                        <asp:DropDownList   ID="ddlSddtj" runat="server"  SelectedValue='<%# Bind("sfddtj") %>' >
                            <asp:ListItem Value="1">允许</asp:ListItem> 
                            <asp:ListItem Value="0">不允许</asp:ListItem>
                            </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label511" runat="server" Text='<%# Convert.ToInt32(Eval("sfddtj"))==1?"允许":"不允许" %>'></asp:Label>
                    </ItemTemplate>
                    <ControlStyle Width="50px" />
                </asp:TemplateField>
                
                
                <asp:TemplateField HeaderText="是否有效" SortExpression="sfyx">
                    <EditItemTemplate>
                        <asp:DropDownList  Enabled ="false" ID="ddlSFYX" runat="server"  SelectedValue='<%# Bind("sfyx") %>' >
                            <asp:ListItem Value="1">有效</asp:ListItem> 
                            <asp:ListItem Value="0">无效</asp:ListItem>
                            </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label5" runat="server" Text='<%# Convert.ToInt32(Eval("sfyx"))==1?"有效":"无效" %>'></asp:Label>
                    </ItemTemplate>
                    <ControlStyle Width="50px" />
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="bjkkwz.aspx?id={0}"
                    Text="卡控位置">
                    <ControlStyle Width="60px" />
                </asp:HyperLinkField>
                
                               <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="bjkkxb.aspx?id={0}"
                    Text="卡控线别">
                    <ControlStyle Width="60px" />
                </asp:HyperLinkField>
                
                               <asp:HyperLinkField DataNavigateUrlFields="id" DataNavigateUrlFormatString="bjkkqylx.aspx?id={0}"
                    Text="卡控牵引类型">
                    <ControlStyle Width="80px" />
                </asp:HyperLinkField>
                 
                
                <asp:TemplateField ShowHeader="False">
                    <EditItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update"
                            Text="更新"></asp:LinkButton>
                        <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel"
                            Text="取消"></asp:LinkButton>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton7" runat="server" CausesValidation="False" CommandName="Edit"
                            Text="编辑" Visible='<%#Convert.ToInt32(Eval("id"))!=1?true:false %>'></asp:LinkButton>
                    </ItemTemplate>
                    <ControlStyle Width="70px" />
                    <ItemStyle Wrap="False" />
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton6" runat="server" CausesValidation="False" CommandName="Delete" Visible='<%#Convert.ToInt32(Eval("id"))!=1?true:false %>'
                         OnClientClick="return confirm('删除项目将会导致该项目所有相关成绩丢失，如非特殊原因请选择将项目置为无效，确认删除吗？')"   Text="删除"></asp:LinkButton>
                    </ItemTemplate>
                    <ControlStyle Width="70px" />
                    <ItemStyle Wrap="False" />
                </asp:TemplateField>
            </Columns>
            <PagerStyle  HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#6699CC" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#6699CC" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="Azure"/>
            <AlternatingRowStyle BackColor="White" />
            <EmptyDataTemplate>
                暂无数据
            </EmptyDataTemplate>
                       <PagerTemplate>
                <table width="100%">
                    <tr>
                        <td width="30%" align="left">
                            第<asp:Label ID="lblcurPage" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1      %>'></asp:Label>页/共<asp:Label
                                ID="lblPageCount" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageCount %>'></asp:Label>页</td>
                        <td width="40%" align="center">
                            <asp:LinkButton ID="cmdFirstPage" runat="server" CommandName="Page" CommandArgument="First"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=0 %>">首页</asp:LinkButton>
                            <asp:LinkButton ID="cmdPreview" runat="server" CommandArgument="Prev" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=0 %>">前页</asp:LinkButton>
                            <asp:LinkButton ID="cmdNext" runat="server" CommandName="Page" CommandArgument="Next"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=((GridView)Container.Parent.Parent).PageCount-1 %>">后页</asp:LinkButton>
                            <asp:LinkButton ID="cmdLastPage" runat="server" CommandArgument="Last" CommandName="Page"
                                Enabled="<%# ((GridView)Container.Parent.Parent).PageIndex!=((GridView)Container.Parent.Parent).PageCount-1 %>">尾页</asp:LinkButton>
                            转第<asp:TextBox ID="txtGoPage" runat="server" Text='<%# ((GridView)Container.Parent.Parent).PageIndex+1 %>'
                                Width="32px" CssClass="inputmini"></asp:TextBox><asp:Button ID="Button1" runat="server"
                                    OnClick="Button1_Click" Text="页" /></td>
                       
                    </tr>
                </table>
            </PagerTemplate>
        </asp:GridView>

    </form>
</body>
</html>
