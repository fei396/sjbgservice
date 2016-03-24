using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using gwxxService;
using System.Text;

public partial class AddNew : System.Web.UI.Page
{
    gwxxWebService _s = new gwxxWebService();
    //youjianService y = new youjianService();
    private StringBuilder _sb = new StringBuilder();//用来获取上传时出错信息
    protected void Page_Load(object sender, EventArgs e)
    {
        //判断用户是否合法
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }
        if (user.RoleID != 20)
        {
            Response.Redirect("error.aspx?errCode=不能打开该页面");
        }

        if (!IsPostBack)
        {
            //初始化页面
            InitPage(user.BuMenID);
            
        }
    }

    private void InitPage(int buMenId)
    {

        //设置webservice传输header格式
        _s.SjbgSoapHeaderValue = Security.getSoapHeader();
        Security.SetCertificatePolicy();


        //绑定公文类型
        GongWenLeiXing[] gwlx = _s.getLeiXing();
        ddlLeiXing.DataSource = gwlx;
        ddlLeiXing.DataTextField = "LXMC";
        ddlLeiXing.DataValueField = "LXID";
        //ddlLeiXing.Items.Clear();
        ddlLeiXing.DataBind();
        ddlLeiXing.SelectedValue = "1";//默认选择路局文
        
        //for (int i = 0; i < gwlx.Length; i++)
        //{
        //    ListItem li = new ListItem(gwlx[i].LXMC, gwlx[i].LXID.ToString());
        //    ddlLeiXing.Items.Add(li);
        //}

        //绑定公文性质
        GongWenXingZhi[] gwxz = _s.getXingZhi();
        //ddlXingZhi.Items.Clear();
        ddlXingZhi.DataSource = gwxz;
        ddlXingZhi.DataTextField = "XZMC";
        ddlXingZhi.DataValueField = "XZID";
        ddlXingZhi.DataBind();
        //for (int i = 0; i < gwxz.Length; i++)
        //{
        //    ListItem li = new ListItem(gwxz[i].XZMC, gwxz[i].XZID.ToString());
        //    ddlXingZhi.Items.Add(li);
        //}

        //绑定送阅领导
        GongWenYongHu[] lingdao = _s.getLingDao(new int[]{21});
        ddlLingDao.DataSource = lingdao;
        ddlLingDao.DataTextField = "XingMing";
        ddlLingDao.DataValueField = "GongHao";
        ddlLingDao.DataBind();
        if (buMenId == 5) //党群办公文处理员
        {
            ddlXingZhi.SelectedValue = "2";
            ddlLingDao.SelectedValue = "0002";
        }
        else
        {
            ddlLeiXing.SelectedValue = "1";
            ddlLingDao.SelectedValue = "0001";
        }
        //ddlLingDao.Items.Clear();
        //for (int i = 0; i < lingdao.Length; i++)
        //{
        //    ListItem li = new ListItem(lingdao[i].NiCheng + "(" + lingdao[i].XingMing + ")", lingdao[i].GongHao);
        //    ddlLingDao.Items.Add(li);
        //}

        //设置文本框内容初始化
        txtBt.Text = "";
        txtFwdw.Text = "";
        txtHt.Text = "";
        txtWh.Text = "";
        txtYj.Text = "";
        txtZw.Text = "";
        ddlLeiXing_SelectedIndexChanged(null,null);
    }



    protected void AddButton_Click(object sender, EventArgs e)
    {
        //判断用户是否合法
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }

        //获取附件信息
        string[] files =UploadFile();
        if (files == null)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "uploadError", "alert('上传文件出错!')", true);
            return;
        }

        //获取公文相关内容
        int uid = Convert.ToInt32(user.GongHao);
        string ht = txtHt.Text;
        string dw = txtFwdw.Text;
        string bt = txtBt.Text;
        string wh = txtWh.Text;
        string zw = txtZw.Text;
        int lxid = Convert.ToInt32(ddlLeiXing.SelectedValue);
        int xzid = Convert.ToInt32(ddlXingZhi.SelectedValue);
        List<string> jsrList  = new List<string>();
        if (lxid == 1) //路局文
        {
            jsrList.Add(ddlLingDao.SelectedValue);
        }
        else // if (lxid == 2)//段发文
        {
            foreach (ListItem item in  lbBuMen.Items)
            {
                if (item.Selected)
                {
                    jsrList.Add(item.Value);
                }
            }
        }
        string yj = txtYj.Text;
        string[] jsr = jsrList.ToArray();
        string jinji = ddlJinJi.SelectedValue;
        //调用web服务添加公文
        INT i = _s.addNewGongWen2016(uid, ht, dw, wh, bt, zw, yj, xzid, lxid, jinji, pbModule.getIP(), jsr, files);
        if (i.Number != 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "发布公文出错", "alert('" + i.Message + "')", true);
            return;
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "发布公文成功", "alert('发布公文成功')", true);
            InitPage(user.BuMenID);
        }
    }

    /// <summary>
    /// 获取javascript控件上传的附件
    /// </summary>
    /// <returns></returns>
    private string[] UploadFile()
    {
        HttpFileCollection files = HttpContext.Current.Request.Files;//获取上传控件的个数  
        if (HaveFile(files))//存在上传文件  
        {
            if (FindError(files))//上传文件不存在错误  
            {
                bool flag = false;
                string dirPath = Server.MapPath(@"~/gwfj");//上传文件夹路径  
                if (!Directory.Exists(dirPath))//不存在上传文件夹  
                {
                    Directory.CreateDirectory(dirPath);//创建文件夹  
                }
                string[] filenames = new string[files.Count];
                for (int i = 0; i < files.Count; i++)//遍历上传控件  
                {
                    string path = files[i].FileName;//上传文件路径  
                    string fileName = Path.GetFileName(path);//获取文件名
                    filenames[i] = fileName;
                    string savePath = dirPath + @"/" + fileName;//上传文件路径  
                    if (path != "")
                    {
                        try
                        {
                            files[i].SaveAs(savePath);
                            flag = false;
                        }
                        catch (Exception ex)
                        {
                            flag = true;
                        }
                    }
                }
                if (flag)//上传文件时出错  
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "uploadError", "alert('上传文件出错!')", true);
                    return null;
                }
                else
                {
                    //Page.ClientScript.RegisterStartupScript(GetType(), "uploadSuccess", "alert('上传文件成功!')", true);
                    return filenames;
                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "fileError", string.Format("fileError('{0}')", _sb.ToString()), true);
                return null;
            }
        }
        else
        {
            //Page.ClientScript.RegisterStartupScript(GetType(), "haveNoFile", "alert('请选择上传文件!')", true);
            return null;
        }
        
    }
    
    
    /// <summary>  
    /// 判断是否有上传文件  
    /// </summary>  
    /// <param name="files"></param>  
    /// <returns></returns>  
    private bool HaveFile(HttpFileCollection files)
    {
        bool flag = false;
        for (int i = 0; i < files.Count; i++)
        {
            string path = files[i].FileName;//上传文件路径  
            if (path != "") { flag = true; break; }//存在上传文件  
        }
        return flag;
    }
    
    
    /// <summary>  
    ///  判断上传文件是否有错  
    /// 上传文件有错return false  
    /// 上传文件没有错return true  
    /// </summary>  
    /// <param name="files">上传控件</param>  
    /// <returns></returns>  
    private bool FindError(HttpFileCollection files)
    {
        bool flag = false;
        for (int i = 0; i < files.Count; i++)//遍历上传控件  
        {
            string path = files[i].FileName;//上传文件路径  
            if (path != "")//上传控件中存在上传文件  
            {
                string fileName = Path.GetFileName(path);//获取上传文件名  
                string fex = Path.GetExtension(path);//获取上传文件的后缀名  
                if (files[i].ContentLength / 102400 > 1024)//上传文件大于4M  
                {
                    _sb.Append(fileName + "的大小超过100M!");
                    break;
                }
                if (fex == ".exe" || fex == ".EXE") //上传文件是exe文件  
                {
                    _sb.Append(fileName + "的格式不正确!");
                    break;
                }
            }
        }
        if (_sb.ToString() == "") { flag = true; }//上传文件没有错误  
        return flag;
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }
        InitPage(user.BuMenID);
    }

    protected void ddlLeiXing_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Convert.ToInt32(ddlLeiXing.SelectedValue) == 1) //路局文
        {
            ddlLingDao.Visible = true;
            lbBuMen.Visible = false;
            
        }
        else
        {
            ddlLingDao.Visible = false;
            lbBuMen.Visible = true;
            GongWenYongHu user = Session["user"] as GongWenYongHu;
            if (user == null)
            {
                Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
                return;
            }
            int uid = Convert.ToInt32(user.GongHao);
            
            BuMenFenLei[] bmfl = _s.getBuMenFenLei(uid, user.RoleID);
            lbBuMen.Items.Clear();
            foreach (BuMenFenLei fenLei in bmfl)
            {
                foreach (GongWenBuMenRenYuan ry in fenLei.RenYuan)
                {
                    lbBuMen.Items.Add(new ListItem(ry.XianShiMingCheng,ry.GongHao));
                }
            }
        }
    }
}


