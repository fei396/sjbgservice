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
    public partial class AddMailbox : System.Web.UI.Page
    {
        string qunid = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["yhgh"] == null)
            {
                Response.Write("<script language=javascript>alert('对不起，请先登录！')</script>");
                Response.Redirect("Login.aspx");
            }
            if (!IsPostBack)
            {
                Person person = new Person();
                DataSet dr = person.GetBumen();
                if (dr == null) return;
                DropDownList1.DataSource = dr;
                DropDownList1.DataValueField = "bm_id";
                DropDownList1.DataTextField = "bm_mc";
                DropDownList1.DataBind();
                DropDownList1.Items.Insert(0, new ListItem("请选择部门", "0"));
                //Dropbind();
                //处理GroupList页面传过来的值。
                if (Request.QueryString["GroupID"] == null)
                { return; }
                else
                {


                    qunid = Request.QueryString["GroupID"].ToString();
                    qunidlb.Text = qunid;
                    if (qunid == "")  //
                    {
                        return;
                    }
                    else//如果有id号，取出数据，写入listbox2
                    {
                        Group group = new Group();
                        DataSet ds = group.GetSingleGroup(int.Parse(qunid));
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            tbgn.Text = ds.Tables[0].Rows[0]["GroupName"].ToString();
                            string split = ";";//多个邮箱分隔符
                            string[] toList = ds.Tables[0].Rows[0]["GroupMember"].ToString().Split(split.ToCharArray());//将多个邮箱收件人写入数组
                            for (int i = 0; i < toList.Length - 1; i++)
                            {
                                ListBox2.Items.Add(new ListItem(toList[i].Remove(0, 4), toList[i].Substring(0, 4)));
                               // ListBox1.Items.Add(toList[i].Substring(0, 4));
                            }
                        }
                        Button6.Text = "修改";
                    }
                }
                //结束



            }

 
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownList1.SelectedItem.Value != null)
            {
                Person person = new Person();
                // DataTable dt = new DataTable();
                DataSet dr = person.GetPersonByBmid(int.Parse(DropDownList1.SelectedItem.Value));
                DataTable dt = dr.Tables[0];
                ListBox1.DataSource = dt;
                ListBox1.DataTextField = "user_name";
                ListBox1.DataValueField = "user_no";
                ListBox1.DataBind();

            }
        }

        protected void btn_pymmQuery_Click(object sender, EventArgs e)
        {
            if (txt_pymm.Text.Length > 0)
            {
                //从数据库中按工号找出人员
                Person person = new Person();
                // DataTable dt = new DataTable();
                DataSet dr = person.GetPersonByGh(int.Parse(txt_pymm.Text));
                DataTable dt = dr.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    //ListBox1.DataSource =dr;
                    ListItem item = new ListItem();
                    item.Text = dt.Rows[0]["user_name"].ToString();
                    item.Value = dt.Rows[0]["user_no"].ToString();
                    ListBox1.Items.Add(item);
                    //ListBox1.DataBind();
                }
                //结束
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (ListBox1.SelectedItem != null)
            {
                AddItemFromSourceListBox(ListBox1, ListBox2);

                RemoveSelectedItem(ListBox1);
            }
            else
            {
                if (ListBox1.SelectedItem == null)
                {
                    int count = ListBox1.Items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        ListBox2.Items.Add(ListBox1.Items[i]);
                    }
                    ListBox1.Items.Clear();
                }

            }

            //统计选中的多少用户.
            lblsum.Text = "选中了：" + ListBox2.Items.Count + "个用户";
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            if (ListBox2.SelectedItem != null)
            {
                AddItemFromSourceListBox(ListBox2, ListBox1);
                RemoveSelectedItem(ListBox2);

            }
            else
            {
                if (ListBox2.SelectedItem == null)
                {
                    if (ListBox2.SelectedItem == null)
                    {
                        int count = ListBox2.Items.Count;
                        for (int i = 0; i < count; i++)
                        {
                            ListBox1.Items.Add(ListBox2.Items[i]);
                        }
                        ListBox2.Items.Clear();
                    }

                }
            }
            //统计选中的多少用户.
            lblsum.Text = "选中了：" + ListBox2.Items.Count + "个用户";
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (ListBox1.SelectedIndex != -1)
            {
                ListItem item = new ListItem(this.ListBox1.SelectedItem.Text, this.ListBox1.SelectedItem.Value);
                if (!IsTrue(ListBox2, ListBox1.SelectedItem.Value))
                {
                    this.ListBox2.Items.Add(item);
                    this.ListBox1.Items.Remove(item);

                }

            }

            //统计选中的多少用户.
            lblsum.Text = "选中了：" + ListBox2.Items.Count + "个用户";
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            if (this.ListBox2.SelectedIndex != -1)
            {
                ListItem item = new ListItem(this.ListBox2.SelectedItem.Text, this.ListBox2.SelectedItem.Value);
                if (!IsTrue(ListBox1, ListBox2.SelectedItem.Value))
                {
                    this.ListBox1.Items.Add(item);
                    this.ListBox2.Items.Remove(item);
                }
            }
            //统计选中的多少用户.
            lblsum.Text = "选中了：" + ListBox2.Items.Count + "个用户";
        }

        protected void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBox1.SelectedItem.Value == "0")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'请选择部门！\');</script>");
                return;
            }
            else
            {
                if (ListBox1.SelectedItem.Value == "请选择部门") return;
                ListItem item = new ListItem(this.ListBox1.SelectedItem.Text, this.ListBox1.SelectedItem.Value);
                if (!IsTrue(ListBox2, ListBox1.SelectedItem.Value))
                {
                    this.ListBox2.Items.Add(item);
                    this.ListBox1.Items.Remove(item);

                }
            }
        }

        protected void Button5_Click(object sender, EventArgs e)
        {

        }

        protected void Button6_Click(object sender, EventArgs e)
        {
            if (tbgn.Text == "")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'请输入群名称！\');</script>");
                tbgn.Focus();
                return;
            }
            if (Button6.Text == "添加")
            {
                string zhu, zhuID;
                zhu = "";
                zhuID = "";

                //如果发件人超过60个，则不让发。
                if (ListBox2.Items.Count > 100)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'同时发送人数不能超过100人\');history.back();</script>");
                }
                else
                {
                    foreach (ListItem item in ListBox2.Items)
                    {

                        zhu += item.Value.Trim() + item.Text.Trim() + ";";
                        zhuID += item.Value.Trim() + ";";

                    }

                    LblName.Text = zhu;
                    LblID.Text = zhuID;
                    try
                    {
                        Group group = new Group();
                        int groupID = group.AddGroup(Session["yhgh"].ToString(), tbgn.Text, zhu);
                        if (groupID <= 0) return;
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'增加群成功！\');</script>");
                            tbgn.Text = "";
                            txt_pymm.Text = "";
                            ListBox1.Items.Clear();
                            ListBox2.Items.Clear();
                            lblsum.Text = "";
                            LblName.Text = "";
                            LblID.Text = "";
                        }
                        // mailbox.AddMailAttachment(fileNameString, url, mailID);

                    }
                    catch (Exception ex) { throw new Exception(ex.Message, ex); }

                }
                // Server.Transfer("SendMail.aspx", true);
            }
            else//群编辑状态
            {

                string zhu, zhuID;
                zhu = "";
                zhuID = "";

                //如果发件人超过60个，则不让发。
                if (ListBox2.Items.Count > 100)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'同时发送人数不能超过100人\');history.back();</script>");
                }
                else
                {
                    foreach (ListItem item in ListBox2.Items)
                    {

                        zhu += item.Value.Trim() + item.Text.Trim() + ";";
                        zhuID += item.Value.Trim() + ";";

                    }

                    LblName.Text = zhu;
                    LblID.Text = zhuID;
                    try
                    {
                        Group group = new Group();
                       // tbgn.Text = qunidlb.Text;
                       // return;
                        int groupID = group.UpdateGroup(int.Parse(qunidlb.Text), tbgn.Text, zhu);
                        if (groupID <= 0) return;
                        else
                        {
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "<script>alert(\'修改群成功！\');</script>");
                            tbgn.Text = "";
                            txt_pymm.Text = "";
                            ListBox1.Items.Clear();
                            ListBox2.Items.Clear();
                            lblsum.Text = "";
                            LblName.Text = "";
                            LblID.Text = "";
                        }
                        // mailbox.AddMailAttachment(fileNameString, url, mailID);

                    }
                    catch (Exception ex) { throw new Exception(ex.Message, ex); }

                }
                Response.Redirect("GroupList.aspx");
                // Server.Transfer("SendMail.aspx", true);
            }
        }
        protected void AddItemFromSourceListBox(ListBox sourceBox, ListBox targetBox)
        {
            foreach (ListItem item in sourceBox.Items)
            {
                if (item.Selected == true && !targetBox.Items.Contains(item))
                {
                    targetBox.Items.Add(item);
                }
            }
        }
        private void RemoveSelectedItem(ListBox listControl)
        {
            while (listControl.SelectedIndex != -1)
            {
                listControl.Items.RemoveAt(listControl.SelectedIndex);
            }
        }

        protected bool IsTrue(ListBox lb, string value)
        {
            bool m = false;

            for (int i = 0; i < lb.Items.Count; i++)
            {
                if (lb.Items[i].Value == value)
                {
                    return m = true;
                }
            }

            return m;
        }

        protected void fhbtn_Click(object sender, EventArgs e)
        {
            Response.Redirect("GroupList.aspx");
        }

    }
}