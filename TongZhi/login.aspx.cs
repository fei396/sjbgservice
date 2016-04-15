using System;
using gwxxService;
using XxjwdSSO;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        XxjwdSSO.XxjwdSSO sso = new XxjwdSSO.XxjwdSSO(Request);
        string code = Request["code"];
        if (string.IsNullOrEmpty(code))
        {
            Response.Redirect("error.aspx?errcode=请通过正当途径登陆本网站");
            return;
        }

        
        VerifyResult vr = sso.VerifyCode(code);

        if (vr.Result <= 0)
        {
            if (vr.Result == -2)
            {
                Response.Redirect("error.aspx?errcode=Session已过期，请重新登录");
                return;
            }
            else
            {
                Response.Redirect("error.aspx?errcode=请通过正当途径登陆本网站" + Convert.ToString(vr.Result));
                return;
            }
        }
        else
        {
            int uid;
            try
            {
                uid = Convert.ToInt32(vr.Uid);
            }
            catch (Exception ex)
            {
                Response.Redirect("error.aspx?errcode=登录失败：" + Convert.ToString(ex.Message));
                return;
            }
            gwxxService.gwxxWebService s = new gwxxService.gwxxWebService();
            Security.SetCertificatePolicy();
            SjbgSoapHeader header = new SjbgSoapHeader();
            s.SjbgSoapHeaderValue = header;
            int role = s.GetTongZhiUserRoleType(uid);
            if (role <=0)
            {
                Response.Redirect("error.aspx?errcode=登录失败：用户不存在或无权限");
                return;
            }
            Session["uid"] = uid;
            Session["role"] = role;
            Response.Redirect("mdefault.aspx");
        }
    }




	protected void loginButton_Click ( object sender , EventArgs e )
	{
       

		dbModule dm = new dbModule();
		pbModule pm = new pbModule();
		string uCode = txtUcode.Text.Trim();
		string uPass = txtUpass.Text.Trim();
        if (!pbModule.isValidString(uCode) || !pbModule.isValidString(uPass))
		{
			errors.Text = "请输入格式正确的用户名和密码！";
			return;
		}
           
        int result = 0;
        Security s = new Security();
        string url = "mdefault.aspx";
        result = dm.Login(uCode, uPass);
        s.setSecurity(result);
        s.setUserCode(uCode);
        s.setUserName(dm.getUnameByUcode(uCode));
		s.setUserDept(dm.getUdeptByUcode(uCode));
		s.setUserId(dm.getUIdByUCode(uCode));
		switch (result)
		{
			case 1://普通管理员
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
		Session["usec"] = s.getSecurity();
		Session["usercode"] = s.getUserCode();
		Session["udept"] = s.getUserDept();
        Response.Redirect(url);
	}
}
