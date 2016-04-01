using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASPNETAJAXWeb.AjaxMail;
using System.Data;
using System.Data.SqlClient;

namespace AjaxMail
{
    public partial class ReadMail : System.Web.UI.Page
    {
        int mailID = -1;
        string fromym = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["yhgh"] == null)
            {
                Response.Write("<script language=javascript>alert('对不起，请先登录！')</script>");
                Response.Redirect("Login.aspx");
            }


           if(Request.Params["MailID"]!=null)
             {
                 mailID=Int32.Parse(Request.Params["MailID"].ToString());
                 if (!Page.IsPostBack && mailID > 0) { BindPageData(mailID); }
             }

           if (Request.QueryString["fromym"] == null) 
           {
               //continue;
           }
           else 
           {
               fromym=Request.QueryString["fromym"].ToString();
               //fjrlbl.Text = fromym;
           }
           if (fromym == "yfsmail")
           {
               fjrlbl.Text = "收件人";
           }
           else { fjrlbl.Text = "发件人"; }
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            Response.Redirect("sendmail.aspx?formaddress=" + tbTo.Text);
        }

        protected void btnCommit_Click(object sender, EventArgs e)
        {
           // Response.Write("<script languange=javascript>history.go(-1);</script>");

           // Response.Redirect(Request.UrlReferrer.ToString());
            if (fromym == "mail")
            {
                Response.Redirect("Mailbox.aspx");
            }
            else if (fromym == "dustibnmail")
            {
                Response.Redirect("DustibnMail.aspx");
            }
            else
            {
                Response.Redirect("yfssendmail.aspx");
            }

           // Response.Redirect("DustibnMailbox.aspx");
          
        }
        private void BindPageData(int mailID)
        {
            Mail mail = new Mail();
            DataSet dr = mail.GetSingleMail(mailID);
            if (dr == null) return;
            if (dr.Tables[0].Rows.Count>0)
            {
                               //将邮件读取标志更改
                if(bool.Parse(dr.Tables[0].Rows[0]["readflag"].ToString())==false)
                {
                   Mail mailread = new Mail();
                   mailread.SetSingleMailRead(mailID);
                }
               // fromadd=
                tbTitle.Text = dr.Tables[0].Rows[0]["Title"].ToString();
                tbBody.Text = dr.Tables[0].Rows[0]["Body"].ToString();
                tbTo.Text = dr.Tables[0].Rows[0]["FromAddress"].ToString();

                //获取邮件的附件

                DataTable dt = new DataTable();
                dt.Columns.Add("filenamestring", typeof(string));
                dt.Columns.Add("urlstring", typeof(string));
                string split = ";";//多个附件分隔符
                string[] toFileNameList = (dr.Tables[0].Rows[0]["fileNameString"].ToString()).Split(split.ToCharArray());//将多个附件实际名称写入数组
                string[] toUrlList = (dr.Tables[0].Rows[0]["url"].ToString()).Split(split.ToCharArray());//将多个附件上传后名称名称写入数组  
               // Label1.Text = toFileNameList.Length.ToString();
                for (int j = 0; j < toFileNameList.Length; j++)
                {
                    DataRow row = dt.NewRow();
                    row["filenamestring"] = toFileNameList[j];
                    row["urlstring"] = "MailAttachments/" + toUrlList[j];

                    dt.Rows.Add(row);
                }
                gvAttachment.DataSource = dt;
                gvAttachment.DataBind();


               // DataSet ds = mail.GetAttachmentByMail(mailID);
                //gvAttachment.DataSource = ds;
               //gvAttachment.DataBind();
            }
        }

        protected void zf_btn_Click(object sender, EventArgs e)
        {
           //清除原session信息

              /* Session["tbTo"]=null;

            Session["tbCC"]= null;

            Session["tbTitle"] = null;

            Session["tbBody"] = null;*/
           // Session["mailid"] = mailID;

            Response.Redirect("sendmail.aspx?mailid=" + mailID);
        }
    }
}