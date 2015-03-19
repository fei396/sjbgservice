using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;


public class dbModule
{

	private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings ["AutoManageConnectionString"].ToString();

	public dbModule()
	{
	}

	public int getKKSFYX(int kkid)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		DataTable dt = new DataTable();
		int rows = 0;

		comm.Connection = conn;

		comm.CommandText = "select sfyx from T_CQKK_KKXMB where id=@id ";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("id", kkid);
		sda.SelectCommand = comm;

		rows = sda.Fill(dt);
		if (rows == 0)
		{
			return -1;
		}
		else if (rows > 1)
		{
			return -1;
		}
		else //rows == 1
		{
			return Convert.ToInt32(dt.Rows[0]["sfyx"]);

		}
	}



	public int updatePasswd(string sqlstr)
	{
		SqlConnection conn = new SqlConnection(connStr);
		conn.Open();
		SqlCommand comm = new SqlCommand(sqlstr, conn);
		if (comm.ExecuteNonQuery() > 0)
		{
			conn.Close();
			return 1;
		}
		else
		{
			conn.Close();
			return -1;
		}

	}

	public int addRYCJ(string rygh, int kkid, int sfhg, string bz, int drfs,string oper)
	{
		//drfs0忽略1覆盖
		int r=0;
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;

		comm.CommandText = "insert into T_CQKK_RYCJB (kkid,rygh,sfhg,bz,czr,czsj) values(@kkid,@rygh,@sfhg,@bz,@czr,getdate())";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("kkid", kkid);
		comm.Parameters.AddWithValue("rygh", rygh);
		comm.Parameters.AddWithValue("sfhg", sfhg);
		comm.Parameters.AddWithValue("bz", bz);
		comm.Parameters.AddWithValue("czr", oper);

		try
		{
			conn.Open();
			comm.ExecuteNonQuery();

			r = 1;
		}
		catch
		{
			if (drfs == 1)
			{
				SqlCommand comm1 = new SqlCommand();
				comm1.Connection = conn;
				comm1.CommandText = "update T_CQKK_RYCJB set sfhg=@sfhg,bz=@bz,czr=@czr,czsj=getdate() where kkid=@kkid and rygh=@rygh";
				comm1.Parameters.Clear();
				comm1.Parameters.AddWithValue("kkid", kkid);
				comm1.Parameters.AddWithValue("rygh", rygh);
				comm1.Parameters.AddWithValue("sfhg", sfhg);
				comm1.Parameters.AddWithValue("bz", bz);
				comm1.Parameters.AddWithValue("czr", oper);

				try
				{
					comm1.ExecuteNonQuery();
					r = 5;
				}
				catch
				{
					return -1;
				}
			}
			else
			{
				return 2;
			}
		}
		finally
		{
			conn.Close();
		}
		return r;
	}

	public bool isPersonExist(string rygh,string sscj)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "select count(*) from person where work_no=@rygh and (department=@sscj or @sscj='_所有')";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("rygh", rygh);
		comm.Parameters.AddWithValue("sscj", sscj);
		try
		{
			conn.Open();
			int r = Convert.ToInt32(comm.ExecuteScalar());
			if (r != 1)
			{
				return false;
			}
			else
			{
				return true;
			}
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

	public bool deleteRYbyKKID(int id , string cj)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "delete from T_CQKK_RYCJB where kkid=@kkid ";
		if (cj != "_所有")
		{
			comm.CommandText += " and rygh in (select work_no from V_CQKK_YXRYXXB where cj='" + cj + "'";
		}
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("kkid", id);
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



	public string getUnameByUcode(string ucode)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		DataTable dt = new DataTable();
		int rows = 0;

		comm.Connection = conn;
		comm.CommandText = "select username from T_CQKK_GLYXXB where usercode=@usercode and isvalid=1";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("usercode", ucode);
		sda.SelectCommand = comm;

		rows = sda.Fill(dt);
		if (rows == 0)
		{
			return "error!";
		}
		else if (rows > 1)
		{
			return "error!";
		}
		else //rows == 1
		{
			return dt.Rows[0]["username"].ToString();

		}
	}


	public string getUdeptByUcode(string ucode)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		DataTable dt = new DataTable();
		int rows = 0;

		comm.Connection = conn;
		comm.CommandText = "select userdept from T_CQKK_GLYXXB where usercode=@usercode and isvalid=1";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("usercode", ucode);
		sda.SelectCommand = comm;

		rows = sda.Fill(dt);
		if (rows == 0)
		{
			return "error!";
		}
		else if (rows > 1)
		{
			return "error!";
		}
		else //rows == 1
		{
			return dt.Rows[0]["userdept"].ToString();

		}
	}

	public int getUIdByUCode(string ucode)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		DataTable dt = new DataTable();
		int rows = 0;

		comm.Connection = conn;
		
		comm.CommandText = "select id from T_CQKK_GLYXXB where usercode=@usercode and isvalid=1";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("usercode", ucode);
		sda.SelectCommand = comm;

		rows = sda.Fill(dt);
		if (rows == 0)
		{
			return -1;
		}
		else if (rows > 1)
		{
			return -1;
		}
		else //rows == 1
		{
			return Convert.ToInt32(dt.Rows[0]["id"]);

		}
	}

	public string getKkxmmcById(int id)
	{
		string str = "";
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "select '(' + cj+'车间)' + xmmc from T_CQKK_KKXMB where id = " + id.ToString();
		try
		{
			conn.Open();
			str = Convert.ToString(comm.ExecuteScalar());
		}
		catch
		{

		}
		finally
		{
			conn.Close();
		}
		return str;
	}

	public DataTable getKkwzByKkid(int kkid)
	{

		
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		comm.Connection = conn;
		comm.CommandText = "select id,kkid,weizhi from  T_CQKK_KKXM_WZ where kkid = " + kkid.ToString() + "order by weizhi";
		sda.SelectCommand = comm;
		DataTable dt = new DataTable();
		int rows = sda.Fill(dt);
		return dt;
	}

	public void addKkwz(int kkid, int weizhi)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "select count(*) from  T_CQKK_KKXM_WZ where kkid = " + kkid.ToString() + " and weizhi=" +  weizhi.ToString();
		try
		{
			conn.Open();
			if (Convert.ToInt32(comm.ExecuteScalar()) == 0)
			{
				comm.CommandText = "insert into T_CQKK_KKXM_WZ (kkid,weizhi) values(" + kkid.ToString() + "," + weizhi.ToString() + ")";
				comm.ExecuteNonQuery();
			}

		}
		catch
		{
		}
		finally
		{
			conn.Close();
		}
	
	}



	public void delKkwz(int kkid, int weizhi)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "delete from T_CQKK_KKXM_WZ where kkid = " + kkid.ToString() + " and weizhi=" + weizhi.ToString();
		try
		{
			conn.Open();
			comm.ExecuteNonQuery();
		}
		catch
		{
			
		}
		finally
		{
			conn.Close();
		}

	}

	public DataTable getAllXb()
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		comm.Connection = conn;
		comm.CommandText = "select line_mode,qduan from meet where line_mode not in ('y','z') order by line_mode";
		sda.SelectCommand = comm;
		DataTable dt = new DataTable();
		int rows = sda.Fill(dt);
		return dt;
	}

	public DataTable getKkxbByKkid(int kkid)
	{


		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		comm.Connection = conn;
		comm.CommandText = "select id,kkid,xbid,qduan from  T_CQKK_KKXM_XB,meet where xbid=line_mode and kkid = " + kkid.ToString() + "order by xbid";
		sda.SelectCommand = comm;
		DataTable dt = new DataTable();
		int rows = sda.Fill(dt);
		return dt;
	}

	public void addKkxb(int kkid, string xianbie)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "select count(*) from  T_CQKK_KKXM_XB where kkid = " + kkid.ToString() + " and xbid='" + xianbie + "'";
		try
		{
			conn.Open();
			if (Convert.ToInt32(comm.ExecuteScalar()) == 0)
			{
				comm.CommandText = "insert into T_CQKK_KKXM_XB (kkid,xbid) values(" + kkid.ToString() + ",'" + xianbie + "')";
				comm.ExecuteNonQuery();
			}

		}
		catch
		{
		}
		finally
		{
			conn.Close();
		}

	}



	public void delKkxb(int kkid, string xianbie)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "delete from T_CQKK_KKXM_XB where kkid = " + kkid.ToString() + " and xbid='" + xianbie + "'";
		try
		{
			conn.Open();
			comm.ExecuteNonQuery();
		}
		catch
		{

		}
		finally
		{
			conn.Close();
		}

	}

	public DataTable getAllQylx()
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		comm.Connection = conn;
		comm.CommandText = "select id,lx_name from   T_Engi_QYLX  order by id";
		sda.SelectCommand = comm;
		DataTable dt = new DataTable();
		int rows = sda.Fill(dt);
		return dt;
	}

	public DataTable getKkqylxByKkid(int kkid)
	{


		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		comm.Connection = conn;
		comm.CommandText = "select a.id,kkid,qyid,lx_name from   T_CQKK_KKXM_QYLX a,  T_Engi_QYLX b where qyid= b.id and kkid = " + kkid.ToString() + "order by qyid";
		sda.SelectCommand = comm;
		DataTable dt = new DataTable();
		int rows = sda.Fill(dt);
		return dt;
	}

	public void addKkqylx(int kkid, int qyid)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "select count(*) from  T_CQKK_KKXM_QYLX where kkid = " + kkid.ToString() + " and qyid=" + qyid.ToString() ;
		try
		{
			conn.Open();
			if (Convert.ToInt32(comm.ExecuteScalar()) == 0)
			{
				comm.CommandText = "insert into T_CQKK_KKXM_QYLX (kkid,qyid) values(" + kkid.ToString() + "," + qyid.ToString() + ")";
				comm.ExecuteNonQuery();
			}

		}
		catch
		{
		}
		finally
		{
			conn.Close();
		}

	}



	public void delKkqylx(int kkid, int qyid)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "delete from T_CQKK_KKXM_QYLX where kkid = " + kkid.ToString() + " and qyid='" + qyid.ToString() + "'";
		try
		{
			conn.Open();
			comm.ExecuteNonQuery();
		}
		catch
		{

		}
		finally
		{
			conn.Close();
		}

	}

	public int Login(string uCode, string uPass)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		comm.Connection = conn;

		//管理员
		comm.CommandText = "select pwd,admin from T_CQKK_GLYXXB where usercode=@usercode and isvalid=1";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("usercode", uCode);
		sda.SelectCommand = comm;
		DataTable dt = new DataTable();
		int rows = sda.Fill(dt);
		string m = pbModule.MD5(uPass).ToUpper();
		if (rows == 1)
		{
			if (pbModule.MD5(uPass).Equals(dt.Rows[0]["pwd"].ToString().ToUpper()))
			{
				return Convert.ToInt32(dt.Rows[0]["admin"]);
			}
			else
			{
				return -1;
			}
		}
		else
		{
			return -1;
		}

	}

	public int addProject(string xmmc, int kklx, int mdlx, DateTime jzsj)
	{
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "insert into T_CQKK_KKXMB (xmmc,kklx,mdlx,jzsj) values(@xmmc,@kklx,@mdlx,@jzsj)";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("xmmc", xmmc);
		comm.Parameters.AddWithValue("kklx", kklx);
		comm.Parameters.AddWithValue("mdlx", mdlx);
		comm.Parameters.AddWithValue("jzsj", jzsj);
		try
		{
			conn.Open();
			return comm.ExecuteNonQuery();
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

	public int addLog(int type, string oper, int opid, string beOper, string cont)
	{
		//type 1添加卡控项目，2编辑卡控项目，3添加人员成绩，4编辑人员成绩
		string userIp = pbModule.getIP();
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "insert into T_CQKK_CZJLB (czlx,czr,czsj,czid,bczry,cznr,czip) values(@czlx,@czr,getdate(),@czid,@bczry,@cznr,@czip)";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("czlx", type);
		comm.Parameters.AddWithValue("czr", oper);
		comm.Parameters.AddWithValue("czid", opid);
		comm.Parameters.AddWithValue("bczry", beOper);
		comm.Parameters.AddWithValue("cznr", cont);
		comm.Parameters.AddWithValue("czip", userIp);

		try
		{
			conn.Open();
			return comm.ExecuteNonQuery();
			//return Convert.ToInt32(comm.ExecuteScalar());
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

	public int addXXFSbyGLYLX(string fsrid, int[] bfsrlx ,int fslx, string fsnr, string wjdz)
	{
		SqlConnection conn = new SqlConnection(connStr);
		string inadmin = "";
		if (bfsrlx.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < bfsrlx.Length; i++)
		{
			if (i != 0) inadmin += ",";
			inadmin += bfsrlx[i].ToString();

		}
		SqlDataAdapter sda = new SqlDataAdapter("select usercode from T_CQKK_GLYXXB where usercode<>'" + fsrid + "' and  admin in (" + inadmin + " ) and isvalid=1",conn);
		DataTable dt = new DataTable();
		try
		{
			sda.Fill(dt);
		}
		catch
		{
			return -1;
		}
		for (int i = 0; i < dt.Rows.Count; i++)
		{
			addXXFS(fsrid, dt.Rows[i]["usercode"].ToString(), fslx, fsnr, wjdz);
		}
		return 1;
	}


	public int addXXFS(string fsrid, string bfsrid, int fslx, string fsnr, string wjdz)
	{
		
		string fsip = pbModule.getIP();
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		comm.Connection = conn;
		comm.CommandText = "insert into T_CQKK_XXFSB (fsrid,bfsrid,fssj,fslx,fsnr,wjdz,fsip,sfck) values(@fsrid,@bfsrid,getdate(),@fslx,@fsnr,@wjdz,@fsip,0)";
		comm.Parameters.Clear();
		comm.Parameters.AddWithValue("fsrid", fsrid);
		comm.Parameters.AddWithValue("bfsrid", bfsrid);
		comm.Parameters.AddWithValue("fslx", fslx);
		comm.Parameters.AddWithValue("fsnr", fsnr);
		comm.Parameters.AddWithValue("wjdz", wjdz);
		comm.Parameters.AddWithValue("fsip", fsip);

		try
		{
			conn.Open();
			return comm.ExecuteNonQuery();
			//return Convert.ToInt32(comm.ExecuteScalar());
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


	public DataTable getBhgryByXmid(int xmid,string cj)
	{
		DataTable dtResult = new DataTable();
		dtResult.TableName = "table";
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		comm.Connection = conn;
		string work_no = "";
		string work_name = "";
		string department = "";
		string xmmc = "";
		string xmcj = "";
		int mdlx=-1;
		comm.CommandText = "select xmmc,mdlx,cj from T_CQKK_KKXMB where sfyx >=0 and  id=" + xmid;
		sda.SelectCommand = comm;
		DataTable dt = new DataTable();
		int rows = sda.Fill(dt);

		if (rows == 1)
		{
			xmmc = dt.Rows[0]["xmmc"].ToString();
			mdlx = Convert.ToInt32(dt.Rows[0]["mdlx"]);
			xmcj = dt.Rows[0]["cj"].ToString();

		}
		else
		{

			dtResult.TableName = "err";
			return dtResult;
		}
		comm.CommandText = "select '" + xmmc + "' as xmmc, rygh ,work_name,duty,cj ,'成绩不合格' as bhgyy from V_CQKK_RYCJB where (cj='" + cj + "' or '" + cj + "'='_所有') and (sscj='" + xmcj + "' or '" + xmcj + "'='_所有') and sfhg=0 and  kkid=" + xmid;
		if (mdlx==0)
			//白名单
		{
			comm.CommandText += " union all select '" + xmmc + "' as xmmc, work_no as rygh,work_name,duty,department as cj,'无考试成绩' as bhgyy from V_CQKK_YXRYXXB where (department='" + cj + "' or '" + cj + "'='_所有') and (department='" + xmcj + "' or '" + xmcj + "'='_所有') and work_no not in (select rygh from V_CQKK_RYCJB where  kkid=" + xmid + ")";
		}
		
		sda.SelectCommand = comm;
		sda.Fill(dtResult);
		return dtResult;
		
	}

	public DataTable getKkxmByRygh(string rygh)
	{
		DataTable dtResult = new DataTable();
		dtResult.TableName = "table";
		SqlConnection conn = new SqlConnection(connStr);
		SqlCommand comm = new SqlCommand();
		SqlDataAdapter sda = new SqlDataAdapter();
		comm.Connection = conn;
		string work_name = "";
		string department = "";
		//管理员
		comm.CommandText = "select work_name,department from V_CQKK_YXRYXXB where work_no='" + rygh + "'" ;
		comm.Parameters.Clear();
		
		sda.SelectCommand = comm;
		DataTable dt = new DataTable();
		int rows = sda.Fill(dt);

		if (rows == 1)
		{
			work_name = dt.Rows[0]["work_name"].ToString();
			department = dt.Rows[0]["department"].ToString();
			
		}
		else
		{

			dtResult.TableName = "err";
			return dtResult;
		}


		comm.CommandText = "select '(' + cj + '车间)' +xmmc as xmmc,id,mdlx from V_CQKK_YXKKXM where cj='_所有' or cj='" + department + "'";
		dtResult.Columns.Add("rygh");
		dtResult.Columns.Add("work_name");
		dtResult.Columns.Add("cj");
		dtResult.Columns.Add("xmmc");
		dtResult.Columns.Add("sfhg");
		sda.SelectCommand = comm;
		dt.Rows.Clear();
		sda.Fill(dt);
		for (int i = 0; i < dt.Rows.Count; i++)
		{
			string xmmc = dt.Rows[i]["xmmc"].ToString();
			int xmid = Convert.ToInt32(dt.Rows[i]["id"]);
			int mdlx = Convert.ToInt32(dt.Rows[i]["mdlx"]);
			string sfhg="";
			comm.CommandText = "select sfhg from T_CQKK_RYCJB where kkid=" + xmid +  "and rygh='" + rygh + "'";
			
			try
			{

				conn.Open();
				SqlDataReader dr = comm.ExecuteReader();
				if (dr.Read())
				//有记录
				{
					if (dr.GetInt32(0) == 1)
					//合格
					{
						sfhg = "1";
					}
					else
					{
						sfhg = "0";
					}
				}
				else
				//无记录
				{
					if (mdlx == 1)
					//黑名单
					{
						sfhg = "1";
					}
					else
					{
						sfhg = "0";
					}

				}
			}
			catch
			{
				dtResult.TableName = "err";
				return dtResult;
			}
			finally
			{
				conn.Close();
			}
			object[] o = new object[5];
			o[0] = rygh;
			o[1] = work_name;
			o[2] = department;
			o[3] = xmmc;
			o[4] = sfhg;
			dtResult.Rows.Add(o);
		}
		return dtResult;
		
	}

}

