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
using System.Collections.Generic;
public partial class BuGongWen : System.Web.UI.Page
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
        if (user.RoleID != 20)
        {
            Response.Redirect("error.aspx?errCode=不能打开该页面");
        }
        btnQianShou.Attributes["OnClick"] = "return confirm('确定补阅该文件？')";
        
        
        if (!IsPostBack)
        {
            int uid, rid, gwid, lzid;
            gwxxService.BuMenFenLei[] bmfl;
            List<string> jsr;
            try
            {
                gwid = Convert.ToInt32(Request["gwid"]);
                lzid = Convert.ToInt32(Request["lzid"]);
                uid = Convert.ToInt32(Request["uid"]);
                GongWenYongHu gwyh = s.getGongWenYongHuByUid(uid);
                rid = gwyh.RoleID;
            }
            catch
            {
                Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
                return;
            }

            bmfl= s.getBuMenFenLei(uid,rid);
            Session["bmfl"] = bmfl;
            jsr = new System.Collections.Generic.List<string>();
            Session["jsr"] = jsr;
            //s.SjbgSoapHeaderValue = Security.getSoapHeader();
            //Security.SetCertificatePolicy();
            getData(gwid, lzid,uid,bmfl);
        }
    }

    


   

    private void getData(int gwid, int lzid,int uid ,BuMenFenLei[] bmfl)
    {
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
       
        Security.SetCertificatePolicy();

        gwxxService.GongWen2016 gw = s.getGongWen2016ByID(gwid);

        if (gw == null)
        {
            return;
        }
        else
        {
            lblBiaoti.Text = gw.BiaoTi;
        }
        gwxxService.GongWenLiuZhuan[] gwlzs = s.getLiuZhuanXian(true, 0, lzid);
        foreach (GongWenLiuZhuan gwlz in gwlzs)
        {
            if (Convert.ToInt32(gwlz.JieShouRen) == uid)
            {
                txtQianShouNeiRong.Text = gwlz.QianShouNeiRong;
                lblLingDao.Text = "批阅领导：" + gwlz.JieShouRenXM;
            }
            if (Convert.ToInt32(gwlz.FaSongRen) == uid)
            {
                jsr.Add(gwlz.JieShouRen);
            }
        }
        gvListBuMen.DataSource = bmfl;
        gvListBuMen.DataBind();
        setCheckBoxListItemSelected();
    }


    protected gwxxService.GongWenBuMenRenYuan[] GetCKBLDataSource(int index)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return null;
        return bmfl[index].RenYuan;
    }


   
    protected void cbl_SelectedIndexChanged(object sender, EventArgs e)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return;
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
        CheckBoxList cbl = (CheckBoxList)(sender);
        string strCheck = Request.Form["__EVENTTARGET"].ToString();
        int strIndex = Convert.ToInt32(strCheck.Substring(strCheck.LastIndexOf("$") + 1));
        int row = Convert.ToInt32(strCheck.Substring(strCheck.IndexOf("$") + 4 ,2)) -2;
        string txt;
        if (bmfl[row].FenLeiID == 1) txt = bmfl[row].RenYuan[strIndex].NiCheng;
        else txt = cbl.Items[strIndex].Text;
        if (cbl.Items[strIndex].Selected)
        {
            //txtQianShouNeiRong.Text += txt + "、";
            jsr.Add(cbl.Items[strIndex].Value);
        }
        else
        {
            //txtQianShouNeiRong.Text = txtQianShouNeiRong.Text.Replace(txt + "、", "");
            jsr.Remove(cbl.Items[strIndex].Value);
        }

    }
    protected void chb_CheckedChanged(object sender, EventArgs e)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return;
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
        CheckBox chb = (CheckBox)(sender);
        string strCheck = Request.Form["__EVENTTARGET"].ToString();
        int row = Convert.ToInt32(strCheck.Substring(strCheck.IndexOf("$") + 4, 2)) - 2;
        if (chb.Checked)
        {
            //txtQianShouNeiRong.Text += chb.Text + "、";
            foreach(gwxxService.GongWenBuMenRenYuan ry in bmfl[row].RenYuan)
            {
                jsr.Add(ry.GongHao);
            }
        }
        else
        {
            //txtQianShouNeiRong.Text = txtQianShouNeiRong.Text.Replace(chb.Text + "、", "");
            foreach (gwxxService.GongWenBuMenRenYuan ry in bmfl[row].RenYuan)
            {
                jsr.Remove(ry.GongHao);
            }
        }
        setCheckBoxListItemSelected();
    }
    protected void btnQianShou_Click(object sender, EventArgs e)
    {
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        //uid = Convert.ToInt32(user);
        int buid=0;
        int gwid, lzid, uid;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        
        string[] jsry = jsr.ToArray();



        try
        {
            gwid = Convert.ToInt32(Request["gwid"]);
            lzid = Convert.ToInt32(Request["lzid"]);
            uid = Convert.ToInt32(Request["uid"]);
            buid = Convert.ToInt32(user.GongHao);
        }
        catch
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "补阅公文失败", "alert('登录已过期。')", true);
            return;
        }

        INT i= s.buGongWen2016(gwid, lzid, uid, buid,jsry);
        if (i.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "补阅公文成功", "alert('补阅公文成功');", true);
            //Response.Redirect("ListGongWen.aspx?type=0");
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "补阅公文失败", "alert('补阅公文失败：" + i.Message + "')", true);
        }
    
    }
   
   
    private void setCheckBoxListItemSelected()
    {
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
        foreach (GridViewRow row in gvListBuMen.Rows) 
        {
            CheckBoxList cbl = row.FindControl("cbl") as CheckBoxList;
            if (cbl == null) continue;
            foreach(ListItem item in cbl.Items)
            {
                
                if (jsr.IndexOf( item.Value) != -1)
                {
                    //存在
                    item.Selected = true;
                }
                else
                {
                    //不存在
                    item.Selected = false;
                    item.Text = "<font color='red'>" + item.Text + "</font>";
                }
            }
        }
    }
    protected void gvListBuMen_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return;
        if (e.Row.RowIndex < 0) return;
        CheckBoxList cbl = e.Row.FindControl("cbl") as CheckBoxList;
        if (cbl == null) return;
        switch (bmfl[e.Row.RowIndex].FenLeiID )
        {
            case 0:
            case 1:
                cbl.RepeatColumns = 8;
                break;
            case 2:
                cbl.RepeatColumns = 8;
                break;
            default:
                cbl.RepeatColumns = 6;
                break;
        }
        
    }

}
