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

        #endregion


        /// <summary>
        /// 执行sql语句或存储过程，返回受影响的行数,不带参数。
        /// </summary>
        /// <param name="ConnString">连接字符串，可以自定义，可以以使用SqlHelper_DG.ConnString</param>
        /// <param name="commandTextOrSpName">sql语句或存储过程名称</param>
        /// <param name="commandType">命令类型 有默认值CommandType.Text</param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(string commandTextOrSpName, CommandType commandType = CommandType.Text)
        {
            using (SqlConnection_Con conn = new SqlConnection_Con(DataBaseType, ConnString))
            {
                using (DbCommandCommon cmd = new DbCommandCommon(DataBaseType))
                {
                    PreparCommand(conn.DbConnection, cmd.DbCommand, commandTextOrSpName, commandType);
                    return cmd.DbCommand.ExecuteNonQuery();
                }
            }
        }


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
