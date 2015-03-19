using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace aqxxptSMSservice
{
    class DAL
    {
        private static string messageConnString = System.Configuration.ConfigurationManager.ConnectionStrings["messageConnectionString"].ConnectionString;
        
        
        internal static DataTable getMessageToSend()
        {
            SqlConnection conn = new SqlConnection(messageConnString);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            DateTime now;
            comm.CommandText = "select getdate()";
            try
            {
                conn.Open();
                now = Convert.ToDateTime(comm.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            comm.CommandText = "SELECT xxid as smid,txt,sjh as mobile  FROM V_AQXXPT_XXJS where setTime < @now and sendtime is null order by xxid,receiverNo";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("now", now);
           
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            
            comm.CommandText = "update t_aqxxpt_xxjs set sendtime = @now where id in (select id from v_aqxxpt_xxjs where sendtime is null and settime<@now)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("now", now);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            SqlTransaction tran = conn.BeginTransaction();
            comm.Transaction = tran;
            try
            {
                int i = comm.ExecuteNonQuery();
                if (dt.Rows.Count == i)
                {
                    tran.Commit();
                }
                else
                {
                    tran.Rollback();
                    dt.TableName = "error";
                    return dt;
                }
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
            finally
            {
                conn.Close();
            }
            dt.TableName = "getMessageToSend";
            return dt;
        }

        internal static void ReceiveRPT(long smId, string mobile, string rptTime)
        {
            SqlConnection conn = new SqlConnection(messageConnString);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_aqxxpt_xxjs set receiveTime = @rptTime where xxid=@smid and receiver in (select user_no from V_User where sjh=@sjh)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("rptTime", rptTime);
            comm.Parameters.AddWithValue("smid", smId);
            comm.Parameters.AddWithValue("sjh", mobile);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        internal static void rollbackMessage(long smId, List<string> mobiles)
        {
            if (mobiles == null) return;
            string strMobiles = "";
            foreach(string mobile in mobiles)
            {
                strMobiles += "'" + mobile + "'" + ","; 
            }
            strMobiles = strMobiles.Substring(0, strMobiles.Length - 1);
            SqlConnection conn = new SqlConnection(messageConnString);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_aqxxpt_xxjs set sendTime = null where xxid=@smid and receiver in (select user_no from V_User where sjh in (" + strMobiles + "))";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("smid", smId);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        internal static void ReceiveReply(long smId, string content, string mobile, string rptTime)
        {
            SqlConnection conn = new SqlConnection(messageConnString);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_aqxxpt_xxjs set replyTime = @rptTime,receivetxt=@content where xxid=@smid and receiver in (select user_no from V_User where sjh=@sjh)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("rptTime", rptTime);
            comm.Parameters.AddWithValue("smid", smId);
            comm.Parameters.AddWithValue("sjh", mobile);
            comm.Parameters.AddWithValue("content", content);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
