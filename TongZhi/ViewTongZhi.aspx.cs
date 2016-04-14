using gwxxService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

    public partial class ViewTongZhi : System.Web.UI.Page
    {
        readonly gwxxWebService s = new gwxxWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["uid"] = 50;

            int uid = Convert.ToInt32(Session["uid"]);

            if (uid < 1)
            {
                Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
                return;
            }

            btnQianShou.Attributes["OnClick"] = "return confirm('确定签阅该文件？')";

            if (!IsPostBack)
            {
                int lzid;
                int tzid;
                int rid;
                try
                {
                    lzid = Convert.ToInt32(Request["lzid"]);
                    tzid = Convert.ToInt32(Request["tzid"]);
                }
                catch
                {
                    Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
                    return;
                }

                gwxxService.BuMenFenLei[] bmfl = s.getTongZhiBuMenFenLei(uid);
                Session["bmfl"] = bmfl;

                List<string> jsr = new List<string>();
                Session["jsr"] = jsr;

                GetData(tzid, lzid,uid);
            }
        }
        private void GetData(int tzid, int lzid, int uid)
        {
            Security.SetCertificatePolicy();
            gwxxService.TongZhi2016 tz = s.getTongZhi2016ByID(tzid);

            if (tz == null)
            {
                return;
            }
            else
            {
                lblBiaoti.Text = tz.BiaoTi;
                txtZhengWen.Text = tz.ZhengWen;
                lblFaWenDanWeiHeShiJian.Text = tz.FaBuRenXM + "              " + Convert.ToDateTime(tz.FaBuShiJian).ToString("yyyy年M月d日") + "发";

                if (tz.FuJian != null)
                {
                    string html = "";
                    for (int i = 0; i < tz.FuJian.Length; i++)
                    {
                        html += "<a style='font-size: large' href='gwfj/" + tz.FuJian[i] + "'>" + tz.FuJian[i] + "</a><br><br>";
                    }
                    tdFuJian.InnerHtml = html;
                }
            }
            GongWenBuMenRenYuan[] bmry = s.getTongZhiBuMenRenYuan(uid);
            CheckBoxList1.DataSource = bmry;
            CheckBoxList1.DataValueField = "Uid";
            CheckBoxList1.DataTextField = "XianShiMingCheng";
            CheckBoxList1.DataBind();

            BindLiuZhuanData(true, 0, lzid);
        }
        private void BindLiuZhuanData(bool sfbr, int lzlvl, int lzid)
        {
            int tongzhi;
            try
            {
                tongzhi = Convert.ToInt32(Request["tongzhi"]);
            }
            catch (Exception ex)
            {
                tongzhi = 0;
            }
            if (tongzhi != 0)
            {
                sfbr = false;
            }
            gwxxService.TongZhiLiuZhuan[] tzlz = s.getTongZhiLiuZhuanXian(sfbr, lzlvl, lzid);

            gvList.DataSource = tzlz;
            gvList.DataBind();
        }
   


        protected void btnQianShou_Click(object sender, EventArgs e)
        {
            List<int> jsr = Session["jsr"] as List<int>;

            int uid = Convert.ToInt32(Session["uid"]);
            if (uid < 1)
            {
                Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            }

            string pishi = txtQianShouNeiRong.Text.Trim();
            if (pishi.Equals(""))
            {
                pishi = "阅。";
            }
            int tzid;
            int lzid;

            try
            {
                tzid = Convert.ToInt32(Request["tzid"]);
                lzid = Convert.ToInt32(Request["lzid"]);
            }
            catch
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "签收公文失败", "alert('登录已过期。')", true);
                return;
            }
            string ip = getIP();
            INT i = s.signTongZhi2016(tzid, lzid, uid, jsr.ToArray(), pishi, ip);
            if (i.Number == 1)
            {
                jsr.Clear();
                Session["jsr"] = null;
                Session["bmfl"] = null;
                Page.ClientScript.RegisterStartupScript(GetType(), "签收公文成功", "alert('签收公文成功');window.open('mdefault.aspx','_parent');", true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "签收公文失败", "alert('签收公文失败：" + i.Message + "')", true);
            }
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