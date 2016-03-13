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
public partial class ViewGongWen : System.Web.UI.Page
{
    gwxxService.gwxxWebService s = new gwxxService.gwxxWebService();
    
    //static gwxxService.BuMenFenLei[] bmfl;
    //static System.Collections.Generic.List<string> jsr;
    //static System.Collections.Generic.List<int> zdybm;
    protected void Page_Load(object sender, EventArgs e)
    {
        int uid, rid, gwid, lzid;
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        //uid = Convert.ToInt32(user);

        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        btnQianShou.Attributes["OnClick"] = "return confirm('确定签阅该文件？')";
        
        
        if (!IsPostBack)
        {

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

            gwxxService.BuMenFenLei[] bmfl = s.getBuMenFenLei(uid, rid);
            Session["bmfl"] = bmfl;
            if (rid == 21 || rid == 22)
            {
                tableZdybm.Visible = true;
                bumen(uid);
                lblYiJian.Visible = true;
            }
            else
            {
                tableZdybm.Visible = false;
                lblYiJian.Visible = false;
            }
            if (rid == 23 || rid == 24)
            {
                tableAllLingDao.Visible = true;
                
            }
            else
            {
                tableAllLingDao.Visible = false;
                
            }
            int type = Convert.ToInt32(Request["type"]);

            if (type == 1 || type == 2)
            {
                tableQianShou.Visible = false;

            }
            else
            {
                tableQianShou.Visible = true;
            }

            List<string> jsr = new System.Collections.Generic.List<string>();
            Session["jsr"] = jsr;
            //zdybm = new System.Collections.Generic.List<int>();
            //s.SjbgSoapHeaderValue = Security.getSoapHeader();
            //Security.SetCertificatePolicy();

            
            getData(gwid, lzid,bmfl);
        }
    }

    protected string duanyu()
    {
        int uid;
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        //uid = Convert.ToInt32(user);
       
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return "";
        }
        try
        {
            uid = Convert.ToInt32(user.GongHao);
            
        }
        catch
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return "";
        }
        gwxxService.GongWenZiDingYiDuanYu[] zdydy = s.getZiDingYiDuanYu(uid,false);
        System.Text.StringBuilder duanyu = new System.Text.StringBuilder();
        duanyu.Append("<tr><td>");
        for (int i = 0; i < zdydy.Length;i++ )
        {
            if (zdydy[i].SiYou == true && zdydy[i-1].SiYou == false)
            {
                duanyu.Append("</td></tr><tr><td>");
            }
            duanyu.Append("<input style='font-size: large' type='button' id='1' value='" + zdydy[i].DuanYuNeiRong + "'  onclick=qing('" + zdydy[i].DuanYuNeiRong + "'); /> ");
        }
            duanyu.Append("</td></tr>");
        return duanyu.ToString();
    }



    private void bumen(int uid)
    {
        gwxxService.GongWenZiDingYiBuMen[] zdybm = s.getZiDingYiBuMen(uid);
        if (zdybm.Equals(null))
        {
            tableZdybm.Visible = false;
        }
        else
        {
            tableZdybm.Visible = true;
            cblZdybm.DataSource = zdybm;
            cblZdybm.DataTextField = "MingCheng";
            cblZdybm.DataValueField = "ID";
            cblZdybm.DataBind();
        }
       
        //if (bmfl.Equals(null))
        //{
        //    tableBm.Visible = false;
        //}
        //else
        //{

        //    CheckBoxList cbl = new CheckBoxList();
        //    TableCell tc3;
        //    for (int i = 0; i < bmfl.Length; i++)
        //    {
        //        int flid = bmfl[i].FenLeiID;

                
        //            TableRow tr = new TableRow();
        //            TableCell tc1 = new TableCell();
        //            tc1.Text = bmfl[i].FenLeiMingCheng;
        //            tr.Cells.Add(tc1);
        //            TableCell tc2 = new TableCell();
        //            CheckBox cbFenLei = new CheckBox();
        //            cbFenLei.ID = "cbbmfl" + bmfl[i].FenLeiID;
        //            cbFenLei.Text = bmfl[i].FenLeiZongCheng;
        //            tc2.Controls.Add(cbFenLei);
        //            tr.Cells.Add(tc2);

        //            cbl = new CheckBoxList();
        //            cbl.RepeatDirection = RepeatDirection.Horizontal;
        //            cbl.RepeatColumns = 8;
        //            cbl.DataSource = bmfl[i].RenYuan;
        //            cbl.DataTextField = "XianShiMingCheng";
        //            cbl.DataValueField = "GongHao";
        //            cbl.DataBind();
        //            cbl.SelectedIndexChanged += cblZdybm_SelectedIndexChanged;
        //            tc3 = new TableCell();
        //            tc3.Controls.Add(cbl);
        //            tr.Cells.Add(tc3);
        //            tableBm.Rows.Add(tr);


        //    }
        //}
    }

    private void getData(int gwid, int lzid ,BuMenFenLei[] bmfl)
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


 
        gvListBuMen.DataSource = bmfl;
        gvListBuMen.DataBind();
        bindLiuZhuanData(true,lzid);
    }

    private void bindLiuZhuanData(bool sfbr ,int lzid)
    {
        gwxxService.GongWenLiuZhuan[] gwlz = s.getLiuZhuanXian(sfbr ,lzid);

        gvList.DataSource = gwlz;
        gvList.DataBind();
    }

    protected gwxxService.GongWenBuMenRenYuan[] GetCKBLDataSource(int index)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return null;
        return bmfl[index].RenYuan;

    }


    protected void cblZdybm_SelectedIndexChanged(object sender, EventArgs e)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return;
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
        string strCheck = Request.Form["__EVENTTARGET"].ToString();
        int strIndex = Convert.ToInt32(strCheck.Substring(strCheck.LastIndexOf("$") + 1));
        if (this.cblZdybm.Items[strIndex].Selected)
        {
            txtQianShouNeiRong.Text += this.cblZdybm.Items[strIndex].Text.Trim() + "、";
            GongWenBuMenRenYuan[] ry = s.getZiDingYiBuMenRenYuan(Convert.ToInt32(this.cblZdybm.Items[strIndex].Value), true);
            if (ry != null)
            {
                foreach (GongWenBuMenRenYuan r in ry)
                {
                    jsr.Add(r.GongHao);
                }
            }
        }
        else
        {
            txtQianShouNeiRong.Text = txtQianShouNeiRong.Text.Replace(this.cblZdybm.Items[strIndex].Text.Trim() + "、", "");
            GongWenBuMenRenYuan[] ry = s.getZiDingYiBuMenRenYuan(Convert.ToInt32(this.cblZdybm.Items[strIndex].Value), true);
            if (ry != null)
            {
                foreach (GongWenBuMenRenYuan r in ry)
                {
                    jsr.Remove(r.GongHao);
                }
            }
            //zdybm.Remove(Convert.ToInt32(this.cblZdybm.Items[strIndex].Value));
        }
        Session["jsr"] = jsr;
        setCheckBoxListItemSelected();

    }
    protected void cbl_SelectedIndexChanged(object sender, EventArgs e)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return;
        List<string> jsr = Session["jsr"] as List<string>;
        CheckBoxList cbl = (CheckBoxList)(sender);
        string strCheck = Request.Form["__EVENTTARGET"].ToString();
        int strIndex = Convert.ToInt32(strCheck.Substring(strCheck.LastIndexOf("$") + 1));
        int row = Convert.ToInt32(strCheck.Substring(strCheck.IndexOf("$") + 4 ,2)) -2;
        string txt;
        if (bmfl[row].FenLeiID == 1) txt = bmfl[row].RenYuan[strIndex].NiCheng;
        else txt = cbl.Items[strIndex].Text;
        if (cbl.Items[strIndex].Selected)
        {
            txtQianShouNeiRong.Text += txt + "、";
            jsr.Add(cbl.Items[strIndex].Value);
        }
        else
        {
            txtQianShouNeiRong.Text = txtQianShouNeiRong.Text.Replace(txt + "、", "");
            jsr.Remove(cbl.Items[strIndex].Value);
        }
        Session["jsr"] = jsr;
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
            txtQianShouNeiRong.Text += chb.Text + "、";
            foreach (gwxxService.GongWenBuMenRenYuan ry in bmfl[row].RenYuan)
            {
                jsr.Add(ry.GongHao);
            }
        }
        else
        {
            txtQianShouNeiRong.Text = txtQianShouNeiRong.Text.Replace(chb.Text + "、", "");
            foreach (gwxxService.GongWenBuMenRenYuan ry in bmfl[row].RenYuan)
            {
                jsr.Remove(ry.GongHao);
            }
        }
        Session["jsr"] = jsr;
        setCheckBoxListItemSelected();
    }

    private List<string > getJsrList()
    {
        List<string> jsrList = new List<string>();
        foreach (GridViewRow row in gvListBuMen.Rows)
        {
            CheckBoxList cbl = row.FindControl("cbl") as CheckBoxList;
            if (cbl == null) continue;
            foreach (ListItem item in cbl.Items)
            {
                if (item.Selected == true)
                {
                    jsrList.Add(item.Value);
                }
            }
        }
        return jsrList;
    }
    protected void btnQianShou_Click(object sender, EventArgs e)
    {
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        //uid = Convert.ToInt32(user);
        int uid, gwid, lzid;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        int[] zdy = null;
        string[] jsry = jsr.ToArray();
        string pishi = txtQianShouNeiRong.Text.Trim();
        if (pishi.Equals(""))
        {
            //Page.ClientScript.RegisterStartupScript(GetType(), "签收公文失败", "alert('签收公文失败：请输入签阅内容。')", true);
            pishi = "阅。";
        }
        
        try
        {
            gwid = Convert.ToInt32(Request["gwid"]);
            lzid = Convert.ToInt32(Request["lzid"]);
            uid = Convert.ToInt32(user.GongHao);
        }
        catch
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "签收公文失败", "alert('登录已过期。')", true);
            return;
        }

        INT i= s.signGongWen2016(gwid, lzid, uid, jsry, pishi,zdy,pbModule.getIP());
        if (i.Number == 1)
        {
            jsr.Clear();
            Session["jsr"] = null;
            Session["bmfl"] = null;
            //zdybm.Clear();
            Page.ClientScript.RegisterStartupScript(GetType(), "签收公文成功", "alert('签收公文成功');window.open('mdefault.aspx','_parent');", true);
            
            //Response.Redirect("ListGongWen.aspx?type=0");
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "签收公文失败", "alert('签收公文失败：" + i.Message + "')", true);
        }
    
    }
   
    protected void cbAll_CheckedChanged(object sender, EventArgs e)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return;
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
        if (cbAll.Checked)
        {
            txtQianShouNeiRong.Text += cbAll.Text + "、";
            foreach (BuMenFenLei b in bmfl)
            {
                if (b.FenLeiID == 1) continue;
                foreach (GongWenBuMenRenYuan r in b.RenYuan)
                {
                    jsr.Add(r.GongHao);
                }
            }
            
        }
        else
        {
            txtQianShouNeiRong.Text = txtQianShouNeiRong.Text.Replace(cbAll.Text + "、", "");
            foreach (BuMenFenLei b in bmfl)
            {
                if (b.FenLeiID == 1) continue;
                foreach (GongWenBuMenRenYuan r in b.RenYuan)
                {
                    jsr.Remove(r.GongHao);
                }
            }
            
        }
        Session["jsr"] = jsr;
        setCheckBoxListItemSelected();
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
                }
            }
        }
    }
    protected void gvListBuMen_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex < 0) return;
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return;
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
    protected void gvList_SelectedIndexChanged(object sender, EventArgs e)
    {
        int lzid = Convert.ToInt32( gvList.SelectedValue.ToString());
        bindLiuZhuanData(false,lzid);
    }
    protected void hyLinkLingDaoPiShi_Click(object sender, EventArgs e)
    {
        
        Response.Write("<script>window.open ('LingDaoPiShi.aspx?gwid=" + Request["gwid"] + "', 'newwindow', 'height=1, width=1, top=1,left=1, toolbar=no, menubar=no, scrollbars=yes, resizable=no,location=no, status=no');</script>");
    }
}
