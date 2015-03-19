using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Net.Sockets;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
namespace MqttSql
{
    public class MqttSql
    {
        

        [SqlFunction(IsDeterministic = true, DataAccess = DataAccessKind.None)]
        public int sendMqtt(string ip,int port,string prefixClientId ,string username ,string pass,string topic,string text)
        {
            
            string cliendId = "MqttSqlCLR";
            try
            {
                MqttClient mc = new MqttClient(ip, port, );
                mc.Connect(cliendId, username, pass);
                mc.Publish(topic, Encoding.UTF8.GetBytes(text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                mc.Disconnect();
                
            }
            catch
            {
                return 0;
            }
            return 1;
            //if (sendText(text)) return 1;
            //else return 0;
        }


        static bool sendText(string txt)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse("10.99.151.124"), 3974);
                if (!s.Connected) s.Connect(ip);
                s.Send(System.Text.Encoding.UTF8.GetBytes(txt));
            }
            catch
            {
                return false;
            }
            finally
            {
                s.Close();
            }
            return true;
        }
    }
}
