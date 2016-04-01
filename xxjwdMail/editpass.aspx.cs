using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASPNETAJAXWeb.AjaxMail;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;

namespace AjaxMail
{
    public partial class editpass : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["yhgh"] == null)
            {
                Response.Write("<script language=javascript>alert('对不起，请先登录！')</script>");
                Response.Redirect("Login.aspx");
            }
            if (Request.QueryString["lx"] == null || Request.QueryString["lx"] == "")
            { return; }
            else
            { 
   
            //  Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'您的用户名和密码一样，请修改！\');</script>");
            Response.Write("<script language=javascript>alert('您的用户名和密码一样，为了安全请修改！')</script>");         }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
		string md5str;
       // Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'密码修改成功！\');</script>");
        if (xmm_txt.Text!=qrmm_txt.Text)
        {
             Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'两次输入的密码不一致！\');</script>");
            xmm_txt.Text = "";
            qrmm_txt.Text = "";
            return;
        }
        
        if (xmm_txt.Text== "")
        {
            Response.Write("<script language=javascript>alert('密码不能为空！')</script>");
             // Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'密码不能为空！\');</script>");        
            xmm_txt.Focus();
            return;
        }

        if (xmm_txt.Text== Session["yhgh"].ToString())
        {
             Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'密码和工号不能一样！\');</script>");
            xmm_txt.Focus();
            return;
        }
		md5str=System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Session["yhgh"]+ (xmm_txt.Text.Trim()), "MD5");
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "update dic_user set user_mima=@md5str where user_no=@yhgh";
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@md5str", SqlDbType.VarChar,300);
                cmd.Parameters.Add("@yhgh", SqlDbType.Int,4);
                cmd.Parameters[0].Value = md5str;
                cmd.Parameters[1].Value = Session["yhgh"];
                //int result = -1;
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    Response.Write("<script language=javascript>alert('密码修改成功！')</script>");
                   // Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'密码修改成功！\');</script>");
                    if (Request.QueryString["lx"] == null || Request.QueryString["lx"] == "")
                    { Response.Redirect("SendMail.aspx"); }
                    else  //不为空时
                    {
                        Response.Redirect("default.aspx");
                    }

                }
                catch (Exception ex) {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'数据库异常！\');</script>");
                }
                finally { con.Close(); }
        }

        protected void ymm_Txt_TextChanged(object sender, EventArgs e)
        {

        }
    }
}