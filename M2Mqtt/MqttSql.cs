using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Net.Sockets;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Data.SqlClient;
namespace MqttSql
{
    public class MqttSql
    {
        //m2mqtt.pfx的密码是xx
        static MqttClient mc = new MqttClient("192.168.2.8", 8883, false, null);
        static string mqttConnectionString = "Data Source=127.0.0.1,2433;Initial Catalog=mosquitto;Persist Security Info=True;User ID=sjbg;Password=sjbg";
        [SqlFunction(IsDeterministic = true, DataAccess = DataAccessKind.None)]
        public static string sendMqtt(string prefixClientId ,string username ,string pass,string topic,string text,int retain)
        {
            
            string cliendId = prefixClientId + "_MqttSqlCLR2012";
            try
            {

                bool isRetain = retain == 1 ? true : false;
                if (!mc.IsConnected)
                {
                    mc.Connect(cliendId, username, pass);
                    mc.Subscribe(new string[]{"SystemMonitor"},new byte[] {MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE});
                    mc.MqttMsgPublishReceived += new MqttClient.MqttMsgPublishEventHandler(mc_MqttMsgPublishReceived);
                    mc.Publish(topic, Encoding.UTF8.GetBytes( text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, isRetain);
                }
                else
                {
                    mc.Publish(topic, Encoding.UTF8.GetBytes( text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, isRetain);
                }
                //mc.Disconnect();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "1";
            //if (sendText(text)) return 1;
            //else return 0;
        }

        static void mc_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            
            string text = Encoding.UTF8.GetString(e.Message);
            
            string[] messages = text.Split(new string[] { "~" }, StringSplitOptions.None);
            if (messages.Length != 4) return;
            string[] para = messages[3].Split(new string[] { "#" }, StringSplitOptions.None);
            switch (messages[0])
            {
                case "setSessionStatus":
                    
                    int type = 0;
                    if (para[0].Equals("on"))
                    {
                       type = 1;
                    }
                    else if (para[0].Equals("lost"))
                    {
                        type = 0;
                    }
                    else return;
                    mqttConnectionStatus(messages[2].PadLeft(4, '0'), messages[1],type);
                    break;
                default:
                    SystemMonitorReceiveError(text);
                    break;
            }
        }

        static void SystemMonitorReceiveError(string text)
        {
            SqlConnection conn = new SqlConnection(mqttConnectionString);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into T_SystemMonitorReceiveError (text) values(@text)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("text", text);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch
            {
            }
            finally
            {
                conn.Close();
            }
        }

        static void mqttConnectionStatus(string work_no,string clientId,int type)
        {
            SqlConnection conn = new SqlConnection(mqttConnectionString);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select count(*) from T_User_session where work_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
            int isin = 0;
            try
            {
                conn.Open();
                isin = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                return;
            }
            finally
            {
                conn.Close();
            }
            if (isin == 1)
            {

                comm.CommandText = "update T_User_Session set isOnline=@type, lastUpdateTime =getdate(),clientid=@clientId where work_no=@work_no";
            }
            else
            {
                comm.CommandText = "insert into T_User_Session (isOnline,lastUpdateTime,work_no,clientid) values(@type,getdate(), @work_no,@clientId)";
            }
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
            comm.Parameters.AddWithValue("type", type);
            comm.Parameters.AddWithValue("clientId", clientId);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch
            {
                
            }
            finally
            {
                conn.Close();
            }
            
        }

        //static bool sendText(string txt)
        //{
        //    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //    try
        //    {
        //        IPEndPoint ip = new IPEndPoint(IPAddress.Parse("10.99.151.124"), 3974);
        //        if (!s.Connected) s.Connect(ip);
        //        s.Send(System.Text.Encoding.UTF8.GetBytes(txt));
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        s.Close();
        //    }
        //    return true;
        //}
    }
}
