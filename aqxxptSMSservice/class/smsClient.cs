using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImApiDotNet;

namespace aqxxptSMSservice
{
	class smsClient
	{

        public const int SM_TYPE_NORMAL = 0;  //  短信类型：普通短信 
        public const int SM_TYPE_PDU = 2;      //  短信类型：PDU短信 
        
        
        
        private APIClient api;
        public smsClient()
        {
            api = null;
        }

        private bool isValidContent(string content)
        {
            return true;
        }

        private bool isValidSmId(long smId)
        {
            return true;
        }
        private bool isValidMobile(string[] mobiles)
        {
            return true;
        }

        public void sendMessageSingle(string mobile ,string content)
        {
            if (api == null) return;
            api.sendSM(mobile, content, 1);
        }

        public void sendMessage()
        {
            
            MessagesToSend mst = new MessagesToSend();
            mst.getMessage();
            if (mst.Messages != null)
            {
                
                foreach (MessageToSend m in mst.Messages)
                {
                    if (m.Mobile != null)
                    {
                        string[] mobiles = m.Mobile.ToArray();
                        if (mobiles.Length > 0)
                        {
                            if (isValidContent(m.Content) && isValidSmId(m.SmID) && isValidMobile(mobiles))
                            {
                                try
                                {
                                    int code = api.sendSM(mobiles, m.Content, m.SmID,m.SmID);
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

        public void receiveRPT()
        {
            if (api == null) return;
            RPTItem[] rpts = api.receiveRPT();
            if (rpts == null) return;
            if (rpts.Length == 0) return;
            try
            {
                MessageRPT mrpt = new MessageRPT();
                mrpt.Items = rpts.ToList();
                mrpt.doReceive();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void receiveReply()
        {
            if (api == null) return;
            MOItem[] mis = api.receiveSM();
            if (mis == null) return;
            if (mis.Length == 0) return;
            try
            {
                MessageRecieve mr = new MessageRecieve();
                mr.Items = mis.ToList();
                mr.doReceive();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void checkAll()
        {
            sendMessage();
            receiveRPT();
            receiveReply();
        }

        public void initApi()
        {
            ApiInfo ai = new ApiInfo();
            ai.initInfo();
            if (api == null)
            {
                api = new APIClient();
                try
                {
                    int code = api.init(ai.IpAddress, ai.UserName, ai.Password, ai.ApiCode, ai.DataBase);
                    ResultCode rc = new ResultCode(code);
                    if (code != 0) throw new Exception(rc.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public void releaseApi()
        {
            if (api != null)
            {
                try
                {
                    api.release();
                    api = null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
	}
}
