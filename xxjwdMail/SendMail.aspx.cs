using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASPNETAJAXWeb.AjaxMail;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace AjaxMail
{
    public partial class SendMail : System.Web.UI.Page
    {
        string mailID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["yhgh"] == null)
            {
                Response.Write("<script language=javascript>alert('对不起，请先登录！')</script>");
                Response.Redirect("Login.aspx");
            }



            //群邮件列表初始化
            if (!Page.IsPostBack)
            {
                BindPageDataGroupListget(Int32.Parse(Session["yhgh"].ToString()));

               /* if (Session["tbTo"] != null)
                {
                    tbTo.Text = Session["tbTo"].ToString();
                }
                if (Session["tbCC"] != null)
                {
                    tbCC.Text = Session["tbCC"].ToString();
                }
                if (Session["tbTitle"] != null)
                {
                    tbTitle.Text = Session["tbTitle"].ToString();
                }
                 if (Session["tbBody"] != null)
                {
                    tbBody.Text = Session["tbBody"].ToString();
                }*/
                 Label1.Text = "发新邮件";
                 Button2.Visible = true;
                 Button4.Visible = true;
                 GroupList.Visible = true;
                 xzq_label.Visible = true;


                 //判断mailid是否为空，不为空将信息写入相应控件
                 if (Request.QueryString["mailid"] != null)
                 {
                     //根据id号查找到邮件
                     mailID = Request.QueryString["mailid"].ToString();
                     Mail mail = new Mail();
                     DataSet dr = mail.GetSingleMail(Int32.Parse(Request.QueryString["mailid"].ToString()));
                     if (dr == null) return;
                     if (dr.Tables[0].Rows.Count > 0)
                     {

                         tbTitle.Text = dr.Tables[0].Rows[0]["Title"].ToString();
                         tbBody.Text = dr.Tables[0].Rows[0]["Body"].ToString();
                         //Session["mailid"] = dr.Tables[0].Rows[0]["id"].ToString();
                         // tbTo.Text = dr.Tables[0].Rows[0]["FromAddress"].ToString();
                         //获取邮件的附件
                         Label1.Text = "转发邮件，原发件人：" + dr.Tables[0].Rows[0]["fromaddress"].ToString();
                         //gvAttachment.Visible = true;
                         fj_listbox.Visible = true;
                         del_btn.Visible = true;
                         DataTable dt = new DataTable();
                         dt.Columns.Add("filenamestring", typeof(string));
                         dt.Columns.Add("urlstring", typeof(string));
                         string split = ";";//多个附件分隔符
                         string[] toFileNameList = (dr.Tables[0].Rows[0]["fileNameString"].ToString()).Split(split.ToCharArray());//将多个附件实际名称写入数组
                         string[] toUrlList = (dr.Tables[0].Rows[0]["url"].ToString()).Split(split.ToCharArray());//将多个附件上传后名称名称写入数组   
                         for (int j = 0; j < toFileNameList.Length; j++)
                         {
                             // DataRow row = dt.NewRow();
                             // row["filenamestring"] = toFileNameList[j];
                             // row["urlstring"] = "MailAttachments/" + toUrlList[j];

                             // dt.Rows.Add(row);
                             ListItem item = new ListItem();
                             //fj_listbox.Items.Add(toFileNameList[j]);
                             item.Text = toFileNameList[j];
                             item.Value = toUrlList[j];
                             fj_listbox.Items.Add(item);
                         }
                         //gvAttachment.DataSource = dt;
                         // gvAttachment.DataBind();
                     }
                 }



            }


            //处理OtherShouJian页面传过来的值。
            if (Context.Handler is OtherShouJian)
            {
                OtherShouJian other;
                other = (OtherShouJian)Context.Handler;
                string zhi, zhi1,zhi2;
                zhi = other.name;
                zhi1 = other.id;
                zhi2 = other.mailid;
                if (zhi == "")
                {
                    tbTo.Text = "请先增加收件人";
                    tbCC.Text = "";
                }
                else
                {
                    if (tbTo.Text == "请先增加收件人")
                    { tbTo.Text = zhi; }
                    else 
                    { tbTo.Text = tbTo.Text + zhi; }
                    tbCC.Text = tbCC.Text + zhi1;
                }
                       //return;//如果来自人员选择，退出，不执行readmail页面程序



                if (zhi2 != "")
                {
                    //根据id号查找到邮件
                    mailID = zhi2;
                    Mail mail = new Mail();
                    DataSet dr = mail.GetSingleMail(Int32.Parse(zhi2));
                    if (dr == null) return;
                    if (dr.Tables[0].Rows.Count > 0)
                    {

                        tbTitle.Text = dr.Tables[0].Rows[0]["Title"].ToString();
                        tbBody.Text = dr.Tables[0].Rows[0]["Body"].ToString();
                        //Session["mailid"] = dr.Tables[0].Rows[0]["id"].ToString();
                        // tbTo.Text = dr.Tables[0].Rows[0]["FromAddress"].ToString();
                        //获取邮件的附件
                        Label1.Text = "转发邮件，原发件人：" + dr.Tables[0].Rows[0]["fromaddress"].ToString();
                       // gvAttachment.Visible = true;
                        fj_listbox.Visible = true;
                        del_btn.Visible = true;
                       // DataTable dt = new DataTable();
                       // dt.Columns.Add("filenamestring", typeof(string));
                       // dt.Columns.Add("urlstring", typeof(string));
                        string split = ";";//多个附件分隔符
                        string[] toFileNameList = (dr.Tables[0].Rows[0]["fileNameString"].ToString()).Split(split.ToCharArray());//将多个附件实际名称写入数组
                        string[] toUrlList = (dr.Tables[0].Rows[0]["url"].ToString()).Split(split.ToCharArray());//将多个附件上传后名称名称写入数组   
                        for (int j = 0; j < toFileNameList.Length; j++)
                        {
                           // DataRow row = dt.NewRow();
                           // row["filenamestring"] = toFileNameList[j];
                           // row["urlstring"] = "MailAttachments/" + toUrlList[j];

                           // dt.Rows.Add(row);
                            ListItem item = new ListItem();
                            //fj_listbox.Items.Add(toFileNameList[j]);
                            item.Text = toFileNameList[j];
                            item.Value = toUrlList[j];
                            fj_listbox.Items.Add(item);
                        }
                       // fj_listbox.DataSource = dt;
                       // fj_listbox.DataBind();
                       // fj_listbox.DataSource = dt;
                       // fj_listbox.DataValueField = "urlstring";
                       // fj_listbox.DataTextField = "filenamestring";
                       // fj_listbox.DataBind();
                       // gvAttachment.DataSource = dt;
                       // gvAttachment.DataBind();
                    }
                }


           }
                //readmail页面传过来的数据
                if (Request.QueryString["formaddress"] == null)
                {
                    // return;
                }
                else
                {

                    Label1.Text = "回复邮件";
                    //tbTo.Text = Request.QueryString["formaddress"].ToString();
                    string split = " ";//多个邮箱分隔符
                    string[] fromaddList = Request.QueryString["formaddress"].ToString().Trim().Split(split.ToCharArray());
                    tbTo.Text = fromaddList[2].Trim() + "(" + fromaddList[1].Trim() + ");";
                    tbCC.Text = fromaddList[1].Trim() + ";";
                    Button2.Visible = false;
                    Button4.Visible = false;
                    GroupList.Visible = false;
                    xzq_label.Visible = false;
                   // return; //如为回复，不执行转发
                }



                 
        }

        protected void btnCommit_Click(object sender, EventArgs e)
        {
            if (tbTo.Text == "请先增加收件人")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'请增加收件人！\');</script>");
                return;
            }
            if (tbTitle.Text.Trim() == "")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'请输入标题！\');</script>");
                tbTitle.Focus();
                return;
            }
            int size = 100;
            string from = Session["yhbm"].ToString() + " " + Session["yhgh"].ToString() + " " + Session["yhxm"].ToString();//发送人
            string split = ";";//多个邮箱分隔符
            string[] toList = tbCC.Text.Trim().Split(split.ToCharArray());//将多个邮箱收件人写入数组
            //先上传附件，并将附件名称写入字符串中
            string fileNameString = "";
            string url = "";
            string filesavename = "";

            other.CreateFile();//根据年月日创建文件目录
            string lujing,filelujing;
              try
              {
                  lujing = "MailAttachments/" + other.GetLuJing() + "/";
                  filelujing = other.GetLuJing() + "/";

              }
              catch (Exception ex)
              {
                  throw ex;
              }

            HttpFileCollection fileList = HttpContext.Current.Request.Files;
            for (int j = 0; j < fileList.Count; j++)
            {
                HttpPostedFile file2 = fileList[j];
                if (file2.FileName.Length <= 0 || file2.ContentLength <= 0)  //没有选择文件,不上传
                {

                    //continue;
                    // Response.Write("<script>alert('请选择文件！');</script>");//
                   // return;
                }
                else //有文件就上传
                {
                    string filename = DateTime.Now.ToString("hhmmssfff") + "_" + Path.GetFileName(file2.FileName);
                   // string extension = Path.GetExtension(file2.FileName);
                    if (fileNameString == "") { fileNameString = Path.GetFileName(file2.FileName); }
                    else { fileNameString = fileNameString + ";" + Path.GetFileName(file2.FileName); }
                    filesavename = lujing  + filename ;
                    //url = "MailAttachments/";
                    size += file2.ContentLength;
                    if (url == "") { url = filelujing+filename; }
                    else { url = url + ";" + filelujing + filename; }//原为 fileNameString + ";" + filename + extension后缀;
                    string fullPath = Server.MapPath(filesavename);
                    file2.SaveAs(fullPath);

                }
              //  mailbox.AddMailAttachment(Path.GetFileName(file2.FileName), url, mailID);
            }
            if (fileNameString != "") { fileNameString= fileNameString+";";}
            if (url != "") { url = url+";"; }
            //结束上传文件
            //如果是转发的，把原有的附件加上
            if (fj_listbox.Visible == true)
            {
                for (int v = 0; v < fj_listbox.Items.Count; v++)
                {
                    url = url + fj_listbox.Items[v].Value+";";
                    fileNameString = fileNameString + fj_listbox.Items[v].Text+";";
                }
            }

            //结束转发附件上传
            //开始上传数据
            size += tbTitle.Text.Length;
            size += tbBody.Text.Length;
            for (int i = 0; i < toList.Length-1; i++)
            {
                if (toList[i].Trim() != "")
                {
                    try
                    {
                        Mail mailbox = new Mail();
                        int mailID = mailbox.AddMail(tbTitle.Text.Trim(), tbBody.Text, from, toList[i].Trim(), HtmlCB.Checked, size, fileNameString, url);
                        if (mailID <= 0) return;
                        Mail dxmess = new Mail();
                        int mailmessage = dxmess.AddMailMessage(from, toList[i].Trim(), tbTitle.Text.Trim());
                        if (mailmessage < 1) return;
                        // mailbox.AddMailAttachment(fileNameString, url, mailID);
                        //tbTo.Text = mailmessage.ToString();                      
                    }
                    catch (Exception ex) { throw new Exception(ex.Message, ex); }
                }

            } //结束上传数据
            //如果副本选中，保存副本，表Mail中字段copyaddress写入发件人工号
            if (CopyCB.Checked == true)
            {
                Mail mailbox = new Mail();
                int mailID = mailbox.AddCopyMail(tbTitle.Text.Trim(), tbBody.Text, tbTo.Text, Session["yhgh"].ToString(), HtmlCB.Checked, size, fileNameString,url);
                if (mailID <= 0) return;
            }
            //结束副本
            //清空表单数据
            tbTo.Text = "请先增加收件人";
            tbCC.Text = "";
            tbBody.Text = "";
            tbTitle.Text = "";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'邮件成功发送！\');</script>");
        }

        protected void Button2_Click1(object sender, EventArgs e)
        {
           // Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'提示！\');</script>");
            tbTo.Text = "请先增加收件人";
            tbCC.Text = "";
            //TextBox1.Text = "";
            // int mailmessage = mailbox.AddMailMessage(from, "3431");
            // if (mailmessage <= 0) return;
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
           // Response.Redirect("Othershoujian.aspx");
                       //保存用户当前的这个状态,收件人，收件人工号，标题
           /* if (String.IsNullOrEmpty(tbTo.Text) == false || String.IsNullOrEmpty(tbTitle.Text) == false || String.IsNullOrEmpty(tbCC.Text) == false)
            {

                if (String.IsNullOrEmpty(tbTo.Text) == false)
                {
                    Session["tbTo"] = tbTo.Text;
                }
                if (String.IsNullOrEmpty(tbCC.Text) == false)
                {
                    Session["tbCC"] = tbCC.Text;
                }
                if (String.IsNullOrEmpty(tbTitle.Text) == false)
                {
                    Session["tbTitle"] = tbTitle.Text;
                }
                if (String.IsNullOrEmpty(tbBody.Text) == false)
                {
                    Session["tbBody"] = tbBody.Text;
                }*/


                //2009年10月27日下午 15：44分保存图片和附件上传控件的路径.

                /*                if (String.IsNullOrEmpty(TxtShouJian.Text) == false)
                                {
                                    Session["textboxshoujian"] = TxtShouJian.Text.Trim();
                                    Session["textboxweiyi"] = TxtWeiYi.Text.Trim();
                                }   */

          //  }
            //if (gvAttachment.Visible == true)
           // { Server.Transfer("Othershoujian.aspx?mailid=" + mailID); }
            //else
           // {
                Server.Transfer("Othershoujian.aspx"); 
            //}
        }

        private void BindPageDataGroupListget(int gh)
        {
            Group group = new Group();
            DataSet ds = group.GetGroups(gh);
            if (ds == null) return;
            GroupList.DataSource = ds;
            GroupList.DataValueField = "id";
            GroupList.DataTextField = "GroupName";
            GroupList.DataBind();
            GroupList.Items.Insert(0, new ListItem("请选择群", "0"));

        }

        protected void GroupList_SelectedIndexChanged(object sender, EventArgs e)
        {

           // tbTo.Text = "sdf";
            //return;
            if (GroupList.SelectedItem.Value == "0")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'请选择群！\');</script>");
                return;
            }
            Group group = new Group();
            DataSet ds =group.GetSingleGroup(int.Parse(GroupList.SelectedItem.Value));
            
            if (ds == null) return;
           /* if (tbTo.Text == "请先增加收件人")
            {
                tbTo.Text = ds.Tables[0].Rows[0]["GroupMember"].ToString();
                // 工号写入另外的tbCC文本框中
                tbCC.Text = "";
            }
            else
            {
                tbTo.Text =tbTo.Text+ds.Tables[0].Rows[0]["GroupMember"].ToString();
            }   */
            tbTo.Text = ds.Tables[0].Rows[0]["GroupMember"].ToString();
                // 工号写入另外的tbCC文本框中
            tbCC.Text = "";
            string split = ";";//多个邮箱分隔符
            string[] toList = tbTo.Text.Trim().Split(split.ToCharArray());//将多个邮箱收件人写入数组
            for (int i = 0; i < toList.Length - 1; i++)
            {
                try
                {
                    if (tbCC.Text == "")
                    { tbCC.Text =toList[i].Trim().Substring(0, 4); }
                    else
                    { tbCC.Text = tbCC.Text +";"+ toList[i].Trim().Substring(0, 4); }

                }

                catch (Exception ex) { throw new Exception(ex.Message, ex); }
            }
            if(tbCC.Text!=""){tbCC.Text=tbCC.Text+";";}
             
        }

        protected void del_btn_Click(object sender, EventArgs e)
        {
            if (fj_listbox.SelectedItem!= null)
            {
                fj_listbox.Items.RemoveAt(fj_listbox.SelectedIndex);
            }
            if (fj_listbox.Items.Count == 0)
            { return; }
        }

        public string mailid
        {
            get
            {
                return mailID;
            }
        }

    }
}