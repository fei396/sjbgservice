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
        private SmsClient _client;
        
        private static readonly string StrAdmin = System.Configuration.ConfigurationManager.AppSettings["AdminNum"];
        
        private static readonly string StrTiXing = System.Configuration.ConfigurationManager.AppSettings["TiXing"];
        private static readonly string StrSpan = System.Configuration.ConfigurationManager.AppSettings["checkSpan"];
        private static readonly string StrMessageToAdminOnError = System.Configuration.ConfigurationManager.AppSettings["MessageToAdminOnError"];
        private static int _checkSpan;
        private static DateTime TiXing;
        private static bool _messageToAdminOnError;
        public Service1()
        {
            InitializeComponent();
            timer1.Enabled = false;
        }

        

        protected override void OnStart(string[] args)
        {
            InitParam();
            //initApi();
            InitTimer();
            Log.WriteLog("服务初始化完毕，开始运行。");
        }


        protected override void OnStop()
        {
            Release();
            Log.WriteLog("服务释放完毕，停止运行。");
        }

        public void DoStop()
        {
            if (_client != null)
            {
                _client.SendMessageSingle(StrAdmin, "安全信息平台短信服务意外终止。");
            }
            this.Stop();
            
        }


        private void Release()
        {
            try
            {
                _client.ReleaseApi();
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.Message);
                //Log.WriteLog("服务意外终止。");
                this.DoStop();
            }
        }



        private void InitTimer()
        {
            timer1.Interval = _checkSpan * 1000;
            timer1.Start();
        }

        private void InitParam()
        {
            try
            {
                _checkSpan = Convert.ToInt32(StrSpan);
                TiXing = Convert.ToDateTime(StrTiXing);
                _messageToAdminOnError = Convert.ToBoolean(StrMessageToAdminOnError);
            }
            catch (Exception ex)
            {
                Log.WriteLog("initParam");
                Log.WriteLog(ex.Message);
                //Log.writeLog("服务意外终止。");
                this.DoStop();
            }
        }

        private void InitApi()
        {
            try
            {
                _client = new SmsClient();
                _client.InitApi();
            }
            catch (Exception ex)
            {
                Log.WriteLog("initApi");
                Log.WriteLog(ex.Message);
                //Log.writeLog("服务意外终止。");
                this.DoStop();
            }
        }



        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer1.Stop();
            if (_client == null)
            {
                try
                {
                    InitApi();
                }
                catch (Exception ex)
                {
                    Log.WriteLog("client.initApi");
                    Log.WriteLog(ex.Message);
                    //Log.writeLog("服务意外终止。");
                    //this.DoStop();
                }
            }
            try
            {
                if (_client != null)
                {
                    _client.CheckAll();
                    if (_messageToAdminOnError == true) _client.SendMessageSingle(StrAdmin, "安全信息平台服务运行正常。");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("client.checkAll");
                Log.WriteLog(ex.Message);
                //Log.writeLog("服务意外终止。");
                //this.DoStop();
            }
            timer1.Start();
        }
    }
}
