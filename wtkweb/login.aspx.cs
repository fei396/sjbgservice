using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Text;
using System.IO;

public partial class login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
		//errors.Text =	pbModule.getIP();
		//dbModule dm = new dbModule();
		//errors.Text = dm.addLog(100, "zcj", 1, "zcj1", "zcjzcj").ToString();
    }




	protected void loginButton_Click ( object sender , EventArgs e )
	{
       

		dbModule dm = new dbModule();
		pbModule pm = new pbModule();
		string uCode = txtUcode.Text.Trim();
		string uPass = txtUpass.Text.Trim();
		if ( !pm.isValidString( uCode) || !pm.isValidString( uPass ) )
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
