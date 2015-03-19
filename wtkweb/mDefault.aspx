<%@ Page Language="C#" AutoEventWireup="true"   CodeFile="mDefault.aspx.cs" Inherits="mDefault" %>



<html>
<head runat="server">
    <title>新乡机务段派班安全卡控系统</title>
    <link href="css/Style.css" rel="stylesheet" type="text/css" />
 <%--   <script language="javascript" type="text/javascript" src="js/madminMain.js" charset="GB2312"></script>--%>
<script language="javascript" type="text/javascript">
<!--
//window.onload = function() {
//	init(2);
//}
window.onresize = function() {
	if(!wResizeEnd) return false;
	wResizeEnd = false;
	resizeTime = window.setTimeout("reSize()", 50);
}
-->
</script>
</head>
<body>
    <form id="form1" runat="server">
<div id="wrap">
	        <!-- 顶部 -->
	        <div id="top">
		        <div id="top_left_img"></div>
	        </div>
	        <table width="100%" border="0" cellspacing="0" cellpadding="0" style="border: 1px solid #95B6CE;">
	          <tr>
			        <!-- 左侧 -->
			        <td id="left" class="left_border">
				        <div id="left_toolbar" class="toolbarBg">
					        <a href="mDefault.aspx" id="btnHome" rel="Home" target="main_data">管理首页</a><span class="split ltSplit" ></span>
                            <a href="logout.aspx"  onclick="return confirm('确认退出系统？');">退出系统</a>				</div>
				        <!-- 左侧导航菜单开始 -->
				        <div id="menuWarp">
					        <%--<div class="left_menu_open">
						        <div class="title toolbarBg"><a href="javascript:void(0);">系统信息</a></div>
						        <div class="link">
							        <ul>
								        <li>版权所有：K</li>
								        <li>设计制作：K</li>
								        <li>技术支持：K</li>
							        </ul>
						        </div>
					        </div>--%>
					        <!-- 左侧导航菜单结束 -->
					        <div id="message"></div>
				        </div>
			        </td>
			        <td id="center"></td>
			        <!-- 内容 -->
			        <td id="main" class="main_border">
				        <div id="main_toolbar" class="toolbarBg">
					        <div id="showDate" class="right"></div>
					        <a href="javascript:void(0);" onclick="goTo(this);" rel="showLeft" id="btnShowLeft" class="sh_left_open" title="隐藏左侧导航菜单"><span class="btnIco btnShLeft"></span><span id="manage_title">系统管理员</span></a>
				        </div>
				        <iframe src="" id="main_data" name="main_data" marginwidth="0" marginheight="0" frameborder="0" scrolling="no" onload="reSize();"></iframe>
				        <div id="footer" class="toolbarBg"><span class="left"> 2011版权所有.</span></div>
			        </td>
		        </tr>
	        </table>
        </div>
    </form>
</body>
</html>
