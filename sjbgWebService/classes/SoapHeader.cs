using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sjbgWebService
{
	public class SjbgSoapHeader : System.Web.Services.Protocols.SoapHeader
	{
		public string A;
		public string P;


		public SjbgSoapHeader()
		{
			A = "-2";
			P = "";
		}

		public bool checkValid()
		{
			if (A.Equals("3974") && P.Equals("zcj")) return true;
            return true;
                
		}
	}

	public class PreSoapHeader : System.Web.Services.Protocols.SoapHeader
	{
		public string A;
		public string P;


		public PreSoapHeader()
		{
			A = "-2";
			P = "";
		}

		public bool checkValid()
		{
			if (A.Equals("3974") && P.Equals("zcj")) return true;
			return false;
		}
	}


}