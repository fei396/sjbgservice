using gwxxService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class AddNew : System.Web.UI.Page
{
    readonly gwxxWebService s = new gwxxWebService();
    private StringBuilder _sb = new StringBuilder();//用来获取上传时出错信息

    protected void Page_Load(object sender, EventArgs e)
    {
        
        Security.SetCertificatePolicy();

        int uid = Convert.ToInt32(Session["uid"]);
        int role = Convert.ToInt32(Session["role"]);
        if (uid <= 0)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }
        if (role != 2 && role !=3)
        {
            Response.Redirect("error.aspx?errCode=不能打开该页面");
        }
        if (!IsPostBack)
        {
            gwxxService.BuMenFenLei[] bmfl = s.getTongZhiBuMenFenLei(uid);
            Session["bmfl"] = bmfl;
            List<string> jsr = new List<string>();
            Session["jsr"] = jsr;
            InitPage(uid);
        }
    }

    public void InitPage(int uid)
    {

        TongZhiLeiXing[] tzlx = s.getTongZhiLeiXing(uid);
        ddlLeiXing.DataSource = tzlx;
        ddlLeiXing.DataTextField = "LXMC";
        ddlLeiXing.DataValueField = "LXID";
        ddlLeiXing.DataBind();
        ddlLeiXing.SelectedValue = "1";//默认行政信息

        txtBt.Text = "";
        txtZw.Text = "";
        lbBM.Text = uid.ToString();
        RadioButtonList1.SelectedValue = "是";
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        gvListBuMen.DataSource = bmfl;
        gvListBuMen.DataBind();

    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        int uid = Convert.ToInt32(Session["uid"]);
        //if (uid == null)
        //{
        //    Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        //    return;
        //}
        InitPage(uid);
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        int uid = Convert.ToInt32(Session["uid"]);
        //if (uid == null)
        //{
        //    Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        //    return;
        //}

        string[] files = UploadFile();

        //if (files == null)
        //{
        //    //Page.ClientScript.RegisterStartupScript(GetType(), "uploadError", "alert('上传文件出错!')", true);
        //    //return;
        //}

        string bt = txtBt.Text;
        string zw = txtZw.Text;
        int fbrid = uid;
        int lxid = Convert.ToInt32(ddlLeiXing.SelectedValue);
        string ip = getIP();
        int sfgk = 1;
        if (RadioButtonList1.SelectedValue == "否")
        {
            sfgk = 0;
        }

        List<string> jsrList = Session["jsr"] as List<string>;
        int[] jsrid;
        if (jsrList == null)
        {
            jsrid = null;
        }
        else
        {
            jsrid = new int[jsrList.Count];
            for (int j = 0; j < jsrList.Count; j++)
            {
                jsrid[j] = Convert.ToInt32(jsrList[j]);
            }
        }
        
        INT i = s.addNewTongZhi2016(bt, zw, fbrid, lxid, jsrid, files, ip, sfgk);
        if (i.Number != 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "发布公文出错", "alert('" + i.Message + "')", true);
            return;
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "发布公文成功", "alert('发布公文成功')", true);
            InitPage(uid);
        }
    }

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
    public static string getIP()
    {
        string userIp = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (userIp == null || userIp == "")
        {
            userIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        return userIp;
    }


    private void setCheckBoxListItemSelected()
    {
        List<string> jsr = Session["jsr"] as List<string>;
        if (jsr == null) return;
        foreach (GridViewRow row in gvListBuMen.Rows)
        {
            CheckBoxList cbl = row.FindControl("cbl") as CheckBoxList;
            if (cbl == null) continue;
            foreach (ListItem item in cbl.Items)
            {

                if (jsr.IndexOf(item.Value) != -1)
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
    public gwxxService.GongWenBuMenRenYuan[] GetCKBLDataSource(int index)
    {
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return null;
        return bmfl[index].RenYuan;
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

            foreach (gwxxService.GongWenBuMenRenYuan ry in bmfl[row].RenYuan)
            {
                jsr.Add(ry.GongHao);
            }
        }
        else
        {
            foreach (gwxxService.GongWenBuMenRenYuan ry in bmfl[row].RenYuan)
            {
                jsr.Remove(ry.GongHao);
            }
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
        int row = Convert.ToInt32(strCheck.Substring(strCheck.IndexOf("$") + 4, 2)) - 2;
        string txt;
        if (bmfl[row].FenLeiID == 1) txt = bmfl[row].RenYuan[strIndex].NiCheng;
        else txt = cbl.Items[strIndex].Text;
        if (cbl.Items[strIndex].Selected)
        {
            jsr.Add(cbl.Items[strIndex].Value);
        }
        else
        {
            jsr.Remove(cbl.Items[strIndex].Value);
        }
        Session["jsr"] = jsr;
    }
    protected void gvListBuMen_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowIndex < 0) return;
        BuMenFenLei[] bmfl = Session["bmfl"] as BuMenFenLei[];
        if (bmfl == null) return;
        CheckBoxList cbl = e.Row.FindControl("cbl") as CheckBoxList;
        if (cbl == null) return;
        switch (bmfl[e.Row.RowIndex].FenLeiID)
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
