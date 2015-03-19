using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sodao.FastSocket.Server;
using Sodao.FastSocket.Server.Command;
using Sodao.FastSocket.SocketBase;
using System.IO;

namespace FastSocketServer
{
    /// <summary>
    /// 实现自定义服务
    /// </summary>
    public class MyService : CommandSocketService<StringCommandInfo>
    {

        private void WriteLog(string strContent)
        {
            FileInfo f = new FileInfo("d:\\log.txt");
            //f.Open( FileMode.OpenOrCreate , FileAccess.Write , FileShare.Read );
            StreamWriter sw;
            if (f.Exists)
            {
                sw = f.AppendText();
            }
            else
            {
                //if (!System.IO.Directory.Exists(strLogFilePath)) System.IO.Directory.CreateDirectory(strLogFilePath);
                sw = f.CreateText();
            }
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "--" + strContent);
            sw.Close();
        }
        /// <summary>
        /// 当连接时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        public override void OnConnected(IConnection connection)
        {
            base.OnConnected(connection);
            //Console.WriteLine(connection.RemoteEndPoint.ToString() + " connected");
            //connection.BeginSend(PacketBuilder.ToCommandLine("welcome"));
            WriteLog(connection.RemoteEndPoint.ToString() + " connected");
        }
        /// <summary>
        /// 当连接断开时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnDisconnected(IConnection connection, Exception ex)
        {
            base.OnDisconnected(connection, ex);
            //Console.ForegroundColor = ConsoleColor.Red;
            //Console.WriteLine(connection.RemoteEndPoint.ToString() + " disconnected");
            //Console.ForegroundColor = ConsoleColor.Gray;
            WriteLog(connection.RemoteEndPoint.ToString() + " disconnected");
        }
        /// <summary>
        /// 当发生错误时会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        public override void OnException(IConnection connection, Exception ex)
        {
            base.OnException(connection, ex);
            WriteLog("error: " + ex.ToString());
        }
        /// <summary>
        /// 处理未知命令
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        protected override void HandleUnKnowCommand(IConnection connection, StringCommandInfo commandInfo)
        {
            commandInfo.Reply(connection, "unknow command:" + commandInfo.CmdName);
        }

        public override void OnReceived(IConnection connection, StringCommandInfo cmdInfo)
        {
            base.OnReceived(connection, cmdInfo);
            WriteLog("收到" + connection.RemoteEndPoint.ToString() + "发来的信息" + cmdInfo.CmdName); 
        }
    }

    /// <summary>
    /// 退出命令
    /// </summary>
    public sealed class ExitCommand : ICommand<StringCommandInfo>
    {
        /// <summary>
        /// 返回命令名称
        /// </summary>
        public string Name
        {
            get { return "exit"; }
        }
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandInfo"></param>
        public void ExecuteCommand(IConnection connection, StringCommandInfo commandInfo)
        {
            connection.BeginDisconnect();//断开连接
        }
    }
}
