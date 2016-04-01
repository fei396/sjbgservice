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
    public class Mail
    {       
            public DataSet GetMails()
          {
              string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
              SqlConnection con = new SqlConnection(connectionString);
              String cmdText = "select * from Mail";
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

            public DataSet GetMailboxs()
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                String cmdText = "select * from Mailbox";
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
            public DataSet GetMailByMailBox(int mailboxID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
               // string cmdText = "select M.*,(select count(*) from attachment as a where a.mailid=m.id) as attachmentcount from mail as M where mailboxid=@MailboxID order by createdata desc";
                string cmdText = "select * from mail_VIEW where toaddress=@MailboxID and dustbin=0 and delflag=0 order by id desc";
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                da.SelectCommand.Parameters.Add("@MailboxID", SqlDbType.Int, 4);
                da.SelectCommand.Parameters[0].Value = mailboxID;
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
            public DataSet GetSingleMail(int mailID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "select * from Mail where id=@ID";
               // SqlCommand cmd = new SqlCommand(cmdText, con);
                //cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                //cmd.Parameters[0].Value = mailID;
                //DataTable dd=new DataTable();
                //SqlDataReader dr;
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                da.SelectCommand.Parameters.Add("@ID", SqlDbType.Int, 4);
                da.SelectCommand.Parameters[0].Value = mailID;
                DataSet ds = new DataSet();
                try
                {
                    con.Open();
                    //dr =cmd.ExecuteReader(CommandBehavior.CloseConnection) ;
                    //dd=cmd.ExecuteReader();
                    da.Fill(ds, "DataTable");
                }
                catch (Exception ex) { throw new Exception(ex.Message, ex); }
                finally { con.Close(); }
                return ds;
            }
            public DataSet GetYfsMailByMailBox(int mailboxID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                // string cmdText = "select M.*,(select count(*) from attachment as a where a.mailid=m.id) as attachmentcount from mail as M where mailboxid=@MailboxID order by createdata desc";
                string cmdText = "select * from mail where copyaddress=@MailboxID and dustbin=0 and delflag=0 order by id desc";
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                da.SelectCommand.Parameters.Add("@MailboxID", SqlDbType.Int, 4);
                da.SelectCommand.Parameters[0].Value = mailboxID;
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
            public DataSet GetDustibnMailByMailBox(int mailboxID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                // string cmdText = "select M.*,(select count(*) from attachment as a where a.mailid=m.id) as attachmentcount from mail as M where mailboxid=@MailboxID order by createdata desc";
                string cmdText = "select * from mail where toaddress=@MailboxID and dustbin=1 and delflag=0 order by id desc";
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                da.SelectCommand.Parameters.Add("@MailboxID", SqlDbType.Int, 4);
                da.SelectCommand.Parameters[0].Value = mailboxID;
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
            public int AddMail(string title,string body,string fromAddress,string toAddress,bool isHtmlFormat,int size,string fileNameString,string url)
            {

                //获取IP
                HttpRequest request = HttpContext.Current.Request;
                string fwip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(fwip))
                {
                    fwip = request.ServerVariables["REMOTE_ADDR"];
                }
                //结束
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "insert into mail(title,body,fromaddress,toaddress,ishtmlformat,createdate,size,status,fileNameString,url,sendip)"
                    + "values(@Title,@Body,@FromAddress,@ToAddress,@ISHtmlFormat,getdate(),@Size,0,@fileNameString, @url,@sendip) set @ID=@@Identity";//identity返回最后更新的id值
                SqlCommand cmd = new SqlCommand(cmdText,con);
                cmd.Parameters.Add("@Title", SqlDbType.VarChar,200);
                cmd.Parameters.Add("@Body", SqlDbType.Text);
                cmd.Parameters.Add("@FromAddress", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@ToAddress", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@ISHtmlFormat", SqlDbType.Bit, 1);
                cmd.Parameters.Add("@Size", SqlDbType.Int, 4);
                cmd.Parameters.Add("@fileNameString", SqlDbType.VarChar,1000);
                cmd.Parameters.Add("@url", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@sendip", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = title;
                cmd.Parameters[1].Value = body;
                cmd.Parameters[2].Value = fromAddress;
                cmd.Parameters[3].Value = toAddress; 
                cmd.Parameters[4].Value = isHtmlFormat;
                cmd.Parameters[5].Value = size;
                cmd.Parameters[6].Value = fileNameString;
                cmd.Parameters[7].Value = url;
                cmd.Parameters[8].Value = fwip;
                cmd.Parameters[9].Direction = ParameterDirection.Output;
                int result = -1;
                try
                {
                    con.Open();
                    result = cmd.ExecuteNonQuery();

                }
                catch (Exception ex) { throw new Exception(ex.Message, ex); }
                finally { con.Close(); }
                return (int)cmd.Parameters[9].Value;
            }
            public int AddMailMessage(string fromAddress, string toAddress,string mailbt)
            {

                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices18"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "insert into DataExchange(Data_NO,Data_Content)"
                    + "values(@data_no,@data_content)";//identity返回最后更新的id值
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@data_no", SqlDbType.VarChar, 200);
                cmd.Parameters.Add("@data_content", SqlDbType.VarChar,300);
                cmd.Parameters[0].Value = toAddress;
                cmd.Parameters[1].Value = fromAddress+"给您发了一封邮件，标题:"+mailbt+"，请查收！";
               // cmd.Parameters[2].Direction = ParameterDirection.Output;
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

            public int AddCopyMail(string title, string body, string fromAddress, string toAddress, bool isHtmlFormat, int size, string fileNameString, string url)
            {

                //获取IP
                HttpRequest request = HttpContext.Current.Request;
                string fwip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(fwip))
                {
                    fwip = request.ServerVariables["REMOTE_ADDR"];
                }
                //结束
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "insert into mail(title,body,fromaddress,copyaddress,ishtmlformat,createdate,size,status,fileNameString,url,sendip)"
                    + "values(@Title,@Body,@FromAddress,@ToAddress,@ISHtmlFormat,getdate(),@Size,0,@fileNameString, @url,@sendip) set @ID=@@Identity";//identity返回最后更新的id值
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@Title", SqlDbType.VarChar, 200);
                cmd.Parameters.Add("@Body", SqlDbType.Text);
                cmd.Parameters.Add("@FromAddress", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@ToAddress", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@ISHtmlFormat", SqlDbType.Bit, 1);
                cmd.Parameters.Add("@Size", SqlDbType.Int, 4);
                cmd.Parameters.Add("@fileNameString", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@url", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@sendip", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = title;
                cmd.Parameters[1].Value = body;
                cmd.Parameters[2].Value = fromAddress;
                cmd.Parameters[3].Value = toAddress;
                cmd.Parameters[4].Value = isHtmlFormat;
                cmd.Parameters[5].Value = size;
                cmd.Parameters[6].Value = fileNameString;
                cmd.Parameters[7].Value = url;
                cmd.Parameters[8].Value = fwip;
                cmd.Parameters[9].Direction = ParameterDirection.Output;
                int result = -1;
                try
                {
                    con.Open();
                    result = cmd.ExecuteNonQuery();

                }
                catch (Exception ex) { throw new Exception(ex.Message, ex); }
                finally { con.Close(); }
                return (int)cmd.Parameters[9].Value;
            }

            public int DustbinMail(int mailID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "update mail set dustbin=1 where id=@ID";
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = mailID;
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
            public int DelFlagMail(int mailID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "update mail set delflag=1 where id=@ID";
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = mailID;
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
            public int DeleteMail(int mailID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "delete mail where id=@ID";
                SqlCommand cmd= new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = mailID;
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
            public int AddMailAttachment(string name,string url,int mailID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "insert into Attachment (name,url,mailid)"+"values(@Name,@Url,@MailID)";
                SqlCommand cmd= new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@Name", SqlDbType.VarChar, 200);
                cmd.Parameters.Add("@Url", SqlDbType.VarChar, 255);
                cmd.Parameters.Add("@MailID",SqlDbType.Int,4);
                cmd.Parameters[0].Value = name;
                cmd.Parameters[1].Value = url;
                cmd.Parameters[2].Value = mailID;
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
            public DataSet GetAttachmentByMail(int mailID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "select Attachment.* from Attachment where mailid=@MailID";
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                da.SelectCommand.Parameters.Add("@MailID", SqlDbType.Int, 4);
               
                da.SelectCommand.Parameters[0].Value =mailID;

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
            public DataSet GetAddresses()
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "select * from Address";
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
            public int SetSingleMailRead(int mailID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "update Mail set readflag=1"
                    + " where ID=@mailid";//identity返回最后更新的id值
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@mailid", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = mailID;
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