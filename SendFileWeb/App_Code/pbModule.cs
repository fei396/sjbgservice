using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

public class pbModule
{
	public static string usercode = "";
	public static string username = "";
	public static int userdept = -1;
	public static int isadmin = 0;


	public bool isNullString(string str)
	{
		if (str == null || str == "")
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool hasForbiddenChar(string str)
	{
		string validString = "\",.'()*&^%$#@![]{}|\\=-/`~:;<>?";
		CharEnumerator e = str.GetEnumerator();
		while (e.MoveNext())
		{
			if (validString.IndexOf(e.Current) >= 0) return true;
		}
		return false;
	}

	public bool isValidString(string str)
	{
		string validString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";
		CharEnumerator e = str.GetEnumerator();
		while (e.MoveNext())
		{
			if (validString.IndexOf(e.Current) == -1) return false;
		}
		return true;
	}

	public static string MD5(string s)
	{
		return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToUpper();
	}

	public static string getIP()
	{
		string userIp = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
		if (userIp == null || userIp == "")
		{
			userIp = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
		}
		return userIp;
	}
}
