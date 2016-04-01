using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASPNETAJAXWeb.AjaxMail;
using System.Data;
using System.Data.SqlClient;

namespace AjaxMail
{
    public partial class GroupList : System.Web.UI.Page
    {
        //string groupid = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["yhgh"] == null)
            {
                Response.Write("<script language=javascript>alert('对不起，请先登录！')</script>");
                Response.Redirect("Login.aspx");
            }
            if (!Page.IsPostBack)
            {
                BindPageData(Int32.Parse(Session["yhgh"].ToString()));
            }
        }

        protected void btnTag_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddGroup.aspx");
        }

        protected void gvMailbox_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvGroupView.PageIndex = e.NewPageIndex;
            BindPageData(Int32.Parse(Session["yhgh"].ToString()));
        }

        protected void gvMailbox_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "del")
            {
                Group group = new Group();
                if (group.DeleteGroup(Int32.Parse(e.CommandArgument.ToString())) > 0)
                {
                    BindPageData(Int32.Parse(Session["yhgh"].ToString()));
                }
            }
            if (e.CommandName.ToLower() == "editgroup")//如果为编辑状态打开编辑窗口
            {
                //groupid = e.CommandArgument.ToString();
               Server.Transfer("AddGroup.aspx");
                //Response.Write("<script language=javascript>alert('对不起，请先登录！')</script>");
            }
        }

        protected void gvMailbox_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            ImageButton imgDelete = (ImageButton)e.Row.FindControl("imgDelete");
            if (imgDelete != null)
            {
                imgDelete.Attributes.Add("onclick", "return confirm(\"您确定要删除当前行的群吗?\");");
            }
        }

        private void BindPageData(int gh)
        {
            Group group = new Group();
            DataSet ds = group.GetGroups(gh);
            gvGroupView.DataSource = ds;
            gvGroupView.DataBind();
        }

      /*  public string grouplistvalue
        {
            get
            {
                string aa = "1";
               // GridViewRow drv = ((GridViewRow)(((LinkButton)(gvGroupView.CommandSource)).Parent.Parent)); //此得出的值是表示那行被选中的索引值
                //int id = Convert.ToInt32(gvGroupView.DataKeys[drv.RowIndex].Value); //此获取的值为GridView中绑定数据库中的主键值
                return aa; 
            }
        }*/

        protected void gvGroupView_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }
    }
}