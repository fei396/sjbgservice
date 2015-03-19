using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using sjbgWebService.gwxx;
using AE.Net.Mail.Imap;
using AE.Net.Mail;
namespace sjbgWebService
{

    public static class DAL
    {
        private static string gwConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["gwxxConnectionString"].ConnectionString;
        private static string baseConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["centerConnectionString"].ConnectionString;
        private static string yyConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["yyConnectionString"].ConnectionString;
        private static string zbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["zbConnectionString"].ConnectionString;
        private static string ysConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ysConnectionString"].ConnectionString;
        private static string cbConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["cbConnectionString"].ConnectionString;
        private static string mqttConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["mqttConnectionString"].ConnectionString;
        public static DataTable getProductByPid(string pname)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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

        public static DataTable getXwlx()
        {
            DataTable dt = new DataTable();

            string sql = "SELECT id, sclb FROM dic_wlsclb where isvalid=1";
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static DataTable getXwlb(int xwlx)
        {
            DataTable dt = new DataTable();

            string sql = "SELECT     id, WYBT, WJMC, wjnr, SCRQ, SCLB,sclbid, yhbm,path FROM V_SJBG_wlscxx where isvalid=1 and sclbid=@xwlx order by scrq desc";
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static BOOLEAN isSigned(GongWen gw, UserGw user)
        {
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static BOOLEAN leaderSign(GongWen gw, UserGw user, Instruction ins, UserGw[] nextUsers)
        {
            string wh = gw.Number;
            if (wh.Equals("")) return new BOOLEAN(false, "");
            if (user.Yhbh == 0) return new BOOLEAN(false, "");
            SqlConnection conn = new SqlConnection(gwConnStr);
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
        public static BOOLEAN sign(GongWen gw, UserGw user)
        {
            string wh = gw.Number;
            if (wh.Equals("")) return new BOOLEAN(false, "");
            if (user.Yhbh == 0) return new BOOLEAN(false, "");
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static DataTable getLdps(GongWen gw)
        {
            string wh = gw.Number;
            DataTable dt = new DataTable();

            string sql = "select id,wh,psr,psnr,psrq from dat_ldps where wh=@wh order by id";
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static DataTable getAllGwlb(UserGw user, gwlx gwlx, dwlx dwlx)
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
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static DataTable getUnfinishedGwlb(UserGw user, dwlx dwlx)
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
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static DataTable getGwxxByWh(string wh)
        {
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static DataTable getUserGwByUid(int uid)
        {
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static DataTable getUserGwByUserName(string userName)
        {
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        public static int getSsxzbmBySsbm(int ssbm)
        {
            SqlConnection conn = new SqlConnection(gwConnStr);
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


        public static bool initPassword()
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select user_no from dic_user where bumen_id=34";
            SqlDataAdapter sda = new SqlDataAdapter(comm);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string work_no = Convert.ToString(dt.Rows[i]["user_no"]);
                string pass = BLL.setEncryptPass(work_no, work_no);
                comm.Connection = conn;
                comm.CommandText = "update dic_user set user_mima=@pass where user_no=@work_no";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("pass", pass);
                comm.Parameters.AddWithValue("work_no", work_no);
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

        public static bool setTqjyPassword(int uid, string newpass)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();
            string pass = BLL.setEncryptPass(uid.ToString(), newpass);
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

        public static string getProductUserIdByBaseNum(string user_no, int pid)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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
                return user_no;
            }
            else
            {
                comm.CommandText = "select yyxt_userbh from dat_UserDuiYing where yyxt_id=@pid and xtbg_user=@user_no";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("pid", pid);
                comm.Parameters.AddWithValue("user_no", user_no);
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

        public static DataTable getUserByNum(string user_no)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select user_no,user_name,Bumen_id from dic_user where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", user_no);
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

        internal static INT loginDirect(string user_no, string user_pass, string code, string ip, string deviceInfo, string deviceVer)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            DataTable dt = new DataTable();
            comm.Connection = conn;

            comm.CommandText = "select user_mima from dic_user where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", user_no);
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
                user_pass = BLL.setEncryptPass(user_no, user_pass);
                if (!user_pass.Equals(pass1)) return new INT(-3, "密码错误");//密码错误
            }

            comm.CommandText = "select mobile from dat_user_mobile where work_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", user_no);
            conn.Open();
            string mobile = comm.ExecuteScalar().ToString();


            comm.CommandText = "insert into dat_User_Device (work_no,deviceID,mobile, isvalid) values(@user_no,@code,@mobile,1)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", user_no);
            comm.Parameters.AddWithValue("code", code);
            comm.Parameters.AddWithValue("mobile", mobile);
            comm.ExecuteNonQuery();
            conn.Close();
            return recordLogin(user_no, code, ip, deviceInfo, deviceVer);

        }

        public static INT login(string user_no, string user_pass, string code, string ip, string deviceInfo, string deviceVer)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            DataTable dt = new DataTable();
            comm.Connection = conn;

            comm.CommandText = "select user_mima from dic_user where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", user_no);
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
                user_pass = BLL.setEncryptPass(user_no, user_pass);
                if (!user_pass.Equals(pass1)) return new INT(-3, "密码错误");//密码错误
            }

            comm.CommandText = "select * from dat_User_Device where work_no=@user_no and isvalid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", user_no);
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
                    if (hasLoginInfo(user_no, deviceInfo, deviceVer))
                    {
                        comm.CommandText = "insert into dat_user_device (work_no, deviceID,mobile,isValid) values(@work_no, @did ,'1234567890',1)";
                        comm.Parameters.Clear();
                        comm.Parameters.AddWithValue("work_no", user_no);
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
                return recordLogin(user_no, code, ip, deviceInfo, deviceVer);
            }
        }

        private static bool hasLoginInfo(string work_no, string dInfo, string dVer)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.CommandText = "select count(*) from dat_logininfo where uid=@work_no and  deviceInfo=@dInfo and deviceVer = @dVer";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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

        private static INT recordLogin(string user_no, string code, string ip, string deviceInfo, string deviceVer)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into dat_logininfo(uid,deviceId,deviceInfo,deviceVer,ipAddress) values(@user_no,@code,@deviceInfo,@deviceVer,@ip)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", user_no);
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

        public static DataTable getZbPerson(DateTime dateTime, int isNight)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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

        public static string getZbLdps(DateTime dateTime)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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

        internal static DataTable getLeaderList()
        {
            DataTable dt = new DataTable();
            string sql = "select yhbh,yhmc,yhsm,yhnc,ssbm,yhmm,yhqx,wjxz,dld_order,shuangqian from V_BanZiChengYuan order by dld_order";
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        internal static DataTable getGwbmList(int lbid)
        {
            DataTable dt = new DataTable();
            string sql = "select bmbh,bmmc,lbmc,ssxzbm,dic_bmlb.id as lbid from dic_bmlb,dic_bmxx where dic_bmlb.id = dic_bmxx.bmlb and dic_bmlb.id=@lbid order by dic_bmlb.orders,dic_bmxx.orders";
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        internal static DataTable getGwbmlbList()
        {
            DataTable dt = new DataTable();
            string sql = "select lbmc ,id from dic_bmlb order by orders";
            SqlConnection conn = new SqlConnection(gwConnStr);
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

        internal static DataTable getJianBaoBuMen()
        {
            DataTable dt = new DataTable();
            string sql = "select bmmc from dic_jb_bm order by orders";
            SqlConnection conn = new SqlConnection(zbConnStr);
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

        internal static DataTable getJianBao(string dept, DateTime datetime)
        {
            DataTable dt = new DataTable();
            string sql = "select bm ,jbxm,jbnr,jbrq,orders from dat_scjb INNER JOIN dic_jb_bm ON dat_scjb.bm = dic_jb_bm.bmmc where jbrq=@jbrq  and (bm=@bm or @bm='all') order by orders,dat_scjb.id";
            SqlConnection conn = new SqlConnection(zbConnStr);
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

        internal static INT getTqUidByWorkNo(string work_no)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select uid from T_TQYJ_USER where work_no=@work_no and isValid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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

        internal static INT getTqUtypeByUid(int uid)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
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

        internal static INT getTqUDeptByUid(int uid)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
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

        internal static BOOLEAN replyTq(int uid, int tid, string replayContent)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select umid from T_TQYJ_USER_MESSAGE where uid=@uid and mid=@tid and isValid=1";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", uid);
            comm.Parameters.AddWithValue("tid", tid);
            int umid = -1;
            try
            {
                conn.Open();
                umid = Convert.ToInt32(comm.ExecuteScalar());
                return setTqReply(umid, replayContent);
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

        public static BOOLEAN setTqReply(int umid, string txt)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
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

        internal static DataTable getTeQingByUid(int uid)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT [uid], [umid], [mid], [senderId],[mtitle], [rGh], [rName], [rRmark], [mtext], [sendTime], [needReply], [sGh], [sName], [sRemark], [deptid], [deptName], [readTime] FROM [V_TQYJ_User_Message] WHERE ([uid] = @uid) order by sendtime desc";
            SqlConnection conn = new SqlConnection(yyConnStr);
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

        internal static DataTable checkReply(int uid)
        {
            DataTable dt = new DataTable();
            int udept = getTqUDeptByUid(uid).Number;
            int utype = getTqUtypeByUid(uid).Number;
            string sql;
            SqlConnection conn = new SqlConnection(yyConnStr);
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

        internal static DataTable checkReplyDetails(int tid)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT txt, rGh, rName, rRmark, mtext, sendTime, needReply,readTime,rdeptname FROM V_TQYJ_Message_Reply WHERE ([mid] = @mid)";
            SqlConnection conn = new SqlConnection(yyConnStr);
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

        internal static BOOLEAN setNewPass(string user_no, string oldPass, string newPass)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            string pass = BLL.setEncryptPass(user_no, newPass);
            comm.Connection = conn;

            //验证旧密码
            comm.CommandText = "select user_mima from dic_user where user_no=@user_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("user_no", user_no);
            try
            {
                conn.Open();
                string passDataBase = Convert.ToString(comm.ExecuteScalar());
                if (!passDataBase.Equals(BLL.setEncryptPass(user_no, oldPass)))
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
            comm.Parameters.AddWithValue("user_no", user_no);
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

        internal static DateTime getServerTime()
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            conn.Open();
            comm.CommandText = "select getdate()";
            return Convert.ToDateTime(comm.ExecuteScalar());
        }

        internal static INT insertRegisterCode(int work_no, string mobile, string code, string uniqueCode)
        {
            string workno = work_no.ToString().PadLeft(4, '0');
            SqlConnection conn = new SqlConnection(baseConnStr);
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
                comm.Parameters.AddWithValue("releaseTime", getServerTime().AddMinutes(2));
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

        internal static INT sendMobileMessage(int work_no, string content)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into dataexchange (data_no,data_content) values(@work_no,@content)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no.ToString().PadLeft(4, '0'));
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

        /// <summary>
        /// 检查用户手机号和移动设备时候已经注册。0未注册，1已注册，-100数据库错误，-1没有该工号,-2工号重复，-3没有该手机号，-4手机号重复，-5移动设备重复
        /// </summary>
        /// <param name="work_no">工号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="uniqueCode">移动设备唯一号</param>
        /// <returns>0未注册，1已注册，-100数据库错误，-1没有该工号,-2工号重复，-3没有该手机号，-4手机号重复，-5移动设备重复</returns>
        internal static INT checkUserMobile(int work_no, string mobile, string uniqueCode)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            string workno = work_no.ToString().PadLeft(4, '0');
            //工号是否在人员信息库内
            comm.CommandText = "select count(*)  from dic_user where user_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workno);
            int i1 = -1;
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
            int i2 = -1;
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
            int i3 = -1;
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
        /// <param name="work_no">工号</param>
        /// <param name="mobile">手机号</param>
        /// <param name="uniqueCode">移动设备唯一号</param>
        /// <returns>0已注册,-98验证码不正确</returns>
        internal static INT registerDevice(int work_no, string mobile, string uniqueCode, string rCode, string sq, string sa, string email)
        {
            string workno = work_no.ToString().PadLeft(4, '0');
            INT i = checkUserMobile(work_no, mobile, uniqueCode);
            if (i.Number < 0) return i;
            else if (i.Number == 1) return new INT(0, i.Message);
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;

            //判断验证码是否正确

            comm.CommandText = "select code from V_DAT_Valid_Register_Code where work_no=@work_no and deviceID=@deviceID and mobile = @mobile order by releaseTime desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", workno);
            comm.Parameters.AddWithValue("deviceID", uniqueCode);
            comm.Parameters.AddWithValue("mobile", mobile);
            string rCode1 = "";
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

        internal static WenJianJia selectMailBox(int uid, string mailBoxName)
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

        internal static YouJian getMailMessage(int uid, int muid, string mailBoxName)
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

        internal static INT sendMail(int uid, YouJian yj)
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

        internal static INT sendMail(int uid, int importance, string subject, string body, string from, string to, string cc, string bcc, string attachment)
        {
            string workno = uid.ToString().PadLeft(4, '0');
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Host = SjbgConfig.MailHost;
            smtp.Port = SjbgConfig.MailSmtpPort;
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

        internal static YouJianSimple[] getMailMessages(int uid, int muids, int muide, string mailBoxName)
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

        internal static YouJianFuJian[] getMailAttachment(int uid, int muid, string mailBoxName, int pos)
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

        internal static WenJianJia[] getMailBoxList(int uid)
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

        internal static INT deleteMailMessage(int uid, int muid, string mailBoxName)
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

        internal static INT moveMailMessage(int uid, int muid, string oldMailBox, string newMailBox)
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

        internal static INT deleteMailBox(int uid, string mailBoxName)
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

        internal static INT renameMailBox(int uid, string oldMailBoxName, string newMailBoxName)
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

        public static DataTable getGpsByNum(string gps_data)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT work_no,work_name,JingDu,WeiDu,WeiZhi,ShiJian FROM dat_gps";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("gps_data", gps_data);
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


        internal static INT gpscs(string work_no, string work_name, string JingDu, string WeiDu, string WeiZhi, string ShiJian)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into dat_gps (work_no,work_name,JingDu,WeiDu,WeiZhi,ShiJian) values(@work_no,@work_name,@JingDu,@WeiDu,@WeiZhi,@ShiJian)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
            comm.Parameters.AddWithValue("work_name", work_name);
            comm.Parameters.AddWithValue("JingDu", Convert.ToDouble(JingDu));
            comm.Parameters.AddWithValue("WeiDu", Convert.ToDouble(WeiDu));
            comm.Parameters.AddWithValue("WeiZhi", WeiZhi);
            comm.Parameters.AddWithValue("ShiJian", ShiJian);
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

        internal static DataTable getUserRole(string work_no)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT rid,rname,descr FROM v_user_role where user_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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

        internal static DataTable getUserMenu(string work_no)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT DISTINCT M2Id, M2Name, M1Id, M1Name, Enabled,ImageRes,ActivityName,Params,order1,order2 FROM V_User_Menu WHERE user_no = @work_no order by order1,order2";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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

        public static DataTable getJcjhByNum(string jcjh_data)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT plan_date,open_time,locus,engi_brand,engi_no FROM duty_cq  order by plan_date desc,open_time desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("jcjh_data", jcjh_data);
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

        public static DataTable getRyjhByNum(string ryjh_data)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT plan_date + ' ' + Open_time AS plan_date,locus,engi_brand+engi_no AS engi_no,Roadway,ZunDian_time,driver_1no+driver_1name as driver_1no,driver_2no+driver_2name as driver_2no,driver_3no+driver_3name as driver_3no FROM duty_cq  order by plan_date desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("ryjh_data", ryjh_data);
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

        public static DataTable getDcjhByNum(string dcjh_data)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT convert(varchar(10),Wait_date,111) + ' ' + left(Wait_time,5) AS plan_date,dri_Room_no,convert(varchar(4),Drive_no)+Drive_name as Drive_name,convert(varchar(5),Dri_time,8) as Dri_time,convert(varchar(4),Ass_no)+Ass_name as Ass_name,convert(varchar(5),Ass_time,8) as Ass_time,convert(varchar(4),Student_no)+Student_name as Student_name,convert(varchar(5),Stu_time,8) as Stu_time FROM Wait_Duty where datediff(day,Wait_date,getdate())='0' order by wait_time desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("dcjh_data", dcjh_data);
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

        public static DataTable getJxjhByNum(string jxjh_data)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT plan_date + ' ' + Open_time AS plan_date,locus,engi_brand+engi_no AS engi_no,Roadway,ZunDian_time,driver_1no,driver_2no,driver_3no FROM duty_cq";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("jxjh_data", jxjh_data);
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

        public static DataTable getCjcxByNum(string cjcx_data)
        {
            SqlConnection conn = new SqlConnection(yyConnStr);
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "SELECT top 10  LEFT(日期, 10) + ' ' + LEFT(时间, 5) AS plan_date, 用户名 AS dd, 工号+姓名 AS work_no, 结果 AS cjjg FROM recordset where datediff(dd,日期,getdate())='0' order by 日期 desc,时间 desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("cjcx_data", cjcx_data);
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


        private static string getConnStringByDataBase(int database)
        {
            string connString;
            switch (database)
            {
                case 1: //运用
                    connString = yyConnStr;
                    break;
                case 2://月山
                    connString = ysConnStr;
                    break;
                default:
                    connString = "";
                    break;
            }
            return connString;
        }

        internal static DataTable getMingPaiByXianBie(int database, string line_or_workno, int type)
        {
            SqlConnection conn = new SqlConnection(getConnStringByDataBase(database));


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
            comm.Parameters.AddWithValue("param", line_or_workno);
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

        internal static DataTable getMingPaiByGongHao(int database, string work_no)
        {
            SqlConnection conn = new SqlConnection(getConnStringByDataBase(database));
            SqlCommand comm = new SqlCommand();

            comm.Connection = conn;
            comm.CommandText = "select work_no,line_mode,state from person_active where work_no=@work_no ";

            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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
                return getMingPaiByXianBie(database, work_no, type);
            }
            else
            {
                return getMingPaiByXianBie(database, dt.Rows[0]["line_mode"].ToString(), type);
            }
        }

        internal static DataTable getXianBie(int database)
        {
            SqlConnection conn = new SqlConnection(getConnStringByDataBase(database));
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

        internal static DataTable getDaMingPai(int database, string line, int type, string filter)
        {
            SqlConnection conn = new SqlConnection(getConnStringByDataBase(database));
            SqlCommand comm = new SqlCommand();
            line = getXbidByXbmc(database, line);
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

        public static string getXbidByXbmc(int database, string xbmc)
        {
            SqlConnection conn = new SqlConnection(getConnStringByDataBase(database));
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

        public static DataTable getCanBu(string work_no, DateTime kssj, DateTime jssj)
        {
            EcardService.Service s = new EcardService.Service();
            return s.getCanBu(work_no, kssj, jssj);
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

        public static DataTable getFyyjcZt()
        {
            OracleService.fyyjcService os = new OracleService.fyyjcService();
            return os.getFyyjcZt();
        }
        public static DataTable getFyyjc(string jczt)
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
        public static DataTable getBookName(string bookname_data)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT id, Name,Address FROM Book_Name";
            if (!bookname_data.Equals(""))
            {
                comm.CommandText += " where id = @id";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("id", bookname_data);
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
        public static DataTable getBookNr(string book_Nr_id)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT id, Txt FROM Book_NR_zhh where id = @id";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("id", book_Nr_id);
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

        internal static int getDutyRoomIdByWork_no(string work_no)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT drid FROM V_SendFile_DutyRoom_Receiver where work_no = @work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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

        internal static DataTable getSentFiles(string work_no)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT fid, fileName, sendTime, allCount, receiveCount,  senderDeptName + senderName  as sender FROM  V_SendFile_SentFiles where sender=@sender ORDER BY sendTime DESC";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("sender", work_no);
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

        internal static DataTable getSentFileDetails(int fid)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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

        internal static DataTable getFilesToReceive(int drid, int type)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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

        internal static INT receiveFile(int fdrid, string work_no)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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
            comm.Parameters.AddWithValue("work_no", work_no);
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
            comm.Parameters.AddWithValue("work_no", work_no);
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





        internal static DataTable getSendFileDept(int did)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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

        internal static DataTable getDutyRoomByDeptId(int did)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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
            SqlConnection conn = new SqlConnection(baseConnStr);

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
                comm.CommandText = "select work_no from T_SendFile_DutyRoom_Receiver where drid in (" + drs + ")";
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
                }

                comm.CommandText = "select bm_mc + user_name as sender from v_user where user_no=@sender";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("sender", sender);
                sender = comm.ExecuteScalar().ToString();

                string content = sender + "发来文件：" + fileName + "." + extName + "，请接收。";
                comm.CommandText = "insert into T_System_Message(toUser, Type, Content, Command) select distinct user_no as toUser ,0 , '" + content + "','SendFile' from dic_user where user_no in (" + worknos + ")";
                comm.Parameters.Clear();
                comm.ExecuteNonQuery();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string topic = dt.Rows[i]["work_no"].ToString();
                    sendMqttMessage(sender, topic, content, 2); 
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

        internal static INT AddFeedBack(string work_no, string txt)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "insert into T_Sjbg_Feedback (work_no,txt) values( @work_no,@txt)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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

        internal static DataTable GetLoginRecord(string work_no)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  id, uid, deviceId, deviceInfo, deviceVer, IpAddress, loginTime FROM dat_LoginInfo where uid=@uid order by loginTime desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", work_no);
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

        internal static string getFileToReceiveFromDataBase(int fid)
        {

            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select  fileContent from T_SendFile_Files where id = @fid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("fid", fid);
            string result = "";
            try
            {
                conn.Open();
                result = Convert.ToBase64String((byte[])(comm.ExecuteScalar()));
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

        internal static DataTable getUnsubTopics(string work_no)
        {
            SqlConnection conn = new SqlConnection(mqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT  id,topic FROM V_User_Topics where work_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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

        internal static INT setTopicsSubed(string tids)
        {
            SqlConnection conn = new SqlConnection(mqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_User_Topics set isSubed=1, lastSubTime =getdate() where id in (" + tids + ")";
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-1, "数据库错误");
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, "");
        }

        internal static INT setMqttStatus(string work_no, int type, string clientId)
        {
            SqlConnection conn = new SqlConnection(mqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select count(*) from T_User_session where work_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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
            comm.Parameters.AddWithValue("work_no", work_no);
            comm.Parameters.AddWithValue("type", type);
            comm.Parameters.AddWithValue("clientId", clientId);
            try
            {
                conn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return new INT(-1, "数据库错误");
            }
            finally
            {
                conn.Close();
            }
            return new INT(1, "");
        }

        internal static INT getMqttStatus(string work_no)
        {
            SqlConnection conn = new SqlConnection(mqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select isOnline from T_User_session where work_no=@work_no";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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
        internal static INT sendMqttMessage(string sender, string topic, string message, int type)
        {
            SqlConnection conn = new SqlConnection(mqttConnStr);
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
                for(int i=0;i<dt.Rows.Count;i++)
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
                comm.Parameters.AddWithValue("topic",topic);
                comm.Parameters.AddWithValue("txt",content);
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

        internal static DataTable getSystemMessage(string work_no, int type)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "SELECT     id, toUser, Type, Content, Command, createTime, hasRead, readTime FROM T_System_Message where toUser=@work_no and (hasread=@type or @type=-1) order by createTime desc";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("work_no", work_no);
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

        internal static INT readSystemMessage(int mid)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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

        internal static INT readMqttMessage(string work_no, int mid)
        {
            SqlConnection conn = new SqlConnection(mqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "update T_Mqtt_message_user set hasread=1 , readtime=getdate() where uid=@uid and mid=@mid";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("mid", mid);
            comm.Parameters.AddWithValue("uid", work_no);
            try
            {
                conn.Open();
                int i = Convert.ToInt32( comm.ExecuteNonQuery());
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

        internal static DataTable getUnReadMqttMessage(string work_no)
        {
            SqlConnection conn = new SqlConnection(mqttConnStr);
            SqlCommand comm = new SqlCommand();
            comm.Connection = conn;
            comm.CommandText = "select json from V_Mqtt_Message_User where  uid=@uid and hasRead=0";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("uid", work_no);
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
        public static DataTable getAqxxptBm()
        {


            SqlConnection conn = new SqlConnection(baseConnStr);
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

        public static DataTable getAqxxptBm(int xxid)
        {


            SqlConnection conn = new SqlConnection(baseConnStr);
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

        public static DataTable getAqxxptShenHe()
        {


            SqlConnection conn = new SqlConnection(baseConnStr);
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

        public static INT ApplyAqxx(string sender, string auditor, string title, string content, string buMens,DateTime setTime)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            int xxid = 0;
            try
            {
                conn.Open();
            }
            catch
            {
                return new INT( -100,"数据库错误");
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
                comm.CommandText = "insert into t_aqxxpt_sh (xxid,auditor) values(@xxid,@auditor)";
                comm.Parameters.Clear();
                comm.Parameters.AddWithValue("xxid", xxid);
                comm.Parameters.AddWithValue("auditor", auditor);
                comm.ExecuteNonQuery();
                if (DAL.sendMqttMessage(sender, auditor, "有一条安全信息需要您审核。", 2).Number == 1)
                {
                    DAL.sendMobileMessage(Convert.ToInt32(auditor), "有一条安全信息需要您审核。");
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                    return new INT(-1,"发送提醒信息失败");
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return new INT( -100,ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return new INT(1,xxid.ToString());
        }


        internal static INT AuditAqxx(int xxid, string auditor, int result, string title, string txt)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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
                    if (DAL.sendMqttMessage("提醒信息：", sender, "安全信息审核未通过，原因：" + txt , 2).Number == 1)
                    {
                        DAL.sendMobileMessage(Convert.ToInt32(sender) ,"安全信息审核未通过，原因：" + txt);
                        trans.Commit();
                    }
                    else
                    {
                        trans.Rollback();
                        return new INT(-1,"发送提醒信息失败");
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


        internal static DataTable getAqxxToAudit(string auditor ,int xxid)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select id,xxid,title,txt,sender,createtime as sendtime,settime from v_aqxxpt_audit where auditTime is null and auditor=@auditor and (xxid=@xxid or @xxid=0)";
            comm.Parameters.Clear();
            comm.Parameters.AddWithValue("auditor", auditor);
            comm.Parameters.AddWithValue("xxid",xxid);
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

        internal static DataTable getAqxxContent(int xxid)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
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

        internal static DataTable getAqxxInfo(int did,int xxid)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select xxid,sender,title,sendTime,readcount,sendcount,status,auditor,audittime from v_aqxxpt_xxjs_info where (xxid = @xxid or @xxid=0) and (senderdept=@did or @did=1) order by sendtime desc";
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

        internal static DataTable getAqxxDetail(int xxid)
        {
            SqlConnection conn = new SqlConnection(baseConnStr);
            SqlCommand comm = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            comm.Connection = conn;
            comm.CommandText = "select title,sender,sendtime,receiverno + receivername as receiver,receivetime,receiverDept from V_AQXXPT_XXJS where xxid=@xxid";
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
            dt.TableName = "getAqxxDetail";
            return dt;
        }
        #endregion




    }
}