using System.Web;
using System.Web.Security;

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
        return false;
    }

    public static bool hasForbiddenChar(string str)
    {
        var validString = "\",.'()*&^%$#@![]{}|\\=-/`~:;<>?";
        var e = str.GetEnumerator();
        while (e.MoveNext())
        {
            if (validString.IndexOf(e.Current) >= 0) return true;
        }
        return false;
    }

    public static bool isValidString(string str)
    {
        var validString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_";
        var e = str.GetEnumerator();
        while (e.MoveNext())
        {
            if (validString.IndexOf(e.Current) == -1) return false;
        }
        return true;
    }

    public static string MD5(string s)
    {
        return FormsAuthentication.HashPasswordForStoringInConfigFile(s, "MD5").ToUpper();
    }

    public static string getIP()
    {
        var userIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (userIp == null || userIp == "")
        {
            userIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        }
        return userIp;
    }
}