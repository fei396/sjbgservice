using System;
using System.Web.UI;

public partial class login : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var sso = new XxjwdSSO.XxjwdSSO(Request);
        var code = Request["code"];
        if (string.IsNullOrEmpty(code))
        {
            Response.Redirect("error.aspx?errcode=请通过正当途径登陆本网站");
            return;
        }


        var vr = sso.verifyCode(code);

        if (vr.Result <= 0)
        {
            if (vr.Result == -2)
            {
                Response.Redirect("error.aspx?errcode=Session已过期，请重新登录");
            }
            else
            {
                Response.Redirect("error.aspx?errcode=请通过正当途径登陆本网站" + Convert.ToString(vr.Result));
            }
        }
        else
        {
            Session["user"] = vr.WorkNo;
            Response.Redirect("mdefault.aspx");
        }
    }


    protected void loginButton_Click(object sender, EventArgs e)
    {
        var dm = new dbModule();
        var pm = new pbModule();
        var uCode = txtUcode.Text.Trim();
        var uPass = txtUpass.Text.Trim();
        if (!pbModule.isValidString(uCode) || !pbModule.isValidString(uPass))
        {
            errors.Text = "请输入格式正确的用户名和密码！";
            return;
        }

        var result = 0;
        var s = new Security();
        var url = "mdefault.aspx";
        result = dm.Login(uCode, uPass);
        s.SetSecurity(result);
        s.SetUserCode(uCode);
        s.SetUserName(dm.getUnameByUcode(uCode));
        s.SetUserDept(dm.getUdeptByUcode(uCode));
        s.SetUserId(dm.getUIdByUCode(uCode));
        switch (result)
        {
            case 1: //普通管理员
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                url = "mdefault.aspx";

                break;

            default:
                errors.Text = "用户名或密码错误，请检查后重新输入！";
                txtUpass.Text = "";
                break;
        }
        if (result == -1) return;
        Session["sec"] = s;
        Session["usec"] = s.GetSecurity();
        Session["usercode"] = s.GetUserCode();
        Session["udept"] = s.GetUserDept();
        Response.Redirect(url);
    }
}