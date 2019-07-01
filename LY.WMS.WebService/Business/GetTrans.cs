using LY.WMS.WebService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace LY.WMS.WebService.Business
{
    public class GetTrans
    {
        public static List<ReqClass> GetReqList(string ParamHeadStr, string ParamItemStr, DateTime ParamLmdate, int ParamRowIndex, int ParamRowNumber, DateTime ParamDownLoadDate)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = stringBuilder;
            stringBuilder2.Append(" SELECT ").Append("\r\n");
            stringBuilder2.Append("  T.* ").Append("\r\n");
            stringBuilder2.Append(" FROM ").Append("\r\n");
            stringBuilder2.Append(" ( ").Append("\r\n");
            stringBuilder2.Append("     SELECT  ").Append("\r\n");
            stringBuilder2.Append("      ROW_NUMBER() OVER(ORDER BY V.LM_DATE,V.CODE) ROW_INDEX, ").Append("\r\n");
            stringBuilder2.Append("      V.*  ").Append("\r\n");
            stringBuilder2.Append("     FROM ").Append(ParamHeadStr).Append(" V ").Append("\r\n");
            stringBuilder2.Append("     WHERE V.LM_DATE > ").Append(Common.DateToFullStrWithFlag(ParamLmdate)).Append("\r\n");
            stringBuilder2.Append("       AND V.LM_DATE <= ").Append(Common.DateToFullStrWithFlag(ParamDownLoadDate)).Append("\r\n");
            stringBuilder2.Append(" ) T ").Append("\r\n");
            stringBuilder2.Append(" WHERE T.ROW_INDEX >  ").Append(ParamRowIndex.ToString()).Append("\r\n");
            stringBuilder2.Append("   AND T.ROW_INDEX <= ").Append(ParamRowNumber.ToString()).Append("\r\n");
            DataTable dataTableBySql = Common.MsSqlDB.GetDataTableBySql(stringBuilder.ToString());
            if (dataTableBySql == null)
            {
                return null;
            }
            if (dataTableBySql.Rows.Count == 0)
            {
                return null;
            }
            List<ReqClass> list = new List<ReqClass>();
            checked
            {
                int num = dataTableBySql.Rows.Count - 1;
                for (int i = 0; i <= num; i++)
                {
                    ReqClass reqClass = new ReqClass();
                    DataRow dataRow = dataTableBySql.Rows[i];
                    reqClass.ORG_ID = Convert.ToString(dataRow["ORG_ID"]);
                    reqClass.WH_CODE = Convert.ToString(dataRow["WH_CODE"]);
                    reqClass.PARTNER_CODE = Convert.ToString(dataRow["PARTNER_CODE"]);
                    reqClass.CODE = Convert.ToString(dataRow["CODE"]);
                    reqClass.STATUS_CODE = Convert.ToString(dataRow["STATUS_CODE"]);
                    reqClass.TYPE_CODE = Convert.ToString(dataRow["TYPE_CODE"]);
                    reqClass.ITEM_COUNT = Convert.ToString(dataRow["ITEM_COUNT"]);
                    reqClass.TOTAL_QTY = Convert.ToDecimal(dataRow["TOTAL_QTY"]);
                    reqClass.TOTAL_AMOUNT = Convert.ToString(dataRow["TOTAL_AMOUNT"]);
                    reqClass.IMP_STATUS_CODE = dataRow["IMP_STATUS_CODE"].ToString();
                    reqClass.EXP_STATUS_CODE = dataRow["EXP_STATUS_CODE"].ToString();
                    reqClass.FROM_OD_ID = dataRow["FROM_OD_ID"].ToString();
                    reqClass.TO_OD_ID = dataRow["TO_OD_ID"].ToString();
                    reqClass.NOTE = dataRow["NOTE"].ToString();
                    reqClass.NOTE_1 = dataRow["NOTE_1"].ToString();
                    reqClass.NOTE_2 = dataRow["NOTE_2"].ToString();
                    reqClass.NOTE_3 = dataRow["NOTE_3"].ToString();
                    reqClass.NOTE_4 = dataRow["NOTE_4"].ToString();
                    reqClass.NOTE_5 = dataRow["NOTE_5"].ToString();
                    reqClass.NOTE_6 = dataRow["NOTE_6"].ToString();
                    reqClass.NOTE_7 = dataRow["NOTE_7"].ToString();
                    reqClass.NOTE_8 = dataRow["NOTE_8"].ToString();
                    reqClass.NOTE_9 = dataRow["NOTE_9"].ToString();
                    reqClass.NOTE_10 = dataRow["NOTE_10"].ToString();
                    reqClass.ERPID = dataRow["ERPID"].ToString();
                    reqClass.BaseId = new BaseIdClass(Convert.ToString(dataRow["REQ_ID"]), String.Empty);
                    reqClass.BaseVersion = new BaseVersionClass(dataRow["CR_BY"].ToString(), Convert.ToDateTime(dataRow["CR_DATE"]), dataRow["LM_BY"].ToString(), Convert.ToDateTime(dataRow["LM_DATE"]), Convert.ToInt32(dataRow["ROW_VER"].ToString()));
                    dataRow = null;
                    list.Add(reqClass);
                    stringBuilder.Clear();
                    stringBuilder.Append(" SELECT * FROM ").Append(ParamItemStr).Append(" I WHERE I.REQ_CODE = '")
                        .Append(dataTableBySql.Rows[i]["CODE"].ToString())
                        .Append("'")
                        .Append("\r\n");
                    DataTable dataTableBySql2 = Common.MsSqlDB.GetDataTableBySql(stringBuilder.ToString());
                    if (dataTableBySql2 != null && dataTableBySql2.Rows.Count != 0)
                    {
                        int num2 = dataTableBySql2.Rows.Count - 1;
                        for (int j = 0; j <= num2; j++)
                        {
                            ReqItemClass reqItemClass = new ReqItemClass();
                            DataRow dataRow2 = dataTableBySql2.Rows[j];
                            reqItemClass.BaseId = new BaseIdClass(dataRow2["REQ_ITEM_ID"].ToString(), String.Empty);
                            reqItemClass.REQ_CODE = Convert.ToString(dataRow2["REQ_CODE"]);
                            reqItemClass.G_CODE = Convert.ToString(dataRow2["G_CODE"]);
                            reqItemClass.GP_CODE = Convert.ToString(dataRow2["GP_CODE"]);
                            reqItemClass.BatchNo = dataRow2["BatchNo"].ToString();
                            reqItemClass.LOT_NO = dataRow2["LOT_NO"].ToString();
                            reqItemClass.WH_CODE = dataRow2["WH_CODE"].ToString();
                            reqItemClass.LOC_CODE = dataRow2["LOC_CODE"].ToString();
                            reqItemClass.STATUS_CODE = Convert.ToString(dataRow2["STATUS_CODE"]);
                            reqItemClass.TYPE_CODE = Convert.ToString(dataRow2["TYPE_CODE"]);
                            reqItemClass.ROW_INDEX = Convert.ToString(dataRow2["ROW_INDEX"]);
                            reqItemClass.NOTE = dataRow2["NOTE"].ToString();
                            reqItemClass.QTY = Convert.ToDecimal(dataRow2["QTY"]);
                            reqItemClass.UNIT_QTY = Convert.ToDecimal(dataRow2["UNIT_QTY"]);
                            reqItemClass.UNIT_PRICE = Convert.ToString(dataRow2["UNIT_PRICE"]);
                            reqItemClass.AMOUNT = Convert.ToString(dataRow2["AMOUNT"]);
                            reqItemClass.BASE_GP_CODE = Convert.ToString(dataRow2["BASE_GP_CODE"]);
                            reqItemClass.BASE_UNIT_QTY = Convert.ToDecimal(dataRow2["BASE_UNIT_QTY"]);
                            reqItemClass.BASE_QTY = Convert.ToDecimal(dataRow2["BASE_QTY"]);
                            reqItemClass.ERPID = Convert.ToString(dataRow2["ERPID"]);
                            reqItemClass.BaseVersion = new BaseVersionClass(dataTableBySql.Rows[i]["CR_BY"].ToString(), Convert.ToDateTime(dataTableBySql.Rows[i]["CR_DATE"].ToString()), dataTableBySql.Rows[i]["LM_BY"].ToString(), Convert.ToDateTime(dataTableBySql.Rows[i]["LM_DATE"].ToString()), 1);
                            dataRow2 = null;
                            reqClass.ItemList.Add(reqItemClass);
                        }
                    }
                }
                return list;
            }
        }
    }
}