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
    public partial class MailTree : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["yhgh"] == null)
            {
                Response.Write("<script language=javascript>alert('对不起，请先登录！')</script>");
                Response.Redirect("Login.aspx");
            }

         // if(!Page.IsPostBack){BindPageData();}

        }
        private void BindPageData()
        {
           /*Mail mail = new Mail();
            DataSet ds = mail.GetMailboxs();
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0) return;
           //显示邮箱
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                TreeNode node = new TreeNode();
                node.Value = row["ID"].ToString();
                node.Text = row["name"].ToString();
                node.Target = "Mailbox";
                node.NavigateUrl = row["url"].ToString();
                OperationView.Nodes[0].ChildNodes.Add(node);
            }*/
        }
    }
}