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
using System.Diagnostics;
using System.IO;
using gwxxService;
using System.Text;

public partial class AddNew : System.Web.UI.Page
{
    gwxxWebService s = new gwxxWebService();
    //youjianService y = new youjianService();
    private StringBuilder sb = new StringBuilder();//用来获取上传时出错信息
    protected void Page_Load(object sender, EventArgs e)
    {
        //判断用户是否合法
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }


        if (!IsPostBack)
        {
            //初始化页面
            initPage();
            
        }
    }
    void initPage()
    {

        //设置webservice传输header格式
        s.SjbgSoapHeaderValue = Security.getSoapHeader();
        Security.SetCertificatePolicy();


        //绑定公文类型
        GongWenLeiXing[] gwlx = s.getLeiXing();
        ddlLeiXing.DataSource = gwlx;
        ddlLeiXing.DataTextField = "LXMC";
        ddlLeiXing.DataValueField = "LXID";
        //ddlLeiXing.Items.Clear();
        ddlLeiXing.DataBind();
        
        
        //for (int i = 0; i < gwlx.Length; i++)
        //{
        //    ListItem li = new ListItem(gwlx[i].LXMC, gwlx[i].LXID.ToString());
        //    ddlLeiXing.Items.Add(li);
        //}

        //绑定公文性质
        GongWenXingZhi[] gwxz = s.getXingZhi();
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
        GongWenYongHu[] lingdao = s.getLingDao(new int[]{21});
        ddlLingDao.DataSource = lingdao;
        ddlLingDao.DataTextField = "XingMing";
        ddlLingDao.DataValueField = "GongHao";
        ddlLingDao.DataBind();
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
        
    }



    protected void AddButton_Click(object sender, EventArgs e)
    {
        //判断用户是否合法
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        //获取附件信息
        string[] files =uploadfile();
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
        string yj = txtYj.Text;
        string jsr = ddlLingDao.SelectedValue;
        
        //调用web服务添加公文
        INT i = s.addNewGongWen2016(uid, ht, dw, wh, bt, zw, yj, xzid, lxid, pbModule.getIP(), jsr, files);
        if (i.Number != 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "发布公文出错", "alert('" + i.Message + "')", true);
            return;
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "发布公文成功", "alert('发布公文成功')", true);
            initPage();
        }
    }

    /// <summary>
    /// 获取javascript控件上传的附件
    /// </summary>
    /// <returns></returns>
    private string[] uploadfile()
    {
        HttpFileCollection files = HttpContext.Current.Request.Files;//获取上传控件的个数  
        if (haveFile(files))//存在上传文件  
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
                Page.ClientScript.RegisterStartupScript(GetType(), "fileError", string.Format("fileError('{0}')", sb.ToString()), true);
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
    private bool haveFile(HttpFileCollection files)
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
                if (files[i].ContentLength / 4096 > 1024)//上传文件大于4M  
                {
                    sb.Append(fileName + "的大小超过4M!");
                    break;
                }
                if (fex == ".exe" || fex == ".EXE") //上传文件是exe文件  
                {
                    sb.Append(fileName + "的格式不正确!");
                    break;
                }
            }
        }
        if (sb.ToString() == "") { flag = true; }//上传文件没有错误  
        return flag;
    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        initPage();
    }
}


