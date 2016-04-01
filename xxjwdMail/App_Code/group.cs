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
    public class Group
    {       
            public DataSet GetGroups(int gh)
          {
              string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
              SqlConnection con = new SqlConnection(connectionString);
              string cmdText = "select * from dic_group where gh=@gh";
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

 
            public DataSet GetSingleGroup(int ID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "select * from dic_group where id=@ID";
               // SqlCommand cmd = new SqlCommand(cmdText, con);
                //cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                //cmd.Parameters[0].Value = mailID;
                //DataTable dd=new DataTable();
                //SqlDataReader dr;
                SqlDataAdapter da = new SqlDataAdapter(cmdText, con);
                da.SelectCommand.Parameters.Add("@ID", SqlDbType.Int, 4);
                da.SelectCommand.Parameters[0].Value = ID;
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
            public int AddGroup(string gh,string GroupName,string GroupMember)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "insert into dic_Group(gh,GroupName,GroupMember)"
                    + "values(@gh,@GroupName,@GroupMember) set @ID=@@Identity";//identity返回最后更新的id值
                SqlCommand cmd = new SqlCommand(cmdText,con);
                cmd.Parameters.Add("@gh", SqlDbType.VarChar,200);
                cmd.Parameters.Add("@GroupName", SqlDbType.Text);
                cmd.Parameters.Add("@GroupMember", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = gh;
                cmd.Parameters[1].Value =GroupName;
                cmd.Parameters[2].Value = GroupMember;
                cmd.Parameters[3].Direction = ParameterDirection.Output;
                int result = -1;
                try
                {
                    con.Open();
                    result = cmd.ExecuteNonQuery();

                }
                catch (Exception ex) { throw new Exception(ex.Message, ex); }
                finally { con.Close(); }
                return (int)cmd.Parameters[3].Value;
            }

            public int UpdateGroup(int id, string GroupName, string GroupMember)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "update dic_Group set GroupName=@GroupName,GroupMember=@GroupMember"
                    + " where ID=@id";//identity返回最后更新的id值
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@GroupName", SqlDbType.Text);
                cmd.Parameters.Add("@GroupMember", SqlDbType.VarChar, 1000);
                cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = GroupName;
                cmd.Parameters[1].Value = GroupMember;
                cmd.Parameters[2].Value = id;
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
 
            public int DeleteGroup(int GroupID)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
                SqlConnection con = new SqlConnection(connectionString);
                string cmdText = "delete dic_Group where id=@ID";
                SqlCommand cmd= new SqlCommand(cmdText, con);
                cmd.Parameters.Add("@ID", SqlDbType.Int, 4);
                cmd.Parameters[0].Value = GroupID;
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