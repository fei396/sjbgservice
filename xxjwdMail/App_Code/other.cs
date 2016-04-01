using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.IO;

namespace ASPNETAJAXWeb.AjaxMail
{
    public class other
    {


        //============================================================
        //public void FenLei(int iparentID, TreeNode TN)
        //{
        //    DataView dvTree = new DataView(ds.Tables[0]);
        //    dvTree.RowFilter = "[ParentID] = " + iparentID;
        //    foreach (DataRowView Row in dvTree)
        //    {
        //        if (TN == null)
        //        { //添加根节点 
        //            TreeNode Node = treeViewClass.Nodes.Add(Row[1].ToString());
        //            Node.Tag = Row[0].ToString();
        //            FenLei(Int32.Parse(Row["ClassID"].ToString()), Node);//进行递归调用 
        //        }
        //        else
        //        { //添加当前节点的子节点 
        //            TreeNode Node = TN.Nodes.Add(Row[1].ToString());
        //            Node.Tag = Row[0].ToString();
        //            FenLei(Int32.Parse(Row["ClassID"].ToString()), Node);//进行递归调用 
        //        }
        //    }

        //}
        //=====================================================================


        //返回当前的上传目录.
        public static string GetUpload()
        {
            return System.Web.HttpContext.Current.Server.MapPath("MailAttachments");
        }
        //返回当前的年份
        public static string GetYear()
        {
            return DateTime.Now.Year.ToString() + "年";
        }
        //返回当前的月份
        public static string GetMonth()
        {
            return DateTime.Now.Month.ToString() + "月";
        }
        //返回当前的天
        public static string GetDay()
        {
            return DateTime.Now.Day.ToString() + "日";
        }
        //返回数据库存储路径
        public static string DBLuJing()
        {
            return "MailAttachments/" + GetYear() + "/" + GetMonth() + "/" + GetDay() + "/";
        }


        //返回绝对路径为添加用
        public static string GetLuJing()
        {
            string lu = GetYear() + "/" + GetMonth() + "/" + GetDay();
            return lu;
        }
        //返回一个当前的年月天时总成的一个字符串
        public static string GetZong()
        {
            return GetYear() + GetMonth() + GetDay() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
        }

        //检测当前上传目录是否有指定的文件夹，没有则创建
        public static void CreateFile()
        {
            string lu = GetUpload() + "\\" + GetYear() + "\\" + GetMonth() + "\\" + GetDay();

            if (!Directory.Exists(lu))
            {
                Directory.CreateDirectory(lu);
            }

        }
       /* public static void CreateImgFile()
        {
            string lu = System.Web.HttpContext.Current.Server.MapPath("UploadImages");
            lu += "\\" + GetYear() + "\\" + GetMonth() + "\\" + GetDay();
            if (!Directory.Exists(lu))
            {
                Directory.CreateDirectory(lu);

            }

        }*/

        /// <summary>
        /// 检测判断用户登陆是否正确
        /// </summary>
        /// <param name="a">参数Ａ</param>
        /// <param name="b">参数Ｂ</param>
        /// <returns>返回Bool类型</returns>
       /* public static bool Check(object a, object b)
        {

            try
            {
                if (String.IsNullOrEmpty(a.ToString().Trim()) == true && a == null)
                {
                    return false;

                }
                else
                {
                    if (String.IsNullOrEmpty(b.ToString().Trim()) == true && b == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }*/

        /// <summary>
        /// 格式化字符．
        /// </summary>
        /// <param name="str">要格式化的内容</param>
        /// <returns></returns>
        //public static string UbbCode(string str)
        //{
        //    if (String.IsNullOrEmpty(str) == false)
        //    {
        //        str = str.Replace("\n", "<br/>");

        //    }
        //    return str;
        //}

        /// <summary>
        /// 反格式化字符
        /// </summary>
        /// <param name="str">反格式化的内容</param>
        /// <returns></returns>




        //截取指定的字符串。



        //==================================================================================================


        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="notEncryptStr">待加密的明文字符串</param>
        /// <returns>加密后的字符串</returns>
    

    }
}