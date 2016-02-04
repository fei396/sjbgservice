﻿using System;
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
public partial class ViewGongWen : System.Web.UI.Page
{
    gwxxService.gwxxWebService s = new gwxxService.gwxxWebService();
    static int uid, rid,gwid,lzid;
    static gwxxService.BuMenFenLei[] bmfl;
    static System.Collections.Generic.List<string> jsr;
    static System.Collections.Generic.List<int> zdybm;
    protected void Page_Load(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        //uid = Convert.ToInt32(user);

        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        btnQianShou.Attributes["OnClick"] = "return confirm('确定签阅该文件？')";
        btnCuiBan.Attributes["OnClick"] = "return confirm('确定催办未签收该文件的人员？')";
        
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
            }
            //string cj = Session["udept"] as string;
            //SqlDataSource1.SelectCommand = "SELECT    xmmc, cj , rygh, work_name , sfhg , kkid FROM V_CQKK_RYCJB  ";
            //if (cj != "_所有") SqlDataSource1.SelectCommand += " where cj='" + cj + "'";

            //SqlDataSource1.SelectCommand += " order by  xmmc,cj,rygh";
            bmfl= s.getBuMenFenLei(uid,rid);
            if (rid == 21 || rid == 22)
            {
                tableZdybm.Visible = true;
                bumen(uid);
            }
            else
            {
                tableZdybm.Visible = false;
            }
            int type = Convert.ToInt32(Request["type"]);

            
            if (rid == 20)//公文处理员
            {
                tableGuiDang.Visible = true;
                tableQianShou.Visible = false;
                if (type == 1)//已经流转完成
                {
                    btnGuiDang.Visible = true;
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
                tableGuiDang.Visible = false;
                
                if (type == 1 || type ==2)
                {
                    tableQianShou.Visible = false;
                    
                }
                else
                {
                    tableQianShou.Visible = true;
                }
            }
            jsr = new System.Collections.Generic.List<string>();
            zdybm = new System.Collections.Generic.List<int>();
            //s.SjbgSoapHeaderValue = Security.getSoapHeader();
            //Security.SetCertificatePolicy();

            
            getData(gwid, lzid);
        }
    }

    protected string duanyu()
    {
        gwxxService.ZiDingYiDuanYu[] zdydy = s.getZiDingYiDuanYu(uid,false);
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
        gwxxService.ZiDingYiBuMen[] zdybm = s.getZiDingYiBuMen(uid);
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
            lblLeiXing.Text = "公文分类：" + gw.WenJianLeiXing;
            lblYiJian.Text = "呈送意见：" + gw.ChengSongYiJian;
            lblFaWenDanWei.Text = "发文单位：" + gw.FaWenDanWei;
            lblFaWenShiJian.Text = "发布时间：" + gw.FaBuShiJian;
            
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
        gwxxService.GongWenLiuZhuan[] gwlz = s.getLiuZhuanXian(lzid);
        
        gvList.DataSource = gwlz;
        gvList.DataBind();

 
        gvListBuMen.DataSource = bmfl;
        gvListBuMen.DataBind();
    }

    protected gwxxService.BuMenRenYuan[] GetCKBLDataSource(int index)
    {
        return bmfl[index].RenYuan;

    }


    protected void cblZdybm_SelectedIndexChanged(object sender, EventArgs e)
    {
      
        string strCheck = Request.Form["__EVENTTARGET"].ToString();
        int strIndex = Convert.ToInt32(strCheck.Substring(strCheck.LastIndexOf("$") + 1));
        if (this.cblZdybm.Items[strIndex].Selected)
        {
            txtQianShouNeiRong.Text += this.cblZdybm.Items[strIndex].Text.Trim() + "、";
            BuMenRenYuan[] ry = s.getZiDingYiBuMenRenYuan(Convert.ToInt32(this.cblZdybm.Items[strIndex].Value),true);
            if (ry != null)
            {
                foreach(BuMenRenYuan r in ry)
                {
                    jsr.Add(r.GongHao);
                }
            }
        }
        else
        {
            txtQianShouNeiRong.Text = txtQianShouNeiRong.Text.Replace(this.cblZdybm.Items[strIndex].Text.Trim() + "、", "");
            BuMenRenYuan[] ry = s.getZiDingYiBuMenRenYuan(Convert.ToInt32(this.cblZdybm.Items[strIndex].Value), true);
            if (ry != null)
            {
                foreach (BuMenRenYuan r in ry)
                {
                    jsr.Remove(r.GongHao);
                }
            }
            //zdybm.Remove(Convert.ToInt32(this.cblZdybm.Items[strIndex].Value));
        }
        setCheckBoxListItemSelected();

    }
    protected void cbl_SelectedIndexChanged(object sender, EventArgs e)
    {
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

    }
    protected void chb_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chb = (CheckBox)(sender);
        string strCheck = Request.Form["__EVENTTARGET"].ToString();
        int row = Convert.ToInt32(strCheck.Substring(strCheck.IndexOf("$") + 4, 2)) - 2;
        if (chb.Checked)
        {
            txtQianShouNeiRong.Text += chb.Text + "、";
            foreach(gwxxService.BuMenRenYuan ry in bmfl[row].RenYuan)
            {
                jsr.Add(ry.GongHao);
            }
        }
        else
        {
            txtQianShouNeiRong.Text = txtQianShouNeiRong.Text.Replace(chb.Text + "、", "");
            foreach (gwxxService.BuMenRenYuan ry in bmfl[row].RenYuan)
            {
                jsr.Remove(ry.GongHao);
            }
        }
        setCheckBoxListItemSelected();
    }
    protected void btnQianShou_Click(object sender, EventArgs e)
    {
        int[] zdy = zdybm.ToArray();
        string[] jsry = jsr.ToArray();
        string pishi = txtQianShouNeiRong.Text.Trim();
        if (pishi.Equals(""))
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "签收公文失败", "alert('签收公文失败：请输入签阅内容。')", true);
        }
        else
        { 
        INT i= s.signGongWen2016(gwid, lzid, uid, jsry, pishi,zdy);
        if (i.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "签收公文成功", "alert('签收公文成功');window.open('mdefault.aspx','_parent');", true);
            //Response.Redirect("ListGongWen.aspx?type=0");
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "签收公文失败", "alert('签收公文失败：" + i.Message + "')", true);
        }
    }
    }
    protected void btnGuiDang_Click(object sender, EventArgs e)
    {

    }
    protected void btnCuiBan_Click(object sender, EventArgs e)
    {
        INT i= s.makeCuiBan(gwid);
        if (i.Number == 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "催办成功", "alert('催办成功');window.location.href='mdefault.aspx'", true);
            //Response.Redirect("ListGongWen.aspx?type=0");
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "催办失败", "alert('催办失败：" + i.Message + "')", true);
        }
    }
    protected void cbAll_CheckedChanged(object sender, EventArgs e)
    {
        if (cbAll.Checked)
        {
            txtQianShouNeiRong.Text += cbAll.Text + "、";
            foreach (BuMenFenLei b in bmfl)
            {
                if (b.FenLeiID == 1) continue;
                foreach (BuMenRenYuan r in b.RenYuan)
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
                foreach (BuMenRenYuan r in b.RenYuan)
                {
                    jsr.Remove(r.GongHao);
                }
            }
            
        }
        setCheckBoxListItemSelected();
    }
    private void setCheckBoxListItemSelected()
    {
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
}