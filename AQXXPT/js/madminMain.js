// Code in 2007.1
var
// 获取元素
    $ = function(elementId) {
        if (elementId == null) return false;
        return document.getElementById(elementId);
    },
// 返回字符串的小写
    LcStr = function(Str) {
        if (Str == null) return false;
        return Str.toLowerCase();
    },
// 输出数据(调试用)
    Print = function(Msg) {
        $("message").innerHTML += Msg + "<br />";
    },
// 判断浏览器
    brower = function() {
        var ua = LcStr(navigator.userAgent);
        var os = new Object();
        os.isFirefox = ua.indexOf("gecko") != -1;
        os.isOpera = ua.indexOf("opera") != -1;
        os.isIE = !os.isOpera && ua.indexOf("msie") != -1;
        return os;
    },
// 获取页面大小
    getPageSize = function() {
        var xScroll, yScroll;
        if (window.innerHeight && window.scrollMaxY) {
            xScroll = document.body.scrollWidth;
            yScroll = window.innerHeight + window.scrollMaxY;
        } else if (document.body.scrollHeight > document.body.offsetHeight) { // all but Explorer Mac
            xScroll = document.body.scrollWidth;
            yScroll = document.body.scrollHeight;
        } else { // Explorer Mac...would also work in Explorer 6 Strict, Mozilla and Safari
            xScroll = document.body.offsetWidth;
            yScroll = document.body.offsetHeight;
        }
        var windowWidth, windowHeight;
        if (self.innerHeight) { // all except Explorer
            windowWidth = self.innerWidth;
            windowHeight = self.innerHeight;
        } else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
            windowWidth = document.documentElement.clientWidth;
            windowHeight = document.documentElement.clientHeight;
        } else if (document.body) { // other Explorers
            windowWidth = document.body.clientWidth;
            windowHeight = document.body.clientHeight;
        }
        // for small pages with total height less then height of the viewport
        if (yScroll < windowHeight) {
            pageHeight = windowHeight;
        } else {
            pageHeight = yScroll;
        }
        // for small pages with total width less then width of the viewport
        if (xScroll < windowWidth) {
            pageWidth = windowWidth;
        } else {
            pageWidth = xScroll;
        }
        arrayPageSize = new Array(pageWidth, pageHeight, windowWidth, windowHeight);
        return arrayPageSize;
    },
// 获取页面滚动位置
    getPageScroll = function() {
        var XScroll, yScroll;
        if (self.pageXOffset) {
            XScroll = self.pageXOffset;
        } else if (document.documentElement && document.documentElement.scrollLeft) { // Explorer 6 Strict
            XScroll = document.documentElement.scrollLeft;
        } else if (document.body) { // all other Explorers
            XScroll = document.body.scrollLeft;
        }
        if (self.pageYOffset) {
            yScroll = self.pageYOffset;
        } else if (document.documentElement && document.documentElement.scrollTop) { // Explorer 6 Strict
            yScroll = document.documentElement.scrollTop;
        } else if (document.body) { // all other Explorers
            yScroll = document.body.scrollTop;
        }
        arrayPageScroll = new Array(XScroll, yScroll);
        return arrayPageScroll;
    },

/*

后台框架应用

*/
oHome = "main.aspx",
sName = "安全信息平台",
iFrame = null,
oFrame = null,
prevMenu = null,
prevMMenu = null,
screenWidth = window.screen.width,
pageSize = null,
wResizeEnd = true,
resizeTime = null,
Menus = new Object(),
// 初始化
init = function() {
    if (window != top) {
        top.location.replace(self.location);
        return false;
    }
    // 初始化菜单列表

    Menus = {
        "操作菜单": { "row": 1, "show": true, "menu": { "发送信息": "applyaqxx.aspx", "信息查询": "editfile.aspx" } }

    };

    // 生成并初始化菜单
    menu.create(Menus);
    menu.show("menu_1");
    menu.moduleMenu(null, "btnHome");
    // 显示当前日期
    dateTime.showObj = $("showDate");
    dateTime.show(true, true, false);
    // 框架内容页指向
    iFrame = $("main_data");
    oFrame = iFrame.Document ? iFrame.Document : iFrame.contentDocument;
    //oFrame.location.replace(oHome);
    oFrame.location = oHome;
    // 解决Opera下隐藏左侧菜单异常
    if (brower().isOpera) $("center").parentNode.removeChild($("center"));
},
// 页面reSize控制布局

reSize = function() {
    window.clearTimeout(resizeTime);
    var oHeight = $("top").clientHeight + $("main_toolbar").clientHeight + 1;
    var footheight = $("footer").clientHeight;
    var menuHeight = $("menuWarp").offsetHeight;
    pageSize = getPageSize();
    var minHeight = pageSize[3] - oHeight;
    minHeight = menuHeight > minHeight - footheight ? menuHeight : minHeight - footheight;
    minHeight = minHeight < 450 ? 450 : minHeight - 20;
    //Print(menuHeight +" : "+ (pageSize[3] - oHeight) +" : "+ (minHeight-footheight));

    // 获取并设置整体框架宽度(为防止框架变形,最小为1003)
    screenWidth = window.screen.width;
    if (pageSize[2] < 1003 || screenWidth < 1024) {
        $("wrap").style.width = "1003px";
    } else {
        $("wrap").style.width = "";
    }

    iFrame = $("main_data");
    oFrame = iFrame.Document ? iFrame.Document : iFrame.contentDocument;
    // 设置整站标题，添加鼠标双击监听，按键监听
    //oFrame.onkeydown = document.onkeydown = function(e) { keyDown(e);}
    oFrame.ondblclick = function(e) { fDblClick(e); };
    document.title = sName + " [" + oFrame.title + "]";

    // 获取并设置框架内容页高度
    var oBodyHeight = iFrame.Document ? oFrame.body.scrollHeight : oFrame.body.offsetHeight;
    iFrame.style.height = oBodyHeight > minHeight ? oBodyHeight + "px" : minHeight + "px";
    wResizeEnd = true;
},
// 菜单
menu = {
    // 生成菜单
    create: function(Menus) {
        if (typeof (Menus) != "object") return false;
        var mHtml = "";
        var row = true;
        var show = true;
        var sMenus = new Object();
        var i = 0, j;
        for (var m in Menus) {
            row = Menus[m].row; // 每行几列,最多两列
            show = Menus[m].show; // 是否显示(暂时无用)
            sMenus = Menus[m].menu; // 菜单项列表
            if (!show) continue;
            i++;
            j = 0;
            // 开始一个新的菜单项
            mHtml += "<div class=\"left_menu\" id=\"menu_" + i + "\" menu=\"" + m + "\">\r\n" +
                "	<div class=\"title toolbarBg\"><a href=\"javascript:void(0);\" onclick=\"menu.show('menu_" + i + "');\"><span class=\"btnIco btnClose\"></span><span class=\"btnTxt\">" + m + "</span></a></div>\r\n" +
                "	<div class=\"link\" onclick=\"menu.moduleMenu(event, 'menu_" + i + "');\">\r\n" +
                "		<table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"3\">\r\n" +
                "			<tr>\r\n";
            for (var sm in sMenus) {
                if (row != 2) { // 是否两列显示
                    mHtml += "				<td><a href=\"" + sMenus[sm] + "\" target=\"main_data\">" + sm + "</a></td>\r\n" +
                        "			</tr>\r\n" +
                        "			<tr>\r\n";
                } else if ((j % 2 == 0) && (row == 2)) {
                    mHtml += "				<td><a href=\"" + sMenus[sm] + "\" target=\"main_data\">" + sm + "</a></td>\r\n";
                } else {
                    mHtml += "				<td width=\"1\">|</td>\r\n" +
                        "				<td align=\"right\"><a href=\"" + sMenus[sm] + "\" target=\"main_data\">" + sm + "</a></td>\r\n" +
                        "			</tr>\r\n" +
                        "			<tr>\r\n";
                }
                j++;
            }
            // 如果菜单表格没结束
            if ((j % 2 != 0) && (row == 2)) {
                mHtml += "				<td width=\"1\">&nbsp;</td>\r\n" +
                    "				<td align=\"right\">&nbsp;</td>\r\n" +
                    "			</tr>\r\n" +
                    "			<tr>\r\n";
            } else if (j == 0) { // 如果菜单项为空
                mHtml += "				<td align=\"center\" height=\"50\">您没权限管理该版块！</td>\r\n";
            }
            // 结束菜单项
            mHtml += "			</tr>\r\n" +
                "		</table>\r\n" +
                "	</div>\r\n" +
                "</div>\r\n";
        }
        // 输出菜单
        $("menuWarp").innerHTML = mHtml + $("menuWarp").innerHTML;
    },
    // 显示隐藏菜单项
    show: function(menu) {
        if (prevMenu != null && prevMenu != menu) $(prevMenu).className = "left_menu";
        //$(menu).className = $(menu).className == "left_menu" ? "left_menu_open" : "left_menu";
        $(menu).className = "left_menu_open";
        prevMenu = menu;
    },
    // 设置模块管理菜单
    moduleMenu: function(e, cMenu) {
        if (e != null) {
            e = e.target || e.srcElement;
            if (LcStr(e.tagName) != "a" || prevMMenu == cMenu) return false;
        }
        var mName = $(cMenu).getAttribute("menu");
        var sMenus = new Object();
        var tMenus = new Array();
        var mHtml = "";
        var i = 0;
        for (var m in Menus) {
            // 如果为当前点击菜单项
            if (m == mName) {
                sMenus = Menus[m].menu;
                // 设置模块标题
                $("manage_title").innerHTML = m;
                for (var sm in sMenus) {
                    tMenus[i] = "<a href=\"" + sMenus[sm] + "\" target=\"main_data\">" + sm.replace(/[ ]/g, "") + "</a>";
                    i++;
                }
                mHtml = tMenus.join("&nbsp;--&nbsp;");
                $("manage_menu").innerHTML = mHtml;
                break;
            }
        }
        prevMMenu = menu;
    },
    // 显示隐藏左侧整列菜单
    showLeft: function() {
        if ($("left").className == "left_close") {
            $("left").className = "left_border";
            $("main").className = "main_border";
            $("btnShowLeft").className = "sh_left_open";
            $("btnShowLeft").setAttribute("title", "隐藏左侧导航菜单");
        } else {
            $("left").className = "left_close";
            $("main").className = "";
            $("btnShowLeft").className = "sh_left";
            $("btnShowLeft").setAttribute("title", "显示左侧导航菜单");
        }
    }
},
// 列表
list = {
    clickList: function(e) {
        e = e.target || e.srcElement;
        if (LcStr(e.tagName) == "input" && LcStr(e.type) == "checkbox") {
            list.setListBg(e);
            return false;
        }

        if (LcStr(e.tagName) != "td") return false;

        var E = e.parentNode;
        if (LcStr(E.tagName) != "tr" || E.id == "Title") return false;

        var Is = E.getElementsByTagName("input")[0];
        Is.checked = Is.checked ? false : true;
        list.setListBg(Is);
    },
    setListBg: function(e) {
        e.parentNode.parentNode.style.cssText = e.checked ? "background-color: #FFFCD9;" : "";
    },
    selectAll: function(Form, SlName, Sort, SetBg) {
        //var El = Form.getElementsByTagName("input");
        var El = Form.elements;
        //反选
        if (Sort == 3) {
            for (var i = 0; i < El.length; i++) {
                var e = El[i];
                if (LcStr(e.tagName) == "input" && LcStr(e.type) == "checkbox" && e.name == SlName) {
                    //if(LcStr(e.tagName) == "input" && LcStr(e.type) == "checkbox"){
                    e.checked = e.checked ? false : true;
                    if (e.name == SlName && SetBg) list.setListBg(e);
                }
            }
            return false;
        }

        //全选,不选
        for (var i = 0; i < El.length; i++) {
            var e = El[i];
            if (LcStr(e.tagName) == "input" && LcStr(e.type) == "checkbox" && e.name == SlName) {
                //if(LcStr(e.tagName) == "input" && LcStr(e.type) == "checkbox"){
                e.checked = Sort == 0 ? true : false;
                if (e.name == SlName && SetBg) list.setListBg(e);
            }
        }
    }

},
// 日期事件
dateTime = {
    now: new Date(),
    showObj: null,
    timer: null,
    tInt: null,
    date: function() {
        var nY = dateTime.now.getFullYear(), nM = dateTime.now.getMonth() + 1, nD = dateTime.now.getDate();
        return (nY + "年" + nM + "月" + nD + "日");
    },
    week: function() {
        var day = dateTime.now.getDay();
        var wStr = null;
        switch (day) {
        case 0:
            wStr = "日";
            break;
        case 1:
            wStr = "一";
            break;
        case 2:
            wStr = "二";
            break;
        case 3:
            wStr = "三";
            break;
        case 4:
            wStr = "四";
            break;
        case 5:
            wStr = "五";
            break;
        case 6:
            wStr = "六";
            break;
        }
        wStr = "星期" + wStr;
        return wStr;
    },
    time: function() {
        var now = new Date();
        var nH = now.getHours(), nM = now.getMinutes(), nS = now.getSeconds();
        nH = nH > 10 ? nH : "0" + nH;
        nM = nM > 10 ? nM : "0" + nM;
        nS = nS > 10 ? nS : "0" + nS;
        var tmpStr = null;
        var tInt = null;
        if (dateTime.timer == null) {
            clearInterval(dateTime.tInt);
            tmpStr = "<span id=\"timer\">" +
                nH + ":" + nM + ":" + nS +
                "</span>";
            dateTime.timer = $("timer");
            dateTime.tInt = setInterval("dateTime.time()", 1000);
            return tmpStr;
        } else {
            tmpStr = nH + ":" + nM + ":" + nS;
            dateTime.timer.innerHTML = tmpStr;
        }
    },
    show: function(d, w, t) {
        if (dateTime.showObj == null) {
            return false;
        }
        var tnpStr = null;
        var tmpDate = new Array();
        tmpDate[0] = d ? dateTime.date() : null;
        tmpDate[1] = w ? dateTime.week() : null;
        tmpDate[2] = t ? dateTime.time() : null;
        tnpStr = tmpDate.join("&nbsp;");
        dateTime.showObj.innerHTML = tnpStr;
    }
},
keyDown = function(e) {
    e = e || event || iFrame.contentWindow.event;
    //alert(e.keyCode);
    //e.keyCode = 0;
    //e.returnValue = false;
    //return false;
},
fDblClick = function(e) {
    var E = e || iFrame.contentWindow.event;
    E = E.target || E.srcElement;
    var peTag = LcStr(E.parentNode.tagName);
    var eTag = LcStr(E.tagName);
    if ((eTag == "html" || eTag == "body" || eTag == "td" || eTag == "div" || eTag == "span") && (eTag != "a" && peTag != "a")) menu.showLeft();
},
goUrl = function(Url) {
    oFrame.location = Url;
},
exit = function() {
    Print("退出系统...");
},
// 按钮单击
goTo = function(e) {
    //e = e.target || e.srcElement;
    //e = LcStr(e.tagName) == "a" ? e : e.parentNode;
    var eRel = LcStr(e.getAttribute("rel"));
    var eUrl = e.getAttribute("href");
    switch (eRel) {
    case "siteinfo":
        goUrl("Config_WebSite.asp");
        //menu.moduleMenu(event, menu_1);
        $("manage_title").innerHTML = "网站配置";
        $("manage_menu").innerHTML = "网站配置";
        break;
    case "userpanel":
        goUrl("");
        break;
    case "message":
        goUrl("");
        break;
    case "home":
        goUrl("Admin_Main.asp");
        menu.moduleMenu(null, "btnHome");
        $("manage_title").innerHTML = "派班安全卡控系统";
        $("manage_menu").innerHTML = "派班安全卡控系统";
        break;
    case "default":
        goUrl("");
        menu.moduleMenu(null, "btnHome");
        break;
    case "column":
        goUrl("Config_Column.asp");
        //menu.moduleMenu(event, menu_2);
        $("manage_title").innerHTML = "栏目设置";
        $("manage_menu").innerHTML = "栏目设置";
        break;
    case "exit":
        exit();
        break;
    case "showleft":
        menu.showLeft();
        break;
    default:
        return false;
    }
};