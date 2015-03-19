using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OracleClient;
using System.Data;
/// <summary>
///DAL 的摘要说明
/// </summary>
public class DAL
{

    static string fyyConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["fyyConnectionString"].ConnectionString;
    public static DataTable getAllFyyjcZt()
    {
        OracleConnection conn = new OracleConnection(fyyConnStr);
        OracleCommand comm = new OracleCommand();
        comm.Connection = conn;
        comm.CommandText = "select distinct jczt_id,zryy from v_sjbg_fyyjc order by jczt_id";
        OracleDataAdapter oda = new OracleDataAdapter(comm);
        DataTable dt = new DataTable();
        try
        {
            oda.Fill(dt);
            dt.TableName = "fyyjcZt";
        }
        catch
        {
            dt.TableName = "error";
        }
        return dt;
    }

    public static DataTable getFyyjc(string jczt)
    {
        OracleConnection conn = new OracleConnection(fyyConnStr);
        OracleCommand comm = new OracleCommand();
        comm.Connection = conn;
        comm.CommandText = "select jclx,jcbh,zryy as jczt,xcdd as dd, zrsj,zcsj,xjzsj as gzsj from v_sjbg_fyyjc where (zryy=:jczt or :jczt='全部') order by jczt_id";
        comm.Parameters.Clear();
        comm.Parameters.AddWithValue("jczt", jczt);
        OracleDataAdapter oda = new OracleDataAdapter(comm);
        DataTable dt = new DataTable();
        try
        {
            oda.Fill(dt);
            dt.TableName = "fyyjc";
        }
        catch
        {
            dt.TableName = "error";
        }
        return dt;
    }
}