using System;
using System.Web.UI.WebControls;
using gwxxService;



public partial class ListDuanWen : System.Web.UI.Page
{
    readonly gwxxWebService _s = new gwxxWebService();
    private static int _cpage;
    static int _maxPage;
    const int PageCount = 15;

    protected void Page_Load(object sender, EventArgs e)
    {
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }


        if (!IsPostBack)
        {
            _cpage = 1;
            GetData(_cpage);
            
        }
    }



    private void GetData(int page)
    {
        GetData(page, "", "", "");
    }


    /// <summary>
    /// 获取公文列表绑定数据
    /// </summary>
    /// <param name="page">页数</param>
    /// <param name="key">关键字</param>
    /// <param name="sTime">开始日期</param>
    /// <param name="eTime">截至日期</param>
    private void GetData(int page, string key, string sTime, string eTime )
    {

        //判断用户是否合法
        
        GongWenYongHu user = Session["user"] as GongWenYongHu;
        if (user == null)
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }

        
        //设置webservice传输header格式
        _s.SjbgSoapHeaderValue = Security.getSoapHeader();
        Security.SetCertificatePolicy();

        int uid = Convert.ToInt32(user.GongHao);
        //获取公文总数
        int allCount = _s.getDuanWenCount(uid, key, sTime, eTime);

        //获取公文列表
        GongWenList[] gwlist = _s.getDuanWenList(uid, key, sTime, eTime,(page - 1) * PageCount + 1, PageCount);

        //设置最大页数和当前页数
        _maxPage = (int)((allCount - 0.1) / PageCount) + 1;
        if (page < 1) page = 1;
        if ((page - 1) * PageCount >= allCount) page = _maxPage;
        _cpage = page;
        
        //绑定数据
        gvList.DataSource = gwlist;
        gvList.DataBind();

        
        //设置分页按钮显示内容
        lblcurPage.Text = page.ToString();
        lblPageCount.Text = _maxPage.ToString();
        if (page == 1)
        {
            cmdFirstPage.Enabled = false;
            cmdPreview.Enabled = false;
            if (_maxPage > 1)
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
        else if (page >= _maxPage)
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
        GetData(1);
    }

    protected void Last_Click(object sender, EventArgs e)
    {
        GetData(_maxPage);
    }
    protected void Pre_Click(object sender, EventArgs e)
    {
        GetData(_cpage - 1);
    }
    protected void Next_Click(object sender, EventArgs e)
    {
        GetData(_cpage + 1);
    }
    protected void Custom_Click(object sender, EventArgs e)
    {
        int pcount = Convert.ToInt32(txtGoPage.Text);
        if (pcount > _maxPage) pcount = _maxPage;
        if (pcount < 1) pcount = 1;
        txtGoPage.Text = pcount.ToString();
        GetData(pcount);
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
        GetData(1, txtBiaoTi.Text.Trim(), txtStart.Text.Trim(), txtEnd.Text.Trim());
    }
    protected void gvList_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        try
        {
            int lzid = Convert.ToInt32(e.Keys["LiuZhuanID"].ToString());
            GongWenYongHu user = Session["user"] as GongWenYongHu;
            if (user == null)
            {
                Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
                return;
            }
            int uid = Convert.ToInt32(user.GongHao);
            INT r = _s.undoGongWen2016(uid, lzid);
            if (r.Number != 1)
            {
                Response.Write(" <script> alert( '撤销签阅失败：" + r.Message + " ') </script> ");
                return;
            }
        }
        catch (Exception ex)
        {
            Response.Write(" <script> alert( '撤销签阅失败：" + ex.Message+ " ') </script> ");
            return;
        }
        Response.Write(" <script> alert( '撤销签阅成功，请重新签阅该文件。') </script> ");
        GetData(_cpage);
    }
}
