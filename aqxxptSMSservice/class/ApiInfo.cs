using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aqxxptSMSservice
{
	class ApiInfo
	{
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ApiCode { get; set; }
        public string DataBase { get; set; }

        public ApiInfo()
        {
            IpAddress = "10.99.81.73";
            UserName = "xxjwd";
            Password = "xxjwd";
            ApiCode = "aqxxpt";
            DataBase = "mas";
        }

        public void initInfo()
        {
            IpAddress = "10.99.81.73";
            UserName = "xxjwd";
            Password = "xxjwd";
            ApiCode = "aqxxpt";
            DataBase = "mas";
        }
	}
}
