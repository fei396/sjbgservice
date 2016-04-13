using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using sjbgWebService.gwxx;
using AE.Net.Mail.Imap;
using AE.Net.Mail;


namespace sjbgWebService
{

    // ReSharper disable once InconsistentNaming
    public static class DAL
    {
        private static readonly string GwConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["gwxxConnectionString"].ConnectionString;
        private static readonly string BaseConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["centerConnectionString"].ConnectionString;
        private static readonly string YyConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["yyConnectionString"].ConnectionString;
        private static readonly string ZbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["zbConnectionString"].ConnectionString;
        private static readonly string YsConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ysConnectionString"].ConnectionString;
        private static string _cbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["cbConnectionString"].ConnectionString;
        private static readonly string MqttConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["mqttConnectionString"].ConnectionString;

        private static bool _sendMessageForDebug = false;


        public static DataTable GetProductByPid(string pname)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select xt_id,xt_mc,webServiceURL from dic_yyxt where xt_mc=@pName";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("pName", pname);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            if (dt.Rows.Count == 0)
            {
                dt.TableName = "empty!";
                return dt;
            }
            dt.TableName = "product";
            return dt;
        }

        internal static DataTable GetXwlx()
        {
            DataTable dt = new DataTable();

            string sql = "SELECT id, sclb FROM dic_wlsclb where isvalid=1";
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "xwlx";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        public static DataTable GetXwlb(int xwlx)
        {
            DataTable dt = new DataTable();

            string sql = "SELECT     id, WYBT, WJMC, wjnr, SCRQ, SCLB,sclbid, yhbm,path FROM V_SJBG_wlscxx where isvalid=1 and sclbid=@xwlx order by scrq desc";
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("xwlx", xwlx);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "xwlb";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        public static BOOLEAN IsSigned(GongWen gw, UserGw user)
        {
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            if (user.Yhqx == 6) //领导
            {
                comm.CommandText = "select count(*) from dat_sqwj where wh=@wh and qyr=@qyr";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("wh", gw.Number);
                comm.Parameters.AddWithValue("qyr", user.Yhbh);
            }
            else //中层
            {
                comm.CommandText = "select count(*) from dat_qswj where qsbz=0 and wh=@wh ";
                if (user.ShuangQian == 1)
                {
                    comm.CommandText += " and (qsdw=@qsdw or qsdw=@qsdw2 )";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("wh", gw.Number);
                    comm.Parameters.AddWithValue("qsdw", user.Ssbm);
                    comm.Parameters.AddWithValue("qsdw2", user.Ssxzbm);
                }
                else
                {
                    comm.CommandText += " and (qsdw=@qsdw)";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("wh", gw.Number);
                    comm.Parameters.AddWithValue("qsdw", user.Ssbm);
                }

            }
            try
            {
                conn.Open();
                int i = Convert.ToInt32(comm.ExecuteScalar());
                if (i > 0) return new BOOLEAN(false, "");
            }
            catch (Exception ex)
            {
                return new BOOLEAN(true, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new BOOLEAN(true, "");
        }

        public static BOOLEAN LeaderSign(GongWen gw, UserGw user, Instruction ins, UserGw[] nextUsers)
        {
            string wh = gw.Number;
            if (wh.Equals("")) return new BOOLEAN(false, "");
            if (user.Yhbh == 0) return new BOOLEAN(false, "");
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            SqlTransaction trans;
            conn.Open();
            trans = conn.BeginTransaction();
            comm.Transaction = trans;
            try
            {


                //把文件信息的qybz改为 true，qybz在第一位领导签阅之前是false
                comm.CommandText = "update dat_wjxx set qybz = 1 where qybz=0 and wh = @wh";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("wh", wh);
                comm.ExecuteNonQuery();

                //把dat_sqwj里面的信息删除
                comm.CommandText = "delete from dat_sqwj where wh=@wh and qyr=@uid";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("wh", wh);
                comm.Parameters.AddWithValue("uid", user.Yhbh);
                comm.ExecuteNonQuery();

                //把领导批示添加进dat_ldps
                comm.CommandText = "insert into dat_ldps (wh,psr,psnr,psrq) values(@wh,@psr,@psnr,@psrq)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("wh", wh);
                comm.Parameters.AddWithValue("psr", user.Yhsm);
                comm.Parameters.AddWithValue("psnr", ins.Content);
                comm.Parameters.AddWithValue("psrq", DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"));
                comm.ExecuteNonQuery();

                //把下一步批示人添加进dat_sqwj
                foreach (UserGw nUser in nextUsers)
                {
                    comm.CommandText = "insert into dat_sqwj (wh,qyr) values(@wh,@uid)";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("wh", wh);
                    comm.Parameters.AddWithValue("uid", nUser.Yhbh);
                    comm.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new BOOLEAN(false, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new BOOLEAN(true, "");
        }
        public static BOOLEAN Sign(GongWen gw, UserGw user)
        {
            string wh = gw.Number;
            if (wh.Equals("")) return new BOOLEAN(false, "");
            if (user.Yhbh == 0) return new BOOLEAN(false, "");
            SqlConnection conn = new SqlConnection(GwConnStr);
            string sql = "update dat_qswj set qsbz = 1, qsr = @qsr ,qsrq='" + DateTime.Now.ToString("yyyy年MM月dd日HH时mm分") + "' where wh = @wh ";
            if (user.ShuangQian == 1)
            {
                sql += " and (qsdw = @qsdw or qsdw= @qsdw2) ";

            }
            else
            {
                sql += " and qsdw = @qsdw";
            }
            SqlCommand comm = new SqlCommand(sql, conn);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("qsr", user.Yhbh);
            comm.Parameters.AddWithValue("wh", wh);
            comm.Parameters.AddWithValue("qsdw", user.Ssbm);
            if (user.ShuangQian == 1)
            {
                comm.Parameters.AddWithValue("qsdw2", user.Ssxzbm);
            }
            int i = 0;
            try
            {
                conn.Open();
                i = comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new BOOLEAN(false, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            if (i == 0) return new BOOLEAN(false, "");
            return new BOOLEAN(true, "");
        }

        public static DataTable GetLdps(GongWen gw)
        {
            string wh = gw.Number;
            DataTable dt = new DataTable();

            string sql = "select id,wh,psr,psnr,psrq from dat_ldps where wh=@wh order by id";
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("wh", wh);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "Ldps";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        public static DataTable GetAllGwlb(UserGw user, gwlx gwlx, dwlx dwlx)
        {
            DataTable dt = new DataTable();

            string sql = "select a.id,a.wh,a.bt,a.fwdw,a.fwrq from dat_wjxx a ";
            if (user.Yhqx == 6) //领导
            {
                sql += " where 1=1 ";
            }
            else
            {
                sql += " , dat_qswj b where a.wh=b.wh and (b.qsdw = '" + user.Ssbm + "' or b.qsdw = '" + user.Ssxzbm + "') ";
            }
            switch (gwlx)
            {
                case gwlx.XZ://行政
                    sql += " and a.wjxz='行政' ";
                    break;
                case gwlx.DQ://党群
                    sql += " and a.wjxz='党群' ";
                    break;
                default:
                    break;
            }
            switch (dwlx)
            {
                case dwlx.LJ://局文
                    sql += " and a.lwlx='路局' ";
                    break;
                case dwlx.DW://段文
                    sql += " and a.lwlx='段文' ";
                    break;
                default:
                    break;
            }
            sql += " order by fwrq desc,id desc";
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "gwlb";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        public static DataTable GetUnfinishedGwlb(UserGw user, dwlx dwlx)
        {
            DataTable dt = new DataTable();

            string sql = "select a.id,a.wh,a.bt,a.fwdw,a.fwrq from dat_wjxx a ";
            if (user.Yhqx == 6) //领导
            {
                sql += " , dat_sqwj b where a.wh=b.wh and b.qyr=" + user.Yhbh;
            }
            else
            {
                sql += " , dat_qswj b where a.wh=b.wh and b.qsbz=0 ";
                if (user.ShuangQian == 1)
                {
                    sql += " and (b.qsdw= " + user.Ssbm + " or b.qsdw= " + user.Ssxzbm + ")";
                }
                else
                {
                    sql += "and b.qsdw = " + user.Ssbm;
                }
            }

            switch (dwlx)
            {
                case dwlx.LJ://局文
                    sql += " and a.lwlx='路局' ";
                    break;
                case dwlx.DW://段文
                    sql += " and a.lwlx='段文' ";
                    break;
                default:
                    break;
            }
            sql += " order by fwrq desc,a.id desc";
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "gwlb";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        public static DataTable GetGwxxByWh(string wh)
        {
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select id,ht,wh,bt,zw,fwdw,fwrq,fj1,fj2,fj3,fj4,fj5,fj6,wjxz,csyj,lwlx from dat_wjxx where wh=@wh";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("wh", wh);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            if (dt.Rows.Count == 0)
            {
                dt.TableName = "empty!";
                return dt;
            }
            dt.TableName = "wh";
            return dt;
        }

        public static DataTable GetUserGwByUid(int uid)
        {
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select yhbh,yhmc,yhsm,yhnc,ssbm,yhmm,yhqx,wjxz,dld_order,shuangqian from dic_yhxx where yhbh=@uid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            if (dt.Rows.Count == 0)
            {
                dt.TableName = "empty!";
                return dt;
            }
            dt.TableName = "usergw";
            return dt;
        }

        public static DataTable GetUserGwByUserName(string userName)
        {
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select yhbh,yhmc,yhsm,ssbm,yhmm,yhqx,wjxz,dld_order,gh,ssxzbm,shuangqian from dic_yhxx where yhmc=@userName";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("userName", userName);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            if (dt.Rows.Count == 0)
            {
                dt.TableName = "empty!";
                return dt;
            }
            dt.TableName = "usergw";
            return dt;
        }

        public static int GetSsxzbmBySsbm(int ssbm)
        {
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select ssxzbm from dic_bmxx where bmbh=@ssbm";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("ssbm", ssbm);
            try
            {
                conn.Open();
                return Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool SetPassword(int uid, string pass)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update dic_user set user_mima=@pass where user_no=@work_no";
            string workNo = uid.ToWorkNo();
            pass = BLL.SetEncryptPass(workNo, pass);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("pass", pass);
            comm.Parameters.AddWithValue("work_no", workNo);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }

            return true;
        }

        public static bool InitPassword()
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select user_no from dic_user where bumen_id=34";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string workNo = Convert.ToString(dt.Rows[i]["user_no"]);
                string pass = BLL.SetEncryptPass(workNo, workNo);
                comm.Connection = conn;
                comm.CommandText = "update dic_user set user_mima=@pass where user_no=@work_no";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("pass", pass);
                comm.Parameters.AddWithValue("work_no", workNo);
                try
                {
                    conn.Open();
                    comm.ExecuteNonQuery();
                }
                catch
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
            return true;
        }

        public static bool SetTqjyPassword(int uid, string newpass)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            string pass = BLL.SetEncryptPass(uid.ToString(), newpass);
            comm.Connection = conn;
            comm.CommandText = "update T_TQYJ_User set pwd=@pwd where uid=@uid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("pwd", pass);
            comm.Parameters.AddWithValue("uid", uid);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }

            return true;
        }

        public static string GetProductUserIdByBaseNum(string userNo, int pid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select is_work_no_login from dic_yyxt where xt_id=@pid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("pid", pid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                return null;
            }
            if (dt.Rows.Count != 1)
            {
                return null;
            }
            if (Convert.ToInt32(dt.Rows[0]["is_work_no_login"]) == 1)
            {
                return userNo;
            }
            else
            {
                comm.CommandText = "select yyxt_userbh from dat_UserDuiYing where yyxt_id=@pid and xtbg_user=@user_no";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("pid", pid);
                comm.Parameters.AddWithValue("user_no", userNo);
                try
                {
                    dt = new DataTable();
                    sda.Fill(dt);
                }
                catch
                {
                    return null;
                }
                if (dt.Rows.Count != 1)
                {
                    return null;
                }
                return Convert.ToString(dt.Rows[0]["yyxt_userbh"]);
            }
        }

        public static DataTable GetUserByNum(string userNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select user_no,user_name,Bumen_id from dic_user where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", userNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            if (dt.Rows.Count != 1)
            {
                dt.TableName = "empty!";
                return dt;
            }
            dt.TableName = "user";
            return dt;
        }

        internal static INT LoginDirect(string userNo, string userPass, string code, string ip, string deviceInfo, string deviceVer)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            DataTable dt = new DataTable();
            comm.Connection = conn;

            comm.CommandText = "select user_mima from dic_user where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", userNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);

            try
            {
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            if (dt.Rows.Count != 1)
            {
                return new INT(-2, "工号不存在");//用户不存在
            }
            else
            {
                string pass1 = Convert.ToString(dt.Rows[0]["user_mima"]);
                userPass = BLL.SetEncryptPass(userNo, userPass);
                if (!userPass.Equals(pass1)) return new INT(-3, "密码错误");//密码错误
            }

            comm.CommandText = "select mobile from dat_user_mobile where work_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", userNo);
            conn.Open();
            string mobile = comm.ExecuteScalar().ToString();


            comm.CommandText = "insert into dat_User_Device (work_no,deviceID,mobile, isvalid) values(@user_no,@code,@mobile,1)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", userNo);
            comm.Parameters.AddWithValue("code", code);
            comm.Parameters.AddWithValue("mobile", mobile);
            comm.ExecuteNonQuery();
            conn.Close();
            return RecordLogin(userNo, code, ip, deviceInfo, deviceVer);

        }

        public static INT Login(string userNo, string userPass, string code, string ip, string deviceInfo, string deviceVer)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            DataTable dt = new DataTable();
            comm.Connection = conn;

            comm.CommandText = "select user_mima from dic_user where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", userNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);

            try
            {
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            if (dt.Rows.Count != 1)
            {
                return new INT(-2, "工号不存在");//用户不存在
            }
            else
            {
                string pass1 = Convert.ToString(dt.Rows[0]["user_mima"]);
                userPass = BLL.SetEncryptPass(userNo, userPass);
                if (!userPass.Equals(pass1)) return new INT(-3, "密码错误");//密码错误
            }

            comm.CommandText = "select * from dat_User_Device where work_no=@user_no and isvalid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", userNo);
            dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            if (dt.Rows.Count == 0)
            {
                return new INT(-1, "该设备未注册");//设备未注册
            }
            else
            {
                bool isin = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string codeDatabase = Convert.ToString(dt.Rows[i]["deviceID"]);
                    if (code.Equals(codeDatabase))
                    {
                        isin = true;
                        break;
                    }
                }
                if (isin == false)
                {
                    if (HasLoginInfo(userNo, deviceInfo, deviceVer))
                    {
                        comm.CommandText = "insert into dat_user_device (work_no, deviceID,mobile,isValid) values(@work_no, @did ,'1234567890',1)";
                        comm.Parameters.Clear();
                        comm.Parameters.AddWithValue("work_no", userNo);
                        comm.Parameters.AddWithValue("did", code);
                        try
                        {
                            conn.Open();
                            comm.ExecuteNonQuery();
                        }
                        catch
                        {
                            return new INT(-100, "数据库错误。");
                        }
                        finally
                        {
                            conn.Close();
                        }

                    }
                    else
                    {
                        return new INT(-1, "该设备未注册");
                    }
                }
                return RecordLogin(userNo, code, ip, deviceInfo, deviceVer);
            }
        }

        private static bool HasLoginInfo(string workNo, string dInfo, string dVer)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.CommandText = "select count(*) from dat_logininfo where uid=@work_no and  deviceInfo=@dInfo and deviceVer = @dVer";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("dInfo", dInfo);
            comm.Parameters.AddWithValue("dVer", dVer);
            comm.Connection = conn;
            conn.Open();
            int i = Convert.ToInt32(comm.ExecuteScalar());
            if (i >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static INT RecordLogin(string userNo, string code, string ip, string deviceInfo, string deviceVer)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into dat_logininfo(uid,deviceId,deviceInfo,deviceVer,ipAddress) values(@user_no,@code,@deviceInfo,@deviceVer,@ip)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", userNo);
            comm.Parameters.AddWithValue("code", code);
            comm.Parameters.AddWithValue("deviceInfo", deviceInfo);
            comm.Parameters.AddWithValue("deviceVer", deviceVer);
            comm.Parameters.AddWithValue("ip", ip);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();

            }
            return new INT(1, "");
        }

        public static DataTable GetZbPerson(DateTime dateTime, int isNight)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select bm,zbrgh,zbrxm,zbrq, zblb,sjh,zbdh,qdsj from dat_ziban where bm<>'段领导' and zbrq=@zbrq";
            if (isNight == 0) comm.CommandText += " and zblb='白班'";
            else if (isNight == 1) comm.CommandText += " and zblb='夜班'";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("zbrq", dateTime);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            if (dt.Rows.Count == 0)
            {
                dt.TableName = "empty!";
                return dt;
            }
            dt.TableName = "zb";
            return dt;
        }

        public static string GetZbLdps(DateTime dateTime)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select ldps from dat_ziban where zbrq=@zbrq and bm='段领导'";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("zbrq", dateTime);
            try
            {
                conn.Open();
                return Convert.ToString(comm.ExecuteScalar());
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        internal static DataTable GetLeaderList()
        {
            DataTable dt = new DataTable();
            string sql = "select yhbh,yhmc,yhsm,yhnc,ssbm,yhmm,yhqx,wjxz,dld_order,shuangqian from V_BanZiChengYuan order by dld_order";
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "leaderList";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        internal static DataTable GetGwbmList(int lbid)
        {
            DataTable dt = new DataTable();
            string sql = "select bmbh,bmmc,lbmc,ssxzbm,dic_bmlb.id as lbid from dic_bmlb,dic_bmxx where dic_bmlb.id = dic_bmxx.bmlb and dic_bmlb.id=@lbid order by dic_bmlb.orders,dic_bmxx.orders";
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("lbid", lbid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "bmList";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        internal static DataTable GetGwbmlbList()
        {
            DataTable dt = new DataTable();
            string sql = "select lbmc ,id from dic_bmlb order by orders";
            SqlConnection conn = new SqlConnection(GwConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "bmlbList";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        internal static DataTable GetJianBaoBuMen()
        {
            DataTable dt = new DataTable();
            string sql = "select bmmc from dic_jb_bm order by orders";
            SqlConnection conn = new SqlConnection(ZbConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "jbbm";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        internal static DataTable GetJianBao(string dept, DateTime datetime)
        {
            DataTable dt = new DataTable();
            string sql = "select bm ,jbxm,jbnr,jbrq,orders from dat_scjb INNER JOIN dic_jb_bm ON dat_scjb.bm = dic_jb_bm.bmmc where jbrq=@jbrq  and (bm=@bm or @bm='all') order by orders,dat_scjb.id";
            SqlConnection conn = new SqlConnection(ZbConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("jbrq", datetime);
            comm.Parameters.AddWithValue("bm", dept);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "JianBao";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        internal static INT GetTqUidByWorkNo(string workNo)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select uid from T_TQYJ_USER where work_no=@work_no and isValid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            try
            {
                conn.Open();
                return new INT(Convert.ToInt32(comm.ExecuteScalar()), "");
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        internal static INT GetTqUtypeByUid(int uid)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select utype from T_TQYJ_USER where uid=@uid and isValid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            try
            {
                conn.Open();
                return new INT(Convert.ToInt32(comm.ExecuteScalar()), "");
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        internal static INT GetTqUDeptByUid(int uid)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select deptid from T_TQYJ_USER where uid=@uid and isValid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            try
            {
                conn.Open();
                return new INT(Convert.ToInt32(comm.ExecuteScalar()), "");
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        internal static BOOLEAN ReplyTq(int uid, int tid, string replayContent)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select umid from T_TQYJ_USER_MESSAGE where uid=@uid and mid=@tid and isValid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            comm.Parameters.AddWithValue("tid", tid);
            try
            {
                conn.Open();
                int umid = Convert.ToInt32(comm.ExecuteScalar());
                return SetTqReply(umid, replayContent);
            }
            catch (Exception ex)
            {
                return new BOOLEAN(false, ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public static BOOLEAN SetTqReply(int umid, string txt)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            conn.Open();
            SqlTransaction sqlTran = conn.BeginTransaction();
            comm.Transaction = sqlTran;
            try
            {
                comm.CommandText = "update T_TQYJ_User_Message set readtime=getdate() where umid=" + umid.ToString();
                comm.ExecuteNonQuery();
                comm.CommandText = "select needReply from  V_TQYJ_User_Message where umid=" + umid.ToString();
                bool needReply = Convert.ToBoolean(comm.ExecuteScalar());
                comm.CommandText = "select umtype from  V_TQYJ_User_Message where umid=" + umid.ToString();
                int umType = Convert.ToInt32(comm.ExecuteScalar());
                if (needReply && umType == 1)//umtype=1表示是发送，umtype=2表示是抄送，抄送不需要回复
                {
                    comm.CommandText = "select count(*) from  T_TQYJ_Reply where umid = " + umid.ToString();
                    if (Convert.ToInt32(comm.ExecuteScalar()) == 0)
                    {
                        comm.CommandText = "insert into T_TQYJ_Reply (umid,txt) values(" + umid.ToString() + ",'" + txt + "')";
                        comm.ExecuteNonQuery();
                    }
                    else
                    {
                        comm.CommandText = "update T_TQYJ_Reply set txt='" + txt + "' where umid=" + umid.ToString();
                        comm.ExecuteNonQuery();
                    }
                }
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();
                return new BOOLEAN(false, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new BOOLEAN(true, "");
        }

        internal static DataTable GetTeQingByUid(int uid)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT [uid], [umid], [mid], [senderId],[mtitle], [rGh], [rName], [rRmark], [mtext], [sendTime], [needReply], [sGh], [sName], [sRemark], [deptid], [deptName], [readTime] FROM [V_TQYJ_User_Message] WHERE ([uid] = @uid) order by sendtime desc";
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "getTeQingByUid";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        internal static DataTable CheckReply(int uid)
        {
            DataTable dt = new DataTable();
            int udept = GetTqUDeptByUid(uid).Number;
            int utype = GetTqUtypeByUid(uid).Number;
            string sql;
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;

            sql = "SELECT deptName, sName, mid, mtitle, mtext, sendTime, sendCountl, readCount FROM V_TQYJ_Message_Count where senderid=@uid or (@utype=1 and @udept=1) or (@utype=1 and @udept=deptid) order by sendtime desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            comm.Parameters.AddWithValue("utype", utype);
            comm.Parameters.AddWithValue("udept", udept);



            comm.CommandText = sql;

            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "checkReply";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        internal static DataTable CheckReplyDetails(int tid)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT txt, rGh, rName, rRmark, mtext, sendTime, needReply,readTime,rdeptname FROM V_TQYJ_Message_Reply WHERE ([mid] = @mid)";
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = sql;
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("mid", tid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
                dt.TableName = "checkReplyDetails";
            }
            catch
            {
                dt.TableName = "error";
            }
            return dt;
        }

        internal static BOOLEAN SetNewPass(string userNo, string oldPass, string newPass)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            string pass = BLL.SetEncryptPass(userNo, newPass);
            comm.Connection = conn;

            //验证旧密码
            comm.CommandText = "select user_mima from dic_user where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", userNo);
            try
            {
                conn.Open();
                string passDataBase = Convert.ToString(comm.ExecuteScalar());
                if (!passDataBase.Equals(BLL.SetEncryptPass(userNo, oldPass)))
                {
                    return new BOOLEAN(false, "原密码不正确。");
                }
            }
            catch (Exception ex)
            {
                return new BOOLEAN(false, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            comm.CommandText = "update dic_user set user_mima=@pwd where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("pwd", pass);
            comm.Parameters.AddWithValue("user_no", userNo);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new BOOLEAN(false, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return new BOOLEAN(true, "");
        }

        internal static DateTime GetServerTime()
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            conn.Open();
            comm.CommandText = "select getdate()";
            return Convert.ToDateTime(comm.ExecuteScalar());
        }

        internal static INT InsertRegisterCode(int workNo, string mobile, string code, string uniqueCode)
        {
            string workno = workNo.ToString().PadLeft(4, '0');
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            conn.Open();
            SqlTransaction sqlTran = conn.BeginTransaction();
            comm.Transaction = sqlTran;
            try
            {
                //把原有code设置为无效
                comm.CommandText = "update dat_Register_Code set isValid=0 where work_no=@work_no and isValid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workno);
                comm.ExecuteNonQuery();

                //添加新code
                comm.CommandText = "insert into dat_Register_Code (work_no,mobile,code,deviceID,releaseTime,isValid) values(@work_no,@mobile,@code,@deviceID,@releaseTime,1)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workno);
                comm.Parameters.AddWithValue("mobile", mobile);
                comm.Parameters.AddWithValue("code", code);
                comm.Parameters.AddWithValue("deviceID", uniqueCode);
                comm.Parameters.AddWithValue("releaseTime", GetServerTime().AddMinutes(2));
                comm.ExecuteNonQuery();
                sqlTran.Commit();
            }
            catch (Exception ex)
            {
                sqlTran.Rollback();
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, "");
        }

        internal static INT SendMobileMessage(int workNo, string content)
        {
            return SendMobileMessage(workNo.ToWorkNo(), content);
        }

        internal static INT SendMobileMessage(string workNo, string content)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into dataexchange (data_no,data_content) values(@work_no,@content)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("content", content);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return new INT(1, "");
        }

        internal static INT SendMobileMessage(string[] workNo, string content)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;

                for (int i = 0; i < workNo.Length; i++)
                {
                    comm.CommandText = "insert into dataexchange (data_no,data_content) values(@work_no,@content)";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("work_no", workNo[i]);
                    comm.Parameters.AddWithValue("content", content);
                    comm.ExecuteNonQuery();
                }
                trans.Commit();
            }

            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return new INT(1, "");


        }

        internal static INT SendMobileMessage2(int[] uid, string content)
        {
            return new INT(1);
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;

                for (int i = 0; i < uid.Length; i++)
                {
                    comm.CommandText = "insert into T_DuanXin_NoReply (uid,txt) values(@uid,@txt)";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("uid", uid[i]);
                    comm.Parameters.AddWithValue("txt", content);
                    comm.ExecuteNonQuery();
                }
                trans.Commit();
            }

            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return new INT(1, "");


        }

        /// <summary>
        /// 检查用户手机号和移动设备时候已经注册。0未注册，1已注册，-100数据库错误，-1没有该工号,-2工号重复，-3没有该手机号，-4手机号重复，-5移动设备重复
        /// </summary>
        /// <param name="workNo">工号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="uniqueCode">移动设备唯一号</param>
        /// <returns>0未注册，1已注册，-100数据库错误，-1没有该工号,-2工号重复，-3没有该手机号，-4手机号重复，-5移动设备重复</returns>
        internal static INT CheckUserMobile(int workNo, string mobile, string uniqueCode)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            string workno = workNo.ToString().PadLeft(4, '0');
            //工号是否在人员信息库内
            comm.CommandText = "select count(*)  from dic_user where user_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workno);
            int i1;
            try
            {
                conn.Open();
                i1 = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            if (i1 == 0) return new INT(-1, "没有该工号");
            else if (i1 > 1) return new INT(-2, "工号重复");
            //手机号是否在手机信息库内
            comm.CommandText = "select count(*) from  dat_User_Mobile where work_no=@work_no and mobile=@mobile";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workno);
            comm.Parameters.AddWithValue("mobile", mobile);
            int i2;
            try
            {
                conn.Open();
                i2 = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            if (i2 == 0) return new INT(-3, "该手机号未登记为本段员工手机");
            else if (i2 > 1) return new INT(-4, "该手机号码重复");
            //设备是否已注册
            comm.CommandText = "select count(*) from  dat_User_Device where  deviceID=@deviceID and isValid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("deviceID", uniqueCode);
            int i3;
            try
            {
                conn.Open();
                i3 = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            if (i3 == 0) return new INT(0, "");
            else if (i3 > 1) return new INT(-5, "该设备已重复注册");

            return new INT(1, "该设备已经注册");
        }

        #region
        /*
		internal static int checkDevice(string work_no, string uniqueCode)
		{
			SqlConnection conn = new SqlConnection(baseConnStr);
			SqlCommand comm = new SqlCommand();
			comm.Connection = conn;
			comm.CommandText = "select count(*) from dat_User_Device where work_no = @work_no and  deviceID = @deviceID and isValid =1";
			comm.Parameters.Clear();
			comm.Parameters.AddWithValue("work_no", work_no);
			comm.Parameters.AddWithValue("deviceID", uniqueCode);
			int i = -1;
			try
			{
				conn.Open();
				i = Convert.ToInt32(comm.ExecuteScalar());
			}
			catch
			{
				return false;
			}
			finally
			{
				conn.Close();
			}
			if (i > 0) return true;
			else return false;
		}
		*/
        #endregion

        /// <summary>
        /// 注册移动设备
        /// </summary>
        /// <param name="workNo">工号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="uniqueCode">移动设备唯一号</param>
        /// <param name="rCode">注册码</param>
        /// <param name="sq">安全问题</param>
        /// <param name="sa">安全问题答案</param>
        /// <param name="email">邮件地址</param>
        /// <returns>0已注册,-98验证码不正确</returns>
        internal static INT RegisterDevice(int workNo, string mobile, string uniqueCode, string rCode, string sq, string sa, string email)
        {
            string workno = workNo.ToString().PadLeft(4, '0');
            INT i = CheckUserMobile(workNo, mobile, uniqueCode);
            if (i.Number < 0) return i;
            else if (i.Number == 1) return new INT(0, i.Message);
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;

            //判断验证码是否正确

            comm.CommandText = "select code from V_DAT_Valid_Register_Code where work_no=@work_no and deviceID=@deviceID and mobile = @mobile order by releaseTime desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workno);
            comm.Parameters.AddWithValue("deviceID", uniqueCode);
            comm.Parameters.AddWithValue("mobile", mobile);
            string rCode1;
            try
            {
                conn.Open();
                rCode1 = Convert.ToString(comm.ExecuteScalar());
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            if (!rCode.Equals(rCode1)) return new INT(-98, "验证码不正确");
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            comm.Transaction = trans;

            try
            {

                //注册码置为无效
                comm.CommandText = "update  dat_Register_Code set isValid=0 where work_no=@work_no and deviceID=@deviceID and mobile = @mobile";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workno);
                comm.Parameters.AddWithValue("deviceID", uniqueCode);
                comm.Parameters.AddWithValue("mobile", mobile);
                comm.ExecuteNonQuery();

                //注册数据入库
                comm.CommandText = "insert into dat_User_Device (work_no,deviceID,mobile,isValid) values(@work_no,@deviceID,@mobile,1)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workno);
                comm.Parameters.AddWithValue("deviceID", uniqueCode);
                comm.Parameters.AddWithValue("mobile", mobile);
                comm.ExecuteNonQuery();

                //更新email信息
                comm.CommandText = "update  dic_user set email=@email where user_no=@user_no";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("user_no", workno);
                comm.Parameters.AddWithValue("email", email);
                comm.ExecuteNonQuery();

                //安全问题入库
                comm.CommandText = "insert into dat_User_SecurityQuestion (work_no,Q,A,isValid) values(@work_no,@Q,@A,1)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workno);
                comm.Parameters.AddWithValue("Q", sq);
                comm.Parameters.AddWithValue("A", sa);
                comm.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, "");
        }

        internal static WenJianJia SelectMailBox(int uid, string mailBoxName)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            string strTrueMailBoxName = WenJianJia.NameToTrueName(mailBoxName);
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            Mailbox mb = ic.SelectMailbox(strTrueMailBoxName);
            ic.Disconnect();
            WenJianJia wjj = new WenJianJia();
            wjj.loadFrom(mb);
            return wjj;
        }

        internal static YouJian GetMailMessage(int uid, int muid, string mailBoxName)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            string strTrueMailBoxName = WenJianJia.NameToTrueName(mailBoxName);
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            ic.SelectMailbox(strTrueMailBoxName);
            MailMessage mm = ic.GetMessage(muid.ToString());

            YouJian yj = new YouJian();
            yj.loadFrom(mm);
            return yj;
        }

        internal static INT SendMail(int uid, YouJian yj)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Host = SjbgConfig.MailHost;
            smtp.Port = SjbgConfig.MailSmtpPort;
            smtp.Credentials = new System.Net.NetworkCredential(workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);

            System.Net.Mail.MailMessage mm = yj.ToSystemMail();



            try
            {
                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            return new INT(1, "");
        }

        internal static INT SendMail(int uid, int importance, string subject, string body, string from, string to, string cc, string bcc, string attachment)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "10.99.81.68";// SjbgConfig.MailHost;
            smtp.Port = 25;// SjbgConfig.MailSmtpPort;
            smtp.Credentials = new System.Net.NetworkCredential(workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);

            System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage();
            switch (importance)
            {
                case 1:
                    mm.Priority = System.Net.Mail.MailPriority.Low;
                    break;
                case 5:
                    mm.Priority = System.Net.Mail.MailPriority.High;
                    break;
                default:
                    mm.Priority = System.Net.Mail.MailPriority.Normal;
                    break;
            }
            mm.Subject = subject;
            mm.Body = body;
            mm.From = BLL.ToSysMailAddress(from)[0];
            List<System.Net.Mail.MailAddress> toList = BLL.ToSysMailAddress(to);
            if (toList != null)
                foreach (System.Net.Mail.MailAddress t in toList)
                {
                    mm.To.Add(t);
                }
            List<System.Net.Mail.MailAddress> ccList = BLL.ToSysMailAddress(cc);
            if (ccList != null)
                foreach (System.Net.Mail.MailAddress c in ccList)
                {
                    mm.CC.Add(c);
                }
            List<System.Net.Mail.MailAddress> bccList = BLL.ToSysMailAddress(bcc);
            if (ccList != null)
                foreach (System.Net.Mail.MailAddress bc in ccList)
                {
                    mm.CC.Add(bc);
                }
            List<System.Net.Mail.Attachment> atts = BLL.ToSysAttachment(attachment);
            if (atts != null)
                foreach (System.Net.Mail.Attachment att in atts)
                {

                    mm.Attachments.Add(att);

                }
            try
            {
                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            return new INT(1, "");
        }

        internal static YouJianSimple[] GetMailMessages(int uid, int muids, int muide, string mailBoxName)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            string strTrueMailBoxName = WenJianJia.NameToTrueName(mailBoxName);
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            ic.SelectMailbox(strTrueMailBoxName);
            int maxUid = ic.GetMessageCount();
            int uids, uide;
            if (maxUid >= muide)
            {
                uids = maxUid - muide;
                uide = maxUid - muids;
            }
            else
            {
                uide = maxUid - muids;
                uids = 0;
            }
            MailMessage[] mms = ic.GetMessages(uids, uide, false);
            YouJianSimple[] yjs = new YouJianSimple[mms.Length];
            for (int i = 0; i < yjs.Length; i++)
            {
                YouJian yj = new YouJian();
                yj.loadFrom(mms[yjs.Length - i - 1]);
                yjs[i] = yj.ToSimple();

            }
            return yjs;
        }

        internal static string[] GetTkmPuser(string workno)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select yyxt_userbh,yyxt_user,yyxt_usermm from dat_userDuiYing where yyxt_id = 3 and xtbg_user=" + workno;
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count == 0) return null;
            string userid = dt.Rows[0]["yyxt_user"].ToString();
            string pass = dt.Rows[0]["yyxt_usermm"].ToString();
            string[] userinfo = new string[2];
            userinfo[0] = userid;
            userinfo[1] = pass;
            return userinfo;
        }

        //internal static YouJianSimple[] GetMailMessagesTkmp(int uid, int startId, int count, bool asc)
        //{
        //    string workno = uid.ToWorkNo();
        //    string[] userinfo = GetTkmPuser(workno);
        //    string user = userinfo[0];
        //    string pass = userinfo[1];
        //    YouJianTKMP yjt = new YouJianTKMP(user, pass);
        //    return yjt.getMailList(startId, count, asc, 0);

        //}

        internal static YouJianFuJian[] GetMailAttachment(int uid, int muid, string mailBoxName, int pos)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            string strTrueMailBoxName = WenJianJia.NameToTrueName(mailBoxName);
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            ic.SelectMailbox(strTrueMailBoxName);
            MailMessage mm = ic.GetMessage(muid.ToString());
            if (mm == null) return null;
            if (mm.Attachments.Count == 0) return null;
            YouJianFuJian[] yjfj = new YouJianFuJian[mm.Attachments.Count];
            for (int i = 0; i < yjfj.Length; i++)
            {
                List<Attachment> atts = mm.Attachments.ToList();
                yjfj[i] = new YouJianFuJian();
                yjfj[i].LoadFrom(atts[i]);
            }
            if (pos <= 0) return yjfj;
            else if (pos > yjfj.Length) return null;
            else return new YouJianFuJian[] { yjfj[pos - 1] };
        }

        //internal static YouJian GetMailMessageTkmp(int uid, int muid)
        //{
        //    string workno = uid.ToWorkNo();

        //    string[] userinfo = GetTkmPuser(workno);
        //    string user = userinfo[0];
        //    string pass = userinfo[1];
        //    YouJianTKMP yjt = new YouJianTKMP(user, pass);
        //    YouJian yj = yjt.getMail(muid);
        //    return yj;
        //}

        internal static WenJianJia[] GetMailBoxList(int uid)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            Mailbox[] mbs = ic.ListMailboxes("", "*");
            WenJianJia[] wjj = new WenJianJia[mbs.Length];
            for (int i = 0; i < wjj.Length; i++)
            {
                wjj[i] = new WenJianJia();

                wjj[i].loadFrom(ic.SelectMailbox(mbs[i].Name));
            }
            return wjj;
        }

        internal static INT DeleteMailMessage(int uid, int muid, string mailBoxName)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            string strTrueMailBoxName = WenJianJia.NameToTrueName(mailBoxName);
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            try
            {
                ic.SelectMailbox(strTrueMailBoxName);
                if (strTrueMailBoxName.Equals("Trash"))
                {
                    ic.DeleteMessage(muid.ToString());
                    ic.Expunge();
                }
                else
                {
                    ic.MoveMessage(muid.ToString(), "Trash");
                }
                ic.Disconnect();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            return new INT(1, "");
        }

        internal static INT MoveMailMessage(int uid, int muid, string oldMailBox, string newMailBox)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            string strOldTrueMailBoxName = WenJianJia.NameToTrueName(oldMailBox);
            string strNewTrueMailBoxName = WenJianJia.NameToTrueName(newMailBox);
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            try
            {
                ic.SelectMailbox(strOldTrueMailBoxName);

                ic.MoveMessage(muid.ToString(), strNewTrueMailBoxName);

            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            return new INT(1, "");
        }

        internal static INT DeleteMailBox(int uid, string mailBoxName)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            string strTrueMailBoxName = WenJianJia.NameToTrueName(mailBoxName);
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            try
            {
                Mailbox mb = ic.SelectMailbox(strTrueMailBoxName);
                if (mb.NumMsg > 0)
                {
                    return new INT(-1, "该文件夹不为空，不能删除。");
                }
                ic.DeleteMailbox(mailBoxName);


            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            return new INT(1, "");
        }

        internal static INT RenameMailBox(int uid, string oldMailBoxName, string newMailBoxName)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            string strTrueMailBoxName = WenJianJia.NameToTrueName(newMailBoxName);
            ImapClient ic = new ImapClient(SjbgConfig.MailHost, workno + "@" + SjbgConfig.MailDomain, SjbgConfig.MailPassword);
            try
            {

                ic.RenameMailbox(oldMailBoxName, strTrueMailBoxName);


            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            return new INT(1, "");
        }

        //2014-8-22  GPS读取

        public static DataTable GetGpsByNum(string gpsData)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT work_no,work_name,JingDu,WeiDu,WeiZhi,ShiJian FROM dat_gps";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gps_data", gpsData);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "gpsRead";
            return dt;
        }


        internal static INT Gpscs(string workNo, string workName, string jingDu, string weiDu, string weiZhi, string shiJian)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into dat_gps (work_no,work_name,JingDu,WeiDu,WeiZhi,ShiJian) values(@work_no,@work_name,@JingDu,@WeiDu,@WeiZhi,@ShiJian)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("work_name", workName);
            comm.Parameters.AddWithValue("JingDu", Convert.ToDouble(jingDu));
            comm.Parameters.AddWithValue("WeiDu", Convert.ToDouble(weiDu));
            comm.Parameters.AddWithValue("WeiZhi", weiZhi);
            comm.Parameters.AddWithValue("ShiJian", shiJian);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return new INT(1, "");
        }

        internal static DataTable GetUserRole(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT rid,rname,descr FROM v_user_role where user_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getUserRole";
            return dt;
        }

        internal static DataTable GetUserMenu(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT DISTINCT M2Id, M2Name, M1Id, M1Name, Enabled,ImageRes,ActivityName,Params,order1,order2 FROM V_User_Menu WHERE user_no = @work_no order by order1,order2";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getUserMenu";
            return dt;
        }

        //2014-09-18 add by zhh for运用
        //2014-9-15  机车计划读取

        public static DataTable GetJcjhByNum(string jcjhData)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT plan_date,open_time,locus,engi_brand,engi_no FROM duty_cq  order by plan_date desc,open_time desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("jcjh_data", jcjhData);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "jcjhRead";
            return dt;
        }

        //2014-9-16  人员计划读取

        public static DataTable GetRyjhByNum(string ryjhData)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT plan_date + ' ' + Open_time AS plan_date,locus,engi_brand+engi_no AS engi_no,Roadway,ZunDian_time,driver_1no+driver_1name as driver_1no,driver_2no+driver_2name as driver_2no,driver_3no+driver_3name as driver_3no FROM duty_cq  order by plan_date desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("ryjh_data", ryjhData);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "ryjhRead";
            return dt;
        }

        //2014-9-17  待乘计划读取

        public static DataTable GetDcjhByNum(string dcjhData)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT convert(varchar(10),Wait_date,111) + ' ' + left(Wait_time,5) AS plan_date,dri_Room_no,convert(varchar(4),Drive_no)+Drive_name as Drive_name,convert(varchar(5),Dri_time,8) as Dri_time,convert(varchar(4),Ass_no)+Ass_name as Ass_name,convert(varchar(5),Ass_time,8) as Ass_time,convert(varchar(4),Student_no)+Student_name as Student_name,convert(varchar(5),Stu_time,8) as Stu_time FROM Wait_Duty where datediff(day,Wait_date,getdate())='0' order by wait_time desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("dcjh_data", dcjhData);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "dcjhRead";
            return dt;
        }

        //2014-9-17  检修计划读取

        public static DataTable GetJxjhByNum(string jxjhData)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT plan_date + ' ' + Open_time AS plan_date,locus,engi_brand+engi_no AS engi_no,Roadway,ZunDian_time,driver_1no,driver_2no,driver_3no FROM duty_cq";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("jxjh_data", jxjhData);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "jxjhRead";
            return dt;
        }

        //2014-9-17  测酒记录读取

        public static DataTable GetCjcxByNum(string cjcxData)
        {
            SqlConnection conn = new SqlConnection(YyConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT top 10  LEFT(日期, 10) + ' ' + LEFT(时间, 5) AS plan_date, 用户名 AS dd, 工号+姓名 AS work_no, 结果 AS cjjg FROM recordset where datediff(dd,日期,getdate())='0' order by 日期 desc,时间 desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("cjcx_data", cjcxData);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "cjcxRead";
            return dt;
        }
        //2014-0918 add by zhh for运用


        private static string GetConnStringByDataBase(int database)
        {
            string connString;
            switch (database)
            {
                case 1: //运用
                    connString = YyConnStr;
                    break;
                case 2://月山
                    connString = YsConnStr;
                    break;
                default:
                    connString = "";
                    break;
            }
            return connString;
        }

        internal static DataTable GetMingPaiByXianBie(int database, string lineOrWorkno, int type)
        {
            SqlConnection conn = new SqlConnection(GetConnStringByDataBase(database));


            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select work_no,work_name,banci,weizhi,a.line_mode,qduan,a.state,c.state as state_name from person_active a inner join meet b on a.line_mode=b.line_mode inner join state c on a.state=c.state_id where  ";
            if (type == 1)//休息状态
                comm.CommandText += " a.line_mode=@param and a.state in (1,2,29,40)";
            else if (type == 2)
            {
                comm.CommandText += " a.line_mode=@param and a.state in (3,8)";
            }
            else if (type == 3)
            {
                comm.CommandText += " work_no = @param ";
            }
            comm.CommandText += " order by banci,weizhi";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("param", lineOrWorkno);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "MingPai";
            return dt;
        }

        internal static DataTable GetMingPaiByGongHao(int database, string workNo)
        {
            SqlConnection conn = new SqlConnection(GetConnStringByDataBase(database));
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select work_no,line_mode,state from person_active where work_no=@work_no ";

            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            int type = 0;
            switch (Convert.ToInt32(dt.Rows[0]["state"]))
            {
                case 1:
                case 2:
                case 29:
                case 40:
                    type = 1;
                    break;
                case 3:
                case 8:
                    type = 2;
                    break;
                default:
                    type = 3;
                    break;
            }
            if (type == 3)
            {
                return GetMingPaiByXianBie(database, workNo, type);
            }
            else
            {
                return GetMingPaiByXianBie(database, dt.Rows[0]["line_mode"].ToString(), type);
            }
        }

        internal static DataTable GetXianBie(int database)
        {
            SqlConnection conn = new SqlConnection(GetConnStringByDataBase(database));
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select line_mode,qduan from meet where bag_turn='轮'";

            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getXianBie";
            return dt;
        }

        internal static DataTable GetDaMingPai(int database, string line, int type, string filter)
        {
            SqlConnection conn = new SqlConnection(GetConnStringByDataBase(database));
            SqlCommand comm = new SqlCommand();
            line = GetXbidByXbmc(database, line);
            if (line == null) return null;
            comm.Connection = conn;
            comm.CommandText = "select * from V_SJBG_DaMingPai where 1=1 ";
            if (filter.Equals(""))
            {
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("line", line);
                comm.Parameters.AddWithValue("type", type);
                comm.CommandText += " and  line_mode=@line and type=@type ";
            }
            else
            {
                comm.CommandText += " and (gh1=@filter or gh2=@filter or gh3=@filter or gh4=@filter or xm1=@filter or xm2=@filter or xm3=@filter or xm4=@filter) ";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("filter", filter);
            }




            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getDaMingPai";
            return dt;
        }

        public static string GetXbidByXbmc(int database, string xbmc)
        {
            SqlConnection conn = new SqlConnection(GetConnStringByDataBase(database));
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select line_mode from meet where qduan=@xbmc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("xbmc", xbmc);
            try
            {
                conn.Open();
                return Convert.ToString(comm.ExecuteScalar());
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataTable GetCanBu(string workNo, DateTime kssj, DateTime jssj)
        {
            EcardService.Service s = new EcardService.Service();
            return s.getCanBu(workNo, kssj, jssj);
            //SqlConnection conn = new SqlConnection(cbConnStr);
            //SqlCommand comm = new SqlCommand();
            //comm.Connection = conn;
            //comm.CommandText = "select  id, rybh, ryxm, je, addtime, engi_brand, engi_no, roadway, open_time, arrive_time, addType, oper, ipAddr, CAST(arrive_time - open_time AS float) * 24 AS shichang from T_AddMoney where rybh=@work_no and arrive_time > @kssj and arrive_time <= @jssj order by arrive_time desc";
            //comm.Parameters.Clear();
            //comm.Parameters.AddWithValue("work_no", work_no);
            //comm.Parameters.AddWithValue("kssj", kssj);
            //comm.Parameters.AddWithValue("jssj", jssj);

            //SqlDataAdapter sda = new SqlDataAdapter(comm);
            //DataTable dt = new DataTable();
            //try
            //{
            //    sda.Fill(dt);
            //}
            //catch
            //{
            //    dt.TableName = "error!";
            //    return dt;
            //}
            //dt.TableName = "getCanBu";
            //return dt;
        }

        public static DataTable GetFyyjcZt()
        {
            OracleService.fyyjcService os = new OracleService.fyyjcService();
            return os.getFyyjcZt();
        }
        public static DataTable GetFyyjc(string jczt)
        {
            OracleService.fyyjcService os = new OracleService.fyyjcService();
            return os.getFyyjc(jczt);
        }




        ////电子书名读取
        //public static DataTable getBookName(string book_Name)
        //{
        //    SqlConnection conn = new SqlConnection(baseConnStr);
        //    SqlCommand comm = new SqlCommand();
        //    comm.Connection = conn;
        //    comm.CommandText = "SELECT id, Name FROM book";
        //    comm.Parameters.Clear();
        //    comm.Parameters.AddWithValue("book_Name", book_Name);
        //    SqlDataAdapter sda = new SqlDataAdapter(comm);
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        sda.Fill(dt);
        //    }
        //    catch
        //    {
        //        dt.TableName = "error!";
        //        return dt;
        //    }

        //    dt.TableName = "BookNameRead";
        //    return dt;
        //}

        //电子书名读取
        public static DataTable GetBookName(string booknameData)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT id, Name,Address FROM Book_Name";
            if (!booknameData.Equals(""))
            {
                comm.CommandText += " where id = @id";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("id", booknameData);
            }

            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "BookNameRead";
            return dt;
        }

        //电子书内容读取
        public static DataTable GetBookNr(string bookNrId)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT id, Txt FROM Book_NR_zhh where id = @id";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("id", bookNrId);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            if (dt.Rows.Count != 1)
            {
                dt.TableName = "error!";
            }
            else
            {
                dt.TableName = "BookNrRead";
            }
            return dt;
        }

        internal static int getDutyRoomIdByWork_no(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT drid FROM V_SendFile_DutyRoom_Receiver where work_no = @work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            try
            {
                conn.Open();
                return Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        internal static DataTable GetSentFiles(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT fid, fileName, sendTime, allCount, receiveCount,  senderDeptName + senderName  as sender FROM  V_SendFile_SentFiles where sender=@sender ORDER BY sendTime DESC";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("sender", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getSentFiles";
            return dt;
        }

        internal static DataTable GetSentFileDetails(int fid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT     fileName + '.' + extName AS filename, sendTime, dutyroom, receiverName, receive_time FROM V_SendFile_File_DutyRoom_User WHERE  (fid = @fid) order by drid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("fid", fid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getSentFileDetails";
            return dt;
        }

        internal static DataTable GetFilesToReceive(int drid, int type)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT fid, fileName, extName, filedesc, sendTime, sender, senderName, senderDept, senderDeptName, drid, dutyroom, receiver, receive_time,  id, receiverName,dutyroom_bmid,dutyroom_bmmc FROM V_SendFile_File_DutyRoom_User where drid=@drid ";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("drid", drid);
            if (type == 0)//选未签收的文件
            {
                comm.CommandText += " and receiver is null ";
            }
            else
            {
                comm.CommandText += " and receiver is not null ";
            }
            comm.CommandText += " order by id desc";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getFilesToReceive";
            return dt;
        }

        internal static INT ReceiveFile(int fdrid, string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;


            //选出该文件接收值班室id
            comm.CommandText = "select drid from T_SendFile_File_DutyRoom where id=@fdrid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("fdrid", fdrid);
            int drid = 0;
            try
            {
                conn.Open();
                drid = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                //fdrid不存在
                return new INT(-1, "fdrid不存在");
            }
            finally
            {
                conn.Close();
            }

            //判断该工号是否具有该值班室接收文件权限
            comm.CommandText = "select count(*) from V_SendFile_DutyRoom_Receiver where drid=@drid and work_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("drid", drid);
            int count = 0;
            try
            {
                conn.Open();
                count = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {

                return new INT(-3, " 数据库错误");
            }
            finally
            {
                conn.Close();
            }
            if (count < 1) return new INT(-2, "该工号没有权限");

            comm.CommandText = "update T_SendFile_File_DutyRoom set receiver=@work_no,receive_time=getdate() where id=@fdrid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("fdrid", fdrid);
            try
            {
                conn.Open();
                if (Convert.ToInt32(comm.ExecuteNonQuery()) == 0) return new INT(-4, "未找到更新对象");
            }
            catch
            {

                return new INT(-3, " 数据库错误");
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, "");
        }





        internal static DataTable GetSendFileDept(int did)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT bm_id, bm_mc FROM dic_bumen";
            if (did == 0) comm.CommandText += " where bm_id in (17,18,19,20,21) order by bm_id";
            else comm.CommandText += " where bm_id = " + did.ToString();
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getSendFileDept";
            return dt;
        }

        internal static DataTable GetDutyRoomByDeptId(int did)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT id, position,bm_id,bm_mc FROM V_SendFile_DutyRoom";
            if (did == 0) comm.CommandText += " where bm_id in (17,18,19,20,21,12) order by bm_id";
            else comm.CommandText += " where bm_id = " + did.ToString();
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getDutyRoomByDeptId";
            return dt;
        }

        internal static INT SendFile(string sender, string fileName, string extName, string fileDesc, string fileContent, string drs)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);

            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into T_SendFile_Files (fileName,extName,fileDesc,sender) values(@fileName,@extName,@fileDesc,@sender)";
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }

            SqlTransaction trans = conn.BeginTransaction();
            comm.Transaction = trans;
            try
            {
                byte[] buffer = Convert.FromBase64String(fileContent);
                comm.CommandText = "select work_no from T_SendFile_DutyRoom_Receiver where onwork=1 and drid in (" + drs + ")";
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(comm);
                da.Fill(dt);
                comm.CommandText = "insert into T_SendFile_Files (fileName,extName,fileDesc,sender,fileContent) values(@fileName,@extName,@fileDesc,@sender,@fileContent);select scope_identity();";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("fileName", fileName);
                comm.Parameters.AddWithValue("extName", extName);
                comm.Parameters.AddWithValue("fileDesc", fileDesc);
                comm.Parameters.AddWithValue("sender", sender);
                comm.Parameters.AddWithValue("fileContent", buffer);
                int fid = Convert.ToInt32(comm.ExecuteScalar());
                comm.CommandText = "insert into T_SendFile_File_DutyRoom (fid,drid) select " + fid.ToString() + " , id  as drid from  T_SendFile_DutyRoom where id in (" + drs + ")";
                comm.Parameters.Clear();
                comm.ExecuteNonQuery();


                string worknos = "";
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        worknos = worknos + Convert.ToString(dt.Rows[i]["work_no"]) + ",";
                    }
                    worknos = worknos.Substring(0, worknos.Length - 1);
                    comm.CommandText = "select bm_mc + user_name as sender from v_user where user_no=@sender";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("sender", sender);
                    sender = comm.ExecuteScalar().ToString();


                    string content = sender + "发来文件：" + fileName + "." + extName + "，请接收。";
                    comm.CommandText = "insert into T_System_Message(toUser, Type, Content, Command) select distinct user_no as toUser ,0 , '" + content + "','SendFile' from dic_user where user_no in (" + worknos + ")";
                    comm.Parameters.Clear();
                    comm.ExecuteNonQuery();

                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{
                    //    string topic = dt.Rows[i]["work_no"].ToString();
                    //    sendMqttMessage(sender, topic, content, 2);
                    //}
                }




                //System.IO.FileStream fs = new System.IO.FileStream(SjbgConfig.SendFilePath + fid.ToString() + "." + extName, System.IO.FileMode.Create);
                //fs.Write(buffer, 0, buffer.Length);
                //fs.Close();
            }
            catch
            {
                trans.Rollback();
                return new INT(-1, "写入数据库错误");
            }
            trans.Commit();
            return new INT(1, "");
        }

        internal static INT AddFeedBack(string workNo, string txt)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into T_Sjbg_Feedback (work_no,txt) values( @work_no,@txt)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("txt", txt);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //return new INT(-1, "数据库错误");
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, "");
        }

        internal static DataTable GetLoginRecord(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  id, uid, deviceId, deviceInfo, deviceVer, IpAddress, loginTime FROM dat_LoginInfo where uid=@uid order by loginTime desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "GetLoginRecord";
            return dt;
        }

        internal static string GetFileToReceiveFromDataBase(int fid)
        {

            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select  fileContent from T_SendFile_Files where id = @fid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("fid", fid);
            string result = "";
            try
            {
                conn.Open();
                result = Convert.ToBase64String((byte[])comm.ExecuteScalar());
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        internal static DataTable GetUnsubTopics(string workNo)
        {
            SqlConnection conn = new SqlConnection(MqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  id,topic FROM V_User_Topics where work_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getUnsubTopics";
            return dt;
        }

        internal static INT SetTopicsSubed(string tids)
        {
            SqlConnection conn = new SqlConnection(MqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_User_Topics set isSubed=1, lastSubTime =getdate() where id in (" + tids + ")";
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new INT(-1, "数据库错误");
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, "");
        }

        internal static INT SetMqttStatus(string workNo, int type, string clientId)
        {
            SqlConnection conn = new SqlConnection(MqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select count(*) from T_User_session where work_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            int isin = 0;
            try
            {
                conn.Open();
                isin = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            finally
            {
                conn.Close();
            }
            if (isin == 1)
            {

                comm.CommandText = "update T_User_Session set isOnline=@type, lastUpdateTime =getdate(),clientid=@clientId where work_no=@work_no";
            }
            else
            {
                comm.CommandText = "insert into T_User_Session (isOnline,lastUpdateTime,work_no,clientid) values(@type,getdate(), @work_no,@clientId)";
            }
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("type", type);
            comm.Parameters.AddWithValue("clientId", clientId);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                return new INT(-1, "数据库错误");
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, "");
        }

        internal static INT GetMqttStatus(string workNo)
        {
            SqlConnection conn = new SqlConnection(MqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select isOnline from T_User_session where work_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            int status = 0;
            try
            {
                conn.Open();
                status = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            finally
            {
                conn.Close();
            }
            return new INT(status);
        }

        /// <summary>
        /// 发送MQTT消息
        /// </summary>
        /// <param name="sender">发送人</param>
        /// <param name="topic">消息主题</param>
        /// <param name="message">消息内容</param>
        /// <param name="type">消息类型:2：提醒型消息；3：警告型信息</param>
        /// <returns></returns>
        internal static INT SendMqttMessage(string sender, string topic, string message, int type)
        {
            SqlConnection conn = new SqlConnection(MqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {

                comm.Transaction = trans;

                //获取该topic有多少人订阅
                comm.CommandText = "select distinct work_no from V_User_Topics where topic=@topic";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("topic", topic);
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(comm);
                sda.Fill(dt);
                if (dt.Rows.Count == 0) return new INT(-1, "无人订阅该topic");
                string users = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    users += "'" + dt.Rows[i]["work_no"].ToString() + "',";
                }
                users = users.Substring(0, users.Length - 1);

                //插入t_mqtt_message表
                comm.CommandText = "insert into t_mqtt_message(sender,topic,type,txt) values(@sender,@topic,@type,@txt);select scope_identity(); ";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("sender", sender);
                comm.Parameters.AddWithValue("topic", topic);
                comm.Parameters.AddWithValue("type", type);
                comm.Parameters.AddWithValue("txt", message);
                int mid = Convert.ToInt32(comm.ExecuteScalar());


                //插入t_mqtt_message_user表
                comm.CommandText = "insert into t_mqtt_message_user (mid,uid) select distinct " + mid.ToString() + ",work_no from T_user_topics where work_no in (" + users + ")";
                comm.Parameters.Clear();
                comm.ExecuteNonQuery();

                //插入t_publish_message表
                MqttMessage mm = new MqttMessage(type.ToMqttMessageType(), mid, sender, topic, message);
                string content = mm.ToJsonString();
                comm.CommandText = "insert into t_publish_message (mid,topic,txt) values(@mid,@topic,@txt)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("topic", topic);
                comm.Parameters.AddWithValue("txt", content);
                comm.Parameters.AddWithValue("mid", mid);
                comm.ExecuteNonQuery();
                trans.Commit();
            }

            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return new INT(1);
        }

        internal static DataTable GetSystemMessage(string workNo, int type)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT     id, toUser, Type, Content, Command, createTime, hasRead, readTime FROM T_System_Message where toUser=@work_no and (hasread=@type or @type=-1) order by createTime desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("type", type);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getSystemMessage";
            return dt;
        }

        internal static INT ReadSystemMessage(int mid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_System_message set hasread=1 , readtime=getdate() where id=@mid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("mid", mid);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch
            {
                return new INT(-1, "数据库错误。");
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static INT ReadMqttMessage(string workNo, int mid)
        {
            SqlConnection conn = new SqlConnection(MqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_Mqtt_message_user set hasread=1 , readtime=getdate() where uid=@uid and mid=@mid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("mid", mid);
            comm.Parameters.AddWithValue("uid", workNo);
            try
            {
                conn.Open();
                int i = Convert.ToInt32(comm.ExecuteNonQuery());
                if (i == 0) return new INT(-2, "未找到该消息");
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static DataTable GetUnReadMqttMessage(string workNo)
        {
            SqlConnection conn = new SqlConnection(MqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select json from V_Mqtt_Message_User where  uid=@uid and hasRead=0";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getUnReadMqttMessage";
            return dt;
        }

        #region 安全信息平台
        public static DataTable GetAqxxptBm()
        {


            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select bmid,bmmc from V_AQXXPT_BuMen";
            sda.SelectCommand = comm;
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getAqxxptBm";
            return dt;
        }


        public static DataTable GetAqxxptBm(int xxid)
        {


            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select bmid,bmmc from V_AQXXPT_XX_BM where xxid=@xxid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("xxid", xxid);
            sda.SelectCommand = comm;
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getAqxxptBm";
            return dt;
        }

        public static DataTable GetAqxxptShenHe()
        {


            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select user_no ,user_name from V_AQXXPT_Auditor";
            sda.SelectCommand = comm;
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getAqxxptShenHe";
            return dt;
        }

        public static INT ApplyAqxx(string sender, string auditor, string title, string content, string buMens, DateTime setTime, string lingDaos)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            int xxid = 0;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-100, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.Transaction = trans;
                comm.CommandText = "insert into t_aqxxpt_xx (title,txt,sender,setTime) values(@title,@txt,@sender,@setTime);select scope_identity(); ";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("@title", title);
                comm.Parameters.AddWithValue("@txt", content);
                comm.Parameters.AddWithValue("@sender", sender);
                comm.Parameters.AddWithValue("@setTime", setTime);
                xxid = Convert.ToInt32(comm.ExecuteScalar());
                //comm.CommandText = "insert into t_aqxxpt_xxjs (xxid,receiver) select " + xxid.ToString() + " as xxid, user_no from V_AQXXPT_Receiver where bumen_id in (" + buMens + ")";
                comm.CommandText = "insert into t_aqxxpt_xx_bm ( xxid,bmid) select " + xxid.ToString() + " as xxid,bm_id as bm_id from dic_bumen where bm_id in  (" + buMens + ")";
                comm.ExecuteNonQuery();
                //if (lingDaos.Equals("") || lingDaos == null)
                //{
                //    //do nothing
                //}
                //else
                //{
                //    string[] lingdao = lingDaos.Split(',');
                //    comm.CommandText = "insert into t_aqxxpt_xxjs (xxid,receiver) values(@xxid,@lingdao)";
                //    for (int i = 0; i < lingdao.Length; i++)
                //    {
                //        comm.Parameters.Clear();
                //        comm.Parameters.AddWithValue("xxid", xxid);
                //        comm.Parameters.AddWithValue("lingdao", lingdao[i]);
                //        comm.ExecuteNonQuery();
                //    }

                //}
                comm.CommandText = "insert into t_aqxxpt_sh (xxid,auditor) values(@xxid,@auditor)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("xxid", xxid);
                comm.Parameters.AddWithValue("auditor", auditor);
                comm.ExecuteNonQuery();
                if (true)//(DAL.sendMqttMessage(sender, auditor, "有一条安全信息需要您审核。", 2).Number == 1)
                {
                    SendMobileMessage(Convert.ToInt32(auditor), "有一条安全信息需要您审核。");
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                    return new INT(-1, "发送提醒信息失败");
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, xxid.ToString());
        }



        internal static INT AuditAqxx(int xxid, string auditor, int result, string title, string txt)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-100, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                SqlCommand comm = new SqlCommand();
                comm.Connection = conn;
                comm.Transaction = trans;
                //更新审核表的状态
                comm.CommandText = "update t_aqxxpt_sh set auditTime = getdate(),auditResult = @result,txt=@txt where xxid=@xxid and auditor=@auditor and auditTime is null and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("result", result);
                comm.Parameters.AddWithValue("txt", txt);
                comm.Parameters.AddWithValue("auditor", auditor);
                comm.Parameters.AddWithValue("xxid", xxid);
                comm.ExecuteNonQuery();

                if (result == 0)//审核不通过，通知发信息人
                {
                    comm.CommandText = "select sender from t_aqxxpt_xx where id = @xxid";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("xxid", xxid);
                    string sender = Convert.ToString(comm.ExecuteScalar());
                    if (SendMqttMessage("提醒信息：", sender, "安全信息审核未通过，原因：" + txt, 2).Number == 1)
                    {
                        SendMobileMessage(Convert.ToInt32(sender), "安全信息审核未通过，原因：" + txt);
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                        return new INT(-1, "发送提醒信息失败");
                    }
                }
                else//审核通过,插入信息接收表
                {
                    //if (result == 2) //审核通过但是有改动
                    {
                        //把改动信息更新进信息表
                        comm.CommandText = "update t_aqxxpt_xx set title=@title,txt=@txt where id =@xxid";
                        comm.Parameters.Clear();
                        comm.Parameters.AddWithValue("title", title);
                        comm.Parameters.AddWithValue("txt", txt);
                        comm.Parameters.AddWithValue("xxid", xxid);
                        comm.ExecuteNonQuery();
                    }
                    //按照部门发送给所有接收人
                    comm.CommandText = "insert into t_aqxxpt_xxjs (xxid,receiver) select " + xxid.ToString() + " as xxid, user_no from V_AQXXPT_Receiver where bumen_id in (select bmid from t_aqxxpt_xx_bm where xxid=" + xxid.ToString() + ")";
                    comm.ExecuteNonQuery();
                    //发送消息给所有接收人
                    ///
                    ///
                    trans.Commit();
                }


            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-100, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }


        internal static DataTable GetAqxxToAudit(string auditor, int xxid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select id,xxid,title,txt,sender,createtime as sendtime,settime from v_aqxxpt_audit where auditTime is null and auditor=@auditor and (xxid=@xxid or @xxid=0)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("auditor", auditor);
            comm.Parameters.AddWithValue("xxid", xxid);
            sda.SelectCommand = comm;
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getAqxxToAudit";
            return dt;
        }


        internal static DataTable GetAqxxInfo(int did, int xxid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select xxid,sender,title,sendTime,readcount,sendcount,failcount,sendingcount,status,auditor,audittime from v_aqxxpt_xxjs_info where (xxid = @xxid or @xxid=0) and (senderdept=@did or @did=1) order by audittime desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("xxid", xxid);
            comm.Parameters.AddWithValue("did", did);
            sda.SelectCommand = comm;
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getAqxxInfo";
            return dt;
        }

        internal static DataTable GetAqxxDetail(int xxid ,int type)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select title,sender,sendtime,receiverno + receivername as receiver,receivetime,receiverDept ,status from V_AQXXPT_XXJS where xxid=@xxid and (statusCode = @type or @type =0)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("xxid", xxid);
            comm.Parameters.AddWithValue("type", type);
            sda.SelectCommand = comm;
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getAqxxDetail";
            return dt;
        }

        internal static DataTable GetAqxxContent(int xxid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select id,xxid,title,txt,sender,createtime as sendtime,settime from v_aqxxpt_audit where (xxid=@xxid or @xxid=0)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("xxid", xxid);
            sda.SelectCommand = comm;
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            dt.TableName = "getAqxxContent";
            return dt;
        }

        #endregion


        #region 2016新公文流转系统
        internal static INT AddNewGongWen2016(string ht, string dw, string wh, string bt, string zw, string yj, int wjxzId, int wjlxId, string fbr, string jinji, string ip, string[] jsrList, string[] gwfj)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }

            //因为添加公文过程中要在多个表中添加数据，所以设置一个事务(SqlTransaction)，确保数据完整性
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;

                //看看公文表内是否已经有该文号
                comm.CommandText = "select count(*) from V_GongWen_Gwxx where wh=@wh";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("@wh", wh);
                if (Convert.ToInt32(comm.ExecuteScalar()) > 0)
                {
                    return new INT(-1,"该文号已存在。");
                }

                //先插入公文信息表
                comm.CommandText = "INSERT INTO [dbo].[T_GongWen_GWXX] (ht,dw,wh,bt,zw,csyj,wjxzID,wjlxID,fbr,fbrq,ip,jinji)";
                comm.CommandText += " VALUES (@ht,@dw,@wh,@bt,@zw,@csyj,@wjxzID,@wjlxID,@fbr,getdate(),@ip,@jinji)";
                comm.CommandText += " ;select scope_identity();";//获取新插入的行ID
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("@ht", ht);
                comm.Parameters.AddWithValue("@dw", dw);
                comm.Parameters.AddWithValue("@wh", wh);
                comm.Parameters.AddWithValue("@bt", bt);
                comm.Parameters.AddWithValue("@zw", zw);
                comm.Parameters.AddWithValue("@csyj", yj);
                comm.Parameters.AddWithValue("@wjxzid", wjxzId);
                comm.Parameters.AddWithValue("@wjlxid", wjlxId);
                comm.Parameters.AddWithValue("@fbr", fbr);
                comm.Parameters.AddWithValue("@ip", ip);
                comm.Parameters.AddWithValue("@jinji", jinji);
                //因为在insert语句后加入了select scope_identity();
                //所以可以用ExecuteScalar获取scope_identity()的内容，即insert以后新生成的ID
                int gid = Convert.ToInt32(comm.ExecuteScalar());

                //依次把附件添加进附件表
                for (int i = 0; i < gwfj.Length; i++)
                {
                    comm.CommandText = "insert into t_gongwen_gwfj (gwid,fjmc,paixu) values(@gwid,@fjmc,@paixu)";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("@gwid", gid);
                    comm.Parameters.AddWithValue("@fjmc", gwfj[i]);
                    comm.Parameters.AddWithValue("@paixu", i + 1);
                    comm.ExecuteNonQuery();
                }

                foreach (string jsr in jsrList)
                {
                    //添加公文流转表开始流转
                    comm.CommandText = "insert into t_gongwen_lz (gwid,pid,fsr,jsr,fssj) values(@gwid,0,@fsr,@jsr,getdate())";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("@gwid", gid);
                    comm.Parameters.AddWithValue("@fsr", fbr);
                    comm.Parameters.AddWithValue("@jsr", jsr);
                    comm.ExecuteNonQuery();
                }
                

                //发短信通知
                string message;
                if (jinji.Equals("一般"))
                {
                    message = "您有一件新公文需要签阅。公文标题：" + bt;
                }
                else
                {
                    message = "您有一件新公文需要签阅。公文标题：" + bt + "。该公文是" + jinji + "公文，请务必尽快签阅。";
                }
                
                 INT r;
                if (_sendMessageForDebug)
                {
                    r =SendMobileMessage("3974", message);
                }
                else
                {
                    r = SendMobileMessage(jsrList, message);
                }
                if (r.Number == 1)
                {
                    //所有操作都成功完成，提交事务，确认操作
                    trans.Commit();
                }
                else
                {
                    //发短信时提醒失败
                    //回滚事务，撤销操作，返回错误信息
                    trans.Rollback();
                    return new INT(-1, "发送提醒短信失败。公文创建未成功。");
                }
            }

            catch (Exception ex)
            {
                //在操作中失败
                //回滚事务，撤销操作，返回错误信息
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            //返回成功
            return new INT(1);
        }

        internal static DataTable getZiDingYiBuMenRenYuan(int[] zdybmid)
        {
            string strZdybm = zdybmid.ToListString();
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select distinct user_no from V_GongWen_ZDYBM_USERNO where zdyid in (" + strZdybm + ")";
            SqlDataAdapter sda = new SqlDataAdapter(comm);

            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getZiDingYiBuMen";
            return dt;

        }

        internal static DataTable GetBuMenRenYuan(int bmid)
        {

            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select user_no,user_name from V_GongWen_YongHu where bm_id =@bmid and (rid=25 or rid=20 or rid=26) order by user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("bmid", bmid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);

            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getZiDingYiBuMen";
            return dt;

        }

        internal static INT BuGongWen2016(int gwid, int lzid, string fsr, string fsrxm, string bt, string[] jsrs, string buyueren)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;


                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = comm;
                comm.CommandText = "select jsr from t_gongwen_lz where pid=@lzid and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("lzid", lzid);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                List<string> newJsrList = jsrs.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string jsr = dt.Rows[i]["jsr"].ToString();
                    newJsrList.Remove(jsr);
                }
                string[] newJsr = newJsrList.ToArray();
                if (newJsr.Length == 0) return new INT(0, "没有新的接收公文人员。");
                string jsrStr = newJsr.ToListString();
                comm.CommandText = "insert into t_gongwen_lz (gwid,pid,fsr,jsr,fssj,bz) select @gwid,@pid,@fsr,user_no,getdate(),@bz from V_GongWen_Yonghu where user_no in (" + jsrStr + ")";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gwid", gwid);
                comm.Parameters.AddWithValue("pid", lzid);
                comm.Parameters.AddWithValue("fsr", fsr);
                comm.Parameters.AddWithValue("bz", buyueren + "补");
                comm.ExecuteNonQuery();

                comm.CommandText = "select jinji from V_GongWen_GWXX where id =@gwid";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("@gwid", gwid);
                string jinji = comm.ExecuteScalar().ToString();
                string message;
                if (jinji.Equals("一般"))
                {
                    message = "您有一件新公文需签阅。发送人：" + fsrxm + " ，公文标题：" + bt;
                }
                else
                {
                    message = "您有一件新公文需签阅。发送人：" + fsrxm + " ，公文标题：" + bt + "。该公文是" + jinji + "公文，请务必尽快签阅。";
                }
                INT r;
                if (_sendMessageForDebug)
                {
                    r = SendMobileMessage("3974", message);
                }
                else
                {
                    r = SendMobileMessage(newJsr, message);
                }

                if (r.Number == 1)
                {
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                    return new INT(-1, "发送提醒短信失败。公文补阅未成功。");
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            return new INT(1);
        }


        internal static void SignGongWen2016Log(int gwid, int lzid, string workNo, string throwJsr)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
                comm.CommandText = "insert into t_GongWen_QianShouRiZhi (uid,gwid,lzid,logTxt) values(@uid,@gwid,@lzid,@logTxt)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("uid", workNo);
                comm.Parameters.AddWithValue("gwid", gwid);
                comm.Parameters.AddWithValue("lzid", lzid);
                comm.Parameters.AddWithValue("logTxt", throwJsr);
                comm.ExecuteNonQuery();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        internal static INT SignGongWen2016(int gwid, int lzid, string fsr, string[] jsr, string bt, string fsrxm, int rid, string qsnr, string device, string ip)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;

                comm.CommandText = "select gwid,jsr,case when qssj is null then 0 else 1 end as sfqs from t_gongwen_lz where id=@lzid and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("lzid", lzid);
                SqlDataAdapter sda = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                int dbGwid = Convert.ToInt32(dt.Rows[0]["gwid"]);
                string dbFsr = Convert.ToString(dt.Rows[0]["jsr"]);
                int sfqs = Convert.ToInt32(dt.Rows[0]["sfqs"]);
                if (dbGwid != gwid || !dbFsr.Equals(fsr))
                {
                    SignGongWen2016Log(gwid, lzid, fsr, "dbGwid:" + dbGwid.ToString() + "gwid:" + gwid.ToString() + ",dbFsr:" + dbFsr + "fsr:" + fsr);
                    return new INT(-1, "公文信息错误。");
                }

                if (sfqs == 1) return new INT(-1, "该公文已经签收，无法再次签收。");



                comm.CommandText = "update t_gongwen_lz set qsnr=@qsnr,qssj=getdate(),device=@device,ipAddress=@ip where gwid=@gwid and jsr=@uid and qssj is null";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gwid", gwid);
                comm.Parameters.AddWithValue("uid", fsr);
                comm.Parameters.AddWithValue("qsnr", qsnr);
                comm.Parameters.AddWithValue("device", device);
                comm.Parameters.AddWithValue("ip", ip);
                int rows = comm.ExecuteNonQuery();

                if (rows == 0) return new INT(-1, "该公文已经签收，无法再次签收。");


                if (jsr != null && jsr.Length > 0)
                {
                    foreach (string jsrStr in jsr)
                    {
                        comm.CommandText = "insert into t_gongwen_lz (gwid,pid,fsr,jsr,fssj) values(@gwid,@pid,@fsr ,@jsr, getdate())";
                        comm.Parameters.Clear();
                        comm.Parameters.AddWithValue("@gwid", gwid);
                        comm.Parameters.AddWithValue("@pid", lzid);
                        comm.Parameters.AddWithValue("@fsr", fsr);
                        comm.Parameters.AddWithValue("@jsr", jsrStr);
                        comm.ExecuteNonQuery();
                        //if (rid == 22)
                        //{
                        //    comm.CommandText = "update t_gongwen_lz set isvalid=0 where gwid=@gwid and jsr=@jsr and fsr in (select user_no from v_gongwen_yonghu_role where rid=21)";
                        //    comm.Parameters.Clear();
                        //    comm.Parameters.AddWithValue("@gwid", gwid);
                        //    comm.Parameters.AddWithValue("@jsr", jsr[i]);
                        //    comm.ExecuteNonQuery();
                        //}
                    }
                    //发短信通知

                    comm.CommandText = "select jinji from V_GongWen_GWXX where id =@gwid";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("@gwid", gwid);
                    string jinji = comm.ExecuteScalar().ToString();
                    string message;
                    if (jinji.Equals("一般"))
                    {
                        message = "您有一件新公文需签阅。发送人：" + fsrxm + " ，公文标题：" + bt;
                    }
                    else
                    {
                        message = "您有一件新公文需签阅。发送人：" + fsrxm + " ，公文标题：" + bt + "。该公文是" + jinji + "公文，请务必尽快签阅。";
                    }
                    INT r;
                    if (_sendMessageForDebug)
                    {
                        r = SendMobileMessage("3974", message); 
                    }
                    else
                    {
                        r = SendMobileMessage(jsr, message);
                    }
                    
                    
                    if (r.Number == 1)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                        return new INT(-1, "发送提醒短信失败。公文创建未成功。");
                    }
                }
                else
                {
                    trans.Commit();
                }
            }

            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return new INT(1);
        }


        internal static int UnfinishedBanZiChengYuanRenShu(int gwid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  count(*)  FROM V_GongWen_List_All where gwid=@gwid and qssj is null and jsr_rid in (21,22) ";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gwid", gwid);
            try
            {
                conn.Open();
                return Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                return -1;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 获取公文信息列表
        /// </summary>
        /// <param name="jsr">接收人</param>
        /// <param name="fsr">发送人，直接流转给接收人的人，不是发布人</param>
        /// <param name="xzid">公文性质ID，1行政，2党群</param>
        /// <param name="lxid">公文类型ID，1路局，2段公文</param>
        /// <param name="keyWord">关键字</param>
        /// <param name="sTime">开始时间</param>
        /// <param name="eTime">截至时间</param>
        /// <param name="gwtype">公文类型，0，未签公文，1，全部公文</param>
        /// <returns>包含公文信息的DataTable</returns>
        internal static DataTable GetGongWenList(string jsr, string fsr, int xzid, int lxid, string keyWord, string sTime, string eTime, int gwtype)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select rid from V_GongWen_YongHu where user_no=@jsr";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("jsr", jsr);

            int jsrrid;
            try
            {
              
                conn.Open();
                jsrrid = Convert.ToInt32(comm.ExecuteScalar());
                
            }
            catch (Exception)
            {
                dt.TableName = "error!";
                return dt;
            }
            finally
            {
                conn.Close();
            }
            

            comm.CommandText = "SELECT  gwid, lzID, ht,dw, wh, bt, zw, wjxzID, wjlxID, fbr, fbrq, wjlx, wjxz, fbrxm, pID,fsr, fsrxm, jsr, jsrxm, fssj, qssj, qsnr,fsr_rid,jsr_rid,jinji, case when qssj is null then 0 when getdate()-qssj <48/24 then 1 else 0 end as chexiao FROM V_GongWen_List_All where jsr = @jsr ";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("jsr", jsr);




            if (!fsr.Equals(string.Empty))
            {
                comm.CommandText += " and fsr=@fsr ";
                comm.Parameters.AddWithValue("fsr", fsr);
            }
            if (xzid > 0)
            {
                comm.CommandText += " and wjzxid=@xzid ";
                comm.Parameters.AddWithValue("xzid", xzid);
            }

            if (!keyWord.Equals(string.Empty))
            {
                comm.CommandText += " and (wh like @keyword or bt like @keyword) ";
                comm.Parameters.AddWithValue("keyword", "%" + keyWord + "%");
            }
            if (!sTime.Equals(string.Empty))
            {
                comm.CommandText += " and fbrq>@sTime ";
                comm.Parameters.AddWithValue("sTime", sTime);
            }
            if (!eTime.Equals(string.Empty))
            {
                comm.CommandText += " and fbrq<@eTime ";
                comm.Parameters.AddWithValue("eTime", Convert.ToDateTime(eTime).AddDays(1).ToString("yyyy-MM-dd"));
            }
            if (gwtype == 0)
            {
                comm.CommandText += " and qssj is null ";
            }
            else
            {
                switch (lxid)
                {
                    case 0://所有类型
                        dt.TableName = "error!";
                        return dt;
                    case 1://只看局文
                        comm.CommandText += "  and wjlxid=1 ";

                        break;
                    case 2://只看段文
                        if (jsrrid == 21 || jsrrid == 22) //段领导
                        {
                            dt.TableName = "error!";
                            return dt;
                        }
                        else
                        {
                            comm.CommandText += " and wjlxid = 2 ";

                        }
                        break;

                    default:
                        dt.TableName = "error!";
                        return dt;
                }
            }

            comm.CommandText += " order by fssj desc ";


            SqlDataAdapter sda = new SqlDataAdapter(comm);
            
            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWenList";
            return dt;
        }

        internal static DataTable GetDuanWenList(string keyWord, string sTime, string eTime)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  gwid, min(lzID) as lzid, wh, bt,  fbrq, fbrxm, jinji FROM V_GongWen_List_All where wjlxid=2 ";
            comm.Parameters.Clear();
           
            if (!keyWord.Equals(string.Empty))
            {
                comm.CommandText += " and (wh like @keyword or bt like @keyword) ";
                comm.Parameters.AddWithValue("keyword", "%" + keyWord + "%");
            }
            if (!sTime.Equals(string.Empty))
            {
                comm.CommandText += " and fbrq>@sTime ";
                comm.Parameters.AddWithValue("sTime", sTime);
            }
            if (!eTime.Equals(string.Empty))
            {
                comm.CommandText += " and fbrq<@eTime ";
                comm.Parameters.AddWithValue("eTime", Convert.ToDateTime(eTime).AddDays(1).ToString("yyyy-MM-dd"));
            }

            comm.CommandText += " group by gwid,wh,bt,fbrq,fbrxm,jinji ";

            comm.CommandText += " order by fbrq desc ";


            SqlDataAdapter sda = new SqlDataAdapter(comm);

            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWenList";
            return dt;
        }

        internal static DataTable GetGongWenGuiDangList(string fbr, string keyWord, string sTime, string eTime, int type)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            //comm.CommandText = "SELECT gwid,lzid, wh, bt, fbr, fbrq, fbrxm, jsr, jsrxm,jinji, dbo.liu_zhuan_wan_cheng(gwid) AS ShiFouLiuZhuanWanCheng FROM V_GongWen_List_All WHERE (pID = 0) AND fbr=@fbr ";
            comm.CommandText = "SELECT gwid, min(lzid) as lzid, wh, bt, fbr, fbrq, fbrxm, jinji,wjlx, dbo.liu_zhuan_wan_cheng(gwid) AS ShiFouLiuZhuanWanCheng FROM V_GongWen_List_All WHERE(pID = 0) AND fbr = @fbr ";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("fbr", fbr);

            if (!keyWord.Equals(string.Empty))
            {
                comm.CommandText += " and (wh like @keyword or bt like @keyword) ";
                comm.Parameters.AddWithValue("keyword", "%" + keyWord + "%");
            }
            if (!sTime.Equals(string.Empty))
            {
                comm.CommandText += " and fbrq>@sTime ";
                comm.Parameters.AddWithValue("sTime", sTime);
            }
            if (!eTime.Equals(string.Empty))
            {
                comm.CommandText += " and fbrq<@eTime ";
                comm.Parameters.AddWithValue("eTime", Convert.ToDateTime(eTime).AddDays(1).ToString("yyyy-MM-dd"));
            }
            if (type == 0)
            {
                comm.CommandText += " and dbo.liu_zhuan_wan_cheng(gwid)=0 ";
            }
            comm.CommandText += " group by gwid, wh, bt, fbr, fbrq, fbrxm, jinji,wjlx ";
            comm.CommandText += " order by fbrq desc ";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWenGuiDangList";
            return dt;
        }

        internal static DataTable GetGongWenXingZhi()
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT id, wjxz FROM V_GongWen_GWXZ  order by paixu";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWenXingZhi";
            return dt;
        }

        internal static DataTable GetGongWenLeiXing()
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT id, wjlx FROM V_GongWen_GWLX  order by paixu";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWenLeiXing";
            return dt;
        }

        internal static DataTable getGongWenYongHu(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;

            comm.CommandText = "SELECT  user_no, user_name, bm_id, bm_mc, sjh, rid, nc FROM V_GongWen_YongHu where user_no=@work_no  ";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWenYongHu";
            return dt;
        }

        internal static DataTable getGongWenYongHu(int[] rid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;

            comm.CommandText = "SELECT  user_no, user_name, bm_id, bm_mc, sjh, rid, nc FROM V_GongWen_YongHu ";
            string str = rid.ToListString();
            if (!str.Equals(string.Empty))
            {
                comm.CommandText += " where rid in (" + str + ")";
            }
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWenYongHu";
            return dt;
        }

        internal static DataTable GetLiuZhuanXianByLzId(bool sfbr,int lzlvl, int lzid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select b.jsr_bm,b.lzid,b.gwid,b.fsr,b.fsrxm,b.fssj,b.jsr,b.jsrxm,b.qssj,b.qsnr ,case when a.lvl >0 then -1 else dbo.xia_ji_liu_zhuan_wan_cheng_shu(b.lzID) end AS wancheng, ";
            comm.CommandText += " case when a.lvl > 0 then -1 else dbo.xia_ji_liu_zhuan_shu(b.lzID)end AS liuzhuan from liu_zhuan_xian(@lzid) a join V_GongWen_LiuZhuan b on a.lzid = b.lzID ";


            if (sfbr == true)
            {
                comm.CommandText += " and a.benren = 1";
                if (lzlvl == 0)
                {
                    comm.CommandText += " and a.lvl >=-1 ";
                }
                else
                {
                    comm.CommandText += " and a.lvl < 0 ";
                }
            }
            else
            {
                if (lzlvl == 0)
                {
                    comm.CommandText += " and a.lvl =0 ";
                }
                else
                {
                    comm.CommandText += " and a.lvl < 0 ";
                }

            }
            comm.CommandText += " order by a.lvl desc";

            //else
            //{
            //    comm.CommandText = "select b.jsr_bm,b.lzid,b.gwid,b.fsr,b.fsrxm,b.fssj,b.jsr,b.jsrxm,b.qssj,b.qsnr ,dbo.xia_ji_liu_zhuan_wan_cheng_shu(b.lzID) AS wancheng, ";

            //    comm.CommandText += " dbo.xia_ji_liu_zhuan_shu(b.lzID) AS liuzhuan from V_GongWen_LiuZhuan b  where pid=@lzid ";

            //}
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("lzid", lzid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getLiuZhuanXianByLzId";
            return dt;
        }

        internal static DataTable GetLingDaoPiShi(int gwid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select jsrxm,qssj,qsnr from V_GongWen_LiuZhuan where jsr_rid in (21,22) and gwid=@gwid order by lzid";

            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gwid", gwid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getLingDaoPiShi";
            return dt;
        }

        internal static DataTable GetSuoYouWeiQian(int gwid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select gwid,jsr,jsrxm,min(fssj) as fssj ,jsr_bm from V_GongWen_LiuZhuan a join dic_bumen b on a.jsr_bmid = b.bm_id where qssj is null  and gwid=@gwid group by gwid,jsr,jsrxm,jsr_bm,px order by b.px ";

            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gwid", gwid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getSuoYouWeiQian";
            return dt;
        }

        internal static DataTable GetGongWen2016ById(int gwid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  ID as gwid, ht, dw, wh, bt, zw, csyj, wjxzID, wjlxID, fbr, fbrq, wjlx, wjxz, fbrxm FROM V_GongWen_GWXX where id=@gwid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gwid", gwid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWen2016ById";
            return dt;
        }

        internal static DataTable GetGongWenFuJian2016ById(int gwid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  fjmc FROM V_GongWen_FuJian where gwid=@gwid order by paixu";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gwid", gwid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getGongWenFuJian2016ById";
            return dt;
        }

        internal static INT UpdateDuanYu(int id, string newTxt)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_GongWen_ZDYDY set dynr=@dynr,createTime =getdate() where id=@id";

            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("id", id);
            comm.Parameters.AddWithValue("dynr", newTxt);

            try
            {
                conn.Open();
                comm.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static INT DeleteDuanYu(int id)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update  T_GongWen_ZDYDY set isvalid=0 ,createTime =getdate() where id=@id";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("id", id);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static INT AddDuanYu(string workNo, string dynr)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;
                comm.CommandText = "select case when max(paixu) is null then 0 else max(paixu) end from V_GongWen_ZiDingYi_DuanYu where uid=@work_no";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workNo);
                int max = Convert.ToInt32(comm.ExecuteScalar());
                max = (max / 100 + 1) * 100;
                comm.CommandText = "insert into T_GongWen_ZDYDY (uid,dynr,paixu) values(@work_no,@dynr,@paixu)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workNo);
                comm.Parameters.AddWithValue("dynr", dynr);
                comm.Parameters.AddWithValue("paixu", max);
                comm.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static INT UpdateZdybm(int id, string newTxt)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update  T_GongWen_ZDYBM set bmnr=@dynr,createTime =getdate() where id=@id";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("id", id);
            comm.Parameters.AddWithValue("dynr", newTxt);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static INT DeleteZdybm(int id)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update  T_GongWen_ZDYBM set isvalid=0 ,createTime =getdate() where id=@id";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("id", id);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static INT AddZdybm(string workNo, string dynr)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;
                comm.CommandText = "select case when max(paixu) is null then 0 else max(paixu) end from V_GongWen_ZiDingYiBuMen where uid=@work_no";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workNo);
                int max = Convert.ToInt32(comm.ExecuteScalar());
                max = (max / 100 + 1) * 100;
                comm.CommandText = "insert into T_GongWen_ZDYBM (uid,bmnr,paixu) values(@work_no,@dynr,@paixu)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workNo);
                comm.Parameters.AddWithValue("dynr", dynr);
                comm.Parameters.AddWithValue("paixu", max);
                comm.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }


        internal static DataTable GetZiDingYiDuanYu(string workNo, bool onlyPrivate)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  uid,id,dynr FROM V_GongWen_ZiDingYi_DuanYu ";
            if (onlyPrivate)
            {
                comm.CommandText += " where (uid=@uid) ";
            }
            else
            {
                comm.CommandText += " where (uid=@uid or uid=0) ";
            }
            comm.CommandText += " order by uid ,paixu ";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getZiDingYiDuanYu";
            return dt;
        }

        internal static DataTable GetBuMenFenLei(int rid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT flid,flmc, flzc FROM V_GongWen_BuMenFenLei ";
            if (rid == 21)
            {

            }
            else if (rid == 22)
            {
                //comm.CommandText += " where flid<>1 ";
            }
            else if (rid == 23)
            {
                comm.CommandText += " where flid<>1 and flid <> 2 ";
            }
            else if (rid == 20)
            {
                comm.CommandText += " where flid<>1 ";
            }
            else
            {
                return null;
            }
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getBuMenFenLei";
            return dt;
        }

        internal static DataTable GetBuMenFenLeiYongHu(int rid, string workNo, int flid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT user_no ,case when nc is null then user_name else nc end as nc,case when flid=1 then user_name when flid=2 then bm_mc else bm_mc+zhiwu end as xsmc FROM V_GongWen_YongHu_BuMenFenLei where flid=@flid and user_no <>@work_no ";
            if (rid != 21)
            {
                comm.CommandText += " and user_no <> '0001' and user_no<> '0002' ";
            }
            comm.CommandText += " order by paixu,px,ncpaixu,zhiwu desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("flid", flid);
            comm.Parameters.AddWithValue("work_no", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getBuMenFenLei";
            return dt;
        }

        internal static DataTable GetZiDingYiBuMen(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT ID, bmnr FROM V_GongWen_ZiDingYiBuMen where uid=@uid order by paixu";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", workNo);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getZiDingYiBuMen";
            return dt;
        }



        internal static DataTable GetBenBuMenRenYuan(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  count(*)  FROM v_user_role where user_no=@work_no and (rid=23 or rid=24)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            try
            {
                conn.Open();
                if (Convert.ToInt32(comm.ExecuteScalar()) == 0) return new DataTable("error!");
            }
            catch
            {
                return new DataTable("error!");
            }
            finally
            {
                conn.Close();
            }
            conn.Close();

            comm.CommandText = "SELECT user_no, user_name, nc,bm_mc,ziwu FROM V_GongWen_YongHu WHERE (rid = 25 or rid=20 or rid= 26) AND (bm_id IN (SELECT bm_id FROM V_User WHERE (user_no = @work_no))) order by zwpaixu";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }
            if (workNo.Equals("0971"))
            {
                comm.CommandText = "SELECT user_no, user_name, nc,bm_mc,ziwu FROM V_GongWen_YongHu WHERE (rid = 23) AND (bm_id IN (58,59,39))";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workNo);
                DataTable dt1 = new DataTable();
                sda.Fill(dt1);
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["user_no"] = dt1.Rows[i]["user_no"];
                    dr["user_name"] = dt1.Rows[i]["user_name"];
                    dr["nc"] = dt1.Rows[i]["nc"];
                    dr["bm_mc"] = dt1.Rows[i]["bm_mc"];
                    dr["ziwu"] = dt1.Rows[i]["ziwu"];
                    dt.Rows.InsertAt(dr, 0);
                }
            }
            if (workNo.Equals("0112"))
            {
                comm.CommandText = "SELECT user_no, user_name, nc,bm_mc,ziwu FROM V_GongWen_YongHu WHERE (rid = 23) AND (bm_id IN (40))";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("work_no", workNo);
                DataTable dt1 = new DataTable();
                sda.Fill(dt1);
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["user_no"] = dt1.Rows[i]["user_no"];
                    dr["user_name"] = dt1.Rows[i]["user_name"];
                    dr["nc"] = dt1.Rows[i]["nc"];
                    dr["bm_mc"] = dt1.Rows[i]["bm_mc"];
                    dr["ziwu"] = dt1.Rows[i]["ziwu"];
                    dt.Rows.InsertAt(dr, 0);
                }
            }

            dt.TableName = "getBenBuMenRenYuan";
            return dt;
        }

        internal static INT makeCuiBan(string[] jsr,string bt)
        {
            

            INT r;
            if (_sendMessageForDebug)
            {
                r = SendMobileMessage("3974", "公文处理员提醒您，请尽快签收公文：" + bt);
            }
            else
            {
                r = SendMobileMessage(jsr, "公文处理员提醒您，请尽快签收公文：" + bt);

            }
            if (r.Number == 1) r.Message = "已成功催办" + jsr.Length.ToString() + "人。";
            return r;
        }

        internal static INT makeCuiBan(int gwid, int rid, string bt)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT jsr,jsrxm from V_GongWen_List_All where qssj is null and gwid=@gwid and jsr_rid=@rid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gwid", gwid);
            comm.Parameters.AddWithValue("rid", rid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            List<string> jsr = new List<string>();
            List<string> jsrxm = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsr.Add(dt.Rows[i]["jsr"].ToString());
                jsrxm.Add(dt.Rows[i]["jsrxm"].ToString());
            }
            

            INT r ;
            if (_sendMessageForDebug)
            {
                r = SendMobileMessage("3974", "公文处理员提醒您，请尽快签收公文：" + bt);
            }
            else
            {
                r = SendMobileMessage(jsr.ToArray(), "公文处理员提醒您，请尽快签收公文：" + bt);
            }
            if (r.Number == 1) r.Message = jsrxm.ToArray().ToListString();
            return r;
        }

        internal static INT SetZiDingYiBuMenRenYuan(int zdybmid, string[] userNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;
                if (userNo == null || userNo.Length == 0)
                {
                    comm.CommandText = "update  T_GongWen_ZDYBM_User set isvalid=0,createtime=getdate() where zdyid=@zdyid ";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("zdyid", zdybmid);
                    comm.ExecuteNonQuery();
                }
                else
                {
                    string usernoString = userNo.ToListString();

                    comm.CommandText = "update  T_GongWen_ZDYBM_User set isvalid=0,createtime=getdate() where zdyid=@zdyid and user_no not in (" + usernoString + ")";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("zdyid", zdybmid);
                    comm.ExecuteNonQuery();

                    comm.CommandText = "insert into T_GongWen_ZDYBM_User (zdyid,user_no) select @zdyid , user_no from V_GongWen_YongHu where user_no in (" + usernoString + ") and user_no not in (select user_no from V_GongWen_ZDYBM_USERNO where zdyid=@zdyid) ";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("zdyid", zdybmid);
                    comm.ExecuteNonQuery();
                }
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static DataTable getZiDingYiBuMenRenYuan(int zdybmid, bool added)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT user_no, bm_mc + '(' + user_name + ')' as xsmc FROM V_GongWen_YongHu WHERE (rid = 23) AND (user_no ";
            if (added)
            {
                comm.CommandText += " in ";
            }
            else
            {
                comm.CommandText += " not in ";
            }
            comm.CommandText += " (SELECT user_no FROM V_GongWen_ZDYBM_USERNO WHERE (zdyid = @zdyid))) ORDER BY px";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("zdyid", zdybmid);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getZiDingYiBuMenRenYuan";
            return dt;
        }


        internal static INT AddGongWenRenYuan(string workNo, int rid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into dat_user_role (uid,rid) values(@work_no,@rid)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            comm.Parameters.AddWithValue("rid", rid);

            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }


        internal static INT DeleteGongWenRenYuan(string gh, int rid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update dat_user_role  set isvalid =0,createtime =getdate() where uid=@gh and rid=@rid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gh", gh);
            comm.Parameters.AddWithValue("rid", rid);

            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static INT DeleteGongWen2016(int uid, int gwid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_GongWen_GWXX  set isvalid =0,fbrq =getdate() where fbr=@uid and id=@gwid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            comm.Parameters.AddWithValue("gwid", gwid);

            try
            {
                conn.Open();
                int r = comm.ExecuteNonQuery();
                if (r == 0)
                {
                    return new INT(-1, "该公文不是你发布的，不能删除。");
                }
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static bool IsGongWenYongHu(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT count(*) FROM V_GongWen_YongHu WHERE user_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            int count = 0;
            try
            {
                conn.Open();
                count = Convert.ToInt32(comm.ExecuteScalar());
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static DataTable GetYongHuXinXiByGh(string workNo)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            if (IsGongWenYongHu(workNo))
            {
                return null;
            }
            comm.CommandText = "SELECT user_no, user_name,bm_mc,bm_id  FROM V_user WHERE user_no=@work_no";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workNo);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getYongHuXinXiByGh";
            return dt;
        }

        internal static INT UndoSignGongWen2016(int uid, int lzid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            //测试一下看看数据库链接是否正常
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            //设置sql事务
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            comm.Transaction = trans;

            comm.Connection = conn;
            try
            {

                comm.CommandText = "select count(*) from  T_GongWen_LZ where id=@lzid and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("lzid", lzid);
                int count = Convert.ToInt32(comm.ExecuteScalar());
                if (count == 0)
                {
                    return new INT(0, "签阅已撤销或不存在。");
                }
                comm.CommandText = "select jsr from  T_GongWen_LZ where id=@lzid and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("lzid", lzid);
                int jsr = Convert.ToInt32(comm.ExecuteScalar());
                if (jsr != uid)
                {
                    return new INT(0, "只能撤销本人签阅。");
                }
                comm.CommandText = "select qssj from  T_GongWen_LZ where id=@lzid and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("lzid", lzid);
                string str = comm.ExecuteScalar().ToString();
                if (str.Equals(string.Empty))
                {
                    return new INT(0, "未签阅不用撤销。");
                }
                else
                {
                    DateTime qssj = Convert.ToDateTime(str);
                    comm.CommandText = "select getdate()";
                    DateTime now = Convert.ToDateTime(comm.ExecuteScalar());
                    TimeSpan ts = now - qssj;
                    if (ts.TotalHours > 48)
                    {
                        return new INT(0, "签阅已超过48小时，不能撤销。");
                    }
                }

                string jsrStr = jsr.ToWorkNo();
                comm.CommandText = "select gwid from  T_GongWen_LZ where id=@lzid and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("lzid", lzid);
                int gwid = Convert.ToInt32(comm.ExecuteScalar());


                comm.CommandText = "select id from  T_GongWen_LZ  where gwid=@gwid and (jsr=@jsr) and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gwid", gwid);
                comm.Parameters.AddWithValue("jsr", jsrStr);
                SqlDataAdapter sda = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                var i = sda.Fill(dataTable: dt);

                comm.CommandText = "update T_GongWen_LZ set isvalid=0 where gwid=@gwid and (jsr=@jsr or fsr=@jsr) and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gwid", gwid);
                comm.Parameters.AddWithValue("jsr", jsrStr);
                comm.ExecuteNonQuery();

                foreach (DataRow dataRow in dt.Rows)
                {
                    int lzid1 = Convert.ToInt32(dataRow["id"]);
                    comm.CommandText = "insert into T_GongWen_Lz (gwid,pid,fsr,jsr,fssj) select gwid,pid,fsr,jsr,fssj from t_gongwen_lz where id=@lzid";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("lzid", lzid1);
                    comm.ExecuteNonQuery();
                }

               
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1);
        }

        internal static DataTable GetUserNameInPiShi(int rid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select user_no as gh,";
            return null;
        }

        #endregion

        #region 2016自制邮件系统
        /// <summary>
        /// 获取邮件内容
        /// </summary>
        /// <param name="mid">邮件ID号</param>
        /// <returns></returns>
        internal static DataTable GetMailById2016(int mid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT   id, title, body, fromaddress, toaddress, copyaddress, ishtmlformat, createdate, size, status, filenamestring, url,readflag FROM Mail where id=@mid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("mid", mid);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {

                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "GetMailById2016";
            return dt;
        }

        /// <summary>
        /// 获取邮件列表
        /// </summary>
        /// <param name="gh">工号</param>
        /// <param name="type">邮件列表类型，1:收件箱，2:垃圾箱，3:已发送</param>
        /// <returns></returns>
        internal static DataTable GetMailList2016(string gh,int type)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT   id, title, fromaddress, createdate, size, filenamestring ,readflag FROM V_YouJian_YJXX ";
            switch ( type)
            {
                case 1://收件箱
                    comm.CommandText += " where toAddress=@gh and dustbin=0 ";
                    break;
                case 2://垃圾箱
                    comm.CommandText += " where toAddress=@gh and dustbin=1 ";
                    break;
                case 3://已发送
                    comm.CommandText += " where copyAddress=@gh and dustbin=0 ";
                    break;
                default:
                    return null;
            }
            comm.CommandText += " order by id desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("@gh", gh);
            DataTable dt = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {

                dt.TableName = "error!";
                return dt;
            }

            dt.TableName = "getMailList2016";
            return dt;
        }


        internal static DataTable GetYouJianFuJian(string gh, int mid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  url,filenamestring FROM V_YouJian_YJXX where id = @mid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("mid", mid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {


                return null;
            }
            return dt;

        }
        #endregion


        #region 系统维护
        /// <summary>
        /// 添加新用户基本信息
        /// </summary>
        /// <param name="gh">工号</param>
        /// <param name="xm">姓名</param>
        /// <param name="bmid">部门id</param>
        /// <param name="sjh">手机号</param>
        /// <param name="zw">职务，公文系统显示用</param>
        /// <param name="gz">工种，暂时没用</param>
        /// <returns></returns>
        internal static INT AddUserBaseInfo(string gh, string xm, int bmid, string sjh, string zw, string gz)
        {
            //初始密码和工号一样，获取初始密码的md5值
            string mm = BLL.SetEncryptPass(gh, gh);


            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            //测试一下看看数据库链接是否正常
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            //设置sql事务
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            comm.Transaction = trans;

            comm.Connection = conn;
            //开始添加数据
            try
            {
                //添加用户基本信息表
                comm.CommandText = "insert into dic_user (user_no,user_name,user_mima,user_mima1,bumen_id,ziwu,gongzhong,sjh) values(@gh,@xm,@mm,@mm,@bmid,@zw,@gz,@sjh)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gh", gh);
                comm.Parameters.AddWithValue("xm", xm);
                comm.Parameters.AddWithValue("mm", mm);
                comm.Parameters.AddWithValue("bmid", bmid);
                comm.Parameters.AddWithValue("zw", zw);
                comm.Parameters.AddWithValue("gz", gz);
                comm.Parameters.AddWithValue("sjh", sjh);
                comm.ExecuteNonQuery();

                // 添加用户角色表，给每个人都添加角色1：所有员工
                comm.CommandText = "insert into dat_user_role (rid,uid) values(1,@gh)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gh", gh);
                comm.ExecuteNonQuery();

                //添加用户手机号表，默认添加移动手机号，mtype=1
                comm.CommandText = "insert into dat_user_mobile (work_no,mobile,mtype) values(@gh,@sjh,1)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gh", gh);
                comm.Parameters.AddWithValue("sjh", sjh);
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            trans.Commit();
            return new INT(1);
        }

        internal static INT DeleteUserBaseInfo(string gh, string xm, int bmid, string sjh, string zw, string gz)
        {
            //初始密码和工号一样，获取初始密码的md5值
            string mm = BLL.SetEncryptPass(gh, gh);


            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            //测试一下看看数据库链接是否正常
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            //设置sql事务
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            comm.Transaction = trans;

            comm.Connection = conn;
            //开始添加数据
            try
            {
                //添加用户基本信息表
                comm.CommandText = "insert into dic_user (user_no,user_name,user_mima,user_mima1,bumen_id,ziwu,gongzhong,sjh) values(@gh,@xm,@mm,@mm,@bmid,@zw,@gz,@sjh)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gh", gh);
                comm.Parameters.AddWithValue("xm", xm);
                comm.Parameters.AddWithValue("mm", mm);
                comm.Parameters.AddWithValue("bmid", bmid);
                comm.Parameters.AddWithValue("zw", zw);
                comm.Parameters.AddWithValue("gz", gz);
                comm.Parameters.AddWithValue("sjh", sjh);
                comm.ExecuteNonQuery();

                // 添加用户角色表，给每个人都添加角色1：所有员工
                comm.CommandText = "insert into dat_user_role (rid,uid) values(1,@gh)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gh", gh);
                comm.ExecuteNonQuery();

                //添加用户手机号表，默认添加移动手机号，mtype=1
                comm.CommandText = "insert into dat_user_mobile (work_no,mobile,mtype) values(@gh,@sjh,1)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("gh", gh);
                comm.Parameters.AddWithValue("sjh", sjh);
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            trans.Commit();
            return new INT(1);
        }

        #endregion


        #region 段内通知

        public static DataTable GetTongZhiLeiXing(int uid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand
            {
                Connection = conn,
                CommandText = "SELECT lxid , lxmc FROM V_TongZhi_bumen_leixing where bmid in( select bmid from V_user where uid=@uid) order by paixu"
            };
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {
                return null;
            }
            return dt;
        }

        public static DataTable GetAllTongZhiLeiXing()
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand
            {
                Connection = conn,
                CommandText = "select 0,'所有类型',0 as paixu union SELECT lxid , lxmc,paixu FROM V_TongZhi_bumen_leixing order by paixu"
            };
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {
                return null;
            }
            return dt;
        }

        internal static DataTable GetTongZhiBuMenFenLeiYongHu(int uid,int flid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT user_no ,case when nc is null then user_name else nc end as nc,case when flid=1 then user_name when flid=2 then bm_mc else bm_mc+zhiwu end as xsmc FROM V_TongZhi_YongHu_BuMenFenLei where flid=@flid and uid <>@uid ";
            comm.CommandText += " order by paixu,px,ncpaixu,zhiwu desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("flid", flid);
            comm.Parameters.AddWithValue("uid", uid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                return null;
            }

            dt.TableName = "getBuMenFenLei";
            return dt;
        }

        internal  static DataTable GetTongZhiBuMenFenLei(int uid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;

            try
            {
                conn.Open();
                comm.CommandText = "select count(*) from v_user_role where uid=@uid and rid=27 ";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("uid", uid);

                if (Convert.ToInt32(comm.ExecuteScalar()) <= 0) //没有发通知的权限
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
            finally
            {

                conn.Close();
            }


            comm.CommandText = "SELECT flid,flmc, flzc FROM V_TongZhi_BuMenFenLei ";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                return null;
            }

            dt.TableName = "getBuMenFenLei";
            return dt;
        }

        public static INT AddNewTongZhi2016(string bt, string zw, int fbrid, int lxid, int[] jsrList, string[] tzfj, string ip, int sfgk)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            //测试一下看看数据库链接是否正常
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                return new INT(-1, ex.Message);
            }
            //设置sql事务
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            comm.Transaction = trans;

            comm.Connection = conn;
            try
            {

                comm.CommandText = "insert into T_TongZhi_TZXX (Bt,Zw,tzlxID,fbrID,sfgk,ip) values(@bt,@zw,@lxid,@fbrid,@sfgk,@ip)";
                comm.CommandText += ";select scope_identity();";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("bt", bt);
                comm.Parameters.AddWithValue("zw", zw);
                comm.Parameters.AddWithValue("lxid", lxid);
                comm.Parameters.AddWithValue("fbrid", fbrid);
                comm.Parameters.AddWithValue("sfgk", sfgk);
                comm.Parameters.AddWithValue("ip", ip);

                int tzid = Convert.ToInt32(comm.ExecuteScalar());

                //依次把附件添加进附件表
                for (int i = 0; i < tzfj.Length; i++)
                {
                    comm.CommandText = "insert into t_tongzhi_tzfj (tzid,fjmc,paixu) values(@tzid,@fjmc,@paixu)";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("tzid", tzid);
                    comm.Parameters.AddWithValue("fjmc", tzfj[i]);
                    comm.Parameters.AddWithValue("paixu", i + 1);
                    comm.ExecuteNonQuery();
                }

                foreach (int jsr in jsrList)
                {
                    //添加公文流转表开始流转
                    comm.CommandText = "insert into t_tongzhi_lz (tzid,pid,fsrid,jsrid,fssj) values(@tzid,0,@fsr,@jsr,getdate())";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("@tzid", tzid);
                    comm.Parameters.AddWithValue("@fsr", fbrid);
                    comm.Parameters.AddWithValue("@jsr", jsr);
                    comm.ExecuteNonQuery();
                }


                //发短信通知
                string message, chenghu;
                comm.CommandText = "select bm_mc+ user_name from V_user where uid=@uid";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("@uid", fbrid);
                chenghu = Convert.ToString( comm.ExecuteScalar());

                message = chenghu + "给您发送了一件段内通知。标题：" + bt + "。请尽快签阅。";
  

                INT r;
                if (_sendMessageForDebug)
                {
                    r = SendMobileMessage("3974", message);
                }
                else
                {
                    r = SendMobileMessage2(jsrList, message);
                }
                if (r.Number == 1)
                {
                    //所有操作都成功完成，提交事务，确认操作
                    trans.Commit();
                }
                else
                {
                    //发短信时提醒失败
                    //回滚事务，撤销操作，返回错误信息
                    trans.Rollback();
                    return new INT(-1, "发送提醒短信失败。公文创建未成功。");
                }
            }

            catch (Exception ex)
            {
                //在操作中失败
                //回滚事务，撤销操作，返回错误信息
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            //返回成功
            return new INT(1);
        }



        internal static DataTable GetTongZhiList(int uid, int lxid, int fsrid, string keys, string sTime, string eTime, int type)
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;



            comm.CommandText = "SELECT  tzid, lzID, bt,  fsrxm, fssj, qssj, qsnr  FROM V_TongZhi_List_All where 1=1 ";
            comm.Parameters.Clear();


            if (uid > 0)
            {
                comm.CommandText += " and jsrid = @uid ";
                comm.Parameters.AddWithValue("uid", uid);
                if (type == 0)
                {
                    comm.CommandText += " and qssj is null ";
                }
            }
            else
            {
                comm.CommandText += " and sfgk=1 ";
                
            }

            if (fsrid>0)
            {
                comm.CommandText += " and fsrid=@fsrid ";
                comm.Parameters.AddWithValue("fsrid", fsrid);
            }
            if (lxid > 0)
            {
                comm.CommandText += " and tzlxid=@lxid ";
                comm.Parameters.AddWithValue("lxid", lxid);
            }

            if (!keys.Equals(string.Empty))
            {
                comm.CommandText += " and (bt like @keyword) ";
                comm.Parameters.AddWithValue("keyword", "%" + keys + "%");
            }
            if (!sTime.Equals(string.Empty))
            {
                comm.CommandText += " and fbrq>@sTime ";
                comm.Parameters.AddWithValue("sTime", sTime);
            }
            if (!eTime.Equals(string.Empty))
            {
                comm.CommandText += " and fbrq<@eTime ";
                comm.Parameters.AddWithValue("eTime", Convert.ToDateTime(eTime).AddDays(1).ToString("yyyy-MM-dd"));
            }
           
           

            comm.CommandText += " order by fssj desc ";


            SqlDataAdapter sda = new SqlDataAdapter(comm);

            try
            {
                sda.Fill(dt);
            }
            catch (Exception)
            {
                return null;
            }

            dt.TableName = "GetTongZhiList";
            return dt;
        }


        internal static DataTable GetTongZhi2016ById(int tzid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  tzid,bt, zw, tzlxID, fbrid, fbrq, fbrbmmc + fbrxm as fbrxm ,lxmc FROM V_TongZhi_TZXX where tzid=@tzid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("tzid", tzid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                return null;
            }

            dt.TableName = "GetTongZhi2016ById";
            return dt;
        }

        internal static DataTable GetTongZhiFuJian2016ById(int tzid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  fjmc FROM V_TongZhi_FuJian where tzid=@tzid order by paixu";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("tzid", tzid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                return null;
            }

            dt.TableName = "getGongWenFuJian2016ById";
            return dt;
        }


        internal static DataTable GetBuMenFenLeiRenYuanByUid(int uid)
        {
            string workno = uid.ToString();
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  user_no FROM V_User where uid=@uid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            try
            {
                conn.Open();
                workno = Convert.ToString(comm.ExecuteScalar());
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
            
            return GetBenBuMenRenYuan(workno);
        }


        internal static INT SignTongZhi2016(int tzid, int lzid, int uid, int[] jsr, string biaoTi, string pishi, string ip)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT(-1, "数据库错误");
            }
            SqlTransaction trans;
            trans = conn.BeginTransaction();
            try
            {
                comm.Transaction = trans;

                comm.CommandText = "select gwid,jsr,case when qssj is null then 0 else 1 end as sfqs from t_tongzhi_lz where id=@lzid and isvalid=1";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("lzid", lzid);
                SqlDataAdapter sda = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                int dbGwid = Convert.ToInt32(dt.Rows[0]["gwid"]);
                string dbFsr = Convert.ToString(dt.Rows[0]["jsr"]);
                int sfqs = Convert.ToInt32(dt.Rows[0]["sfqs"]);
                if (sfqs == 1) return new INT(-1, "该公文已经签收，无法再次签收。");



                comm.CommandText = "update t_tongzhi_lz set qsnr=@qsnr,qssj=getdate(),ipAddress=@ip where tzid=@tzid and jsridd=@uid and qssj is null";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("tzid", tzid);
                comm.Parameters.AddWithValue("uid", uid);
                comm.Parameters.AddWithValue("qsnr", pishi);
                comm.Parameters.AddWithValue("ip", ip);
                int rows = comm.ExecuteNonQuery();

                if (rows == 0) return new INT(-1, "该公文已经签收，无法再次签收。");


                if (jsr != null && jsr.Length > 0)
                {
                    foreach (int jsrid in jsr)
                    {
                        comm.CommandText = "insert into t_tongzhi_lz (tzid,pid,fsr,jsr,fssj) values(@tzid,@pid,@fsr ,@jsr, getdate())";
                        comm.Parameters.Clear();
                        comm.Parameters.AddWithValue("@gwid", tzid);
                        comm.Parameters.AddWithValue("@pid", lzid);
                        comm.Parameters.AddWithValue("@fsr", uid);
                        comm.Parameters.AddWithValue("@jsr", jsrid);
                        comm.ExecuteNonQuery();
                    }
                    //发短信通知

                  
                    string message = "您有一件新通知需签阅。标题：" + biaoTi;
                    
                
                    INT r;
                    if (_sendMessageForDebug)
                    {
                        r = SendMobileMessage("3974", message);
                    }
                    else
                    {
                        r = SendMobileMessage2(jsr, message);
                    }


                    if (r.Number == 1)
                    {
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                        return new INT(-1, "发送提醒短信失败。公文创建未成功。");
                    }
                }
                else
                {
                    trans.Commit();
                }
            }

            catch (Exception ex)
            {
                trans.Rollback();
                return new INT(-1, ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return new INT(1);
        }


        internal static DataTable GetTongZhiLiuZhuanXianByLzId(bool sfbr, int lzlvl, int lzid)
        {
            SqlConnection conn = new SqlConnection(BaseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select b.jsrbmmc,b.lzid,b.tzid,b.fsrid,b.fsrxm,b.fssj,b.jsrid,b.jsrxm,b.qssj,b.qsnr ,case when a.lvl >0 then -1 else dbo.tongzhi_xia_ji_liu_zhuan_wan_cheng_shu(b.lzID) end AS wancheng, ";
            comm.CommandText += " case when a.lvl > 0 then -1 else dbo.tongzhi_xia_ji_liu_zhuan_shu(b.lzID)end AS liuzhuan from tongzhi_liu_zhuan_xian(@lzid) a join V_TongZhi_LiuZhuan b on a.lzid = b.lzID ";


            if (sfbr == true)
            {
                comm.CommandText += " and a.benren = 1";
                if (lzlvl == 0)
                {
                    comm.CommandText += " and a.lvl >=-1 ";
                }
                else
                {
                    comm.CommandText += " and a.lvl < 0 ";
                }
            }
            else
            {
                if (lzlvl == 0)
                {
                    comm.CommandText += " and a.lvl =0 ";
                }
                else
                {
                    comm.CommandText += " and a.lvl < 0 ";
                }

            }
            comm.CommandText += " order by a.lvl desc";

            //else
            //{
            //    comm.CommandText = "select b.jsr_bm,b.lzid,b.gwid,b.fsr,b.fsrxm,b.fssj,b.jsr,b.jsrxm,b.qssj,b.qsnr ,dbo.xia_ji_liu_zhuan_wan_cheng_shu(b.lzID) AS wancheng, ";

            //    comm.CommandText += " dbo.xia_ji_liu_zhuan_shu(b.lzID) AS liuzhuan from V_GongWen_LiuZhuan b  where pid=@lzid ";

            //}
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("lzid", lzid);
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            try
            {
                sda.Fill(dt);
            }
            catch
            {
                return null;
            }

            dt.TableName = "getLiuZhuanXianByLzId";
            return dt;
        }
        #endregion
    }
}