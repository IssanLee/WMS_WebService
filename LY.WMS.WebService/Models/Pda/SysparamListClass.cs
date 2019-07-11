using LY.WMS.Framework.DataBase;
using System;
using System.Collections.Generic;
using System.Data;

namespace LY.WMS.WebService.Models
{
    /// <summary>
    /// 系统参数列表
    /// </summary>
    public class SysparamListClass : List<SysparamItemClass>
    {

        public bool IsReadAgain { get; set; }

        public SysparamListClass()
        {
            IsReadAgain = true;
        }

        public bool GetFromDb(DBHelper paramDb)
        {
            base.Clear();
            DataTable dataTableBySql = new DataTable();
            dataTableBySql = paramDb.GetDataTableBySql("SELECT * FROM V_SYS_param_LIST ORDER BY CODE");
            if (dataTableBySql == null)
            {
                return false;
            }
            if (dataTableBySql.Rows.Count == 0)
            {
                return false;
            }
            for (int num = 0; num < dataTableBySql.Rows.Count; num++)
            {
                DataRow row = dataTableBySql.Rows[num];
                base.Add(new SysparamItemClass(Convert.ToInt32(row["SYS_param_LIST_ID"]),
                    Common.IsDbNullToStr(row["CODE"]),
                    Common.IsDbNullToStr(row["NAME"]),
                    Common.IsDbNullToStr(row["NOTE"]),
                    Convert.ToInt32(row["SYS_param_GP_ID"]),
                    Convert.ToString(row["SYS_param_GP_CODE"]),
                    Convert.ToString(row["SYS_param_GP_NAME"]),
                    Common.IsNullStrToZero(row["DEF_VALUE_ID"].ToString()),
                    Common.IsDbNullToStr(row["DEF_VALUE_CODE"].ToString()),
                    Common.IsDbNullToStr(row["DEF_VALUE_NAME"].ToString()), ""));
            }
            return true;

        }

        public SysparamItemClass GetparamById(int paramId)
        {
            checked
            {
                SysparamItemClass result;
                if (paramId <= 0)
                {
                    result = null;
                }
                else
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (base[i].Id == paramId)
                        {
                            result = base[i];
                            return result;
                        }
                    }
                    result = null;
                }
                return result;
            }
        }

        public string GetparamCodeById(int paramId)
        {
            checked
            {
                string result;
                if (paramId <= 0)
                {
                    result = null;
                }
                else
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (base[i].Id == paramId)
                        {
                            result = base[i].Code;
                            return result;
                        }
                    }
                    result = null;
                }
                return result;
            }
        }

        public string GetparamValueCodeById(int paramId)
        {
            checked
            {
                string result;
                if (paramId <= 0)
                {
                    result = null;
                }
                else
                {
                    for (int i = 0; i < Count; i++)
                    {
                        if (base[i].Id == paramId)
                        {
                            result = base[i].ValueCode;
                            return result;
                        }
                    }
                    result = null;
                }
                return result;
            }
        }

        public string GetparamValueCodeByCode(string paramCode)
        {
            checked
            {
                string result;
                if (paramCode.Trim() == string.Empty)
                {
                    result = null;
                }
                else
                {
                    if (IsReadAgain)
                    {
                        if (!GetFromDb(Common.OracleDB))
                        {
                            result = null;
                            return result;
                        }
                        IsReadAgain = false;
                    }
                    for (int i = 0; i < Count; i++)
                    {
                        if (base[i].Code == paramCode)
                        {
                            result = base[i].ValueCode;
                            return result;
                        }
                    }
                    result = null;
                }
                return result;
            }
        }
    }
}