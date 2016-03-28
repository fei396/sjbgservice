using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using aqxxptWebService;

/// <summary>
///     Security 的摘要说明
/// </summary>
public class Security
{
    private int _isAdmin;
    private string _userCode;
    private string _userDept;

    private int _userId;
    private string _userName;

    public Security(int uI, int i, string uC, string uN, string uD)
    {
        _userId = uI;
        _isAdmin = i;
        _userCode = uC;
        _userName = uN;
        _userDept = uD;
    }

    public Security(int uI, string uC, string uN, string uD)
    {
        _userId = uI;
        _isAdmin = 0;
        _userCode = uC;
        _userName = uN;
        _userDept = uD;
    }

    public Security(int uI, string uC, string uN)
    {
        _userId = uI;
        _isAdmin = 0;
        _userCode = uC;
        _userName = uN;
        _userDept = "";
    }

    public Security()
    {
        _isAdmin = 0;
        _userCode = "";
        _userName = "";
        _userDept = "";
        _userId = 0;
    }


    /// <summary>
    ///     Sets the cert policy.
    /// </summary>
    public static void SetCertificatePolicy()
    {
        ServicePointManager.ServerCertificateValidationCallback
            += RemoteCertificateValidate;
    }

    /// <summary>
    ///     Remotes the certificate validate.
    /// </summary>
    private static bool RemoteCertificateValidate(
        object sender, X509Certificate cert,
        X509Chain chain, SslPolicyErrors error)
    {
        // trust any certificate!!!
        Console.WriteLine("Warning, trust any certificate");
        return true;
    }


    public static SjbgSoapHeader GetSoapHeader()
    {
        var ssh = new SjbgSoapHeader
        {
            A = "3974",
            P = "zcj"
        };
        return ssh;
    }

    public int GetSecurity()
    {
        return _isAdmin;
    }

    public void SetSecurity(int c)
    {
        _isAdmin = c;
    }

    public void SetUserCode(string c)
    {
        _userCode = c;
    }

    public string GetUserCode()
    {
        return _userCode;
    }

    public void SetUserName(string n)
    {
        _userName = n;
    }

    public string GetUserName()
    {
        return _userName;
    }

    public string GetUserDept()
    {
        return _userDept;
    }

    public void SetUserDept(string n)
    {
        _userDept = n;
    }

    public void SetUserId(int uI)
    {
        _userId = uI;
    }

    public int GetUserId()
    {
        return _userId;
    }
}