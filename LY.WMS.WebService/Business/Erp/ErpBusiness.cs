using LY.WMS.Framework.DataBase;
using LY.WMS.WebService.Models.Erp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Business.Erp
{
    /// <summary>
    /// ERP相关业务
    /// </summary>
    public class ErpBusiness
    {
        /// <summary>
        /// 上架作业ERP回传【一般模式】
        /// </summary>
        /// <param name="paramRgl"></param>
        /// <returns></returns>
        public static ResultMessage SubmitRglToErp(RglClass paramRgl)
        {
            ResultMessage result;
            if (paramRgl.ItemCount != paramRgl.ItemList.Count)
            {
                result = new ResultMessage(false, "提交的数据行数不一致!", "提交的数据行数不一致!");
            }
            else
            {
                List<SqlItem> list = new List<SqlItem>();
                string sql = string.Format(@"   IF NOT EXISTS(SELECT * FROM WMS_SWAP_RGL WHERE PO_CODE = '{0}'
                                                INSERT INTO WMS_SWAP_RGL
                                                    (PO_CODE, WORK_BY_CODE, WORK_TIME, WH_ID, ITEM_COUNT, CR_DATE, LM_DATE,
                                                     STATUS_ID, WMSID, N_1, N_2, N_3, N_4, SHIP_CODE)
                                                SELECT
                                                    '{1}' PO_CODE,
                                                    '{2}' WORK_BY_CODE,
                                                    '{3}' WORK_TIME,
                                                    '{4}' WH_ID,
                                                    '{5}' ITEM_COUNT,
                                                    '{6}' CR_DATE,
                                                    '{7}' LM_DATE,
                                                    '{8}' STATUS_ID,
                                                    '{9}' WMSID,
                                                    '{10}' N_1,
                                                    '{11}' N_2,
                                                    '{12}' N_3,
                                                    '{13}' N_4,
                                                    '{14}' SHIP_CODE",
                    paramRgl.PoCode,
                    paramRgl.PoCode, (paramRgl.WorkByCode), paramRgl.WorkTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    paramRgl.WhId.ToString(), paramRgl.ItemCount.ToString(), paramRgl.CrDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    paramRgl.LmDate.ToString("yyyy-MM-dd HH:mm:ss"), paramRgl.StatusId.ToString(), paramRgl.WmsId.ToString(),
                    paramRgl.N_1.ToString(), paramRgl.N_2.ToString(), paramRgl.N_3.ToString(), paramRgl.N_4.ToString(),
                    paramRgl.ShipCode.ToString());
                list.Add(new SqlItem(sql, -1));

                foreach (RglItemClass item in paramRgl.ItemList)
                {
                    string itemSql = string.Format(@"   IF NOT EXISTS (SELECT * FROM WMS_SWAP_RGL_ITEM P WHERE P.WMSID = '{0}' AND P.PO_CODE = '{1}')
                                                        INSERT INTO WMS_SWAP_RGL_ITEM
                                                            (WMS_SWAP_RGL_ID, PO_ITEM_ID, PO_CODE, PO_ITEM_INDEX, GOODS_CODE, BATCH_NO,
                                                            EXP_DATE, MANU_DATE, UNIT_CODE, QTY, UNIT_QTY, WORK_BY_CODE,
                                                            WORK_TIME, WH_ID, WMSID, CR_DATE, N_1, N_2,
                                                            N_3, N_4, ERP_G_ID, FROM_LOC_CODE, TO_LOC_CODE, UNIT_PRICE, CHECK_QTY, REJECT_QTY, REJECT_REASON)
                                                        SELECT
                                                            (SELECT H.WMS_SWAP_RGL_ID FROM WMS_SWAP_RGL H WHERE H.PO_CODE = '{2}',
                                                            '{3}' PO_ITEM_ID,
                                                            '{4}' PO_CODE,
                                                            '{5}' PO_ITEM_INDEX,
                                                            '{6}' GOODS_CODE,
                                                            '{7}' BATCH_NO,
                                                            {8} EXP_DATE,
                                                            {9} MANU_DATE,
                                                            '{10}' UNIT_CODE,
                                                            '{11}' QTY,
                                                            '{12}' UNIT_QTY,
                                                            '{13}' WORK_BY_CODE,
                                                            '{14}' WORK_TIME,
                                                            '{15}' WH_ID,
                                                            '{16}' WMSID,
                                                            '{17}' CR_DATE,
                                                            '{18}' N_1,
                                                            '{19}' N_2,
                                                            '{20}' N_3,
                                                            '{21}' N_4,
                                                            '{22}' ERP_G_ID,
                                                            '{23}' FROM_LOC_CODE,
                                                            '{24}' TO_LOC_CODE,
                                                            '{25}' UNIT_PRICE,
                                                            '{26}' CHECK_QTY,
                                                            '{27}' REJECT_QTY,
                                                            '{28}' REJECT_REASON",
                        item.WmsId, paramRgl.PoCode, paramRgl.PoCode,
                        item.PoItemId.ToString(), paramRgl.PoCode.ToString(), item.PoItemIndex.ToString(), item.GCode.ToString(), item.BatchNo.ToString(),
                        !string.IsNullOrEmpty(item.ExpDate) && DateTime.TryParse(item.ExpDate, out DateTime tmpDt1) ? item.ExpDate : "NULL",
                        !string.IsNullOrEmpty(item.ManuDate) && DateTime.TryParse(item.ManuDate, out DateTime tmpDt2) ? item.ManuDate : "NULL",
                        item.GpCode.ToString(), item.Qty.ToString(), item.GpUnitQty.ToString(), item.WorkByCode.ToString(), item.WorkTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        item.WhId, item.WmsId, item.CrDate.ToString("yyyy-MM-dd HH:mm:ss"), item.N_1.ToString(), item.N_2.ToString(), item.N_3.ToString(), item.N_4.ToString(),
                        item.ErpGoodsId == null ? "" : item.ErpGoodsId.ToString(),
                        item.FromLocCode == null ? "" : item.FromLocCode.ToString(),
                        item.ToLocCode == null ? "" : item.ToLocCode.ToString(),
                        item.UnitPrice.ToString(), item.CheckQty.ToString(), item.RejectQty.ToString(), item.RejectReason.ToString());
                    list.Add(new SqlItem(itemSql, -1));
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
                    List<DbParameter> parameterList = new List<DbParameter>();

                    SqlParameter paramPoCode = new SqlParameter("@PoCode", SqlDbType.NVarChar, 40)
                    {
                        Value = paramRgl.PoCode
                    };
                    SqlParameter paramWmsId = new SqlParameter("@WmsId", SqlDbType.NVarChar, 40)
                    {
                        Value = paramRgl.WmsId.ToString()
                    };
                    SqlParameter paramResultMsg = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200)
                    {
                        Direction = ParameterDirection.Output,
                        Value = String.Empty
                    };

                    parameterList.Add(paramPoCode);
                    parameterList.Add(paramWmsId);
                    parameterList.Add(paramResultMsg);

                    SqlItemResult sqlItemResultClass;
                    try
                    {
                        sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("P_WMS_TO_RGL", parameterList);
                    }
                    catch (Exception ex)
                    {
                        return new ResultMessage(false, "执行数据转换出错,", ex.Message.ToString());
                    }
                    result = (sqlItemResultClass.SqlResult ? (
                        paramResultMsg.Value.ToString() == "Success" ? 
                            new ResultMessage(true, String.Empty, String.Empty) : 
                            new ResultMessage(false, "执行数据转换失败;" + paramResultMsg.Value.ToString(), paramResultMsg.Value.ToString() + sqlItemResultClass.ErrorDescript.ToString())) 
                        : new ResultMessage(false, "执行数据转换时未收到成功标识,", paramResultMsg.Value.ToString() + sqlItemResultClass.ErrorMessage.ToString() + sqlItemResultClass.ErrorDescript.ToString()));
                }
            }
            return result;
        }

        /// <summary>
        /// 上架作业ERP回传【销退模式】
        /// </summary>
        /// <param name="paramSwapSr"></param>
        /// <returns></returns>
        public static ResultMessage SubmitSrToErp(SwapSrClass paramSwapSr)
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
                string sql = "";
                try
                {
                    sql = string.Format(@"   IF NOT EXISTS(SELECT * FROM WMS_SWAP_Sr WHERE SR_CODE = '{0}'
                                                INSERT INTO WMS_SWAP_SR
                                                    (SR_CODE, WORK_BY_CODE, WORK_TIME, WH_ID, ITEM_COUNT,
                                                    CR_DATE, LM_DATE, STATUS_ID, WMSID, N_1, N_2, N_3, N_4, SHIP_CODE)
                                                SELECT
                                                    '{1}' SR_CODE,
                                                    '{2}' WORK_BY_CODE,
                                                    '{3}' WORK_TIME,
                                                    '{4}' WH_ID,
                                                    '{5}' ITEM_COUNT,
                                                    '{6}' CR_DATE,
                                                    '{7}' LM_DATE,
                                                    '{8}' STATUS_ID,
                                                    '{9}' WMSID,
                                                    '{10}' N_1,
                                                    '{11}' N_2,
                                                    '{12}' N_3,
                                                    '{13}' N_4,
                                                    '{14}' SHIP_CODE",
                    paramSwapSr.SrCode,
                    paramSwapSr.SrCode,
                    paramSwapSr.WorkByCode, paramSwapSr.WorkTime.ToString("yyyy-MM-dd HH:mm:ss"), paramSwapSr.WhId.ToString(),
                    paramSwapSr.ItemCount.ToString(),
                    paramSwapSr.CrDate.ToString("yyyy-MM-dd HH:mm:ss"), paramSwapSr.LmDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    paramSwapSr.StatusId.ToString(), paramSwapSr.WmsId.ToString(),
                    paramSwapSr.N_1.ToString(), paramSwapSr.N_2.ToString(), paramSwapSr.N_3.ToString(), paramSwapSr.N_4.ToString(), paramSwapSr.ShipCode.ToString());
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    return new ResultMessage(false, "提交错误", ex2.Message.ToString());
                }
                list.Add(new SqlItem(sql, -1));

                foreach (SwapSrItemClass item in paramSwapSr.ItemList)
                {
                    string itemSql = string.Format(@"   IF NOT EXISTS (SELECT * FROM WMS_SWAP_SR_ITEM P WHERE P.WMSID = '{0}' AND P.SR_CODE = '{1}')
                                                        INSERT INTO WMS_SWAP_SR_ITEM 
                                                            (WMS_SWAP_SR_ID,     SR_ITEM_ID,       SR_CODE,      SR_ITEM_INDEX,      GOODS_CODE,      BATCH_NO,
                                                            EXP_DATE,            MANU_DATE,        UNIT_CODE,    QTY,                UNIT_QTY,        WORK_BY_CODE,
                                                            WORK_TIME,           WH_ID,            WMSID,        CR_DATE,            N_1,             N_2,
                                                            N_3,                 N_4,              ERP_G_ID,     FROM_LOC_CODE,     TO_LOC_CODE,      UNIT_PRICE,    CHECK_QTY,    REJECT_QTY,    REJECT_REASON)
                                                        SELECT
                                                            (SELECT H.WMS_SWAP_SR_ID FROM WMS_SWAP_SR H WHERE H.SR_CODE = '{2}'),
                                                            '{3}' SR_ITEM_ID,
                                                            '{4}' SR_CODE,
                                                            '{5}' SR_ITEM_INDEX,
                                                            '{6}' GOODS_CODE,
                                                            '{7}' BATCH_NO,
                                                            {8} EXP_DATE,
                                                            {9} MANU_DATE,
                                                            '{10}' UNIT_CODE,
                                                            '{11}' QTY,
                                                            '{12}' UNIT_QTY,
                                                            '{13}' WORK_BY_CODE,
                                                            '{14}' WORK_TIME,
                                                            '{15}' WH_ID,
                                                            '{16}' WMSID,
                                                            '{17}' CR_DATE,
                                                            '{18}' N_1,
                                                            '{19}' N_2,
                                                            '{20}' N_3,
                                                            '{21}' N_4,
                                                            '{22}' ERP_G_ID,
                                                            '{23}' FROM_LOC_CODE,
                                                            '{24}' TO_LOC_CODE,
                                                            '{25}' UNIT_PRICE,
                                                            '{26}' CHECK_QTY,
                                                            '{27}' REJECT_QTY,
                                                            '{28}' REJECT_REASON",
                        item.WmsId, paramSwapSr.SrCode.ToString(), paramSwapSr.SrCode,
                        item.SrItemId.ToString(), paramSwapSr.SrCode.ToString(), item.SrItemIndex.ToString(), item.GCode.ToString(), item.BatchNo.ToString(),
                        !string.IsNullOrEmpty(item.ExpDate) && DateTime.TryParse(item.ExpDate, out DateTime tmpDt1) ? item.ExpDate : "NULL",
                        !string.IsNullOrEmpty(item.ManuDate) && DateTime.TryParse(item.ManuDate, out DateTime tmpDt2) ? item.ManuDate : "NULL",
                        item.GpCode.ToString(), item.Qty.ToString(), item.GpUnitQty.ToString(), item.WorkByCode.ToString(),
                        item.WorkTime.ToString("yyyy-MM-dd HH:mm:ss"), item.WhId, item.WmsId, item.CrDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        item.N_1.ToString(), item.N_2.ToString(), item.N_3.ToString(), item.N_4.ToString(),
                        item.ErpGoodsId == null ? "" : item.ErpGoodsId.ToString(),
                        item.FromLocCode == null ? "" : item.FromLocCode.ToString(),
                        item.ToLocCode == null ? "" : item.ToLocCode.ToString(),
                        item.UnitPrice.ToString(), item.CheckQty.ToString(), item.RejectQty.ToString(), item.RejectReason.ToString());
                    list.Add(new SqlItem(itemSql, -1));
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
                if (paramSwapSr.ItemCount != Common.MsSqlDB.GetIntegerBySql("SELECT COUNT(*) ITEM_COUNT FROM WMS_SWAP_RGL_ITEM I WHERE I.PO_CODE = '" + paramSwapSr.SrCode.ToString() + "'"))
                {
                    result = new ResultMessage(false, "提交的数据行数不一致!", "提交的数据行数不一致!");
                }
                else
                {
                    List<DbParameter> parameterList = new List<DbParameter>();

                    SqlParameter paramSrCode = new SqlParameter("@SrCode", SqlDbType.NVarChar, 40)
                    {
                        Value = paramSwapSr.SrCode
                    };
                    SqlParameter paramWmsId = new SqlParameter("@WmsId", SqlDbType.NVarChar, 40)
                    {
                        Value = paramSwapSr.WmsId.ToString()
                    };
                    SqlParameter paramResultMsg = new SqlParameter("@ResultMsg", SqlDbType.NVarChar, 200)
                    {
                        Direction = ParameterDirection.Output,
                        Value = String.Empty
                    };

                    parameterList.Add(paramSrCode);
                    parameterList.Add(paramWmsId);
                    parameterList.Add(paramResultMsg);

                    SqlItemResult sqlItemResultClass;
                    try
                    {
                        sqlItemResultClass = Common.MsSqlDB.RunProcParamWithTran("P_WMS_TO_SR", parameterList);
                    }
                    catch (Exception ex)
                    {
                        return new ResultMessage(false, "执行数据转换出错,", ex.Message.ToString());
                    }
                    result = (sqlItemResultClass.SqlResult ? (
                        paramResultMsg.Value.ToString() == "Success" ?
                            new ResultMessage(true, String.Empty, String.Empty) :
                            new ResultMessage(false, "执行数据转换失败;" + paramResultMsg.Value.ToString(), paramResultMsg.Value.ToString() + sqlItemResultClass.ErrorDescript.ToString()))
                        : new ResultMessage(false, "执行数据转换时未收到成功标识,", paramResultMsg.Value.ToString() + sqlItemResultClass.ErrorMessage.ToString() + sqlItemResultClass.ErrorDescript.ToString()));
                }
            }
            return result;
        }
    }
}