using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sjbgWebService.xwxx
{
	public class XinWen
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string AttachFiles { get; set; }
		public string Content { get; set; }
		public string Date { get; set; }
		public int TypeId { get; set; }
		public string Publisher { get; set; }
		public string Path { get; set; }
		
	}
	public class XinWenLeiXing
	{
		public int ID { get; set; }
		public string Type { get; set; }
	}
}