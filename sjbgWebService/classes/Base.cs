using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sjbgWebService
{

	public class RegisterInfo
	{
    


		public int WorkNo { get; set; }
		public string Mobile { get; set; }
		public string UniqueCode { get; set; }
		public string RegisterCode { get; set; }
		public string SecurityQuestion { get; set; }
		public string SecurityAnswer { get; set; }
		public string EmailAddress { get; set; }

		public RegisterInfo()
		{
			WorkNo = -1;
			Mobile = "";
			UniqueCode = "";
			RegisterCode = "";
			SecurityAnswer = "";
			SecurityQuestion = "";
            EmailAddress = "";
		}

        public RegisterInfo(int workno, string mobile, string ucode, string rcode, string sq, string sa, string email)
        {
            // TODO: Complete member initialization
            WorkNo = workno;
            Mobile = mobile;
            UniqueCode = ucode;
            RegisterCode = rcode;
            SecurityQuestion = sq;
            SecurityAnswer = sa;
            EmailAddress = email;
        }

        public RegisterInfo(int workno, string mobile, string ucode)
        {
            // TODO: Complete member initialization
            WorkNo = workno;
            Mobile = mobile;
            UniqueCode = ucode;
        }
		public bool isRegisterValid()
		{
			if (WorkNo <= 0) return false ;

			return true;
		}
	}


	public class ApkInfo
	{
		public int VerCode { get; set; }
		public string VerName { get; set; }
		public string URL { get; set; }
		public string FileName { get; set; }
        public string UpdateContent { get; set; }
		public ApkInfo()
		{
			VerCode = 0;
			VerName = "1.0";
			URL = @"http://61.163.45.215:808/update/";
			FileName = "xxjwdsjbg.apk";
            UpdateContent = "null";
		}

		public ApkInfo(int code, string name, string url, string filename,string updatecontent)
		{
			VerCode = code;
			VerName = name;
			URL = url;
			FileName = filename;
            UpdateContent = updatecontent;
		}
	}


	public class BOOLEAN
	{
		public bool IsTrue { get; set; }
        public string Message { get; set; }

        public BOOLEAN(bool value, string str)
        {
            IsTrue = value;
            Message = str;
        }
        //public BOOLEANSystem.Web.Services.Description.Messagealue)
        //{
        //    this.IsTrue = value;
        //    Message = "";
        //}
		public BOOLEAN()
		{
			IsTrue = false;
            Message = "";
		}
	}

	public class INT
	{

		public int Number { get; set; }
        public string Message { get; set; }
		public INT(int value ,string str)
		{
			Number = value;
            Message = str;
		}

        //public INT(int value)
        //{
        //    this.Number = value;
        //    Message = "";
        //}

		public INT()
		{
			Number = 0;
            Message = "";
		}

        public INT(int p)
        {
            // TODO: Complete member initialization
            Number = p;
            Message = "";
        }
	}

	public class Product
	{
		public int Pid { get; set; }
		public string PName { get; set; }
		public string ServicePage { get; set; }
	}
	public class User
	{
		public int Uid { get; set; }
		public string UserName { get; set; }
		public string UserNo { get; set; }
		public int UserDept { get; set; }
        
        public int TqLevel { get; set; }
        public int GwLevel { get; set; }
        public int GwRoleID { get; set; }
		public User()
		{
			Uid = 0;
			UserName = "测试";
			UserNo = "0000";

		}
		public User(int uid)
		{
			if (uid == 1)
			{
				UserName = "郑杰";
				UserNo = "0001";
			}
			else if (uid == 0)
			{
				UserName = "张纯杰";
				UserNo = "3974";
			}
			else
			{
				UserName = "测试";
				UserNo = "0000";
			}
		}
	}

	public class Department
	{
		public int ID { get; set; }
		public string Name { get; set; }

		public Department()
		{
			ID = 0;
			Name = "";
		}
	}

    public class UserRole
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public UserRole()
        {
            ID = 0;
            Name = "";
            Description = "";
        }
    }

    public class MenuItem
    {

        public int M1Id { get; set; }
        public string M1Name { get; set; }
        public int M2Id { get; set; }
        public string M2Name {get;set;}
        public int Enabled {get;set;}
        public string ImageRes { get; set; }
        public string ActivityName { get; set; }
        public string Params { get; set; }
    }

    public class Feedback
    {
        public int ID { get; set; }
        public string Sender { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public string CreateTime { get; set; }
        public int IsReplied { get; set; }
        public string Replier { get; set; }
        public string ReplierName { get; set; }
        public string ReplyContent { get; set; }
        public string ReplyTime { get; set; }
    }

    public class LoginInfo
    {
        public int ID { get; set; }
        public string UserNo { get; set; }
        public string DeviceID { get; set; }
        public string DeviceInfo { get; set; }
        public string DeviceVersion { get; set; }
        public string IpAddress { get; set; }
        public string LoginTime { get; set; }
    }

    public class MqttTopic
    {
        public int ID { get; set; }
        public string Topic { get; set; }
    }



    public enum MqttMessageType
    {
        CHAT_MESSAGE, REMIND_MESSAGE, ALARM_MESSAGE
    }
    public class MqttMessageBase
    {
        public int ID { get; set; }
        public string Sender { get; set; }
    }

    public class SystemMessage : MqttMessageBase
    {
        
        public string ToUser { get; set; }
        public int Type { get; set; }
        public string Content { get; set; }
         public string Command { get; set; }
        public string SendTime { get; set; }
        public int HasRead { get; set; }
        public string ReadTime { get; set; }
    }

    public class WarningMessage : MqttMessageBase
    {
        public string Content { get; set; }
        public string Topic { get; set; }

        public WarningMessage(int mid,string sender,string topic,string content)
        {
            ID = mid;
            Sender = sender;
            Topic = topic;
            Content = content;
        }

    }

    public class ChatMessage : MqttMessageBase
    {
        public string Content { get; set; }
    }

    public class MqttMessage
    {
        public int Type { get; set; }
        public MqttMessageBase MqttMessageContent { get; set; }

        public MqttMessage(MqttMessageType type ,int mid,string sender,string topic,string content)
        {
            if (type == MqttMessageType.CHAT_MESSAGE)
            {
                Type = 1;
                MqttMessageContent = new ChatMessage();
                return;
            }
            else if (type == MqttMessageType.REMIND_MESSAGE)
            {
                Type = 2;
            }
            else if (type == MqttMessageType.ALARM_MESSAGE)
            {
                Type = 3;
            }

            MqttMessageContent = new WarningMessage(mid, sender, topic, content);
           
        }
    }
}