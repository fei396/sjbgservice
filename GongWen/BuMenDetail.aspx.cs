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
using gwxxService;
public partial class BuMenDetail : System.Web.UI.Page
{
    readonly gwxxWebService s = new gwxxWebService();

    protected void Page_Load(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }


        if (!IsPostBack)
        {
            //string cj = Session["udept"] as string;
            //SqlDataSource1.SelectCommand = "SELECT    xmmc, cj , rygh, work_name , sfhg , kkid FROM V_CQKK_RYCJB  ";
            //if (cj != "_所有") SqlDataSource1.SelectCommand += " where cj='" + cj + "'";

            //SqlDataSource1.SelectCommand += " order by  xmmc,cj,rygh";

            s.SjbgSoapHeaderValue = Security.getSoapHeader();
            Security.SetCertificatePolicy();


            getData();
        }
    }


    private void getData()
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        Security.SetCertificatePolicy();

        int zdybmid = Convert.ToInt32(Request["id"]);
        string zdybmmc = Convert.ToString(Request["mc"]);
        lblBuMen.Text = "自定义部门：" + zdybmmc;

        GongWenBuMenRenYuan[] added = s.getZiDingYiBuMenRenYuan(zdybmid, true);
        GongWenBuMenRenYuan[] notadded = s.getZiDingYiBuMenRenYuan(zdybmid, false);

        lbDRn.Items.Clear();
        lbDR.Items.Clear();
        for (int i = 0; i < added.Length; i++)
        {
            ListItem li = new ListItem(added[i].XianShiMingCheng, added[i].GongHao.ToString());
            lbDR.Items.Add(li);
        }
        for (int i = 0; i < notadded.Length; i++)
        {
            ListItem li = new ListItem(notadded[i].XianShiMingCheng, notadded[i].GongHao.ToString());
            lbDRn.Items.Add(li);
        }
    }

    protected void zu1AddButton_Click(object sender, EventArgs e)
    {
        ArrayList arr = new ArrayList();
        foreach (ListItem li in lbDRn.Items)
        {
            if (li.Selected == true)
            {
                arr.Add(li);
            }
        }
        foreach (ListItem li in arr)
        {
            lbDRn.Items.Remove(li);
            lbDR.Items.Add(li);
        }
    }
    protected void zu1DelButton_Click(object sender, EventArgs e)
    {
        ArrayList arr = new ArrayList();
        foreach (ListItem li in lbDR.Items)
        {
            if (li.Selected == true)
            {
                arr.Add(li);
            }
        }
        foreach (ListItem li in arr)
        {
            lbDR.Items.Remove(li);
            lbDRn.Items.Add(li);
        }
    }





    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int zdybmid = Convert.ToInt32(Request["id"]);
        string[] ry = new string[lbDR.Items.Count];
        for(int i=0;i<lbDR.Items.Count;i++)
        {
            ry[i] = lbDR.Items[i].Value;
        }
        INT r =s.setZiDingYiBuMenRenYuan(zdybmid, ry);
        if (r.Number != 1)
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "错误", "alert('修改自定义部门人员失败：" + r.Message + "')", true);
           
        }
        else
        {
            Page.ClientScript.RegisterStartupScript(GetType(), "成功", "alert('修改自定义部门人员成功');window.open('bumen.aspx','_self');", true);
            
        }
    }

}
