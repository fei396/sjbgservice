using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sjbgWebService.gwxx
{
	public class GongWen
	{
		#region 私有变量
		int _id;
		string _redTitle;
		string _number;
		string _title;
		string _content;
		string _sendDept;
		string _sendDate;
		string _attachPath01;
		string _attachPath02;
		string _attachPath03;
		string _attachPath04;
		string _attachPath05;
		string _attachPath06;
		string _fileType;
		string _sendType;
		string _suggestion;
		#endregion

		#region 属性
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}
		public string RedTitle
		{
			get { return _redTitle; }
			set { _redTitle = value; }
		}
		public string Number
		{
			get { return _number; }
			set { _number = value; }
		}
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}
		public string Content
		{
			get { return _content; }
			set { _content = value; }
		}
		public string SendDept
		{
			get { return _sendDept; }
			set { _sendDept = value; }
		}
		public string SendDate
		{
			get { return _sendDate; }
			set { _sendDate = value; }
		}
		public string FileType
		{
			get { return _fileType; }
			set { _fileType = value; }
		}
		public string SendType
		{
			get { return _sendType; }
			set { _sendType = value; }
		}
		public string Suggestion
		{
			get { return _suggestion; }
			set { _suggestion = value; }
		}
		public string AttachPath01
		{
			get { return _attachPath01; }
			set { _attachPath01 = value; }
		}
		public string AttachPath02
		{
			get { return _attachPath02; }
			set { _attachPath02 = value; }
		}
		public string AttachPath03
		{
			get { return _attachPath03; }
			set { _attachPath03 = value; }
		}
		public string AttachPath04
		{
			get { return _attachPath04; }
			set { _attachPath04 = value; }
		}
		public string AttachPath05
		{
			get { return _attachPath05; }
			set { _attachPath05 = value; }
		}
		public string AttachPath06
		{
			get { return _attachPath06; }
			set { _attachPath06 = value; }
		}
		#endregion

		//构造函数
		public GongWen()
		{
			_id = 0;
		}

		

	}
	public class Instruction
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public string Content { get; set; }
		public string Date { get; set; }

		public Instruction()
		{
			ID = 0;
			Title = "";
			Name = "";
			Content = "";
			Date = "";
		}
	}

	public class UserGw
	{
		public int Yhbh { get; set; }
		public string Yhmc { get; set; }
		public string Yhsm { get; set; }
		public int Ssbm { get; set; }
		public int Yhqx { get; set; }
		public string Yhnc { get; set; }
		//public int Wjxz { get; set; }
		public int Ssxzbm { get; set; }
		public int ShuangQian { get; set; }
		public UserGw()
		{
			Yhbh = 0;
			Ssbm = 0;
			Yhqx = 0;
			//Wjxz = 0;
			Ssxzbm = 0;
			Yhmc = "";
			Yhsm = "";
			Yhnc = "";
			ShuangQian = 0;
		}
		public UserGw(int yhbh)
		{
			UserGw gw = BLL.getUserGwByUid(yhbh);
			Yhbh = gw.Yhbh;
			Ssbm = gw.Ssbm;
			Yhqx = gw.Yhqx;
			//Wjxz = gw.Wjxz;
			Ssxzbm = gw.Ssxzbm;
			Yhmc = gw.Yhmc;
			Yhsm = gw.Yhsm;
			Yhnc = gw.Yhnc;

		}
		public UserGw(string yhmc)
		{
			UserGw gw = BLL.getUserGwByUserName(yhmc);
			Yhbh = gw.Yhbh;
			Ssbm = gw.Ssbm;
			Yhqx = gw.Yhqx;
			//Wjxz = gw.Wjxz;
			Ssxzbm = gw.Ssxzbm;
			Yhmc = gw.Yhmc;
			Yhsm = gw.Yhsm;
			Yhnc = gw.Yhnc;
		}


	}

    public class BuMenGw
    {
        public int Bmbh { get; set; }
        public int Bmlb { get; set; }
        public string Bmmc { get; set; }
        public int Bmsqbh { get; set; }
        public string Bmlbmc { get; set; }
    }

    public class BuMenLeiBie
    {
        public int ID { get; set; }
        public string Lbmc{get;set;}
    }

    public class GongWenLeiXing
    {
        public int LXID{get;set;}
        public string LXMC{get;set;}
    }
    public class GongWenXingZhi
    {
        public int XZID{get;set;}
        public string XZMC{get;set;}
    }

    public class GongWenYongHu
    {
        public string GongHao { get; set; }
        public string XingMing { get; set; }
        public string NiCheng { get; set; }
        public string BuMen { get; set; }
        public int BuMenID { get; set; }
        public int[] RoleID { get; set; }
        public int[] XingZhiID { get; set; }

        public string XingZhiIDString
        {
            get
            {
                
                if (XingZhiID == null || XingZhiID.Length == 0) return string.Empty;
                string str = "";
                for(int i=0;i<XingZhiID.Length;i++)
                {
                    str += XingZhiID[i].ToString() + ",";
                }
                str = str.Substring(0, str.Length - 1);
                return str;
            }
        }


    }
}