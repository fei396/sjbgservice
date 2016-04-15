using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace XxjwdSSO
{
    class DAL
    {
        static string _strConn = "Data Source=192.168.2.10,2433;Initial Catalog=xtbg;Persist Security Info=True;User ID=xtbg;Password=xtbg";

        internal static DateTime GetServerTime()
        {
            SqlConnection conn = new SqlConnection(_strConn);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select getdate()";
            try
            {
                conn.Open();
                return Convert.ToDateTime(comm.ExecuteScalar());
            }
            catch
            {
                return DateTime.Now;
            }
            finally
            {
                conn.Close();

            }
        }

        internal static string GetYyxtUserNo(int yyxtId,string workno)
        {
            if (yyxtId == 0)
            {
                return workno;
            }
            else
            {
                SqlConnection conn = new SqlConnection(_strConn);
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.CommandText = "select yyxt_userbh from dat_userduiying where xtbg_user=@workno and yyxt_id=@yyxtid";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("workno", workno);
                comm.Parameters.AddWithValue("yyxtid", yyxtId);
                try
                {
                    conn.Open();
                    return Convert.ToString(comm.ExecuteScalar());
                }
                catch
                {
                    return null;
                }
                finally
                {
                    conn.Close();

                }
            }
        }

        internal static int GetUidByWorkNo(string workno)
        {
            SqlConnection conn = new SqlConnection(_strConn);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select uid from V_user where user_no=@workno";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("workno", workno);
            try
            {
                conn.Open();
                return Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();

            }
        }

        internal static int InsertSso(string guid, string mac, string ip, string userAgent,string workno ,int yyxtid)
        {
            SqlConnection conn = new SqlConnection(_strConn);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into T_SSO_Redirect(guid,mac,ip,userAgent,workno,createTime,yyxtid,isVerified) values(@guid,@mac,@ip,@userAgent,@workno,getdate(),@yyxtid,0)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("guid", guid);
            comm.Parameters.AddWithValue("mac", mac);
            comm.Parameters.AddWithValue("ip", ip);
            comm.Parameters.AddWithValue("userAgent", userAgent);
            comm.Parameters.AddWithValue("workno", workno);
            comm.Parameters.AddWithValue("yyxtid", yyxtid);
            try
            {
                conn.Open();
                return comm.ExecuteNonQuery();
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();

            }
        }

        internal static DataTable GetSsoByGuid(string guid)
        {
            SqlConnection conn = new SqlConnection(_strConn);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select guid,mac,ip,userAgent,workno,createTime,yyxtId,isVerified from  T_SSO_Redirect where guid=@guid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("guid", guid);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                dt.TableName = ex.Message;
            }
            dt.TableName = "getSSOByGuid";
            return dt;
        }

        internal static int UpdateVerifiedStatus(string guid)
        {
            SqlConnection conn = new SqlConnection(_strConn);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_SSO_Redirect set isVerified=1 ,verifyTime=getdate() where guid=@guid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("guid", guid);
            
            try
            {
                conn.Open();
                return comm.ExecuteNonQuery();
            }
            catch
            {
                return -1;
            }
        }
    }
}
