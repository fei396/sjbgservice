using System;
using gwxxService;

public partial class ListTongZhi : System.Web.UI.Page
    {
        readonly gwxxWebService s = new gwxxWebService();
        private static int _cpage;
        static int _maxPage;
        const int PageCount = 15;

        protected void Page_Load(object sender, EventArgs e)
        {
            s.SjbgSoapHeaderValue = Security.getSoapHeader();
            Security.SetCertificatePolicy();

            Session["uid"] = 50;

            int uid = Convert.ToInt32(Session["uid"]);
            //if (uid <1)
            //{
            //    Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            //    return;
            //}
            if (!IsPostBack)
            {
                gwxxService.TongZhiLeiXing[] tzlx = s.getAllTongZhiLeiXing();
                ddlLeiXing.DataSource = tzlx;
                ddlLeiXing.DataValueField = "LXID";
                ddlLeiXing.DataTextField = "LXMC";
                ddlLeiXing.DataBind();
                ddlLeiXing.SelectedValue = "0";
                _cpage = 1;
                GetData(_cpage);
            }
        }
        private void GetData(int page)
        {
            int lxid = Convert.ToInt32(ddlLeiXing.SelectedValue);
            GetData(page, "", "", "", lxid);
        }
        private void GetData(int page, string key, string sTime, string eTime, int lxid)
        {
            //判断用户是否合法
            int uid = Convert.ToInt32(Session["uid"]);

            //从request获取公文类型，0是未签公文，1是所有公文
            int tztype;
            try
            {
                tztype = Convert.ToInt32(Request["type"]);
            }
            catch
            {
                tztype = 0;
            }

            //设置webservice传输header格式

            //获取公文总数
            int fsrid = 0;

            int allCount = s.getTongZhiCount(uid, lxid, fsrid,key, sTime, eTime, tztype);

            //获取公文列表
            TongZhiList[] tzlist = s.getTongZhiList(uid, lxid,fsrid, key, sTime, eTime, tztype, (page - 1) * PageCount + 1, PageCount);

            //设置最大页数和当前页数
            _maxPage = (int)((allCount - 0.1) / PageCount) + 1;
            if (page < 1) page = 1;
            if ((page - 1) * PageCount >= allCount) page = _maxPage;
            _cpage = page;

            //绑定数据
            gvList.DataSource = tzlist;
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

        protected void btnChaXun_Click(object sender, EventArgs e)
        {
            int lxid = Convert.ToInt32(ddlLeiXing.SelectedValue);
            GetData(1, txtBiaoTi.Text.Trim(), txtStart.Text.Trim(), txtEnd.Text.Trim(), lxid);
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

    }

