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
public partial class ListGongWen : System.Web.UI.Page
{
    gwxxService.gwxxWebService s = new gwxxService.gwxxWebService();
    static int cpage,maxPage,allCount;
    const int pageCount = 15;

    protected void Page_Load(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }
        int type = Convert.ToInt32(Request["type"]);
        if (type == 0)
        {
            tableChaXun.Visible = false;
        }

        if (!IsPostBack)
        {
            cpage = 1;
            allCount = 0;
            getData(cpage);
        }
    }

    private void getData(int page)
    {
        getData(page, "", "", "");
    }


    /// <summary>
    /// 获取公文列表绑定数据
    /// </summary>
    /// <param name="page">页数</param>
    /// <param name="key">关键字</param>
    /// <param name="sTime">开始日期</param>
    /// <param name="ETime">截至日期</param>
    private void getData(int page, string key, string sTime, string ETime)
    {

        //判断用户是否合法
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        //从request获取公文类型，0是未签公文，1是所有公文
        int gwtype;
        try
        {
            gwtype = Convert.ToInt32(Request["type"]);
        }
        catch
        {
            gwtype = 0;
        }
        
        
        //设置webservice传输header格式
        s.SjbgSoapHeaderValue = Security.getSoapHeader();
        Security.SetCertificatePolicy();

        
        //获取公文总数
        if (allCount == 0) allCount = s.getGongWenCount(user.GongHao, "", key, sTime, ETime, gwtype);

        //获取公文列表
        GongWenList[] gwlist = s.getGongWenList(user.GongHao, "", key, sTime, ETime, gwtype, (page - 1) * pageCount + 1, pageCount);

        //设置最大页数和当前页数
        maxPage = (int)((allCount - 0.1) / pageCount) + 1;
        if (page < 1) page = 1;
        if ((page - 1) * pageCount >= allCount) page = maxPage;
        cpage = page;
        
        //绑定数据
        gvList.DataSource = gwlist;
        gvList.DataBind();

        
        //设置分页按钮显示内容
        lblcurPage.Text = page.ToString();
        lblPageCount.Text = maxPage.ToString();
        if (page == 1)
        {
            cmdFirstPage.Enabled = false;
            cmdPreview.Enabled = false;
            if (maxPage > 1)
            {
                cmdNext.Enabled = true;
                cmdLastPage.Enabled = true;
            }
            else
            {
                cmdNext.Enabled = false;
                cmdLastPage.Enabled = false;
            }
        }
        else if (page >= maxPage)
        {
            cmdFirstPage.Enabled = true;
            cmdPreview.Enabled = true;
            cmdNext.Enabled = false;
            cmdLastPage.Enabled = false;
        }
        else
        {
            cmdFirstPage.Enabled = true;
            cmdPreview.Enabled = true;
            cmdNext.Enabled = true;
            cmdLastPage.Enabled = true;
        }
    }




    #region 翻页按钮功能区

    protected void First_Click(object sender, EventArgs e)
    {
        getData(1);
    }

    protected void Last_Click(object sender, EventArgs e)
    {
        getData(maxPage);
    }
    protected void Pre_Click(object sender, EventArgs e)
    {
        getData(cpage - 1);
    }
    protected void Next_Click(object sender, EventArgs e)
    {
        getData(cpage + 1);
    }
    protected void Custom_Click(object sender, EventArgs e)
    {
        int pcount = Convert.ToInt32(txtGoPage.Text);
        if (pcount > maxPage) pcount = maxPage;
        if (pcount < 1) pcount = 1;
        txtGoPage.Text = pcount.ToString();
        getData(pcount);
    }

    protected void Button1_Click(object sender, EventArgs e)
    {

        try
        {
            //GridView1.PageIndex = Convert.ToInt32(txtGoPage.Text) - 1;
        }
        catch
        {
            Response.Write(" <script> alert( '请输入正确的数字！ ') </script> ");
        }
    }
    #endregion


    protected void btnChaXun_Click(object sender, EventArgs e)
    {
        getData(1, txtBiaoTi.Text.Trim(), txtStart.Text.Trim(), txtEnd.Text.Trim());
    }
}
