
var bMoveable=true; 
var _VersionInfo="修改者:不人不义;MAIL: xhunanpp@hotmail.com" ;
//==================================================== WEB 页面显示部分 ===================================================== 
var strFrame;        //存放日历层的HTML代码 
document.writeln('<iframe id=meizzDateLayer Author=wayx frameborder=0 style="position: absolute; width: 144; height: 28; z-index: 9998; display: none"></iframe>'); 
strFrame='<style>'; 
strFrame+='INPUT.button{BORDER-RIGHT: #ff9900 1px solid;BORDER-TOP: #ff9900 1px solid;BORDER-LEFT: #ff9900 1px solid;'; 
strFrame+='BORDER-BOTTOM: #ff9900 1px solid;BACKGROUND-COLOR: #fff8ec;font-family:宋体;}'; 
strFrame+='TD{FONT-SIZE: 9pt;font-family:宋体;}'; 
strFrame+='</style>'; 
strFrame+='<scr' + 'ipt>'; 
strFrame+='var datelayerx,datelayery;    /*存放日历控件的鼠标位置*/'; 
strFrame+='var bDrag;    /*标记是否开始拖动*/'; 
strFrame+='function document.onmousemove()    /*在鼠标移动事件中，如果开始拖动日历，则移动日历*/'; 
strFrame+='{if(bDrag && window.event.button==1)'; 
strFrame+='    {var DateLayer=parent.document.all.meizzDateLayer.style;'; 
strFrame+='        DateLayer.posLeft += window.event.clientX-datelayerx;/*由于每次移动以后鼠标位置都恢复为初始的位置，因此写法与div中不同*/'; 
strFrame+='        DateLayer.posTop += window.event.clientY-datelayery;}}'; 
strFrame+='function DragStart()        /*开始日历拖动*/'; 
strFrame+='{var DateLayer=parent.document.all.meizzDateLayer.style;'; 
strFrame+='    datelayerx=window.event.clientX;'; 
strFrame+='    datelayery=window.event.clientY;'; 
strFrame+='    bDrag=true;}'; 
strFrame+='function DragEnd(){        /*结束日历拖动*/'; 
strFrame+='    bDrag=false;}'; 
strFrame+='</scr' + 'ipt>'; 
strFrame+='<div style="z-index:9999;position: absolute; left:0; top:0;" onselectstart="return false"><span id=tmpSelectYearLayer Author=wayx style="z-index: 9999;position: absolute;top: 3; left: 19;display: none"></span>'; 
strFrame+='<span id=tmpSelectMonthLayer Author=wayx style="z-index: 9999;position: absolute;top: 3; left: 78;display: none"></span>'; 
strFrame+='<table border=1 cellspacing=0 cellpadding=0 width=142 height=15 bordercolor=#ff9900 bgcolor=#ff9900 Author="wayx">'; 
strFrame+=' <tr Author="wayx"><td width=142 height=23 Author="wayx" bgcolor=#FFFFFF><table border=0 cellspacing=1 cellpadding=0 width=140 Author="wayx" height=23>'; 
strFrame+=' <tr align=center Author="wayx"><td width=16 align=center bgcolor=#ff9900 style="font-size:12px;cursor: hand;color: #ffffff" '; 
strFrame+=' onclick="parent.meizzPrevM()" title="向前翻 1 月" Author=meizz><b Author=meizz>&lt;</b>'; 
strFrame+=' </td><td width=60 align=center style="font-size:12px;cursor:default" Author=meizz '; 
strFrame+='onmouseover="style.backgroundColor=\'#FFD700\'" onmouseout="style.backgroundColor=\'white\'" '; 
strFrame+='onclick="parent.tmpSelectYearInnerHTML(this.innerText.substring(0,4))" title="点击这里选择年份"><span Author=meizz id=meizzYearHead></span></td>'; 
strFrame+='<td width=48 align=center style="font-size:12px;cursor:default" Author=meizz onmouseover="style.backgroundColor=\'#FFD700\'" '; 
strFrame+=' onmouseout="style.backgroundColor=\'white\'" onclick="parent.tmpSelectMonthInnerHTML(this.innerText.length==3?this.innerText.substring(0,1):this.innerText.substring(0,2))"'; 
strFrame+=' title="点击这里选择月份"><span id=meizzMonthHead Author=meizz></span></td>'; 
strFrame+=' <td width=16 bgcolor=#ff9900 align=center style="font-size:12px;cursor: hand;color: #ffffff" '; 
strFrame+=' onclick="parent.meizzNextM()" title="向后翻 1 月" Author=meizz><b Author=meizz>&gt;</td></tr>'; 
strFrame+=' </table>'; 

window.frames.meizzDateLayer.document.writeln(strFrame); 
window.frames.meizzDateLayer.document.close();        //解决ie进度条不结束的问题 
var meizzTheYear=new Date().getFullYear(); //定义年的变量的初始值 
var meizzTheMonth=new Date().getMonth()+1; //定义月的变量的初始值 
var meizzWDay=new Array(39); //定义写日期的数组 

function meizzWriteHead(yy,mm) //往 head 中写入当前的年与月 
{ 
    odatelayer.meizzYearHead.innerText = yy + " 年"; 
odatelayer.meizzMonthHead.innerText = mm + " 月"; 
} 
var outObject; 
var outButton;        //点击的按钮 
var outDate="";        //存放对象的日期 
var odatelayer=window.frames.meizzDateLayer.document.all;        //存放日历对象 

function setday(tt,obj) //主调函数 
{ 

      var dads = document.all.meizzDateLayer.style; 
    var th = tt; 
    var ttop = tt.offsetTop; //TT控件的定位点高 
    var thei = tt.clientHeight; //TT控件本身的高 
    var tleft = tt.offsetLeft; //TT控件的定位点宽 
    var ttyp = tt.type; //TT控件的类型 
    while (tt = tt.offsetParent){ttop+=tt.offsetTop; tleft+=tt.offsetLeft;} 
    dads.top = (ttyp=="image")? ttop+thei : ttop+thei+6; 
    dads.left = tleft; 
    outObject = (arguments.length == 1) ? th : obj; 
    outButton = (arguments.length == 1) ? null : th;    //设定外部点击的按钮 
     //根据当前输入框的日期显示日历的年月 
    var reg = /^(\d+)-(\d{1,2})-(\d{1,2})$/; 
    var r = outObject.value.match(reg); 
    if(r!=null){ 
        r[2]=r[2]-1; 
        var d= new Date(r[1], r[2],r[3]); 
        if(d.getFullYear()==r[1] && d.getMonth()==r[2] && d.getDate()==r[3]){ 
            outDate=d;        //保存外部传入的日期 
        } 
        else outDate=""; 
            meizzSetDay(r[1],r[2]+1); 
    } 
    else{ 
        outDate=""; 
        meizzSetDay(new Date().getFullYear(), new Date().getMonth() + 1); 
    } 

dads.display = ''; 
   


}
function meizzPrevY() //往前翻 Year 
{ 
if(meizzTheYear > 999 && meizzTheYear <10000){meizzTheYear--;} 
else{alert("年份超出范围（1000-9999）！");} 
meizzSetDay(meizzTheYear,meizzTheMonth); 
} 
function meizzNextY() //往后翻 Year 
{ 
if(meizzTheYear > 999 && meizzTheYear <10000){meizzTheYear++;} 
else{alert("年份超出范围（1000-9999）！");} 
meizzSetDay(meizzTheYear,meizzTheMonth); 
} 
function meizzPrevM() //往前翻月份 
{ 
if(meizzTheMonth>1){meizzTheMonth--}else{meizzTheYear--;meizzTheMonth=12;} 
meizzSetDay(meizzTheYear,meizzTheMonth); 
} 
function meizzNextM() //往后翻月份 
{ 
if(meizzTheMonth==12){meizzTheYear++;meizzTheMonth=1}else{meizzTheMonth++} 
meizzSetDay(meizzTheYear,meizzTheMonth); 
} 

function tmpSelectYearInnerHTML(strYear) //年份的下拉框 
{ 
if (strYear.match(/\D/)!=null){alert("年份输入参数不是数字！");return;} 
var m = (strYear) ? strYear : new Date().getFullYear(); 
if (m < 1000 || m > 9999) {alert("年份值不在 1000 到 9999 之间！");return;} 
var n = m - 10; 
if (n < 1000) n = 1000; 
if (n + 26 > 9999) n = 9974; 
var s = "<select Author=meizz name=tmpSelectYear style='font-size: 12px' " 
s += "onblur='document.all.tmpSelectYearLayer.style.display=\"none\"' " 
s += "onchange='document.all.tmpSelectYearLayer.style.display=\"none\";" 
s += "parent.meizzTheYear = this.value; parent.meizzSetDay(parent.meizzTheYear,parent.meizzTheMonth)'>\r\n"; 
var selectInnerHTML = s; 
for (var i = n; i < n + 26; i++) 
{ 
if (i == m) 
{selectInnerHTML += "<option Author=wayx value='" + i + "' selected>" + i + "年" + "</option>\r\n";} 
else {selectInnerHTML += "<option Author=wayx value='" + i + "'>" + i + "年" + "</option>\r\n";} 
} 
selectInnerHTML += "</select>"; 
odatelayer.tmpSelectYearLayer.style.display=""; 
odatelayer.tmpSelectYearLayer.innerHTML = selectInnerHTML; 
odatelayer.tmpSelectYear.focus(); 
} 

function tmpSelectMonthInnerHTML(strMonth) //月份的下拉框 
{ 
if (strMonth.match(/\D/)!=null){alert("月份输入参数不是数字！");return;} 
var m = (strMonth) ? strMonth : new Date().getMonth() + 1; 
var s = "<select Author=meizz name=tmpSelectMonth style='font-size: 12px' " 
s += "onblur='document.all.tmpSelectMonthLayer.style.display=\"none\"' " 
s += "onchange='document.all.tmpSelectMonthLayer.style.display=\"none\";" 
s += "parent.meizzTheMonth = this.value; parent.meizzSetDay(parent.meizzTheYear,parent.meizzTheMonth)'>\r\n"; 
var selectInnerHTML = s; 
for (var i = 1; i < 13; i++) 
{ 
if (i == m) 
{selectInnerHTML += "<option Author=wayx value='"+i+"' selected>"+i+"月"+"</option>\r\n";} 
else {selectInnerHTML += "<option Author=wayx value='"+i+"'>"+i+"月"+"</option>\r\n";} 
} 
selectInnerHTML += "</select>"; 
odatelayer.tmpSelectMonthLayer.style.display=""; 
odatelayer.tmpSelectMonthLayer.innerHTML = selectInnerHTML; 
odatelayer.tmpSelectMonth.focus(); 
} 

function closeLayer() //这个层的关闭 
{ 
document.all.meizzDateLayer.style.display="none"; 
} 



function meizzSetDay(yy,mm) //主要的写程序********** 
{ 
meizzWriteHead(yy,mm); 
//设置当前年月的公共变量为传入值 
meizzTheYear=yy; 
meizzTheMonth=mm; 
 outObject.value=yy + "-" + mm;
  } 


function meizzToday() //Today Button 
{ 
    var today; 
meizzTheYear = new Date().getFullYear(); 
meizzTheMonth = new Date().getMonth()+1; 
today=new Date().getDate(); 
//meizzSetDay(meizzTheYear,meizzTheMonth); 
if(outObject){ 
        outObject.value=meizzTheYear + "-" + meizzTheMonth; 
} 
closeLayer(); 
} 
function document.onclick() //任意点击时关闭该控件    //ie6的情况可以由下面的切换焦点处理代替 
{ 
with(window.event) 
{ if (srcElement.getAttribute("Author")==null && srcElement != outObject && srcElement != outButton) 
closeLayer(); 
} 
} 
