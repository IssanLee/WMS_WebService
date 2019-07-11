using LY.WMS.WebService.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace LY.WMS.WebService.Business
{
    public class Info
    {

        /// <summary>
        /// 系统信息
        /// </summary>
        /// <returns></returns>
        public static SystemInformation GetSystemInformation()
        {
            return new SystemInformation
            {
                Code = "WMS WINCE SERVICE",
                Name = "LY WMS FOR WINDOWS CE",
                Descript = "乐药WMS终端作业系统",
                Version = "1.0.0.1",
                Copyright = "广州 乐药 2018-2019 CopyRight",
                UsedBy = "医药仓储"
            };
        }

        /// <summary>
        /// 获取系统参数
        /// </summary>
        /// <returns></returns>
        public static List<SystemparamClass> GetSystemparamList()
        {
            List<SystemparamClass> list = new List<SystemparamClass>();
            string sql = string.Format(@"SELECT SYS_param_LIST_ID, CODE, NAME, DEF_VALUE_CODE FROM V_SYS_param_LIST WHERE SYS_param_GP_CODE = 'MOBILEPARAM'");
            DataTable dataTableBySql = Common.OracleDB.GetDataTableBySql(sql);
            checked
            {
                List<SystemparamClass> result;
                if (dataTableBySql == null)
                {
                    result = null;
                }
                else if (dataTableBySql.Rows.Count == 0)
                {
                    result = null;
                }
                else
                {
                    int num = dataTableBySql.Rows.Count - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        SystemparamClass systemparamClass = new SystemparamClass();
                        DataRow dataRow = dataTableBySql.Rows[i];
                        systemparamClass.Id = Convert.ToInt32(dataRow["SYS_PARAM_LIST_ID"]);
                        systemparamClass.Code = dataRow["CODE"].ToString();
                        systemparamClass.Name = dataRow["NAME"].ToString();
                        systemparamClass.Value = dataRow["DEF_VALUE_CODE"].ToString();
                        list.Add(systemparamClass);
                    }
                    result = list;
                }
                return result;
            }
        }

        /// <summary>
        /// 获取系统时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetSystemDatetime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 获取数据库时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDbDatetime()
        {
            return Convert.ToDateTime(Common.OracleDB.GetStringBySql("SELECT TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS') DT FROM DUAL"));
        }

    }
}