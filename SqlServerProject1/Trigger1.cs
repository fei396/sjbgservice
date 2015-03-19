using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Net.Sockets;
using System.Net;


public partial class Triggers
{
    // 为目标输入现有表或视图并取消对特性行的注释
    [Microsoft.SqlServer.Server.SqlTrigger (Name="mqtt", Target="t1", Event="after insert")]
    public static void Trigger1()
    {
        // 用您的代码替换
       
        SqlTriggerContext context = SqlContext.TriggerContext;
        if (context.TriggerAction == TriggerAction.Insert)
        {
            SqlConnection conn = new SqlConnection("context connection=true");
            conn.Open();
            SqlCommand comm = new SqlCommand();
            SqlPipe pipe = SqlContext.Pipe;
            comm.Connection = conn;
            comm.CommandText = "select txt from inserted";
            string txt = Convert.ToString(comm.ExecuteScalar());
            if (sendText(txt))
            {
                SqlContext.Pipe.Send("成功");
            }
            else
            {
                SqlContext.Pipe.Send("失败");
            }
        }
    }

    static bool sendText(string txt)
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("10.99.151.124"),3974);
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
