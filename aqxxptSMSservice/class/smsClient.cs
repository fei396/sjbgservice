using System;
using System.Linq;
using ImApiDotNet;

namespace aqxxptSMSservice
{
	class SmsClient
	{

        public const int SmTypeNormal = 0;  //  短信类型：普通短信 
        public const int SmTypePdu = 2;      //  短信类型：PDU短信 
        
        
        
        private APIClient _api;
        public SmsClient()
        {
            _api = null;
        }

        private bool IsValidContent(string content)
        {
            return true;
        }

        private bool IsValidSmId(long smId)
        {
            return true;
        }
        private bool IsValidMobile(string[] mobiles)
        {
            return true;
        }

        public void SendMessageSingle(string mobile ,string content)
        {
            if (_api == null) return;
            _api.sendSM(mobile, content, 1);
        }

        public void SendMessage()
        {
            
            MessagesToSend mst = new MessagesToSend();
            mst.GetMessage();
            if (mst.Messages != null)
            {
                
                foreach (MessageToSend m in mst.Messages)
                {
                    if (m.Mobile != null)
                    {
                        string[] mobiles = m.Mobile.ToArray();
                        if (mobiles.Length > 0)
                        {
                            if (IsValidContent(m.Content) && IsValidSmId(m.SmID) && IsValidMobile(mobiles))
                            {
                                try
                                {
                                    int code = _api.sendSM(mobiles, m.Content, m.SmID,m.SmID);
                                    ResultCode rc = new ResultCode(code);
                                    if (code != 0)
                                    {
                                        DAL.rollbackMessage(m.SmID,m.Mobile);
                                        throw new Exception(rc.Message);
                                    }
                                    
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ReceiveRpt()
        {
            if (_api == null) return;
            RPTItem[] rpts = _api.receiveRPT();
            if (rpts == null) return;
            if (rpts.Length == 0) return;
            try
            {
                MessageRPT mrpt = new MessageRPT();
                mrpt.Items = rpts.ToList();
                mrpt.DoReceive();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ReceiveReply()
        {
            if (_api == null) return;
            MOItem[] mis = _api.receiveSM();
            if (mis == null) return;
            if (mis.Length == 0) return;
            try
            {
                MessageRecieve mr = new MessageRecieve();
                mr.Items = mis.ToList();
                mr.DoReceive();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CheckAll()
        {
            SendMessage();
            ReceiveRpt();
            ReceiveReply();
        }

        public void InitApi()
        {
            ApiInfo ai = new ApiInfo();
            ai.InitInfo();
            if (_api == null)
            {
                _api = new APIClient();
                try
                {
                    int code = _api.init(ai.IpAddress, ai.UserName, ai.Password, ai.ApiCode, ai.DataBase);
                    ResultCode rc = new ResultCode(code);
                    if (code != 0) throw new Exception(rc.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void ReleaseApi()
        {
            if (_api != null)
            {
                try
                {
                    _api.release();
                    _api = null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
	}
}
