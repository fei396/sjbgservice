

var bMoveable=true; 
var _VersionInfo="修改者:wsggy;MAIL: wsggy@hotmail.com" ;
//==================================================== WEB 页面显示部分 ===================================================== 
var strFrame;        
document.writeln('<iframe id=meizzDateLayer frameborder=0 style="position: absolute; width: 180; height: 150; z-index: 9998;display: none "></iframe>'); 

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

strFrame='                  <TABLE id="Table1" height="90" cellSpacing="0" cellPadding="0" bordercolor=#ff9900 bgColor="#ff9900"';
strFrame+='						border="0">';
strFrame+='						<TR bordercolor=#ffffff>';
strFrame+='							<TD align=center width="20" onclick="parent.FastPrevHour()" style="font-size:12px;cursor: hand;color: #ffffff">&lt;&lt;</FONT></TD>';
strFrame+='							<TD align=center width="20" onclick="parent.PrevHour()" style="font-size:12px;cursor: hand;color: #ffffff"><FONT face="宋体" style="cursor: hand;">&lt;</FONT></TD>';
strFrame+='							<TD align=center width="58" style="font-size:12px;"><INPUT id="tbHour" type="text" size="1"  value="12" name="Text1" onblur = "parent.InputHour(this.value)">时</TD>';
strFrame+='							<TD align=center width="20" onclick="parent.NextHour()" style="font-size:12px;cursor: hand;color: #ffffff">&gt;</FONT></TD>';
strFrame+='							<TD align=center width="20" onclick="parent.FastNextHour()" style="font-size:12px;cursor: hand;color: #ffffff">&gt;&gt;</FONT></TD>';
strFrame+='						</TR>';
strFrame+='						<TR bordercolor=#ffffff>';
strFrame+='							<TD align=center  width="20" onclick="parent.FastPrevMinute()" style="font-size:12px;cursor: hand;color: #ffffff">&lt;&lt;</FONT></TD>';
strFrame+='							<TD align=center width="20" onclick="parent.PrevMinute()" style="font-size:12px;cursor: hand;color: #ffffff">&lt;</FONT></TD>';
strFrame+='							<TD align=center width="58" style="font-size:12px;"><INPUT id="tbMinute" type="text" size="1" value="30" name="Text1" onblur = "parent.InputMinute(this.value)">分</TD>';
strFrame+='							<TD align=center width="20" onclick="parent.NextMinute()" style="font-size:12px;cursor: hand;color: #ffffff">&gt;</FONT></TD>';
strFrame+='							<TD align=center width="20" onclick="parent.FastNextMinute()" style="font-size:12px;cursor: hand;color: #ffffff">&gt;&gt;</FONT></TD>';
strFrame+='						</TR>';
strFrame+='						<TR bordercolor=#ffffff>';
strFrame+='							<TD align=center width="20" onclick="parent.FastPrevSecond()" style="font-size:12px;cursor: hand;color: #ffffff">&lt;&lt;</FONT></TD>';
strFrame+='							<TD align=center width="20" onclick="parent.PrevSecond()" style="font-size:12px;cursor: hand;color: #ffffff">&lt;</FONT></TD>';
strFrame+='							<TD align=center width="58" style="font-size:12px;"><INPUT id="tbSecond" type="text" size="1" value="30" name="Text1" onblur = "parent.InputSecond(this.value)">秒</TD>';
strFrame+='							<TD align=center width="20" onclick="parent.NextSecond()" style="font-size:12px;cursor: hand;color: #ffffff">&gt;</FONT></TD>';
strFrame+='							<TD align=center width="20" onclick="parent.FastNextSecond()" style="font-size:12px;cursor: hand;color: #ffffff">&gt;&gt;</FONT></TD>';
strFrame+='						</TR>';
strFrame+='                     <TR bordercolor=#ffffff>';
strFrame+='	 					    <TD align=center colSpan="2" onclick="parent.onConfirm()" style="font-size:12px;cursor: hand;">确定</FONT></TD>';
strFrame+='							<TD width="58"></TD>';
strFrame+='							';
strFrame+='							<TD align=center colSpan="2" onclick="parent.closeLayer()" style="font-size:12px;cursor: hand;">关闭</FONT></TD>';
strFrame+='						</TR>';
strFrame+='					</TABLE>';
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

window.frames.meizzDateLayer.document.writeln(strFrame); 
window.frames.meizzDateLayer.document.close();        //解决ie进度条不结束的问题 

//==================================================== WEB 页面显示部分 ====================================================== 
var outObject; 
var outButton;        //点击的按钮 
var outDate="";        //存放对象的日期 
var outTime = "";      //存放对象的时间

var odatelayer=window.frames.meizzDateLayer.document.all;        //存放日历对象 

var Hour = 12;
var Minute = 30;
var Second = 30;

function setclock(tt,obj) //主调函数 
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
    //设置时间
    setTime();
      
    dads.display = ''; 
    
    event.returnValue=false; 
    
} 

//设置时间
function setTime()
{
   window.frames.meizzDateLayer.document.all('tbHour').value = Hour;
   window.frames.meizzDateLayer.document.all('tbMinute').value =  Minute;
   window.frames.meizzDateLayer.document.all('tbSecond').value = Second;

}
//输入分钟
function InputMinute(val)
{
   if((val>=0)&&(val<59))
   {
      Minute = val;
   }
   else
   {
     window.frames.meizzDateLayer.document.all('tbMinute').value = Minute;
   }
}
//输入秒

function InputSecond(val)
{
   if((val>=0)&&(val<59))
   {
      Second = val;
   }
   else
   {
     window.frames.meizzDateLayer.document.all('tbSecond').value = Second;
   }
}
//输入小时
function InputHour(val)
{
   if((val>=0)&&(val<24))
   {
      Hour = val;
   }
   else
   {
     window.frames.meizzDateLayer.document.all('tbHour').value = Hour;
   }
}

//前翻一个小时

function PrevHour()
{
   if(Hour>0) Hour--;
   setTime();
}

//快速前翻四个小时

function FastPrevHour()
{
   if(Hour>=4) Hour = Hour - 4;
   setTime();
}
//后翻一个小时

function NextHour()
{
   if(Hour<23) Hour++;
   setTime();
}

//快速后翻四个小时

function FastNextHour()
{
   if(Hour<=19) 
   {      
     Hour ++;
     Hour = Hour + 3;
   }
   setTime();
}

//前翻一个分钟

function PrevMinute()
{
   if(Minute>0) Minute--;
   setTime();
}

//快速前翻十分钟
function FastPrevMinute()
{
   if(Minute>=10) Minute = Minute - 10;
   setTime();
}
//后翻一个分钟

function NextMinute()
{
   if(Minute<59) Minute++;
   setTime();
}

//快速后翻十分钟
function FastNextMinute()
{
   if(Minute<=49)
   {
      Minute ++;
      Minute = Minute + 9;
   }
   setTime();
}

//前翻一个秒钟

function PrevSecond()
{
   if(Second>0) Second--;
   setTime();
}

//快速前翻十秒钟
function FastPrevSecond()
{
   if(Second>=10) Second = Second - 10;
   setTime();
}
//后翻一个秒钟

function NextSecond()
{
   if(Second<59) Second++;
   setTime();
}

//快速后翻十秒钟
function FastNextSecond()
{
   if(Second<=49)
   {
      Second ++;
      Second = Second + 9;
   }
   setTime();
}

function onConfirm()
{
   //outObject.value= yy + "-" + mm + "-" + n ; 
   outObject.value= Hour + ":" + Minute + ":" + Second ; 
   closeLayer();
}


function document.onclick() //任意点击时关闭该控件    //ie6的情况可以由下面的切换焦点处理代替 
{ 
with(window.event) 
{ if (srcElement.getAttribute("Author")==null && srcElement != outObject && srcElement != outButton) 
closeLayer(); 
} 
} 

function document.onkeyup()        //按Esc键关闭，切换焦点关闭 
{ 
if (window.event.keyCode==27){ 
        if(outObject)outObject.blur(); 
        closeLayer(); 
    } 
    else if(document.activeElement) 
        if(document.activeElement.getAttribute("Author")==null && document.activeElement != outObject && document.activeElement != outButton) 
        { 
            closeLayer(); 
        } 
} 

function closeLayer() //这个层的关闭 
{ 
document.all.meizzDateLayer.style.display="none"; 
} 
