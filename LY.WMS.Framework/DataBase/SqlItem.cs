namespace LY.WMS.Framework.DataBase
{
    public class SqlItem
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
        /// 修改行数
        /// </summary>
        /// <returns></returns>
        public int ModifyRowCount { get; set; }

        /// <summary>
        /// 执行的SQL正文
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public string SqlStr { get; set; }

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
        public SqlItem() { }

        /// <summary>
        /// 新实例
        /// </summary>
        /// <param name="ParamSqlStr">执行的SQL</param>
        /// <param name="ParamResutlCount">期望会影响的行数,-1表示不检查结果</param>
        /// <remarks></remarks>
        public SqlItem(string ParamSqlStr, int ParamResutlCount)
        {
            SqlStr = ParamSqlStr;
            ResultCount = ParamResutlCount;
        }
    }
}
