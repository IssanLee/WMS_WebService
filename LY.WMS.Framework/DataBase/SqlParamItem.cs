using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMS.Framework.DataBase
{
    public class SqlParamItem
    {
        /// <summary>
        /// 名称
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <returns></returns>
        public string Description { get; set; }

        /// <summary>
        /// 执行的SQL正文
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public string SqlStr { get; set; }

        /// <summary>
        /// 执行后受影响的行数
        /// </summary>
        /// <returns></returns>
        public int ModifyRowCount { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        /// <returns></returns>
        public List<DbParameter> ParamList { get; set; }

        /// <summary>
        /// 预计SQL所影响的行数
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public int ResultCount { get; set; }

        /// <summary>
        /// 执行失败的出错信息
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public string ErrorString { get; set; }

        /// <summary>
        /// 新实例
        /// </summary>
        /// <remarks></remarks>
        public SqlParamItem(int paramModifyRowCount)
        {
            ParamList = new List<DbParameter>();
            ModifyRowCount = paramModifyRowCount;
        }

        /// <summary>
        /// 新实例
        /// </summary>
        /// <param name="paramSqlStr">执行的SQL</param>
        /// <param name="paramResutlCount">期望会影响的行数,-1表示不检查结果</param>
        /// <remarks></remarks>
        public SqlParamItem(string paramSqlStr, int paramResutlCount, int paramModifyRowCount)
        {
            ParamList = new List<DbParameter>();
            SqlStr = paramSqlStr;
            ResultCount = paramResutlCount;
            ModifyRowCount = paramModifyRowCount;
        }

        /// <summary>
        /// 新实例
        /// </summary>
        /// <param name="paramSqlStr">执行的SQL</param>
        /// <param name="sqlParamList">参数列表</param>
        /// <param name="paramResutlCount">期望会影响的行数,-1表示不检查结果</param>
        public SqlParamItem(string paramSqlStr, List<DbParameter> sqlParamList, int paramResutlCount, int paramModifyRowCount)
        {
            ParamList = new List<DbParameter>();
            SqlStr = paramSqlStr;
            ParamList = sqlParamList;
            ResultCount = paramResutlCount;
            ModifyRowCount = paramModifyRowCount;
        }

        /// <summary>
        /// 新实例
        /// </summary>
        /// <param name="paramSqlStr">执行的SQL</param>
        /// <param name="paramSqlParamList">参数列表</param>
        /// <param name="paramResutlCount">期望会影响的行数,-1表示不检查结果</param>
        /// <param name="paramName">名称</param>
        public SqlParamItem(string paramSqlStr, List<DbParameter> paramSqlParamList, int paramResutlCount, string paramName, int paramModifyRowCount)
        {
            ParamList = new List<DbParameter>();
            Name = paramName;
            ModifyRowCount = paramModifyRowCount;
            SqlStr = paramSqlStr;
            ParamList = paramSqlParamList;
            ResultCount = paramResutlCount;
        }

        /// <summary>
        /// 获取执行的Sql字符串
        /// </summary>
        /// <returns></returns>
        public string GetSqlStr()
        {
            if ((ParamList == null) | (ParamList.Count == 0))
            {
                return SqlStr;
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (DbParameter param in ParamList)
            {
                stringBuilder.Append(param.ParameterName);
            }
            return SqlStr + "\r\nParam -> " + stringBuilder.ToString();
        }
    }
}
