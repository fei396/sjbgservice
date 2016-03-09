using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using gwxxService;
public partial class CuiBanGongWen : System.Web.UI.Page
{
    gwxxService.gwxxWebService s = new gwxxService.gwxxWebService();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        //uid = Convert.ToInt32(user);

        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        btnCuiBan.Attributes["OnClick"] = "return confirm('确定催办未签收该文件的人员？')";
        int uid, rid, gwid, lzid;
        try
        {

            gwid = Convert.ToInt32(Request["gwid"]);
            lzid = Convert.ToInt32(Request["lzid"]);
            uid = Convert.ToInt32(user.GongHao);
            rid = user.RoleID;
        }
        catch
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }
        if (!IsPostBack)
        {
            

  
           
            int type = Convert.ToInt32(Request["type"]);

            
            if (rid == 20)//公文处理员
            {
                lblYiJian.Visible = true;
                tableGuiDang.Visible = true;
                if (type == 1)//已经流转完成
                {
                    btnGuiDang.Visible = false;
                    btnCuiBan.Visible = false;
                }
                else
                {
                    btnGuiDang.Visible = false;
                    btnCuiBan.Visible = true;
                }
            }
            else //其它人员
            {
               
            }
           
            
            getData(gwid, lzid);
        }
    }




    private void getData(int gwid, int lzid)
    {

       
        Security.SetCertificatePolicy();

        gwxxService.GongWen2016 gw = s.getGongWen2016ByID(gwid);

        if (gw == null)
        {
            return;
        }
        else
        {
            lblBiaoti.Text = gw.BiaoTi;
            lblHongTou.Text = gw.HongTou;
            lblWenHao.Text = gw.WenHao;
            txtZhengWen.Text = gw.ZhengWen;
            //lblLeiXing.Text = "公文分类：" + gw.WenJianLeiXing;
            lblYiJian.Text = "呈送意见：" + gw.ChengSongYiJian;
            lblFaWenDanWeiHeShiJian.Text = gw.FaWenDanWei + "                   " + Convert.ToDateTime(gw.FaBuShiJian).ToString("yyyy年M月d日") + "发";
            //lblFaWenShiJian.Text = "发布时间：" + gw.FaBuShiJian;
            
            if (gw.FuJian != null)
            {
                string html ="";
                for(int i=0;i<gw.FuJian.Length;i++)
                {
                    html += "<a style='font-size: large' href='gwfj/" + gw.FuJian[i] + "'>" + gw.FuJian[i] + "</a><br><br>";
                }
                    tdFuJian.InnerHtml = html;
            }
            
        }
        bindLiuZhuanData(true,lzid);
    }

    private void bindLiuZhuanData(bool sfbr ,int lzid)
    {
        gwxxService.GongWenLiuZhuan[] gwlz = s.getLiuZhuanXian(sfbr ,lzid);

        gvList.DataSource = gwlz;
        gvList.DataBind();
    }




   
    protected void btnGuiDang_Click(object sender, EventArgs e)
    {

    }
    protected void btnCuiBan_Click(object sender, EventArgs e)
    {
        int gwid,rid;
        try
        {

            gwid = Convert.ToInt32(Request["gwid"]);

            rid = Convert.ToInt32(ddlCuiBanDuiXiang.SelectedValue);
        }
        catch
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }
        INT i = s.makeCuiBan(gwid, rid);
        if (i.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "催办成功", "alert('催办成功，给以下人员发送短信：" + i.Message +"');", true);
            //Response.Redirect("ListGongWen.aspx?type=0");
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "催办失败", "alert('催办失败：" + i.Message + "')", true);
        }
    }
   
    protected void gvList_SelectedIndexChanged(object sender, EventArgs e)
    {
        int lzid = Convert.ToInt32( gvList.SelectedValue.ToString());
        bindLiuZhuanData(false,lzid);
    }
}