using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Net.Sockets;
using Sodao.FastSocket.Server;
using Sodao.FastSocket.Server.Command;
using Sodao.FastSocket.SocketBase;
using Sodao.FastSocket.Server.Config;
using System.Threading;
using System.Net;
using System.IO;

namespace WindowsFormsApplication2
{
	public partial class Form1 : Form
	{
        MqttClient mc;
		public Form1()
		{
			InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string[] city = new string[] {"安阳","保定","北京","亳州","长治","阜阳","高平","邯郸",
"菏泽","鹤壁","侯马","济南","济宁","济源","焦作","晋城","临沂","洛阳","日照","商丘","石家庄","泰安",
"新乡","徐州","郑州","南阳","汤阴" };
			WeatherWebService.WeatherWebService ws = new WeatherWebService.WeatherWebService();
			foreach (string c in city)
			{
				string[] str = ws.getWeatherbyCityName(c);
				string txt = "";
				try
				{
					txt = str[1] + "               " + str[12] + "          " + str[6].Substring(str[6].LastIndexOf(' '), str[6].Length - str[6].LastIndexOf(' ')) + "          " + str[7];
					//
				}
				catch
				{
					txt = c + "不支持";
				}
				textBox1.AppendText(txt + "\r\n");
				System.Threading.Thread.Sleep(1000);
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
            string fileFullName = "abc.doc";
            
            int posOfDot = fileFullName.LastIndexOf(".");
            string fileName = fileFullName.Substring(0, posOfDot);
            string extName = fileFullName.Substring(posOfDot+1, fileFullName.Length - posOfDot - 1);
            textBox2.Text = fileName;
            textBox1.Text = extName;
            return;

			gwxxWebService.baseService web = new gwxxWebService.baseService();
			gwxxWebService.SjbgSoapHeader header = new gwxxWebService.SjbgSoapHeader();
			header.A= "3974";
			header.P = "zcj";
			web.SjbgSoapHeaderValue = header;
			gwxxWebService.ApkInfo apk = web.getApkVerCode();
			textBox2.Text = apk.FileName;
		}

        private void button3_Click(object sender, EventArgs e)
        {
            //FileStream fs = new FileStream("d:\\dzgz.zip", FileMode.Open, FileAccess.Read);

            //byte[] b = new byte[(int)fs.Length];
            //int k = fs.Read(b, 0, (int)fs.Length);
            //fs.Close();
            //string base64 = Convert.ToBase64String(b);
            string message = Convert.ToString(textBox2.Text);
            //message = base64;
            int a=0;
            try
            {
                a = mc.Publish("SystemAdmin", Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            }
            catch
            {
                a = -123456;
            }
            //textBox1.Text = a.ToString();
        }



        private void button4_Click(object sender, EventArgs e)
        {

            mc.Subscribe(new string[] { "3974", "SystemMonitor" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        void mc_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string r = Encoding.UTF8.GetString(e.Message);
            //MessageBox.Show(r);
            MqttClient mc = (MqttClient)sender;
            textBox1.AppendText(mc.ClientId + ":" + r + "\r\n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string cliendId = "xxjwdsjbg_pub";
            string user = "sjbgmqtt";
            string password = "kG@D0B-#8x+V";
            if (mc == null)
            {
                mc = new MqttClient("10.99.81.106", 8883, false, null);

                if (!mc.IsConnected) mc.Connect(cliendId, user, password, false, 10);
                
                mc.MqttMsgPublishReceived += mc_MqttMsgPublishReceived;
                mc.MqttMsgPublished += new MqttClient.MqttMsgPublishedEventHandler(mc_MqttMsgPublished);
                
            }
            else
            {
                if ( !mc.IsConnected)                mc.Connect(cliendId, user, password);
            }
        }

        void mc_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
//            textBox1.Text += e.MessageId.GetTypeCode == TypeCode. +"\r\n";
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            
            SocketServerManager.Init();
            SocketServerManager.Start();
        }
        void threadMqtt(object clientId)
        {
            
            MqttClient tmc = new MqttClient("10.99.81.106");
            string cid = Dns.GetHostName() + "_YaLi" + clientId.ToString().PadLeft(5, '0');
            if (!tmc.IsConnected) tmc.Connect(cid, "admin", "password");
            tmc.Subscribe(new string[] { "3974" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            tmc.MqttMsgPublishReceived += mc_MqttMsgPublishReceived;
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int maxCount = Convert.ToInt32(textBox2.Text);
            for (int i = 1; i <= maxCount; i++)
            {
                Thread th = new Thread(new ParameterizedThreadStart(threadMqtt));
                th.Start(i);
            }
        }
	}

   
}
