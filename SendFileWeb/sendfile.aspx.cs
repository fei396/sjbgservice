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
using SendFileService;
using System.Diagnostics;
using System.IO;

public partial class sendfile : System.Web.UI.Page
{
    static string fileContent = "";
    string filePath = System.Configuration.ConfigurationManager.AppSettings["TempFilePath"];
    protected void Page_Load(object sender, EventArgs e)
    {

        string s = Session["user"] as string;
        if (s == null || s == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
        }

        if (!IsPostBack)
        {
            initPage();


        }
        else
        {
            string sent = Session["sent"] as string;

            if (sent.Equals("sent"))
            {
                initPage();
            }
        }
    }
    public string testPrint()
    {
        Process pss = new Process();
        pss.StartInfo.CreateNoWindow = false;
        pss.StartInfo.FileName = @"D:\Program Files\FlashPaper\FlashPrinter.exe"; ;
        pss.StartInfo.Arguments = string.Format("{0} {1} -o {2}", @"D:\Program Files\FlashPaper\FlashPrinter.exe", @"D:\SendFileTemp\1.doc", @"D:\SendFileTemp\1.pdf");
        pss.StartInfo.UseShellExecute = false;
        pss.StartInfo.RedirectStandardInput = false;
        pss.StartInfo.RedirectStandardOutput = false;
        pss.StartInfo.CreateNoWindow = true;
        try
        {
            //log.Info("开始打印\r\n 源文件：" + strSourceFile + "\r\n目的文件:" + outPutFile);
            //StartTime = DateTime.Now;
            pss.Start();
            while (!pss.HasExited)
            {
                //PrintTime = DateTime.Now - StartTime;
                //if (PrintTime.Seconds > PrintTimerOut)
                //{
                //    throw new Exception("打印超时!");
                //}
                continue;
            }
            //System.Threading.Thread.Sleep(2000);
        }
        catch (Exception ex)
        {
            //杀掉进程
            //procClass.KillProcess(pss.Id);
            //log.Error(ex);
            //strMsg = "生成只读文件失败，请检查文件是否支持!";


            return ex.Message;

        }
        finally
        {
            pss.Close();
        }

        return "true";
    }

    public string testPrint(string fileName, string extName)
    {
        switch (extName)
        {
            case "doc":
            case "xls":
            case "txt":
                break;
            default:
                return "文件格式不正确";
        }
        string printPath = System.Configuration.ConfigurationManager.AppSettings["PrinterPath"];

        Process pss = new Process();
        pss.StartInfo.CreateNoWindow = false;
        pss.StartInfo.FileName = printPath;
        pss.StartInfo.Arguments = string.Format("{0} {1} -o {2}", printPath, filePath + fileName + "." + extName, filePath + fileName + ".pdf");
        pss.StartInfo.UseShellExecute = false;
        pss.StartInfo.RedirectStandardInput = false;
        pss.StartInfo.RedirectStandardOutput = false;
        pss.StartInfo.CreateNoWindow = true;
        try
        {
            //log.Info("开始打印\r\n 源文件：" + strSourceFile + "\r\n目的文件:" + outPutFile);
            //StartTime = DateTime.Now;
            pss.Start();
            while (!pss.HasExited)
            {
                //PrintTime = DateTime.Now - StartTime;
                //if (PrintTime.Seconds > PrintTimerOut)
                //{
                //    throw new Exception("打印超时!");
                //}
                continue;
            }
            //System.Threading.Thread.Sleep(2000);
        }
        catch (Exception ex)
        {
            //杀掉进程
            //procClass.KillProcess(pss.Id);
            //log.Error(ex);
            //strMsg = "生成只读文件失败，请检查文件是否支持!";


            return ex.Message;

        }
        finally
        {
            pss.Close();
        }

        return "true";
    }

    void initPage()
    {
        SendFileService.sendfileService ss = new sendfileService();
        ss.SjbgSoapHeaderValue = Security.getSoapHeader();
        int did =  Convert.ToInt32(Session["dept"] as string);
        if (did == 0)
        {
        }
        else
        {

            DutyRoom[] drs = ss.getDutyRoomByDeptId(did);
            for (int i = 0; i < drs.Length; i++)
            {
                ListItem li = new ListItem(drs[i].WeiZhi, drs[i].ID.ToString());
                lbDRn.Items.Add(li);
            }

        }
        fileContent = "";
        TextBox1.Visible = false;
        FileUpload1.Visible = true;
        TextBox1.Text = "";
        Label1.Text = "选择要发送的文件：";
        wjsmTextBox.Text = "";
        Session["sent"] = "NotSent";
    }


    protected void zhibiaozu1DDL_DataBound(object sender, EventArgs e)
    {
        //zhi1DDL.DataBind();
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

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (lbDR.Items.Count == 0)
        {
            Response.Write(" <script> alert( '请选择要发送的指导室！ ') </script> ");
            return;
        }

        if (fileContent == null || fileContent.Equals(""))
        {
            Response.Write(" <script> alert( '请先上传要发送的文件！ ') </script> ");
            return;
        }
        string miaoshu = wjsmTextBox.Text;

        string dutyRooms = "";
        foreach (ListItem li in lbDR.Items)
        {
            dutyRooms += li.Value + ",";
        }

        dutyRooms = dutyRooms.Substring(0, dutyRooms.Length - 1);


        sendfileService sfs = new sendfileService();
        sfs.SjbgSoapHeaderValue = Security.getSoapHeader();
        string fileSender =  Session["user"] as string;
        if (fileSender == null || fileSender == "")
        {
            Response.Redirect("error.aspx?errCode=登录已过期，请重新登录");
            return;
        }
        INT result = sfs.SendFile(Convert.ToInt32(fileSender), TextBox1.Text, miaoshu, fileContent, dutyRooms);
        if (result.Number == 1)
        {
            Response.Write(" <script> alert( '发送文件成功！ ') </script> ");
            //Response.Redirect("sendfile.aspx");
            Session["sent"] = "sent";
        }
        else
        {
            Response.Write(" <script> alert( '发送文件失败失败：" + result.Message + "  ') </script> ");
        }
    }
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        int blzbid = Convert.ToInt32(Request["fid"]);
        Response.Redirect("sendfile.aspx");
    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {

        if (FileUpload1.HasFile)
        {
            string fileFullName = FileUpload1.FileName;
            FileUpload1.SaveAs(filePath + fileFullName);
            int posOfDot = fileFullName.LastIndexOf(".");

            if (posOfDot <= 1)
            {
                Response.Write(" <script> alert( '文件格式不正确') </script> ");
                return;
            }
            string fileName = fileFullName.Substring(0, posOfDot);
            string extName = fileFullName.Substring(posOfDot + 1, fileFullName.Length - posOfDot - 1);
            switch (extName.ToLower())
            {
                case "jpg":
                case "bmp":
                case "png":
                case "pdf":
                    try
                    {
                        FileStream file = new FileStream(fileFullName, FileMode.Open);
                        byte[] buffer = new byte[file.Length];
                        file.Read(buffer, 0, Convert.ToInt32(file.Length));
                        //byte[] buffer = new byte[FileUpload1.PostedFile.ContentLength - 1];
                        //buffer = FileUpload1.FileBytes;
                        fileContent = Convert.ToBase64String(buffer);
                        file.Close();
                    }
                    catch
                    {
                        Response.Write(" <script> alert( '文件上传出错') </script> ");
                        return;
                    }
                    
                    break;
                case "doc":
                case "xls":
                    string message = FileConverter.FileConverter.ToPdf(filePath, fileName, extName);
                    extName = "pdf";
                    if (!message.Equals("true"))
                    {
                        Response.Write(" <script> alert( '" + message + "') </script> ");
                        return;
                    }
                    try
                    {
                        FileStream file = new FileStream(filePath + fileName + ".pdf", FileMode.Open);
                        byte[] buffer = new byte[file.Length];
                        file.Read(buffer, 0, Convert.ToInt32(file.Length));
                        //byte[] buffer = new byte[FileUpload1.PostedFile.ContentLength - 1];
                        //buffer = FileUpload1.FileBytes;
                        fileContent = Convert.ToBase64String(buffer);
                        file.Close();
                        
                    }
                    catch
                    {
                        Response.Write(" <script> alert( '文件转换出错') </script> ");
                        return;
                    }
                    
                    break;
                default:
                    Response.Write(" <script> alert( '不支持的文件格式') </script> ");
                    return;
            }


            TextBox1.Text = fileName + "." + extName;

            
            if (fileContent == null || fileContent.Equals(""))
            {
                TextBox1.Visible = false;
                FileUpload1.Visible = true;
                TextBox1.Text = "";
                Label1.Text = "选择要发送的文件：";
            }
            else
            {
                TextBox1.Visible = true;
                FileUpload1.Visible = false;
                btnUpload.Visible = false;
                Label1.Text = "上传成功";
                if (extName.Equals("pdf"))
                {
                    Label1.Text += "，已自动转换为PDF！";
                }
                else
                {
                    Label1.Text += "！";
                }
            }
        }
        else
        {
            Response.Write(" <script> alert( '请选择要发送的文件！ ') </script> ");
            return;
        }
    }
}
