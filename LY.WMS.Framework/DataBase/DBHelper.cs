using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "CFG/LogConfig.XML", Watch = true)]
namespace LY.WMS.Framework.DataBase
{
    public class DBHelper
    {
        #region 变量属性

        /// <summary>
        /// 连接字符串 ConnString 读写数据库使用
        /// </summary>
        public string ConnString { get; set; }


        /// <summary>
        /// 数据库类型 默认SqlServer
        /// </summary>
        public DBType DataBaseType;

        /// <summary>
        /// 日志(SQL日志、错误日志)
        /// </summary>
        private ILog sqlLog, errorLog;

        // Events
        public event SqlLogEventHandler SqlLogEvent;

        // Nested Types
        public delegate void SqlLogEventHandler(bool paramIsError, string paramOperationString, string paramErrorMessage);

        #endregion

        #region 构造方法

        public DBHelper()
        {
            SqlLogEvent += new SqlLogEventHandler(SaveLogToFile);
            sqlLog = LogManager.GetLogger("SqlLog");
            errorLog = LogManager.GetLogger("ErrorSqlLog");
            DataBaseType = DBType.SqlServer;
            ReadConfigAndTestConnect();
        }

        public DBHelper(DBType dBType)
        {
            SqlLogEvent += new SqlLogEventHandler(SaveLogToFile);
            sqlLog = LogManager.GetLogger("SqlLog");
            errorLog = LogManager.GetLogger("ErrorSqlLog");
            DataBaseType = dBType;
            ReadConfigAndTestConnect();
        }

        #endregion

        #region 基础方法(读取配置文件、测试连接状态等)

        /// <summary>
        /// 读取配置文件和测试连接状态
        /// </summary>
        /// <param name="dBType"></param>
        /// <returns></returns>
        public bool ReadConfigAndTestConnect()
        {
            #region 1.读取配置文件
            string configName;
            if (DataBaseType == DBType.SqlServer) configName = "MsSqlConfig.py";
            else configName = "OracleConfig.PY";
            try
            {
                ConnString = GetSourStr(new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"CFG\" + configName).ReadLine());
                if (ConnString.Trim().Length == 0) return false;
            }
            catch(Exception e)
            {
                // 读取数据库配置文件出错
                SqlLogEvent(false, "读取数据库配置文件出错", e.Message.ToString());
                return false;
            }
            #endregion

            #region 2.测试连接状态

            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                try
                {
                    conn.DbConnection.Open();
                    return true;
                }
                catch(Exception e)
                {
                    SqlLogEvent(false, "测试数据库连接出错", conn.ConnectionString + ":" + e.Message.ToString());
                    return false;
                }
                finally { conn.DbConnection.Close(); }
            }
            #endregion
        }

        /// <summary>
        /// 解密配置文件字符串
        /// </summary>
        /// <param name="ParamPasswordStr">密码字符串</param>
        /// <returns></returns>
        private static string GetSourStr(string paramPasswordStr)
        {
            return Encoding.Default.GetString(Convert.FromBase64String(paramPasswordStr));
        }

        /// <summary>
        /// 日志保存
        /// </summary>
        /// <param name="paramIsError">是否出错</param>
        /// <param name="paramSqlString">SQL文</param>
        /// <param name="paramErrorMessage">错误消息</param>
        public void SaveLogToFile(bool paramIsError, string paramSqlString, string paramErrorMessage)
        {
            if (!paramIsError)
            {
                errorLog.Info("--" + paramSqlString + "\r\n" + paramErrorMessage);
                sqlLog.Error("--" + paramSqlString + "\r\n" + paramErrorMessage);
            }
            else
            {
                sqlLog.Info("--" + paramSqlString + "\r\n" + paramErrorMessage);
            }
        }
        #endregion

        #region 查询

        /// <summary>
        /// 返回一个查询的整型值
        /// </summary>
        /// <param name="paramSql">SQL文</param>
        /// <returns></returns>
        public int GetIntegerBySql(string paramSql)
        {
            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, paramSql, CommandType.Text);
                    SqlLogEvent(true, "执行查询", paramSql);
                    try
                    {
                        DbDataReader dbDataReader;
                        dbDataReader = cmd.DbCommand.ExecuteReader();
                        if (dbDataReader.Read())
                        {
                            return Convert.ToInt32(dbDataReader[0].ToString());
                        }
                    }
                    catch (Exception exception)
                    {
                        SqlLogEvent(false, "执行查询出错\r\n" + paramSql, exception.Message.ToString());
                    }

                    return default;
                }
            }
        }

        /// <summary>
        /// 返回一个查询的字符串
        /// </summary>
        /// <param name="paramSql">SQL文</param>
        /// <returns></returns>
        public string GetStringBySql(string paramSql)
        {
            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, paramSql, CommandType.Text);
                    SqlLogEvent(true, "执行查询", paramSql);
                    try
                    {
                        DbDataReader dbDataReader;
                        dbDataReader = cmd.DbCommand.ExecuteReader();
                        if (dbDataReader.Read())
                        {
                            return dbDataReader[0].ToString();
                        }
                    }
                    catch (Exception exception)
                    {
                        SqlLogEvent(false, "执行查询出错\r\n" + paramSql, exception.Message.ToString());
                    }

                    return default;
                }
            }
        }

        /// <summary>
        /// 根据指定SQL获取一个List Of String
        /// </summary>
        /// <param name="paramSql">查询数据的SQL,只会返回查询到的第1个列</param>
        /// <returns></returns>
        public List<string> GetStringListBySql(string paramSql)
        {
            List<string> list = new List<string>();

            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, paramSql, CommandType.Text);
                    SqlLogEvent(true, "执行查询", paramSql);
                    try
                    {
                        DbDataReader dbDataReader;
                        dbDataReader = cmd.DbCommand.ExecuteReader();
                        while (dbDataReader.Read())
                        {
                            list.Add(Convert.ToString(dbDataReader[0]));
                        }
                        return list;
                    }
                    catch (Exception exception)
                    {
                        SqlLogEvent(false, "执行查询出错\r\n" + paramSql, exception.Message.ToString());
                    }
                    return list;
                }
            }
        }

        /// <summary>
        /// 根据指定SQL获取一个DataTable
        /// </summary>
        /// <param name="paramSql">SQL文</param>
        /// <returns></returns>
        public DataTable GetDataTableBySql(string paramSql)
        {
            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, paramSql, CommandType.Text);
                    SqlLogEvent(true,  "执行查询", paramSql);
                    using (DbDataAdapterCommon adapter = new DbDataAdapterCommon(DataBaseType, cmd.DbCommand))
                    {
                        try
                        {
                            DataSet ds = new DataSet();
                            adapter.Fill(ds);
                            if (ds.Tables.Count > 0)
                            {
                                return ds.Tables[0];
                            }
                        }
                        catch(Exception exception)
                        {
                            SqlLogEvent(false, "执行查询出错\r\n" + paramSql, exception.Message.ToString());
                        }
                        
                        return default;
                    }
                }
            }
        }

        /// <summary>
        /// 根据指定SqlItem列表获取一个DataSet
        /// </summary>
        /// <param name="paramSqlList">SqlItem列表</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DataSet GetDataSetBySql(List<SqlItem> paramSqlList)
        {
            if ((paramSqlList == null) | (paramSqlList.Count == 0))
            {
                return new DataSet();
            }
            DataSet dataSet = new DataSet();
            for (int i = 0; i < paramSqlList.Count; i++)
            {
                DataTable dataTable = new DataTable();
                using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
                {
                    using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                    {
                        PreparCommand(conn.DbConnection, cmd.DbCommand, paramSqlList[i].SqlStr, CommandType.Text);
                        SqlLogEvent(true, "执行查询", paramSqlList[i].SqlStr);
                        using (DbDataAdapterCommon adapter = new DbDataAdapterCommon(DataBaseType, cmd.DbCommand))
                        {
                            try
                            {
                                adapter.Fill(dataTable);
                                if (paramSqlList[i].Name.Trim() != string.Empty)
                                {
                                    dataTable.TableName = paramSqlList[i].Name.Trim();
                                }
                                else
                                {
                                    dataTable.TableName = "Result" + i.ToString();
                                }
                                dataSet.Tables.Add(dataTable);
                            }
                            catch (Exception exception)
                            {
                                SqlLogEvent(false, "执行查询出错\r\n" + paramSqlList[i].SqlStr, exception.Message.ToString());
                            }
                        }
                    }
                }   
            }
            return dataSet;
        }

        /// <summary>
        /// 获了下一个序列值
        /// </summary>
        /// <param name="paramSeq">数据库中的序列名称</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetNextSeq(string paramSeq)
        {
            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    string sql = "SELECT " + paramSeq + ".NEXTVAL FROM DUAL";
                    PreparCommand(conn.DbConnection, cmd.DbCommand, sql, CommandType.Text);
                    SqlLogEvent(true, "执行查询", sql);
                    try
                    {
                        DbDataReader dbDataReader;
                        dbDataReader = cmd.DbCommand.ExecuteReader();
                        if (dbDataReader.Read())
                        {
                            return Convert.ToString(dbDataReader[0]);
                        }
                    }
                    catch (Exception exception)
                    {
                        SqlLogEvent(false, "执行查询出错\r\n" + sql, exception.Message.ToString());
                    }
                    return null;
                }
            }
        }

        #endregion

        #region 事务
        /// <summary>
        /// 执行无参数的存储过程
        /// </summary>
        /// <param name="paramProcName">过程名</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SqlItemResult RunProcWithTran(string paramProcName)
        {
            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, paramProcName, CommandType.StoredProcedure);

                    SqlLogEvent(true, "开始一个事务", "");
                    DbTransaction dbTransaction;
                    try
                    {
                        dbTransaction = cmd.DbCommand.Connection.BeginTransaction();
                    }
                    catch(Exception e)
                    {
                        SqlLogEvent(false,  "开始一个事务出错", e.Message.ToString());
                        return new SqlItemResult(false, "开始一个事务出错!", e.Message.ToUpper(), new SqlItem(paramProcName, -1));
                    }

                    SqlLogEvent(true, "执行存储过程", paramProcName);
                    try
                    {
                        cmd.DbCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        SqlLogEvent(false, "执行存储过程出错,准备回滚\r\n" + paramProcName, e.Message.ToString());
                        dbTransaction.Rollback();
                        dbTransaction.Dispose();
                        return new SqlItemResult(false, "执行SQL出错!", e.Message.ToUpper(), new SqlItem(paramProcName, -1)); ;
                    }

                    SqlLogEvent(true, "提交事务", dbTransaction.ToString());
                    try
                    {
                        dbTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Dispose();
                        SqlLogEvent(false, "提交事务出错", e.Message.ToString());
                        return new SqlItemResult(false, "提交事务出错!", "", new SqlItem(paramProcName, -1));
                    }
                    dbTransaction.Dispose();
                    return new SqlItemResult(true, "执行成功!", "", new SqlItem(paramProcName, -1));
                }
            }
        }

        /// <summary>
        /// 执行带参数存储过程
        /// </summary>
        /// <param name="paramProcName">过程名</param>
        /// <param name="paramList">参数列表</param>
        /// <returns></returns>
        public SqlItemResult RunProcParamWithTran(string paramProcName, List<DbParameter> paramList)
        {
            string sql = "";
            if (paramList.Count > 0)
            {
                for (int i = 0; i < paramList.Count; i++)
                {
                    sql += paramList[i].ParameterName.ToString() + "=" + paramList[i].Value.ToString() + ",";
                }
            }

            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, paramProcName, CommandType.StoredProcedure);

                    SqlLogEvent(true, "开始一个事务", "");
                    DbTransaction dbTransaction;
                    try
                    {
                        dbTransaction = cmd.DbCommand.Connection.BeginTransaction();
                    }
                    catch (Exception e)
                    {
                        SqlLogEvent(false, "开始一个事务出错", e.Message.ToString());
                        return new SqlItemResult(false, "开始一个事务出错!", e.Message.ToUpper(), new SqlItem(paramProcName, -1));
                    }

                    if (paramList.Count > 0)
                    {
                        for (int i = 0; i < paramList.Count; i++)
                        {
                            cmd.DbCommand.Parameters.Add(paramList[i]);
                        }
                    }


                    SqlLogEvent(true,  "执行存储过程", paramProcName + " " + sql);
                    try
                    {
                        cmd.DbCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        SqlLogEvent(false, "执行存储过程出错,准备回滚\r\n" + paramProcName + " " + sql, e.Message.ToString());
                        dbTransaction.Rollback();
                        dbTransaction.Dispose();
                        return new SqlItemResult(false, "执行SQL出错!", e.Message.ToUpper(), new SqlItem(paramProcName, -1)); ;
                    }

                    SqlLogEvent(true, "提交事务", dbTransaction.ToString());
                    try
                    {
                        dbTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Dispose();
                        SqlLogEvent(false, "提交事务出错", e.Message.ToString());
                        return new SqlItemResult(false, "提交事务出错!", "", new SqlItem(paramProcName, -1));
                    }
                    dbTransaction.Dispose();
                    return new SqlItemResult(true, "执行成功!", "", new SqlItem(paramProcName, -1));
                }
            }
        }

        /// <summary>
        /// 批量执行带参数的存储过程
        /// </summary>
        /// <param name="paramSqlParamItem"></param>
        /// <returns></returns>
        public SqlParamItemResult RunProcBatchWithParamWithTran(List<SqlParamItem> paramSqlParamItem)
        {
            if (paramSqlParamItem == null)
            {
                return new SqlParamItemResult(false, "数据无效", "传入的数据无效", null);
            }
            if (paramSqlParamItem.Count == 0)
            {
                return new SqlParamItemResult(false, "数据无效", "传入的数据无效", null);
            }

            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    SqlLogEvent(true, "开始一个事务", "");
                    DbTransaction dbTransaction;
                    try
                    {
                        dbTransaction = cmd.DbCommand.Connection.BeginTransaction();
                    }
                    catch (Exception e)
                    {
                        SqlLogEvent(false, "开始一个事务出错", e.Message.ToString());
                        return new SqlParamItemResult(false, "开始一个事务出错", e.Message.ToUpper(), null);
                    }

                    string sql = "";
                    foreach (SqlParamItem item in paramSqlParamItem)
                    {
                        PreparCommand(conn.DbConnection, cmd.DbCommand, item.GetSqlStr(), CommandType.StoredProcedure);

                        if (item.ParamList.Count > 0)
                        {
                            for (int i = 0; i < item.ParamList.Count; i++)
                            {
                                try
                                {
                                    cmd.DbCommand.Parameters.Add(item.ParamList[i]);
                                    sql += item.ParamList[i].ParameterName.ToString() + "=" + item.ParamList[i].Value.ToString() + ",";

                                }
                                catch (Exception e)
                                {
                                    return new SqlParamItemResult(false, item.ParamList[i].ParameterName.ToString(), e.Message.ToString(), null);
                                }
                           }
                        }

                        SqlLogEvent(true, "执行存储过程", item.SqlStr + " " + sql);
                        try
                        {
                            cmd.DbCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            SqlLogEvent(false, "执行存储过程出错,准备回滚\r\n" + item.SqlStr + " " + sql, e.Message.ToString());
                            dbTransaction.Rollback();
                            dbTransaction.Dispose();
                            return new SqlParamItemResult(false, "执行SQL出错!", e.Message.ToUpper(), new SqlParamItem(item.SqlStr, -1, -1));
                        }
                    }

                    SqlLogEvent(true, "提交事务", dbTransaction.ToString());
                    try
                    {
                        dbTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Dispose();
                        SqlLogEvent(false, "提交事务出错", e.Message.ToString());
                        return new SqlParamItemResult(false, "提交事务出错!", "提交事务出错!", null);
                    }
                    dbTransaction.Dispose();
                    return new SqlParamItemResult(true, "执行成功!", "", null);
                }
            }
        }

        /// <summary>
        /// 执行单条更新语句
        /// </summary>
        /// <param name="paramSqlItem">执行更新的sql对象</param>
        /// <returns>True 表示执行成功,False表示执行失败</returns>
        /// <remarks>如果影响的行数不等于计划的行数,执行事务会回滚并返回失败</remarks>
        public SqlItemResult RunUpdateSqlWithTran(SqlItem paramSqlItem)
        {
            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, paramSqlItem.SqlStr, CommandType.Text);

                    SqlLogEvent(true, "开始一个事务", "");
                    DbTransaction dbTransaction;
                    int num;
                    try
                    {
                        dbTransaction = cmd.DbCommand.Connection.BeginTransaction();
                    }
                    catch (Exception e)
                    {
                        SqlLogEvent(false, "开始一个事务出错", e.Message.ToString());
                        return new SqlItemResult(false, "开始一个事务出错!", e.Message.ToUpper(), paramSqlItem);
                    }

                    SqlLogEvent(true, "执行更新SQL", paramSqlItem.SqlStr);
                    try
                    {
                        num = cmd.DbCommand.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        SqlLogEvent(false, "执行存储过程出错,准备回滚\r\n" + paramSqlItem.SqlStr, e.Message.ToString());
                        dbTransaction.Rollback();
                        dbTransaction.Dispose();
                        return new SqlItemResult(false, "执行SQL出错!", e.Message.ToUpper(), paramSqlItem);
                    }

                    if ((paramSqlItem.ResultCount != -1) && (paramSqlItem.ResultCount != num))
                    {
                        SqlLogEvent(false, "执行更新SQL条目出错,准备回滚", "Update条数" + num.ToString() + "不等于计划更新条数");
                        try
                        {
                            dbTransaction.Rollback();
                            dbTransaction.Dispose();
                        }
                        catch (Exception exception)
                        {
                            dbTransaction.Dispose();
                            SqlLogEventHandler handler6 = this.SqlLogEvent;
                            SqlLogEvent(false, "回滚出错", exception.Message.ToString());
                        }
                        return new SqlItemResult(false, "Update条数" + num.ToString() + "不等于计划更新条数" + paramSqlItem.ResultCount.ToString(), "", paramSqlItem);
                    }


                    SqlLogEvent(true, "提交事务", dbTransaction.ToString());
                    try
                    {
                        dbTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Dispose();
                        SqlLogEvent(false, "提交事务出错", e.Message.ToString());
                        return new SqlItemResult(false, "提交事务出错!", "", paramSqlItem);
                    }
                    dbTransaction.Dispose();
                    return new SqlItemResult(true, "执行成功!", "", paramSqlItem);
                }
            }
        }

        /// <summary>
        ///  批量执行执行更新语句
        /// </summary>
        /// <param name="paramSqlItemList">需要执行的SQL列表.类型为List(Of SqlItem)</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public SqlItemResult RunUpdateSqlBatchWithTran(List<SqlItem> paramSqlItemList)
        {
            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    SqlLogEvent(true, "开始一个事务", "");
                    DbTransaction dbTransaction;
                    int num;
                    try
                    {
                        dbTransaction = cmd.DbCommand.Connection.BeginTransaction();
                    }
                    catch (Exception e)
                    {
                        SqlLogEvent(false, "开始一个事务出错", e.Message.ToString());
                        return new SqlItemResult(false, "开始一个事务出错!", e.Message.ToUpper(), null);
                    }
                    foreach (SqlItem item in paramSqlItemList)
                    {
                        PreparCommand(conn.DbConnection, cmd.DbCommand, item.SqlStr, CommandType.Text);

                        SqlLogEvent(true, "执行更新SQL", item.SqlStr);
                        try
                        {
                            num = cmd.DbCommand.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            SqlLogEvent(false, "执行存储过程出错,准备回滚\r\n" + item.SqlStr, e.Message.ToString());
                            dbTransaction.Rollback();
                            dbTransaction.Dispose();
                            return new SqlItemResult(false, "执行SQL出错!", e.Message.ToUpper(), item);
                        }

                        if ((item.ResultCount != -1) && (item.ResultCount != num))
                        {
                            SqlLogEvent(false, "执行更新SQL条目出错,准备回滚", "Update条数" + num.ToString() + "不等于计划更新条数");
                            try
                            {
                                dbTransaction.Rollback();
                            }
                            catch (Exception e)
                            {
                                dbTransaction.Dispose();
                                SqlLogEvent(false, "回滚出错", e.Message.ToString());
                            }
                            return new SqlItemResult(false, "Update条数" + num.ToString() + "不等于计划更新条数" + item.ResultCount.ToString(), "", item);
                        }

                    }

                    SqlLogEvent(true, "提交事务", "");
                    try
                    {
                        dbTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbTransaction.Dispose();
                        SqlLogEvent(false, "提交事务出错,准备回滚!", e.Message.ToString());
                        return new SqlItemResult(false, "执行SQL出错!", e.Message.ToUpper(), null);
                    }
                    dbTransaction.Dispose();
                    return new SqlItemResult(true, "执行成功!", "", null);
                }
            }
        }
        #endregion

        #region ---PreparCommand 构建一个通用的command对象供内部方法进行调用---

        /// <summary>
        /// 不带参数的设置sqlcommand对象
        /// </summary>
        /// <param name="conn">sqlconnection对象</param>
        /// <param name="cmd">sqlcommmand对象</param>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">语句的类型</param>
        private void PreparCommand(DbConnection conn, DbCommand cmd, string commandTextOrSpName, CommandType commandType)
        {
            //打开连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //设置SqlCommand对象的属性值
            cmd.Connection = conn;
            cmd.CommandType = commandType;
            cmd.CommandText = commandTextOrSpName;
            cmd.CommandTimeout = 60;
        }

        /// <summary>
        /// 设置一个等待执行的SqlCommand对象
        /// </summary>
        /// <param name="conn">sqlconnection对象</param>
        /// <param name="cmd">sqlcommmand对象</param>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">语句的类型</param>
        /// <param name="parms">参数，sqlparameter类型，需要指出所有的参数名称</param>
        private void PreparCommand(DbConnection conn, DbCommand cmd, string commandTextOrSpName, CommandType commandType, params SqlParameter[] parms)
        {
            //打开连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //设置SqlCommand对象的属性值
            cmd.Connection = conn;
            cmd.CommandType = commandType;
            cmd.CommandText = commandTextOrSpName;
            cmd.CommandTimeout = 60;

            if (parms != null)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(parms);
            }
        }
        /// <summary>
        /// PreparCommand方法，可变参数为object需要严格按照参数顺序传参
        /// 之所以会用object参数方法是为了我们能更方便的调用存储过程，不必去关系存储过程参数名是什么，知道它的参数顺序就可以了 sqlparameter必须指定每一个参数名称
        /// </summary>
        /// <param name="conn">sqlconnection对象</param>
        /// <param name="cmd">sqlcommmand对象</param>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">语句的类型</param>
        /// <param name="parms">参数，object类型，需要按顺序赋值</param>
        private void PreparCommand(DbConnection conn, DbCommand cmd, string commandTextOrSpName, CommandType commandType, params object[] parms)
        {
            //打开连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //设置SqlCommand对象的属性值
            cmd.Connection = conn;
            cmd.CommandType = commandType;
            cmd.CommandText = commandTextOrSpName;
            cmd.CommandTimeout = 60;

            cmd.Parameters.Clear();
            if (parms != null)
            {
                cmd.Parameters.AddRange(parms);
            }
        }
        #endregion

        #region 共通配置

        #region SqlConnection_Con

        /// <summary>
        /// 共通SqlConnection
        /// </summary>
        internal class SqlConnection_Con : IDisposable
        {
            /// <summary>
            /// 共通SqlConnection
            /// </summary>
            public DbConnection DbConnection { get; set; }

            /// <summary>
            /// 连接符
            /// </summary>
            public string ConnectionString { get { return DbConnection.ConnectionString; } }

            /// <summary>
            /// 共通的SqlConnection
            /// </summary>
            /// <param name="dataBaseType"></param>
            /// <param name="ConnString_RW"></param>
            public SqlConnection_Con(DBType dataBaseType, string ConnString_RW)
            {
                DbConnection = GetDbConnection(dataBaseType, ConnString_RW);
            }

            /// <summary>
            /// GetDataBase ConnectionString by database type and connection string -- private use
            /// </summary>
            /// <param name="dataBaseType"></param>
            /// <param name="ConnString"></param>
            /// <returns></returns>
            private DbConnection GetDbConnection(DBType dataBaseType, string ConnString)
            {
                switch (dataBaseType)
                {
                    case DBType.SqlServer:
                        return new SqlConnection(ConnString);
                    case DBType.Oracle:
                        return new OracleConnection(ConnString);
                    default:
                        return new SqlConnection(ConnString);
                }
            }

            public void Dispose()
            {
                if (DbConnection != null)
                {
                    DbConnection.Dispose();
                }
            }
        }

        #endregion

        #region DbCommandCommon

        /// <summary>
        /// 共通SqlCommand
        /// </summary>
        internal class DbCommandCommon : IDisposable
        {
            /// <summary>
            /// 共通SqlCommand
            /// </summary>
            public DbCommand DbCommand { get; set; }

            /// <summary>
            /// 共通SqlCommand
            /// </summary>
            /// <param name="dataBaseType"></param>
            public DbCommandCommon(DBType dataBaseType)
            {
                DbCommand = GetDbCommand(dataBaseType);
            }

            /// <summary>
            /// Get DbCommand select database type
            /// </summary>
            /// <param name="dataBaseType"></param>
            /// <returns></returns>
            private DbCommand GetDbCommand(DBType dataBaseType)
            {
                switch (dataBaseType)
                {
                    case DBType.SqlServer:
                        return new SqlCommand();
                    case DBType.Oracle:
                        return new OracleCommand();
                    default:
                        return new SqlCommand();
                }
            }

            /// <summary>
            /// must dispose after use
            /// </summary>
            public void Dispose()
            {
                if (DbCommand != null)
                {
                    DbCommand.Dispose();
                }
            }
        }

        #endregion

        #region DbDataAdapterCommon

        /// <summary>
        /// 共通DataAdapter
        /// </summary>
        internal class DbDataAdapterCommon : DbDataAdapter, IDisposable
        {
            public DbDataAdapter DbDataAdapter { get; set; }

            /// <summary>
            /// 共通DataAdapter
            /// </summary>
            /// <param name="dataBaseType"></param>
            /// <param name="dbCommand"></param>
            public DbDataAdapterCommon(DBType dataBaseType, DbCommand dbCommand)
            {
                //get dbAdapter
                DbDataAdapter = GetDbAdapter(dataBaseType, dbCommand);
                //provid select command
                SelectCommand = dbCommand;
            }
            private DbDataAdapter GetDbAdapter(DBType dataBaseType, DbCommand dbCommand)
            {
                switch (dataBaseType)
                {
                    case DBType.SqlServer:
                        return new SqlDataAdapter();
                    case DBType.Oracle:
                        return new OracleDataAdapter();
                    default:
                        return new SqlDataAdapter();
                }
            }
            /// <summary>
            /// must dispose after use
            /// </summary>
            public new void Dispose()
            {
                if (DbDataAdapter != null)
                {
                    DbDataAdapter.Dispose();
                }
            }
        }

        #endregion


        #endregion




        /// <summary>
        /// 数据库类型
        /// </summary>
        public enum DBType
        {
            SqlServer,
            Oracle
        }
    }
}
