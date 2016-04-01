using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Configuration;

namespace ASPNETAJAXWeb.AjaxMail
{
    public class Person
    {
        public DataSet GetBumen()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            String cmdText = "select * from dic_bumen order by px";
            SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                da.Fill(ds, "DataTable");
            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            finally { con.Close(); }
            return ds;
        }

        public DataSet GetPersonByBmid(int bmID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            // string cmdText = "select M.*,(select count(*) from attachment as a where a.mailid=m.id) as attachmentcount from mail as M where mailboxid=@MailboxID order by createdata desc";
            string cmdText = "select * from dic_user,V_Mail_YongHu where dic_user.user_no=V_Mail_YongHu.user_no and bumen_id=@bmID";
            SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
            da.SelectCommand.Parameters.Add("@bmID", SqlDbType.Int, 4);
            da.SelectCommand.Parameters[0].Value = bmID;
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                da.Fill(ds, "DataTable");

            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            finally { con.Close(); }
            return ds;
        }

        public DataSet GetPersonByGh(int gh)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            // string cmdText = "select M.*,(select count(*) from attachment as a where a.mailid=m.id) as attachmentcount from mail as M where mailboxid=@MailboxID order by createdata desc";
            string cmdText = "select * from dic_user,V_Mail_YongHu where dic_user.user_no=V_Mail_YongHu.user_no and dic_user.user_no=@gh";
            SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
            da.SelectCommand.Parameters.Add("@gh", SqlDbType.Int, 4);
            da.SelectCommand.Parameters[0].Value = gh;
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                da.Fill(ds, "DataTable");

            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            finally { con.Close(); }
            return ds;
        }

        public int EditPass(string mima,int gh)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            SqlConnection con = new SqlConnection(connectionString);
            String cmdText = "update dic_user set user_mima=@mima where user_no=@gh";
            SqlCommand cmd = new SqlCommand(cmdText, con);
           cmd.Parameters.Add("@mima", SqlDbType.VarChar, 200);
            cmd.Parameters.Add("@gh", SqlDbType.Int, 4);
            cmd.Parameters[0].Value = mima;
            cmd.Parameters[1].Value = gh;
            int result = -1;
            try
            {
                con.Open();
                result = cmd.ExecuteNonQuery();

            }
            catch (Exception ex) { throw new Exception(ex.Message, ex); }
            finally { con.Close(); }
            return result;
        }


    }
}