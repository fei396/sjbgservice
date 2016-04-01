using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace AjaxMail
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //处理主页上传过来的登录信息
            if (Request.QueryString["code"] == null || Request.QueryString["code"]=="")
            { return; }
            else  //不为空时
            {
                string code = System.Text.Encoding.Default.GetString(Convert.FromBase64String(Request.QueryString["code"].ToString().Replace("%2B", "+")));
                string strscript = "", xianshi = "/script>";
                string md5str = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(user_txt.Text.Trim() + pass_txt.Text.Trim(), "MD5");
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "Select * From dat_user,V_Mail_YongHu where gh=V_Mail_YongHu.user_no and gh=@user_txt";
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                da.SelectCommand.Parameters.Add("@user_txt", SqlDbType.VarChar, 20);
                da.SelectCommand.Parameters[0].Value =code;
                DataSet ds = new DataSet();
                try
                {
                    con.Open();
                    da.Fill(ds, "DataTable");
                    if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)  //数据为空
                    {
                        strscript = "<script language=javascript>alert('" + "对不起，您没有通过身份验证！" + "')<";
                        strscript = strscript + xianshi;
                        Response.Write(strscript);
                        user_txt.Text = "";
                        pass_txt.Text = "";
                        return;
                    }
                    //登录成功
                    else
                    {

                        Session["yhgh"] = ds.Tables[0].Rows[0]["gh"].ToString();
                        Session["yhxm"] = ds.Tables[0].Rows[0]["xm"].ToString();
                        Session["yhmm"] = ds.Tables[0].Rows[0]["mm"].ToString();
                        Session["yhid"] = ds.Tables[0].Rows[0]["id"].ToString();
                        Session["yhbm"] = ds.Tables[0].Rows[0]["bm"].ToString();
                        Session["yhpd"] = "24533";
                        // Response.Cookies("yhqx").Value = Server.UrlEncode(Trim(dt.Rows(0).Item("qx").ToString))
                        //Response.Cookies("yhxm").Value = Server.UrlEncode(dt.Rows(0).Item("xm"))
                        // Response.Cookies("yhpd").Value = "24533"
                        Session["yhqx"] = ds.Tables[0].Rows[0]["qx"].ToString();

                        Response.Redirect("default.aspx"); 
                        //Response.Redirect("default.aspx");
                        // tzym = "<script language=javascript>opener.location.href=opener.location.href;<" + xianshi
                        // Response.Write(tzym)*/
                        // Response.Write(Session["yhxm"].ToString());
                    }
                }
                catch (Exception ex)
                { //throw new Exception(ex.Message, ex);
                }
                finally { con.Close(); }
            }
            //结束
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
               string strscript="",xianshi="/script>";
               string md5str = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(user_txt.Text.Trim() + pass_txt.Text.Trim(), "MD5");
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "Select * From dat_user,V_Mail_YongHu where gh=V_Mail_YongHu.user_no and gh=@user_txt and mm=@pass_txt";
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                da.SelectCommand.Parameters.Add("@user_txt", SqlDbType.VarChar, 20);
                da.SelectCommand.Parameters.Add("@pass_txt", SqlDbType.VarChar, 50);
                da.SelectCommand.Parameters[0].Value = user_txt.Text.Trim();
                da.SelectCommand.Parameters[1].Value = md5str;
                DataSet ds = new DataSet();
                try
                {
                    con.Open();
                    da.Fill(ds, "DataTable");
                    if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)  //数据为空
                    {
                          strscript = "<script language=javascript>alert('" + "对不起，您没有通过身份验证！" + "')<";
                          strscript = strscript + xianshi;
                          Response.Write(strscript);
                          user_txt.Text = "";
                          pass_txt.Text = "";
                          return;
                    }
           //登录成功
                    else
                   {

                          Session["yhgh"] = ds.Tables[0].Rows[0]["gh"].ToString();
                          Session["yhxm"] =ds.Tables[0].Rows[0]["xm"].ToString();
                          Session["yhmm"] =ds.Tables[0].Rows[0]["mm"].ToString();
                          Session["yhid"] = ds.Tables[0].Rows[0]["id"].ToString();
                          Session["yhbm"] = ds.Tables[0].Rows[0]["bm"].ToString();
                          Session["yhpd"] = "24533";
                         // Response.Cookies("yhqx").Value = Server.UrlEncode(Trim(dt.Rows(0).Item("qx").ToString))
                         //Response.Cookies("yhxm").Value = Server.UrlEncode(dt.Rows(0).Item("xm"))
                         // Response.Cookies("yhpd").Value = "24533"
                         Session["yhqx"] =ds.Tables[0].Rows[0]["qx"].ToString();
                         if (user_txt.Text.Trim() == pass_txt.Text.Trim())
                         {
                           //  Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'您的用户名和密码一样，请修改！\');</script>");
                             //Response.Write("<script language=javascript>alert('您的用户名和密码一样，请修改！')</script>");
                             Response.Redirect("editpass.aspx?lx=dlxg");
                         }
                         else
                         { Response.Redirect("default.aspx"); }
                         //Response.Redirect("default.aspx");
                         // tzym = "<script language=javascript>opener.location.href=opener.location.href;<" + xianshi
                         // Response.Write(tzym)*/
                        // Response.Write(Session["yhxm"].ToString());
                 }
              }
              catch (Exception ex) 
              { //throw new Exception(ex.Message, ex);
              }
              finally { con.Close(); }   
        }

        protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
        {
            user_txt.Text = "";
            pass_txt.Text = "";
        }
    }
}