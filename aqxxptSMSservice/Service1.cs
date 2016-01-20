using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;

namespace aqxxptSMSservice
{
    public partial class Service1 : ServiceBase
    {
        smsClient client;
        
        private static string strAdmin = System.Configuration.ConfigurationManager.AppSettings["AdminNum"];
        
        private static string strTiXing = System.Configuration.ConfigurationManager.AppSettings["TiXing"];
        private static string strSpan = System.Configuration.ConfigurationManager.AppSettings["checkSpan"];
        private static string strMessageToAdminOnError = System.Configuration.ConfigurationManager.AppSettings["MessageToAdminOnError"];
        private static int checkSpan;
        private static DateTime TiXing;
        private static bool MessageToAdminOnError;
        public Service1()
        {
            InitializeComponent();
            timer1.Enabled = false;
        }

        

        protected override void OnStart(string[] args)
        {
            initParam();
            //initApi();
            initTimer();
            Log.writeLog("服务初始化完毕，开始运行。");
        }


        protected override void OnStop()
        {
            release();
            Log.writeLog("服务释放完毕，停止运行。");
        }

        public void DoStop()
        {
            if (client != null)
            {
                client.sendMessageSingle(strAdmin, "安全信息平台短信服务意外终止。");
            }
            this.Stop();
            
        }


        private void release()
        {
            try
            {
                client.releaseApi();
            }
            catch (Exception ex)
            {
                Log.writeLog(ex.Message);
                Log.writeLog("服务意外终止。");
                this.DoStop();
            }
        }



        private void initTimer()
        {
            timer1.Interval = checkSpan * 1000;
            timer1.Start();
        }

        private void initParam()
        {
            try
            {
                checkSpan = Convert.ToInt32(strSpan);
                TiXing = Convert.ToDateTime(strTiXing);
                MessageToAdminOnError = Convert.ToBoolean(strMessageToAdminOnError);
            }
            catch (Exception ex)
            {
                Log.writeLog("initParam");
                Log.writeLog(ex.Message);
                Log.writeLog("服务意外终止。");
                this.DoStop();
            }
        }

        private void initApi()
        {
            try
            {
                client = new smsClient();
                client.initApi();
            }
            catch (Exception ex)
            {
                Log.writeLog("initApi");
                Log.writeLog(ex.Message);
                Log.writeLog("服务意外终止。");
                this.DoStop();
            }
        }



        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer1.Stop();
            if (client == null)
            {
                try
                {
                    initApi();
                }
                catch (Exception ex)
                {
                    Log.writeLog("client.initApi");
                    Log.writeLog(ex.Message);
                    Log.writeLog("服务意外终止。");
                    this.DoStop();
                }
            }
            try
            {
                client.checkAll();
                if (MessageToAdminOnError == true) client.sendMessageSingle(strAdmin, "安全信息平台服务运行正常。");
            }
            catch (Exception ex)
            {
                Log.writeLog("client.checkAll");
                Log.writeLog(ex.Message);
                //Log.writeLog("服务意外终止。");
                //this.DoStop();
            }
            timer1.Start();
        }
    }
}
