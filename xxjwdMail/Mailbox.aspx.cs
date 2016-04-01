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
    public partial class Mailbox : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["yhgh"] == null)
            {
                Response.Write("<script language=javascript>alert('对不起，请先登录！')</script>");
                Response.Redirect("Login.aspx");
            }
            if (!Page.IsPostBack)
            {
                BindPageData(Int32.Parse(Session["yhgh"].ToString()));
            }
        }
        private void BindPageData(int mailboxID)
       {
          Mail mail=new Mail();
          DataSet ds=mail.GetMailByMailBox(mailboxID);
          gvMailbox.DataSource=ds;
          gvMailbox.DataBind();
          yjzslbl.Text = gvMailbox.Rows.Count.ToString();
        }

        protected void gvMailbox_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMailbox.PageIndex = e.NewPageIndex;
            BindPageData(Int32.Parse(Session["yhgh"].ToString()));
        }

        protected void btnTag_Click(object sender, EventArgs e)
        {
            //Tag tag = new Tag();

            //遍历邮箱记录，集中删除
            foreach (GridViewRow row in gvMailbox.Rows)
            {   ///找到CheckBox控件
                CheckBox cbMail = (CheckBox)row.FindControl("cbMail");
                if (cbMail == null) continue;
                if (cbMail.Checked == true)
                {   ///凡是选中的一块进入垃圾箱，标记邮件
                    ///
                    Mail mail=new Mail();

                    mail.DustbinMail(int.Parse(gvMailbox.DataKeys[row.RowIndex].Value.ToString()));
                }
            }
            BindPageData(Int32.Parse(Session["yhgh"].ToString()));
        }

        protected void gvMailbox_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "del")
            {
                Mail mail = new Mail();
                if (mail.DustbinMail(Int32.Parse(e.CommandArgument.ToString())) > 0)
                {
                    BindPageData(Int32.Parse(Session["yhgh"].ToString()));
                }
            }
        }

        protected void gvMailbox_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
            if (imgDelete != null)
            {
                imgDelete.Attributes.Add("onclick", "return confirm(\"您确定要删除当前行的邮件吗?\");");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                bool city = bool.Parse(DataBinder.Eval(e.Row.DataItem, "ReadFlag").ToString());
                if (city == false)
                {
                    e.Row.BackColor = System.Drawing.Color.LightPink;
                    e.Row.ForeColor = System.Drawing.Color.Maroon;
                }
            }
        }

        protected void qxckb_CheckedChanged(object sender, EventArgs e)
        {
            //qxckb.Text = "aaa";
            if (qxckb.Checked == false)
            {
               // qxckb.Checked = false;
                qxckb.Text="全选";
                foreach (GridViewRow row in gvMailbox.Rows)
                {   ///找到CheckBox控件
                    CheckBox cbMail = (CheckBox)row.FindControl("cbMail");
                    cbMail.Checked = false;
                }
            }
            else
            {   
               //qxckb.Checked = true;
                qxckb.Text="全消";
                foreach (GridViewRow row in gvMailbox.Rows)
                  {   ///找到CheckBox控件
                      CheckBox cbMail = (CheckBox)row.FindControl("cbMail");
                      cbMail.Checked= true;
                  }
            }

        }

        protected void btnTag1_Click(object sender, EventArgs e)
        {

            //遍历邮箱记录，集中删除
            foreach (GridViewRow row in gvMailbox.Rows)
            {   ///找到CheckBox控件
                CheckBox cbMail = (CheckBox)row.FindControl("cbMail");
                if (cbMail == null) continue;
                if (cbMail.Checked == true)
                {   ///凡是选中的一块进入垃圾箱，标记邮件
                    ///
                    Mail mail = new Mail();

                    mail.DelFlagMail(int.Parse(gvMailbox.DataKeys[row.RowIndex].Value.ToString()));
                }
            }
            BindPageData(Int32.Parse(Session["yhgh"].ToString()));
        }

    }
}