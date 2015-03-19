using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;
using System.Data;

namespace sjbgWebService
{
    /// <summary>
    /// yunyongService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://sjbg.xxjwd.org/")]

    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [SoapRpcService]
    public class sendfileService : System.Web.Services.WebService
    {
		public SjbgSoapHeader sjbgHeader = new SjbgSoapHeader();

        
        /// <summary>
        /// 获取uid的文件接收列表
        /// </summary>
        /// <param name="uid">人员工号</param>
        /// <param name="type">文件列表类型1:所有文件;0未接收文件</param>
        /// <returns></returns>
        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public ReceiveFile[] getFileListToReceive(int uid , int type,int ksxh,int count)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getFilesToReceive(uid,type,ksxh,count);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public SentFileList[] getSentFileList(int uid )
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getSentFiles(uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public SentFileDetail[] getSentFileDetails(int fid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getSentFileDetails(fid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public string getFileToReceive(int fid , string extName)
        {
            if (!sjbgHeader.checkValid()) return null;
            FileStream fs = new FileStream(SjbgConfig.SendFilePath  + fid.ToString() + "." + extName, FileMode.Open, FileAccess.Read);

            byte[] b = new byte[(int)fs.Length];
            int k = fs.Read(b, 0, (int)fs.Length);
            fs.Close();
            string base64 = Convert.ToBase64String(b);
            return base64;
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public string getFileToReceiveFromDataBase(int fid)
        {
            if (!sjbgHeader.checkValid()) return null;
            //FileStream fs = new FileStream(SjbgConfig.SendFilePath + fid.ToString() + "." + extName, FileMode.Open, FileAccess.Read);
            return DAL.getFileToReceiveFromDataBase(fid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT receiveFile(int fdrid, int uid)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.receiveFile(fdrid, uid);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public Department[] getSendFileDept( int did)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getDeptsByDeptId(did);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public int getDeptByUserNo(string userno)
        {
            if (!sjbgHeader.checkValid()) return 0;
            return BLL.getUserByNum(userno).UserDept;
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public DutyRoom[] getDutyRoomByDeptId(int did)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.getDutyRoomByDeptId(did);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT SendFile(int sender, string fileFullName, string fileDesc,string fileContent,string receivers)
        {
            if (!sjbgHeader.checkValid()) return null;
            return BLL.SendFile(sender,fileFullName,fileDesc ,fileContent,receivers);
        }

        [SoapHeader("sjbgHeader", Direction = SoapHeaderDirection.In)]
        [SoapRpcMethod, WebMethod]
        public INT SendFiletest(string sender,string topic,string content,int type)
        {
            if (!sjbgHeader.checkValid()) return null;
            return DAL.sendMqttMessage(sender, topic, content, type);
        }
    }
}
