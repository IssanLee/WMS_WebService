using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LY.WMS.Framework.DataBase
{
    public class SqlItemResult
    {
        /// <summary>
        /// 执行结果
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SqlResult { get; set; }

        /// <summary>
        ///     执行出错的信息
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string ErrorDescript { get; set; }

        /// <summary>
        /// 执行出错的SQL对象
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public SqlItem ErrorSqlItem { get; set; }

        /// <summary>
        /// 新实例
        /// </summary>
        /// <remarks></remarks>
        public SqlItemResult()
        {
            ErrorSqlItem = new SqlItem();
        }

        /// <summary>
        /// 新实例
        /// </summary>
        /// <param name="ParamSqlResult"></param>
        /// <param name="ParamErrorDescript"></param>
        /// <param name="ParamErrorMessage"></param>
        /// <param name="ParamErrorSqlItem"></param>
        /// <remarks></remarks>
        public SqlItemResult(bool ParamSqlResult, string ParamErrorDescript, string ParamErrorMessage, SqlItem ParamErrorSqlItem)
        {
            ErrorSqlItem = new SqlItem();
            SqlResult = ParamSqlResult;
            ErrorDescript = ParamErrorDescript;
            ErrorMessage = ParamErrorMessage;
            ErrorSqlItem = ParamErrorSqlItem;
        }

        /// <summary>
        /// 返回出错信息
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetErrorString()
        {
            return ErrorDescript + "\r\n" + ErrorMessage;
        }
    }
}
