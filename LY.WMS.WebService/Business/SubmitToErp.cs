using LY.WMS.Framework.DataBase;
using LY.WMS.WebService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace LY.WMS.WebService.Business
{
    public class SubmitToErp
    {
        public static ResultMessage DoSubmitAccLocGbnToErp(List<ErpAccLocGbnClass> paramErpAccLocGbnList)
        {
            checked
            {
                ResultMessage result;
                if ((paramErpAccLocGbnList == null) | (paramErpAccLocGbnList.Count == 0))
                {
                    result = new ResultMessage(false, "提交的数据无效!", "提交的数据无效!");
                }
                else
                {
                    List<SqlItem> list = new List<SqlItem>();
                    StringBuilder stringBuilder = new StringBuilder();
                    new StringBuilder();
                    int num = paramErpAccLocGbnList.Count - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        ErpAccLocGbnClass erpAccLocGbnClass = paramErpAccLocGbnList[i];
                        StringBuilder stringBuilder2 = stringBuilder;
                        stringBuilder2.Clear();
                        stringBuilder2.Append("IF EXISTS(SELECT * FROM WMS_SWAP_ACC_LOC_GBN T WHERE T.WMS_ID =").Append(erpAccLocGbnClass.WMS_ID.ToString()).Append(" )")
                            .Append("\r\n");
                        stringBuilder2.Append("BEGIN").Append("\r\n");
                        stringBuilder2.Append("    UPDATE WMS_SWAP_ACC_LOC_GBN").Append("\r\n");
                        stringBuilder2.Append("    SET").Append("\r\n");
                        stringBuilder2.Append("       WH_CODE    ='").Append(erpAccLocGbnClass.WH_CODE.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,LOC_CODE   ='").Append(erpAccLocGbnClass.LOC_CODE.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,G_CODE     ='").Append(erpAccLocGbnClass.G_CODE.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,ERP_G_ID   ='").Append(erpAccLocGbnClass.ERP_G_ID.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,BATCH_NO   ='").Append(erpAccLocGbnClass.BATCH_NO.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,GP_CODE    ='").Append(erpAccLocGbnClass.GP_CODE.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,QTY        = ").Append(erpAccLocGbnClass.QTY.ToString()).Append("\r\n");
                        stringBuilder2.Append("      ,CR_BY      ='").Append(erpAccLocGbnClass.CR_BY.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,CR_DATE    ='").Append(erpAccLocGbnClass.CR_DATE.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,LM_BY      ='").Append(erpAccLocGbnClass.LM_BY.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,LM_DATE    ='").Append(erpAccLocGbnClass.LM_DATE.ToString()).Append("'")
                            .Append("\r\n");
                        stringBuilder2.Append("      ,ROW_VER    = ").Append(erpAccLocGbnClass.ROW_VER.ToString()).Append("\r\n");
                        stringBuilder2.Append("    WHERE WMS_ID =").Append(erpAccLocGbnClass.WMS_ID.ToString()).Append("\r\n");
                        stringBuilder2.Append("END").Append("\r\n");
                        stringBuilder2.Append("ELSE").Append("\r\n");
                        stringBuilder2.Append("BEGIN").Append("\r\n");
                        stringBuilder2.Append("    INSERT INTO dbo.WMS_SWAP_ACC_LOC_GBN").Append("\r\n");
                        stringBuilder2.Append("            ( WH_CODE ,              LOC_CODE ,              G_CODE ,              ERP_G_ID ,              BATCH_NO ,").Append("\r\n");
                        stringBuilder2.Append("              GP_CODE ,              QTY      ,              CR_BY  ,              CR_DATE  ,              LM_BY    ,").Append("\r\n");
                        stringBuilder2.Append("              LM_DATE ,              WMS_ID   ,              ROW_VER").Append("\r\n");
                        stringBuilder2.Append("            )").Append("\r\n");
                        stringBuilder2.Append("    SELECT     '").Append(erpAccLocGbnClass.WH_CODE.ToString()).Append("'         WH_CODE")
                            .Append("\r\n");
                        stringBuilder2.Append("              ,'").Append(erpAccLocGbnClass.LOC_CODE.ToString()).Append("'         LOC_CODE")
                            .Append("\r\n");
                        stringBuilder2.Append("              ,'").Append(erpAccLocGbnClass.G_CODE.ToString()).Append("'         G_CODE")
                            .Append("\r\n");
                        stringBuilder2.Append("              ,'").Append(erpAccLocGbnClass.ERP_G_ID.ToString()).Append("'         ERP_G_ID")
                            .Append("\r\n");
                        stringBuilder2.Append("              ,'").Append(erpAccLocGbnClass.BATCH_NO.ToString()).Append("'         BATCH_NO")
                            .Append("\r\n");
                        stringBuilder2.Append("              ,'").Append(erpAccLocGbnClass.GP_CODE.ToString()).Append("'         GP_CODE")
                            .Append("\r\n");
                        stringBuilder2.Append("              , ").Append(erpAccLocGbnClass.QTY.ToString()).Append("          QTY")
                            .Append("\r\n");
                        stringBuilder2.Append("              ,'").Append(erpAccLocGbnClass.CR_BY.ToString()).Append("'         CR_BY")
                            .Append("\r\n");
                        stringBuilder2.Append("              ,GETDATE()  CR_DATE").Append("\r\n");
                        stringBuilder2.Append("              ,'").Append(erpAccLocGbnClass.LM_BY.ToString()).Append("'         LM_BY")
                            .Append("\r\n");
                        stringBuilder2.Append("              ,GETDATE()  LM_DATE").Append("\r\n");
                        stringBuilder2.Append("              ,").Append(erpAccLocGbnClass.WMS_ID.ToString()).Append("          WMS_ID")
                            .Append("\r\n");
                        stringBuilder2.Append("              , ").Append(erpAccLocGbnClass.ROW_VER.ToString()).Append("          ROW_VER")
                            .Append("\r\n");
                        stringBuilder2.Append("END").Append("\r\n");
                        list.Add(new SqlItem(stringBuilder.ToString(), -1));
                    }
                    SqlItemResult sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlBatchWithTran(list);
                    result = ((!sqlItemResultClass.SqlResult) ? new ResultMessage(false, sqlItemResultClass.ErrorMessage.ToString(), sqlItemResultClass.ErrorDescript.ToString()) : new ResultMessage(true, "操作成功", String.Empty));
                }
                return result;
            }
        }


        public static ResultMessage DoSubmitStockMoveToErp(StockMoveToClass paramStockMoveTo)
        {
            List<SqlItem> list = new List<SqlItem>();
            StringBuilder stringBuilder = new StringBuilder();
            new StringBuilder().Clear();
            foreach (StockMoveItemClass item in paramStockMoveTo.ItemList)
            {
                StringBuilder stringBuilder2 = stringBuilder;
                stringBuilder2.Clear();
                stringBuilder2.Append("IF NOT EXISTS (SELECT * FROM WMS_SWAP_MOVE_ITEM P WHERE P.WMS_ID = '").Append(item.WmsId).Append("' AND P.COMMIT_KEY = '")
                    .Append(paramStockMoveTo.CommitKey.ToString())
                    .Append("')")
                    .Append("\r\n");
                stringBuilder2.Append("INSERT INTO WMS_SWAP_MOVE_ITEM").Append("\r\n");
                stringBuilder2.Append("    (COMMIT_KEY,      WORK_TIME,    WORK_BY_CODE,     SOUR_LOC_CODE,  DEST_LOC_CODE,    GOODS_CODE,");
                stringBuilder2.Append("     BATCH_NO,        EXP_DATE,     MANU_DATE,        QTY,           UNIT_CODE,");
                stringBuilder2.Append("     UNIT_QTY,        CR_DATE,      LM_DATE,          STATUS_ID,     WMS_ID,");
                stringBuilder2.Append("     N_1,             N_2,          N_3,              N_4,           ERP_G_ID ");
                stringBuilder2.Append("    )").Append("\r\n");
                stringBuilder2.Append("SELECT");
                stringBuilder2.Append(" '").Append(paramStockMoveTo.CommitKey.ToString()).Append("'      COMMIT_KEY");
                stringBuilder2.Append(",'").Append(item.WorkTime.ToString()).Append("'      WORK_TIME");
                stringBuilder2.Append(",'").Append(item.WorkByCode.ToString()).Append("'      WORK_BY_CODE");
                stringBuilder2.Append(",'").Append(item.SourLocCode.ToString()).Append("'      SOUR_LOC_CODE");
                stringBuilder2.Append(",'").Append(item.DestLocCode.ToString()).Append("'      DEST_LOC_CODE");
                stringBuilder2.Append(",'").Append(item.GoodsCode.ToString()).Append("'      GOODS_CODE");
                stringBuilder2.Append(",'").Append(item.BatchNo.ToString()).Append("'      BATCH_NO");

                DateTime tmpDt;
                if (!string.IsNullOrEmpty(item.ExpDate) && DateTime.TryParse(item.ExpDate, out tmpDt))
                {
                    stringBuilder2.Append(",'").Append(item.ExpDate).Append("'      EXP_DATE");
                }
                else
                {
                    stringBuilder2.Append(",NULL      EXP_DATE");
                }
                if (!string.IsNullOrEmpty(item.ManuDate) && DateTime.TryParse(item.ManuDate, out tmpDt))
                {
                    stringBuilder2.Append(",'").Append(item.ManuDate).Append("'      MANU_DATE");
                }
                else
                {
                    stringBuilder2.Append(",NULL      MANU_DATE");
                }
                stringBuilder2.Append(",'").Append(item.Qty.ToString()).Append("'      QTY");
                stringBuilder2.Append(",'").Append(item.UnitCode.ToString()).Append("'      UNIT_CODE");
                stringBuilder2.Append(",'").Append(item.UnitQty.ToString()).Append("'      UNIT_QTY");
                stringBuilder2.Append(",'").Append(item.CrDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'      CR_DATE");
                stringBuilder2.Append(",'").Append(item.LmDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'      LM_DATE");
                stringBuilder2.Append(",'").Append(item.StatusId).Append("'      STATUS_ID");
                stringBuilder2.Append(",'").Append(item.WmsId).Append("'      WMS_ID");
                stringBuilder2.Append(",'").Append(item.N_1.ToString()).Append("'      N_1");
                stringBuilder2.Append(",'").Append(item.N_2.ToString()).Append("'      N_2");
                stringBuilder2.Append(",'").Append(item.N_3.ToString()).Append("'      N_3");
                stringBuilder2.Append(",'").Append(item.N_4.ToString()).Append("'      N_4");
                stringBuilder2.Append(",'").Append(item.ErpGoodsId.ToString()).Append("'      ERP_G_ID");
                stringBuilder2 = null;
                list.Add(new SqlItem(stringBuilder.ToString(), -1));
            }
            SqlItemResult sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlBatchWithTran(list);
            ResultMessage result;
            if (!sqlItemResultClass.SqlResult)
            {
                result = new ResultMessage(false, sqlItemResultClass.ErrorMessage.ToString(), sqlItemResultClass.ErrorDescript.ToString());
            }
            else if (paramStockMoveTo.ItemList.Count != Common.MsSqlDB.GetIntegerBySql("SELECT COUNT(*) ITEM_COUNT FROM WMS_SWAP_MOVE_ITEM I WHERE I.COMMIT_KEY = '" + paramStockMoveTo.CommitKey.ToString() + "'"))
            {
                result = new ResultMessage(false, "提交的数据行数不一致!", "提交的数据行数不一致!");
            }
            else
            {
                List<DbParameter> list2 = new List<DbParameter>();
                DbParameter sqlParameter = new SqlParameter("@CommitKey", SqlDbType.NVarChar, 40);
                sqlParameter.Value = paramStockMoveTo.CommitKey;
                list2.Add(sqlParameter);
                sqlParameter = new SqlParameter("@WmsId", SqlDbType.NVarChar, 40);
                sqlParameter.Value = paramStockMoveTo.WmsId.ToString();
                list2.Add(sqlParameter);
                DbParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
                sqlParameter2.Direction = ParameterDirection.Output;
                sqlParameter2.Value = String.Empty;
                list2.Add(sqlParameter2);
                try
                {
                    sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("P_WMS_TO_MOVE", list2);
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    result = new ResultMessage(false, "执行数据转换出错," + ex2.Message.ToString());
                    return result;
                }
                result = (sqlItemResultClass.SqlResult ? ((sqlParameter2.Value == null) ? new ResultMessage(false, "执行数据转换未收到成功标识!") : (sqlParameter2.Value.ToString() == "Success" ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, "执行数据转换失败" + sqlParameter2.Value.ToString()))) : ((sqlParameter2.Value == null) ? new ResultMessage(false, "执行数据转换失败," + sqlItemResultClass.ErrorMessage.ToString()) : new ResultMessage(false, "执行数据转换失败," + sqlParameter2.Value.ToString() + sqlItemResultClass.ErrorMessage.ToString())));
            }
            return result;
        }


        public static ResultMessage SaveReceiveWork(WhWorkItem paramWhWorkItem)
        {
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("Operator", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.WorkByCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BillNo", SqlDbType.NVarChar, 20);
            sqlParameter.Value = paramWhWorkItem.SourTransCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("ItemNo", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.SourTransItemId.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BatchNo", SqlDbType.NVarChar, 30);
            sqlParameter.Value = paramWhWorkItem.GoodsBatchNo.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("ValidDate", SqlDbType.DateTime);

            DateTime tmpDt;
            if (!string.IsNullOrEmpty(paramWhWorkItem.GoodsExpDate) && DateTime.TryParse(paramWhWorkItem.GoodsExpDate, out tmpDt))
            {
                sqlParameter.Value = Convert.ToDateTime(paramWhWorkItem.GoodsExpDate.ToString());
            }
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("ProdDate", SqlDbType.DateTime);
            if (!string.IsNullOrEmpty(paramWhWorkItem.GoodsManuDate) && DateTime.TryParse(paramWhWorkItem.GoodsManuDate, out tmpDt))
            {
                sqlParameter.Value = Convert.ToDateTime(paramWhWorkItem.GoodsManuDate.ToString());
            }
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("Qty", SqlDbType.Decimal, 12);
            sqlParameter.Value = paramWhWorkItem.WorkQty.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("DepotID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.DestWhAreaCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BerthNo", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.DestLocCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("WMS_ID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.WhWorkId.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("ProvSellBillNo", SqlDbType.NVarChar, 40);
            sqlParameter.Value = paramWhWorkItem.ShipCode.ToString();
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            ResultMessage result;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_PchCheckIn", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                result = new ResultMessage(false, string.Empty, ex2.Message.ToString());
                return result;
            }
            if (!sqlItemResultClass.SqlResult)
            {
                result = new ResultMessage(false, string.Empty, sqlParameter2.Value.ToString() + "\r\n" + sqlItemResultClass.ErrorMessage.ToString());
            }
            else if (sqlParameter2.Value.ToString() != "Success")
            {
                result = new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString());
            }
            else
            {
                ReceivedFinished(paramWhWorkItem.SourTransCode.ToString());
                result = new ResultMessage(true, String.Empty, String.Empty);
            }
            return result;
        }


        public static ResultMessage SaveReceiveUpWork(WhWorkItem paramWhWorkItem)
        {
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("Operator", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.WorkByCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("WMS_ID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.WhWorkId.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("DepotID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.DestWhAreaCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BerthNo", SqlDbType.NVarChar, 8);
            sqlParameter.Value = "Def01";
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_StockIn", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                ResultMessage result = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result;
            }
            return sqlItemResultClass.SqlResult ? (sqlParameter2.Value.ToString() == "Success" ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString())) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString() + "\r\n" + sqlItemResultClass.ErrorMessage.ToString());
        }


        public static ResultMessage SaveStockTakeWork(WhWorkItem paramWhWorkItem)
        {
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("@Checker", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.WorkByCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("@CheckTime", SqlDbType.DateTime);
            sqlParameter.Value = paramWhWorkItem.WorkDate.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("@DepotID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.DestWhAreaCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("@BerthNo", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.DestLocCode.ToString();
            sqlParameter.Value = "Def01";
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("@GoodsID", SqlDbType.NVarChar, 16);
            sqlParameter.Value = paramWhWorkItem.GoodsCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("@BatchNo", SqlDbType.NVarChar, 30);
            sqlParameter.Value = paramWhWorkItem.GoodsBatchNo.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("@Unit", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.GoodsPackCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("@RealQty", SqlDbType.Decimal, 12);
            sqlParameter.Value = paramWhWorkItem.WorkQty;
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BillFlagStr", SqlDbType.NVarChar, 5);
            sqlParameter.Value = paramWhWorkItem.WorkDeviceCode.ToString();
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_StockChecks", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                ResultMessage result = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result;
            }
            return sqlParameter2.Value.ToString() != "Success" ? new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString()) : (sqlItemResultClass.SqlResult ? (sqlParameter2.Value.ToString() == "Success" ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString())) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString() + sqlItemResultClass.ErrorMessage.ToString()));
        }


        public static ResultMessage SaveMoveFromWhWork(WhWorkItem paramWhWorkItem)
        {
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("Operator", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.WorkByCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("OpTime", SqlDbType.DateTime);
            sqlParameter.Value = paramWhWorkItem.WorkDate;
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("SrcDepotID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.SourWhAreaCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("SrcBerthNo", SqlDbType.NVarChar, 20);
            sqlParameter.Value = paramWhWorkItem.SourLocCode.ToString();
            sqlParameter.Value = "Def01";
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("DestDepotID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.DestWhAreaCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("DestBerthNo", SqlDbType.NVarChar, 20);
            sqlParameter.Value = paramWhWorkItem.DestLocCode.ToString();
            sqlParameter.Value = "Def01";
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("GoodsID", SqlDbType.NVarChar, 16);
            sqlParameter.Value = paramWhWorkItem.GoodsCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BatchNo", SqlDbType.NVarChar, 30);
            sqlParameter.Value = paramWhWorkItem.GoodsBatchNo.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("Qty", SqlDbType.Decimal, 12);
            sqlParameter.Value = paramWhWorkItem.WorkQty.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BillFlagStr", SqlDbType.NVarChar, 5);
            sqlParameter.Value = paramWhWorkItem.WorkDeviceCode.ToString();
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            ResultMessage result;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_StockMove", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                result = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result;
            }
            if (!sqlItemResultClass.SqlResult)
            {
                result = new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString() + sqlItemResultClass.ErrorMessage.ToString());
            }
            else if (sqlParameter2.Value.ToString() != "Success")
            {
                result = new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString());
            }
            else
            {
                ReceivedFinished(paramWhWorkItem.SourTransCode.ToString());
                result = new ResultMessage(true, String.Empty, String.Empty);
            }
            return result;
        }


        public static ResultMessage SaveMoveToWhWork(WhWorkItem paramWhWorkItem)
        {
            return new ResultMessage(false, "00", paramWhWorkItem.WhWorkType.ToString() + ":未实现此接口");
        }

        public static ResultMessage SaveOctDiffWork(WhWorkItem paramWhWorkItem)
        {
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("Operator", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.WorkByCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("DepotID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.SourWhAreaCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("InDepotID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.DestWhAreaCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("InBerthNo", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItem.DestLocCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BillNo", SqlDbType.NVarChar, 20);
            sqlParameter.Value = paramWhWorkItem.SourTransCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("ItemNo", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItem.SourTransItemId.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("LessQty", SqlDbType.Decimal, 12);
            sqlParameter.Value = paramWhWorkItem.WorkQty.ToString();
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            ResultMessage result;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_StockOutDiff", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                result = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result;
            }
            if (!sqlItemResultClass.SqlResult)
            {
                result = new ResultMessage(false, String.Empty, sqlItemResultClass.ErrorMessage.ToString());
            }
            else if (sqlParameter2.Value.ToString() != "Success")
            {
                result = new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString());
            }
            else
            {
                SetYbxOctCheckFinishedWithReturnFlag(paramWhWorkItem.SourTransCode.ToString());
                result = new ResultMessage(true, String.Empty, String.Empty);
            }
            return result;
        }

        public static ResultMessage DoSubmitSocToErp(List<SocClass> paramSocList)
        {
            checked
            {
                ResultMessage result;
                if ((paramSocList == null) | (paramSocList.Count == 0))
                {
                    result = new ResultMessage(false, "提交的数据无效!", "提交的数据无效!");
                }
                else
                {
                    int num = paramSocList.Count - 1;
                    int num2 = 0;
                    while (true)
                    {
                        if (num2 <= num)
                        {
                            if (paramSocList[num2].ItemCount != paramSocList[num2].ItemList.Count)
                            {
                                result = new ResultMessage(false, "提交的数据行不一致!", "提交的数据行不一致!");
                                break;
                            }
                            num2++;
                            continue;
                        }
                        StringBuilder stringBuilder = new StringBuilder();
                        StringBuilder stringBuilder2 = new StringBuilder();
                        int num3 = paramSocList.Count - 1;
                        int num4 = 0;
                        while (true)
                        {
                            if (num4 > num3)
                            {
                                result = new ResultMessage(true, "操作成功", String.Empty);
                                break;
                            }
                            SocClass socClass = paramSocList[num4];
                            List<SqlItem> list = new List<SqlItem>();
                            if (Common.MsSqlDB.GetIntegerBySql("SELECT H.STATUS_ID FROM WMS_SWAP_OC H WHERE H.SO_CODE = '" + socClass.SoCode + "'") != 1)
                            {
                                stringBuilder2.Clear();
                                stringBuilder.Clear();
                                StringBuilder stringBuilder3 = stringBuilder;
                                stringBuilder3.Append("IF NOT EXISTS(SELECT * FROM WMS_SWAP_OC WHERE SO_CODE = '").Append(socClass.SoCode).Append("')")
                                    .Append("\r\n");
                                stringBuilder3.Append("INSERT INTO WMS_SWAP_OC").Append("\r\n");
                                stringBuilder3.Append("  (SO_CODE,     WORK_BY_CODE,       WORK_TIME,     WH_ID,           ITEM_COUNT,").Append("\r\n");
                                stringBuilder3.Append("   CR_DATE,     LM_DATE,            STATUS_ID,     WMSID,           N_1,").Append("\r\n");
                                stringBuilder3.Append("   N_2,         N_3,                N_4,           SHIP_ORDER_COUNT ").Append("\r\n");
                                stringBuilder3.Append("  )").Append("\r\n");
                                stringBuilder3.Append("SELECT").Append("\r\n");
                                stringBuilder3.Append("    '").Append(socClass.SoCode).Append("'     SO_CODE")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.WorkByCode).Append("'     WORK_BY_CODE")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.WorkTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     WORK_TIME")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.WhId.ToString()).Append("'     WH_ID")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.ItemCount.ToString()).Append("'     ITEM_COUNT")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.CrDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     CR_DATE")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.LmDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     LM_DATE")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.StatusId.ToString()).Append("'     STATUS_ID")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.WmsId.ToString()).Append("'     WMSID")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.N_1.ToString()).Append("'     N_1")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.N_2.ToString()).Append("'     N_2")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.N_3.ToString()).Append("'     N_3")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.N_4.ToString()).Append("'     N_4")
                                    .Append("\r\n");
                                stringBuilder3.Append("   ,'").Append(socClass.ShipOrderCount.ToString()).Append("'     SHIP_ORDER_COUNT")
                                    .Append("\r\n");
                                list.Add(new SqlItem(stringBuilder.ToString(), -1));
                                foreach (SocItemClass item in socClass.ItemList)
                                {
                                    stringBuilder2.Clear();
                                    StringBuilder stringBuilder4 = stringBuilder2;
                                    stringBuilder4.Append("IF NOT EXISTS (SELECT * FROM WMS_SWAP_OC_ITEM P WHERE P.WMSID = '").Append(item.WmsId).Append("' AND P.SO_CODE = '")
                                        .Append(socClass.SoCode.ToString())
                                        .Append("')")
                                        .Append("\r\n");
                                    stringBuilder4.Append("INSERT INTO WMS_SWAP_OC_ITEM");
                                    stringBuilder4.Append("  (WMS_SWAP_OC_ID,     SO_ITEM_ID,       SO_CODE,      SO_ITEM_INDEX,      GOODS_CODE,      BATCH_NO,");
                                    stringBuilder4.Append("   EXP_DATE,            MANU_DATE,        UNIT_CODE,    QTY,                UNIT_QTY,        WORK_BY_CODE,");
                                    stringBuilder4.Append("   WORK_TIME,           WH_ID,            WMSID,        CR_DATE,            N_1,             N_2,");
                                    stringBuilder4.Append("   N_3,                 N_4,              ERP_G_ID,     OFFSET_QTY          ,OFFSET_REASON,  DIFF_QTY, ORDER_QTY      ) ").Append("\r\n");
                                    stringBuilder4.Append("SELECT");
                                    stringBuilder4.Append(" (SELECT H.WMS_SWAP_OC_ID FROM WMS_SWAP_OC H WHERE H.SO_CODE = '").Append(socClass.SoCode.ToString()).Append("')");
                                    stringBuilder4.Append("  ,'").Append(item.SoItemId.ToString()).Append("'");
                                    stringBuilder4.Append("  ,'").Append(socClass.SoCode.ToString()).Append("'");
                                    stringBuilder4.Append("  ,'").Append(item.SoItemIndex.ToString()).Append("'");
                                    stringBuilder4.Append("  ,'").Append(item.GCode.ToString()).Append("'");
                                    stringBuilder4.Append("  ,'").Append(item.BatchNo.ToString()).Append("'");
                                    DateTime tmpDt;
                                    if (!string.IsNullOrEmpty(item.ExpDate) && DateTime.TryParse(item.ExpDate, out tmpDt))
                                    {
                                        stringBuilder4.Append(",'").Append(Convert.ToDateTime(item.ExpDate).ToString("yyyy-MM-dd")).Append("'");
                                    }
                                    else
                                    {
                                        stringBuilder4.Append(",NULL");
                                    }
                                    if (!string.IsNullOrEmpty(item.ManuDate) && DateTime.TryParse(item.ManuDate, out tmpDt))
                                    {
                                        stringBuilder4.Append(",'").Append(Convert.ToDateTime(item.ManuDate).ToString("yyyy-MM-dd")).Append("'");
                                    }
                                    else
                                    {
                                        stringBuilder4.Append(",NULL");
                                    }
                                    stringBuilder4.Append(",'").Append(item.GpCode.ToString()).Append("'");
                                    stringBuilder4.Append(",").Append(item.Qty.ToString()).Append(String.Empty);
                                    stringBuilder4.Append(",'").Append(item.GpUnitQty.ToString()).Append("'");
                                    stringBuilder4.Append(",'").Append(item.WorkByCode.ToString()).Append("'");
                                    stringBuilder4.Append(",'").Append(item.WorkTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("'");
                                    stringBuilder4.Append(",'").Append(item.WhId).Append("'");
                                    stringBuilder4.Append(",'").Append(item.WmsId).Append("'");
                                    stringBuilder4.Append(",'").Append(item.CrDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'");
                                    stringBuilder4.Append(",'").Append(item.N_1.ToString()).Append("'");
                                    stringBuilder4.Append(",'").Append(item.N_2.ToString()).Append("'")
                                        .Append("\r\n");
                                    stringBuilder4.Append(",'").Append(item.N_3.ToString()).Append("'");
                                    stringBuilder4.Append(",'").Append(item.N_4.ToString()).Append("'");
                                    stringBuilder4.Append(",'").Append(item.ErpGoodsId.ToString()).Append("'");
                                    stringBuilder4.Append(",").Append(item.OffsetQty.ToString()).Append(String.Empty);
                                    stringBuilder4.Append(",'").Append(item.OffsetReason.ToString()).Append("'");
                                    stringBuilder4.Append(",").Append(item.DiffQty.ToString()).Append(String.Empty);
                                    stringBuilder4.Append(",").Append(item.OrderQty.ToString()).Append(String.Empty);
                                    stringBuilder4 = null;
                                    list.Add(new SqlItem(stringBuilder2.ToString(), -1));
                                    if (item.EcodeList != null && item.EcodeList.Count != 0)
                                    {
                                        StringBuilder stringBuilder5 = new StringBuilder();
                                        foreach (EcodeItem ecode in item.EcodeList)
                                        {
                                            stringBuilder5.Clear();
                                            StringBuilder stringBuilder6 = stringBuilder5;
                                            stringBuilder6.Append("IF NOT EXISTS (SELECT *").Append("\r\n");
                                            stringBuilder6.Append("                 FROM dbo.WMS_SWAP_OC_ITEM_ECODE E").Append("\r\n");
                                            stringBuilder6.Append("                 INNER JOIN dbo.WMS_SWAP_OC_ITEM I ON E.WMS_SWAP_OC_ITEM_ID = I.WMS_SWAP_OC_ITEM_ID").Append("\r\n");
                                            stringBuilder6.Append("                 WHERE I.SO_CODE    =  '").Append(socClass.SoCode.ToString()).Append("'")
                                                .Append("\r\n");
                                            stringBuilder6.Append("                   AND I.SO_ITEM_ID =  '").Append(item.SoItemId.ToString()).Append("'")
                                                .Append("\r\n");
                                            stringBuilder6.Append("                   AND E.ECODE =  '").Append(ecode.Ecode.ToString()).Append("'")
                                                .Append("\r\n");
                                            stringBuilder6.Append("              )").Append("\r\n");
                                            stringBuilder6.Append("INSERT INTO dbo.WMS_SWAP_OC_ITEM_ECODE").Append("\r\n");
                                            stringBuilder6.Append("    (").Append("\r\n");
                                            stringBuilder6.Append("        WMS_SWAP_OC_ITEM_ID, ECODE, UNIT_CODE, UNIT_QTY, WMS_ID").Append("\r\n");
                                            stringBuilder6.Append("    )").Append("\r\n");
                                            stringBuilder6.Append("SELECT (SELECT  I.WMS_SWAP_OC_ITEM_ID").Append("\r\n");
                                            stringBuilder6.Append("          FROM  dbo.WMS_SWAP_OC_ITEM I").Append("\r\n");
                                            stringBuilder6.Append("         WHERE I.SO_CODE    =  '").Append(socClass.SoCode.ToString()).Append("'")
                                                .Append("\r\n");
                                            stringBuilder6.Append("           AND I.SO_ITEM_ID =  '").Append(item.SoItemId.ToString()).Append("'")
                                                .Append("\r\n");
                                            stringBuilder6.Append("        ) WMS_SWAP_OC_ITEM_ID").Append("\r\n");
                                            stringBuilder6.Append("    ,'").Append(ecode.Ecode.ToString()).Append("' ECODE")
                                                .Append("\r\n");
                                            stringBuilder6.Append("    ,'").Append(ecode.UnitCode.ToString()).Append("' UNIT_CODE")
                                                .Append("\r\n");
                                            stringBuilder6.Append("    ,'").Append(ecode.UnitQty.ToString()).Append("' UNIT_QTY")
                                                .Append("\r\n");
                                            stringBuilder6.Append("    ,'").Append(ecode.WmsId.ToString()).Append("' WMS_ID")
                                                .Append("\r\n");
                                            list.Add(new SqlItem(stringBuilder5.ToString(), -1));
                                        }
                                    }
                                }
                                SqlItemResult sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlBatchWithTran(list);
                                if (!sqlItemResultClass.SqlResult)
                                {
                                    result = new ResultMessage(false, sqlItemResultClass.ErrorMessage.ToString(), sqlItemResultClass.ErrorDescript.ToString());
                                    break;
                                }
                                if (socClass.ItemCount != Common.MsSqlDB.GetIntegerBySql("SELECT COUNT(*) ITEM_COUNT FROM WMS_SWAP_OC_ITEM I WHERE I.SO_CODE = '" + socClass.SoCode.ToString() + "'"))
                                {
                                    result = new ResultMessage(false, "提交的数据行数不一致!", "提交的数据行数不一致!");
                                    break;
                                }
                                stringBuilder2.Clear();
                                if ((socClass.ShipOrderList != null) & (socClass.ShipOrderList.Count > 0))
                                {
                                    foreach (SocShipOrderClass shipOrder in socClass.ShipOrderList)
                                    {
                                        StringBuilder stringBuilder7 = stringBuilder;
                                        stringBuilder7.Clear();
                                        stringBuilder7.Append("IF  NOT EXISTS(SELECT * FROM WMS_SWAP_OC_SHIP_ORDER I WHERE I.SO_CODE = '").Append(shipOrder.SoCode.ToString()).Append("' AND I.SHIP_ORDER_CODE = '")
                                            .Append(shipOrder.ShipOrderCode.ToString())
                                            .Append("')")
                                            .Append("\r\n");
                                        stringBuilder7.Append("INSERT INTO WMS_SWAP_OC_SHIP_ORDER").Append("\r\n");
                                        stringBuilder7.Append("  (SO_CODE   , SHIP_ORDER_CODE,  WHOLE_COUNT,  BULK_COUNT  , BAG_COUNT,");
                                        stringBuilder7.Append("   COOL_COUNT, SPEC_COUNT     ,  TCM_COUNT  ,  TOTAL_COUNT , CR_DATE  , PACK_BY_NAME ");
                                        stringBuilder7.Append("  )").Append("\r\n");
                                        stringBuilder7.Append("SELECT");
                                        stringBuilder7.Append(" '").Append(socClass.SoCode.ToString()).Append("'      SO_CODE");
                                        stringBuilder7.Append(",'").Append(shipOrder.ShipOrderCode.ToString()).Append("'      SHIP_ORDER_CODE");
                                        stringBuilder7.Append(",'").Append(shipOrder.WholeCount.ToString()).Append("'      WHOLE_COUNT");
                                        stringBuilder7.Append(",'").Append(shipOrder.BulkCount.ToString()).Append("'      BULK_COUNT");
                                        stringBuilder7.Append(",'").Append(shipOrder.BagCount.ToString()).Append("'      BAG_COUNT");
                                        stringBuilder7.Append(",'").Append(shipOrder.SpecCount.ToString()).Append("'      SPEC_COUNT");
                                        stringBuilder7.Append(",'").Append(shipOrder.CoolCount.ToString()).Append("'      COOL_COUNT");
                                        stringBuilder7.Append(",'").Append(shipOrder.TcmCount.ToString()).Append("'      TCM_COUNT");
                                        stringBuilder7.Append(",'").Append(shipOrder.TotaclCount.ToString()).Append("'      TOTAL_COUNT");
                                        stringBuilder7.Append(",'").Append(shipOrder.CrDate.ToString()).Append("'      CR_DATE ");
                                        stringBuilder7.Append(",'").Append(shipOrder.PackByName.ToString()).Append("'      PACK_BY_NAME ");
                                        list.Add(new SqlItem(stringBuilder.ToString(), -1));
                                    }
                                }
                                sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlBatchWithTran(list);
                                if (!sqlItemResultClass.SqlResult)
                                {
                                    result = new ResultMessage(false, sqlItemResultClass.ErrorMessage.ToString(), sqlItemResultClass.ErrorDescript.ToString());
                                    break;
                                }
                                if (((socClass.ShipOrderList != null) & (socClass.ShipOrderList.Count > 0)) && socClass.ShipOrderCount != Common.MsSqlDB.GetIntegerBySql("SELECT COUNT(*) ITEM_COUNT FROM WMS_SWAP_OC_SHIP_ORDER I WHERE I.SO_CODE = '" + socClass.SoCode.ToString() + "'"))
                                {
                                    result = new ResultMessage(false, "提交的装箱行数不一致!", "提交的装箱行数不一致!");
                                    break;
                                }
                                List<DbParameter> list2 = new List<DbParameter>();
                                SqlParameter sqlParameter = new SqlParameter("@SoCode", SqlDbType.NVarChar, 40);
                                sqlParameter.Value = socClass.SoCode;
                                list2.Add(sqlParameter);
                                sqlParameter = new SqlParameter("@WmsId", SqlDbType.NVarChar, 40);
                                sqlParameter.Value = socClass.WmsId.ToString();
                                list2.Add(sqlParameter);
                                SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
                                sqlParameter2.Direction = ParameterDirection.Output;
                                sqlParameter2.Value = String.Empty;
                                list2.Add(sqlParameter2);
                                try
                                {
                                    sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("P_WMS_TO_OC", list2);
                                }
                                catch (Exception ex)
                                {
                                    Exception ex2 = ex;
                                    result = new ResultMessage(false, "执行数据转换出错,", ex2.Message.ToString());
                                    return result;
                                }
                                if (!sqlItemResultClass.SqlResult)
                                {
                                    result = new ResultMessage(false, "执行数据转换失败,", sqlItemResultClass.ErrorMessage.ToString());
                                    break;
                                }
                                if (sqlParameter2.Value.ToString() != "Success")
                                {
                                    result = new ResultMessage(false, "执行数据转换失败", sqlParameter2.Value.ToString());
                                    break;
                                }
                            }
                            num4++;
                        }
                        break;
                    }
                }
                return result;
            }
        }

        public static ResultMessage OctCheckFinished(string paramBillNo)
        {
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("@BillNo", SqlDbType.NVarChar, 40);
            sqlParameter.Value = paramBillNo;
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            ResultMessage result;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_SellCheckCompleted", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                result = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result;
            }
            if (!sqlItemResultClass.SqlResult)
            {
                result = new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString() + sqlItemResultClass.ErrorMessage.ToString());
            }
            else if (sqlParameter2.Value.ToString() != "Success")
            {
                result = new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString());
            }
            else
            {
                SetYbxOctCheckFinishedFlag(paramBillNo);
                result = new ResultMessage(true, String.Empty, String.Empty);
            }
            return result;
        }

        public static ResultMessage SaveOctCheckEcode(WhWorkItemEcode paramWorkItemEcode)
        {
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("Operator", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWorkItemEcode.WorkByCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("DepotID", SqlDbType.Int);
            sqlParameter.Value = paramWorkItemEcode.SourWhAreaCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BillNo", SqlDbType.NVarChar, 20);
            sqlParameter.Value = paramWorkItemEcode.SourTransCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("ItemNo", SqlDbType.Int);
            sqlParameter.Value = paramWorkItemEcode.SourTransItemId.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("Code", SqlDbType.NVarChar, 20);
            sqlParameter.Value = paramWorkItemEcode.Ecode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("PackLevel", SqlDbType.Int);
            switch (paramWorkItemEcode.EcodePackLevel)
            {
                case EnumEcodePackLevel.BASEPACK:
                    sqlParameter.Value = 1;
                    break;
                case EnumEcodePackLevel.GROUPPACK:
                    sqlParameter.Value = 2;
                    break;
                case EnumEcodePackLevel.BOXPACK:
                    sqlParameter.Value = 3;
                    break;
            }
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_StockOutEWCode", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                ResultMessage result = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result;
            }
            return sqlItemResultClass.SqlResult ? (sqlParameter2.Value.ToString() == "Success" ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString())) : new ResultMessage(false, String.Empty, sqlItemResultClass.ErrorMessage.ToString());
        }

        public static ResultMessage SaveReceivedEcode(WhWorkItemEcode paramWorkItemEcode)
        {
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("Operator", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWorkItemEcode.WorkByCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("BillNo", SqlDbType.NVarChar, 20);
            sqlParameter.Value = paramWorkItemEcode.SourTransCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("ItemNo", SqlDbType.Int);
            sqlParameter.Value = paramWorkItemEcode.SourTransItemId.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("Code", SqlDbType.NVarChar, 20);
            sqlParameter.Value = paramWorkItemEcode.Ecode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("PackLevel", SqlDbType.Int);
            switch (paramWorkItemEcode.EcodePackLevel)
            {
                case EnumEcodePackLevel.BASEPACK:
                    sqlParameter.Value = 1;
                    break;
                case EnumEcodePackLevel.GROUPPACK:
                    sqlParameter.Value = 2;
                    break;
                case EnumEcodePackLevel.BOXPACK:
                    sqlParameter.Value = 3;
                    break;
            }
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_StockInEWCode", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                ResultMessage result = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result;
            }
            return sqlItemResultClass.SqlResult ? (sqlParameter2.Value.ToString() == "Success" ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString())) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString() + "\r\n" + sqlItemResultClass.ErrorMessage.ToString());
        }

        public static ResultMessage DoSubmitRglToErp(RglClass paramRgl)
        {
            ResultMessage result;
            if (paramRgl.ItemCount != paramRgl.ItemList.Count)
            {
                result = new ResultMessage(false, "提交的数据行数不一致!", "提交的数据行数不一致!");
            }
            else
            {
                List<SqlItem> list = new List<SqlItem>();
                StringBuilder stringBuilder = new StringBuilder();
                new StringBuilder();
                stringBuilder.Clear();
                StringBuilder stringBuilder2 = stringBuilder;
                stringBuilder2.Append("IF NOT EXISTS(SELECT * FROM WMS_SWAP_RGL WHERE PO_CODE = '").Append(paramRgl.PoCode).Append("')")
                    .Append("\r\n");
                stringBuilder2.Append("INSERT INTO WMS_SWAP_RGL").Append("\r\n");
                stringBuilder2.Append("  (PO_CODE,     WORK_BY_CODE,       WORK_TIME,     WH_ID,           ITEM_COUNT,").Append("\r\n");
                stringBuilder2.Append("   CR_DATE,     LM_DATE,            STATUS_ID,     WMSID,           N_1,").Append("\r\n");
                stringBuilder2.Append("   N_2,         N_3,                N_4,           SHIP_CODE").Append("\r\n");
                stringBuilder2.Append("  )").Append("\r\n");
                stringBuilder2.Append("SELECT");
                stringBuilder2.Append("    '").Append(paramRgl.PoCode).Append("'     PO_CODE");
                stringBuilder2.Append("   ,'").Append(paramRgl.WorkByCode).Append("'     WORK_BY_CODE");
                stringBuilder2.Append("   ,'").Append(paramRgl.WorkTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     WORK_TIME");
                stringBuilder2.Append("   ,'").Append(paramRgl.WhId.ToString()).Append("'     WH_ID");
                stringBuilder2.Append("   ,'").Append(paramRgl.ItemCount.ToString()).Append("'     ITEM_COUNT")
                    .Append("\r\n");
                stringBuilder2.Append("   ,'").Append(paramRgl.CrDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     CR_DATE");
                stringBuilder2.Append("   ,'").Append(paramRgl.LmDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     LM_DATE");
                stringBuilder2.Append("   ,'").Append(paramRgl.StatusId.ToString()).Append("'     STATUS_ID");
                stringBuilder2.Append("   ,'").Append(paramRgl.WmsId.ToString()).Append("'     WMSID");
                stringBuilder2.Append("   ,'").Append(paramRgl.N_1.ToString()).Append("'     N_1")
                    .Append("\r\n");
                stringBuilder2.Append("   ,'").Append(paramRgl.N_2.ToString()).Append("'     N_2");
                stringBuilder2.Append("   ,'").Append(paramRgl.N_3.ToString()).Append("'     N_3");
                stringBuilder2.Append("   ,'").Append(paramRgl.N_4.ToString()).Append("'     N_4");
                stringBuilder2.Append("   ,'").Append(paramRgl.ShipCode.ToString()).Append("' SHIP_CODE ");
                list.Add(new SqlItem(stringBuilder.ToString(), -1));
                foreach (RglItemClass item in paramRgl.ItemList)
                {
                    StringBuilder stringBuilder3 = stringBuilder;
                    stringBuilder3.Clear();
                    stringBuilder3.Append("IF NOT EXISTS (SELECT * FROM WMS_SWAP_RGL_ITEM P WHERE P.WMSID = '").Append(item.WmsId).Append("' AND P.PO_CODE = '")
                        .Append(paramRgl.PoCode.ToString())
                        .Append("')")
                        .Append("\r\n");
                    stringBuilder3.Append("INSERT INTO WMS_SWAP_RGL_ITEM");
                    stringBuilder3.Append("  (WMS_SWAP_RGL_ID,     PO_ITEM_ID,       PO_CODE,      PO_ITEM_INDEX,      GOODS_CODE,      BATCH_NO,");
                    stringBuilder3.Append("   EXP_DATE,            MANU_DATE,        UNIT_CODE,    QTY,                UNIT_QTY,        WORK_BY_CODE,");
                    stringBuilder3.Append("   WORK_TIME,           WH_ID,            WMSID,        CR_DATE,            N_1,             N_2,");
                    stringBuilder3.Append("   N_3,                 N_4,              ERP_G_ID,     FROM_LOC_CODE,     TO_LOC_CODE,      UNIT_PRICE,    CHECK_QTY,    REJECT_QTY,    REJECT_REASON");
                    stringBuilder3.Append("  )").Append("\r\n");
                    stringBuilder3.Append("SELECT");
                    stringBuilder3.Append("(SELECT H.WMS_SWAP_RGL_ID FROM WMS_SWAP_RGL H WHERE H.PO_CODE = '").Append(paramRgl.PoCode).Append("')")
                        .Append("\r\n");
                    stringBuilder3.Append(",'").Append(item.PoItemId.ToString()).Append("'      PO_ITEM_ID");
                    stringBuilder3.Append(",'").Append(paramRgl.PoCode.ToString()).Append("'      PO_CODE");
                    stringBuilder3.Append(",'").Append(item.PoItemIndex.ToString()).Append("'      PO_ITEM_INDEX");
                    stringBuilder3.Append(",'").Append(item.GCode.ToString()).Append("'      GOODS_CODE");
                    stringBuilder3.Append(",'").Append(item.BatchNo.ToString()).Append("'      BATCH_NO");

                    DateTime tmpDt;
                    if (!string.IsNullOrEmpty(item.ExpDate) && DateTime.TryParse(item.ExpDate, out tmpDt))
                    {
                        stringBuilder3.Append(",'").Append(item.ExpDate).Append("'      EXP_DATE");
                    }
                    else
                    {
                        stringBuilder3.Append(",NULL      EXP_DATE");
                    }
                    if (!string.IsNullOrEmpty(item.ManuDate) && DateTime.TryParse(item.ManuDate, out tmpDt))
                    {
                        stringBuilder3.Append(",'").Append(item.ManuDate).Append("'      MANU_DATE");
                    }
                    else
                    {
                        stringBuilder3.Append(",NULL      MANU_DATE");
                    }
                    stringBuilder3.Append(",'").Append(item.GpCode.ToString()).Append("'      UNIT_CODE");
                    stringBuilder3.Append(",'").Append(item.Qty.ToString()).Append("'      QTY");
                    stringBuilder3.Append(",'").Append(item.GpUnitQty.ToString()).Append("'      UNIT_QTY");
                    stringBuilder3.Append(",'").Append(item.WorkByCode.ToString()).Append("'      WORK_BY_CODE");
                    stringBuilder3.Append(",'").Append(item.WorkTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("'      WORK_TIME");
                    stringBuilder3.Append(",'").Append(item.WhId).Append("'      WH_ID");
                    stringBuilder3.Append(",'").Append(item.WmsId).Append("'      WMSID");
                    stringBuilder3.Append(",'").Append(item.CrDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'      CR_DATE");
                    stringBuilder3.Append(",'").Append(item.N_1.ToString()).Append("'      N_1");
                    stringBuilder3.Append(",'").Append(item.N_2.ToString()).Append("'      N_2");
                    stringBuilder3.Append(",'").Append(item.N_3.ToString()).Append("'      N_3");
                    stringBuilder3.Append(",'").Append(item.N_4.ToString()).Append("'      N_4");
                    if (item.ErpGoodsId == null)
                    {
                        stringBuilder3.Append(",''      ERP_G_ID").Append("\r\n");
                    }
                    else
                    {
                        stringBuilder3.Append(",'").Append(item.ErpGoodsId.ToString()).Append("'      ERP_G_ID");
                    }
                    if (item.FromLocCode == null)
                    {
                        stringBuilder3.Append(",''      FROM_LOC_CODE");
                    }
                    else
                    {
                        stringBuilder3.Append(",'").Append(item.FromLocCode.ToString()).Append("'      FROM_LOC_CODE");
                    }
                    if (item.ToLocCode == null)
                    {
                        stringBuilder3.Append(",''      TO_LOC_CODE");
                    }
                    else
                    {
                        stringBuilder3.Append(",'").Append(item.ToLocCode.ToString()).Append("'      TO_LOC_CODE");
                    }
                    stringBuilder3.Append(",'").Append(item.UnitPrice.ToString()).Append("'      UNIT_PRICE");
                    stringBuilder3.Append(",'").Append(item.CheckQty.ToString()).Append("'      CHECK_QTY");
                    stringBuilder3.Append(",'").Append(item.RejectQty.ToString()).Append("'      REJECT_QTY");
                    stringBuilder3.Append(",'").Append(item.RejectReason.ToString()).Append("'      REJECT_REASON");
                    stringBuilder3 = null;
                    list.Add(new SqlItem(stringBuilder.ToString(), -1));
                }
                try
                {
                    SqlItemResult sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlBatchWithTran(list);
                    if (!sqlItemResultClass.SqlResult)
                    {
                        return new ResultMessage(false, sqlItemResultClass.ErrorMessage.ToString(), sqlItemResultClass.ErrorMessage.ToString() + ";" + sqlItemResultClass.ErrorDescript.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    result = new ResultMessage(false, "执行数据转换失败!", ex2.Message.ToString());
                    return result;
                }
                if (paramRgl.ItemCount != Common.MsSqlDB.GetIntegerBySql("SELECT COUNT(*) ITEM_COUNT FROM WMS_SWAP_RGL_ITEM I WHERE I.PO_CODE = '" + paramRgl.PoCode.ToString() + "'"))
                {
                    result = new ResultMessage(false, "提交的数据行数不一致!", "提交的数据行数不一致!");
                }
                else
                {
                    List<DbParameter> list2 = new List<DbParameter>();
                    SqlParameter sqlParameter = new SqlParameter("@PoCode", SqlDbType.NVarChar, 40);
                    sqlParameter.Value = paramRgl.PoCode;
                    list2.Add(sqlParameter);
                    sqlParameter = new SqlParameter("@WmsId", SqlDbType.NVarChar, 40);
                    sqlParameter.Value = paramRgl.WmsId.ToString();
                    list2.Add(sqlParameter);
                    SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
                    sqlParameter2.Direction = ParameterDirection.Output;
                    sqlParameter2.Value = String.Empty;
                    list2.Add(sqlParameter2);
                    SqlItemResult sqlItemResultClass;
                    try
                    {
                        sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("P_WMS_TO_RGL", list2);
                    }
                    catch (Exception ex3)
                    {
                        Exception ex4 = ex3;
                        result = new ResultMessage(false, "执行数据转换出错,", ex4.Message.ToString());
                        return result;
                    }
                    result = (sqlItemResultClass.SqlResult ? (sqlParameter2.Value.ToString() == "Success" ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, "执行数据转换失败;" + sqlParameter2.Value.ToString(), sqlParameter2.Value.ToString() + sqlItemResultClass.ErrorDescript.ToString())) : new ResultMessage(false, "执行数据转换时未收到成功标识,", sqlParameter2.Value.ToString() + sqlItemResultClass.ErrorMessage.ToString() + sqlItemResultClass.ErrorDescript.ToString()));
                }
            }
            return result;
        }

        public static ResultMessage SaveReceiveItemListWork(List<WhWorkItem> paramWhWorkItemList)
        {
            foreach (WhWorkItem paramWhWorkItem in paramWhWorkItemList)
            {
                ResultMessage result = SaveReceiveWork(paramWhWorkItem);
                if (!result.Result)
                {
                    return result;
                }
            }
            List<DbParameter> list = new List<DbParameter>();
            SqlParameter sqlParameter = new SqlParameter("Operator", SqlDbType.NVarChar, 8);
            sqlParameter.Value = paramWhWorkItemList[0].WorkByCode.ToString();
            list.Add(sqlParameter);
            sqlParameter = new SqlParameter("WMS_ID", SqlDbType.Int);
            sqlParameter.Value = paramWhWorkItemList[0].WhWorkId.ToString();
            list.Add(sqlParameter);
            SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
            sqlParameter2.Direction = ParameterDirection.Output;
            sqlParameter2.Value = String.Empty;
            list.Add(sqlParameter2);
            SqlItemResult sqlItemResultClass;
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("proc_JOOBYTE_WMS_PchCheckInConfirm", list);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                ResultMessage result2 = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result2;
            }
            return sqlItemResultClass.SqlResult ? (sqlParameter2.Value.ToString() == "Success") ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString()) : new ResultMessage(false, String.Empty, sqlParameter2.Value.ToString() + "\r\n" + sqlItemResultClass.ErrorMessage.ToString());
        }

        public static ResultMessage DoSubmitSrToErp(SwapSrClass paramSwapSr)
        {
            ResultMessage result;
            if (paramSwapSr.ItemCount != paramSwapSr.ItemList.Count)
            {
                result = new ResultMessage(false, "提交的数据行数不一致!", "提交的数据行数不一致!");
            }
            else if (Common.MsSqlDB.GetIntegerBySql("SELECT H.STATUS_ID FROM WMS_SWAP_SR H WHERE H.SR_CODE = '" + paramSwapSr.SrCode + "'") == 1)
            {
                result = new ResultMessage(true, String.Empty, String.Empty);
            }
            else
            {
                List<SqlItem> list = new List<SqlItem>();
                StringBuilder stringBuilder = new StringBuilder();
                new StringBuilder();
                stringBuilder.Clear();
                StringBuilder stringBuilder2 = stringBuilder;
                try
                {
                    stringBuilder2.Append("IF NOT EXISTS(SELECT * FROM WMS_SWAP_Sr WHERE SR_CODE = '").Append(paramSwapSr.SrCode).Append("')")
                        .Append("\r\n");
                    stringBuilder2.Append("INSERT INTO WMS_SWAP_SR").Append("\r\n");
                    stringBuilder2.Append("  (SR_CODE,     WORK_BY_CODE,       WORK_TIME,     WH_ID,           ITEM_COUNT,").Append("\r\n");
                    stringBuilder2.Append("   CR_DATE,     LM_DATE,            STATUS_ID,     WMSID,           N_1,").Append("\r\n");
                    stringBuilder2.Append("   N_2,         N_3,                N_4,           SHIP_CODE").Append("\r\n");
                    stringBuilder2.Append("  )").Append("\r\n");
                    stringBuilder2.Append("SELECT").Append("\r\n");
                    stringBuilder2.Append("    '").Append(paramSwapSr.SrCode).Append("'     SR_CODE")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.WorkByCode).Append("'     WORK_BY_CODE")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.WorkTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     WORK_TIME")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.WhId.ToString()).Append("'     WH_ID")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.ItemCount.ToString()).Append("'     ITEM_COUNT")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.CrDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     CR_DATE")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.LmDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'     LM_DATE")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.StatusId.ToString()).Append("'     STATUS_ID")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.WmsId.ToString()).Append("'     WMSID")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.N_1.ToString()).Append("'     N_1")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.N_2.ToString()).Append("'     N_2")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.N_3.ToString()).Append("'     N_3")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.N_4.ToString()).Append("'     N_4")
                        .Append("\r\n");
                    stringBuilder2.Append("   ,'").Append(paramSwapSr.ShipCode.ToString()).Append("' SHIP_CODE ")
                        .Append("\r\n");
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    result = new ResultMessage(false, "提交错误", ex2.Message.ToString());
                    return result;
                }
                stringBuilder2 = null;
                list.Add(new SqlItem(stringBuilder.ToString(), -1));
                foreach (SwapSrItemClass item in paramSwapSr.ItemList)
                {
                    StringBuilder stringBuilder3 = stringBuilder;
                    stringBuilder3.Clear();
                    stringBuilder3.Append("IF NOT EXISTS (SELECT * FROM WMS_SWAP_SR_ITEM P WHERE P.WMSID = '").Append(item.WmsId).Append("' AND P.SR_CODE = '")
                        .Append(paramSwapSr.SrCode.ToString())
                        .Append("')")
                        .Append("\r\n");
                    stringBuilder3.Append("INSERT INTO WMS_SWAP_SR_ITEM");
                    stringBuilder3.Append("  (WMS_SWAP_SR_ID,     SR_ITEM_ID,       SR_CODE,      SR_ITEM_INDEX,      GOODS_CODE,      BATCH_NO,");
                    stringBuilder3.Append("   EXP_DATE,            MANU_DATE,        UNIT_CODE,    QTY,                UNIT_QTY,        WORK_BY_CODE,");
                    stringBuilder3.Append("   WORK_TIME,           WH_ID,            WMSID,        CR_DATE,            N_1,             N_2,");
                    stringBuilder3.Append("   N_3,                 N_4,              ERP_G_ID,     FROM_LOC_CODE,     TO_LOC_CODE,      UNIT_PRICE,    CHECK_QTY,    REJECT_QTY,    REJECT_REASON");
                    stringBuilder3.Append("  )").Append("\r\n");
                    stringBuilder3.Append("SELECT");
                    stringBuilder3.Append("(SELECT H.WMS_SWAP_SR_ID FROM WMS_SWAP_SR H WHERE H.SR_CODE = '").Append(paramSwapSr.SrCode).Append("')")
                        .Append("\r\n");
                    stringBuilder3.Append(",'").Append(item.SrItemId.ToString()).Append("'      SR_ITEM_ID");
                    stringBuilder3.Append(",'").Append(paramSwapSr.SrCode.ToString()).Append("'      SR_CODE");
                    stringBuilder3.Append(",'").Append(item.SrItemIndex.ToString()).Append("'      SR_ITEM_INDEX");
                    stringBuilder3.Append(",'").Append(item.GCode.ToString()).Append("'      GOODS_CODE");
                    stringBuilder3.Append(",'").Append(item.BatchNo.ToString()).Append("'      BATCH_NO");

                    DateTime tmpDt;
                    if (!string.IsNullOrEmpty(item.ExpDate) && DateTime.TryParse(item.ExpDate, out tmpDt))
                    {
                        stringBuilder3.Append(",'").Append(item.ExpDate).Append("'      EXP_DATE");
                    }
                    else
                    {
                        stringBuilder3.Append(",NULL      EXP_DATE");
                    }

                    if (!string.IsNullOrEmpty(item.ManuDate) && DateTime.TryParse(item.ManuDate, out tmpDt))
                    {
                        stringBuilder3.Append(",'").Append(item.ManuDate).Append("'      MANU_DATE");
                    }
                    else
                    {
                        stringBuilder3.Append(",NULL      MANU_DATE");
                    }
                    stringBuilder3.Append(",'").Append(item.GpCode.ToString()).Append("'      UNIT_CODE");
                    stringBuilder3.Append(",'").Append(item.Qty.ToString()).Append("'      QTY");
                    stringBuilder3.Append(",'").Append(item.GpUnitQty.ToString()).Append("'      UNIT_QTY");
                    stringBuilder3.Append(",'").Append(item.WorkByCode.ToString()).Append("'      WORK_BY_CODE");
                    stringBuilder3.Append(",'").Append(item.WorkTime.ToString("yyyy-MM-dd HH:mm:ss")).Append("'      WORK_TIME");
                    stringBuilder3.Append(",'").Append(item.WhId).Append("'      WH_ID");
                    stringBuilder3.Append(",'").Append(item.WmsId).Append("'      WMSID");
                    stringBuilder3.Append(",'").Append(item.CrDate.ToString("yyyy-MM-dd HH:mm:ss")).Append("'      CR_DATE");
                    stringBuilder3.Append(",'").Append(item.N_1.ToString()).Append("'      N_1");
                    stringBuilder3.Append(",'").Append(item.N_2.ToString()).Append("'      N_2");
                    stringBuilder3.Append(",'").Append(item.N_3.ToString()).Append("'      N_3");
                    stringBuilder3.Append(",'").Append(item.N_4.ToString()).Append("'      N_4");
                    if (item.ErpGoodsId == null)
                    {
                        stringBuilder3.Append(",''      ERP_G_ID").Append("\r\n");
                    }
                    else
                    {
                        stringBuilder3.Append(",'").Append(item.ErpGoodsId.ToString()).Append("'      ERP_G_ID");
                    }
                    if (item.FromLocCode == null)
                    {
                        stringBuilder3.Append(",''      FROM_LOC_CODE");
                    }
                    else
                    {
                        stringBuilder3.Append(",'").Append(item.FromLocCode.ToString()).Append("'      FROM_LOC_CODE");
                    }
                    if (item.ToLocCode == null)
                    {
                        stringBuilder3.Append(",''      TO_LOC_CODE");
                    }
                    else
                    {
                        stringBuilder3.Append(",'").Append(item.ToLocCode.ToString()).Append("'  TO_LOC_CODE");
                    }
                    stringBuilder3.Append(",'").Append(item.UnitPrice.ToString()).Append("'      UNIT_PRICE");
                    stringBuilder3.Append(",'").Append(item.CheckQty.ToString()).Append("'       CHECK_QTY");
                    stringBuilder3.Append(",'").Append(item.RejectQty.ToString()).Append("'      REJECT_QTY");
                    stringBuilder3.Append(",'").Append(item.RejectReason.ToString()).Append("'   REJECT_REASON");
                    stringBuilder3 = null;
                    list.Add(new SqlItem(stringBuilder.ToString(), -1));
                }
                SqlItemResult sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlBatchWithTran(list);
                if (!sqlItemResultClass.SqlResult)
                {
                    result = new ResultMessage(false, sqlItemResultClass.ErrorMessage.ToString(), sqlItemResultClass.ErrorMessage.ToString() + ";" + sqlItemResultClass.ErrorDescript.ToString());
                }
                else if (paramSwapSr.ItemCount != Common.MsSqlDB.GetIntegerBySql("SELECT COUNT(*) ITEM_COUNT FROM WMS_SWAP_SR_ITEM I WHERE I.SR_CODE = '" + paramSwapSr.SrCode.ToString() + "'"))
                {
                    result = new ResultMessage(false, "提交的数据行数不一致!", "提交的数据行数不一致!");
                }
                else
                {
                    List<DbParameter> list2 = new List<DbParameter>();
                    SqlParameter sqlParameter = new SqlParameter("@SrCode", SqlDbType.NVarChar, 40);
                    sqlParameter.Value = paramSwapSr.SrCode;
                    list2.Add(sqlParameter);
                    sqlParameter = new SqlParameter("@WmsId", SqlDbType.NVarChar, 40);
                    sqlParameter.Value = paramSwapSr.WmsId.ToString();
                    list2.Add(sqlParameter);
                    SqlParameter sqlParameter2 = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200);
                    sqlParameter2.Direction = ParameterDirection.Output;
                    sqlParameter2.Value = String.Empty;
                    list2.Add(sqlParameter2);
                    try
                    {
                        sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("P_WMS_TO_SR", list2);
                    }
                    catch (Exception ex3)
                    {
                        Exception ex4 = ex3;
                        result = new ResultMessage(false, "执行数据转换出错,", ex4.Message.ToString());
                        return result;
                    }
                    result = (sqlItemResultClass.SqlResult ? (sqlParameter2.Value.ToString() == "Success" ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, "执行数据转换失败;" + sqlParameter2.Value.ToString(), sqlParameter2.Value.ToString() + sqlItemResultClass.ErrorDescript.ToString())) : new ResultMessage(false, "执行数据转换时未收到成功标识,", sqlParameter2.Value.ToString() + sqlItemResultClass.ErrorMessage.ToString() + sqlItemResultClass.ErrorDescript.ToString()));
                }
            }
            return result;
        }




        public static ResultMessage SetYbxOctCheckFinishedWithReturnFlag(string paramBillCode)
        {
            SqlItemResult sqlItemResultClass = new SqlItemResult();
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlWithTran(new SqlItem("UPDATE SelExport SET Remark = '有退单,' + isnull(Remark,'') WHERE BillNo = '" + paramBillCode + "'", -1));
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                ResultMessage result = new ResultMessage(false, "更新ERP复核完成标识出错", ex2.Message.ToString());
                return result;
            }
            return (!sqlItemResultClass.SqlResult) ? new ResultMessage(false, "更新ERP复核完成标识失败", sqlItemResultClass.ErrorMessage.ToString() + sqlItemResultClass.ErrorDescript.ToString()) : new ResultMessage(true, String.Empty, String.Empty);
        }


        public static ResultMessage ReceivedFinished(string paramBillNo)
        {
            string text = String.Empty;
            SqlItemResult sqlItemResultClass;
            try
            {
                text = paramBillNo.Substring(0, 2);
                if (text == "IN")
                {
                    sqlItemResultClass = (paramBillNo.Substring(0, 5) != "IN_SR" ?
                        Common.MsSqlDB.RunUpdateSqlWithTran(new SqlItem("UPDATE PchReceive SET Remark = 'WMS上架完成，'+ ISNULL(Remark,'') where CHARINDEX('WMS上架完成',ISNULL(Remark,'')) = 0 AND BillNo =  '" + paramBillNo.ToString() + "'", -1))
                        : Common.MsSqlDB.RunUpdateSqlWithTran(new SqlItem("UPDATE SelExport SET Remark = 'WMS上架完成，'+ ISNULL(Remark,'') where CHARINDEX('WMS上架完成',ISNULL(Remark,'')) = 0 AND BillNo = '" + paramBillNo.ToString().Substring(3, checked(paramBillNo.Length - 3)) + "'", -1)));
                }
                else
                {
                    if (text != "SR")
                    {
                        return new ResultMessage(false, String.Empty, "单据类型无效,");
                    }
                    sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlWithTran(new SqlItem("UPDATE SelExport SET Remark = 'WMS上架完成，'+ ISNULL(Remark,'') where CHARINDEX('WMS上架完成',ISNULL(Remark,'')) = 0 AND BillNo =  '" + paramBillNo.ToString() + "'", -1));
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                ResultMessage result = new ResultMessage(false, String.Empty, ex2.Message.ToString());
                return result;
            }
            return sqlItemResultClass.SqlResult ? new ResultMessage(true, String.Empty, String.Empty) : new ResultMessage(false, String.Empty, "更新ERP单据备注出错," + sqlItemResultClass.ErrorMessage.ToString());
        }


        public static ResultMessage SetYbxOctCheckFinishedFlag(string paramBillCode)
        {
            SqlItemResult sqlItemResultClass = new SqlItemResult();
            try
            {
                sqlItemResultClass = Common.MsSqlDB.RunUpdateSqlWithTran(new SqlItem("UPDATE SelExport SET Remark = 'FFF,' + isnull(Remark,'') WHERE BillNo = '" + paramBillCode + "'", -1));
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                ResultMessage result = new ResultMessage(false, "更新ERP复核完成标识出错", ex2.Message.ToString());
                return result;
            }
            return (!sqlItemResultClass.SqlResult) ? new ResultMessage(false, "更新ERP复核完成标识失败", sqlItemResultClass.ErrorMessage.ToString() + sqlItemResultClass.ErrorDescript.ToString()) : new ResultMessage(true, String.Empty, String.Empty);
        }
    }
}