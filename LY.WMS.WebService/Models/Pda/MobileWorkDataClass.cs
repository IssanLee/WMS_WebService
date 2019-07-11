using System;
using System.Collections.Generic;

namespace LY.WMS.WebService.Models.Pda
{
    /// <summary>
    /// Pda作业数据
    /// </summary>
    public class MobileWorkDataClass
    {
        /// <summary>
        /// 作业类型
        /// </summary>
        public EnumWorkType WorkType { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 源货位ID
        /// </summary>
        public int SourLocId { get; set; }

        /// <summary>
        /// 源货品ID
        /// </summary>
        public int SourGoodsId { get; set; }

        /// <summary>
        /// 源货品包装ID
        /// </summary>
        public int SourGoodsPackId { get; set; }

        /// <summary>
        /// 源货品批号ID
        /// </summary>
        public int SourGoodsBatchNoId { get; set; }

        /// <summary>
        /// 源货品批号
        /// </summary>
        public string SourBatchNo { get; set; }

        /// <summary>
        /// 源有效期
        /// </summary>
        public string SourExtDate { get; set; }

        /// <summary>
        /// 源生产日期
        /// </summary>
        public string SourManuDate { get; set; }

        /// <summary>
        /// 源数量
        /// </summary>
        public decimal SourQty { get; set; }

        /// <summary>
        /// 源单价
        /// </summary>
        public decimal SourUnitPrice { get; set; }

        /// <summary>
        /// 目标货位ID
        /// </summary>
        public int DestLocId { get; set; }

        /// <summary>
        /// 目标货品ID
        /// </summary>
        public int DestGoodsId { get; set; }

        /// <summary>
        /// 目标货品包装ID
        /// </summary>
        public int DestGoodsPackId { get; set; }

        /// <summary>
        /// 目标货品批号ID
        /// </summary>
        public int DestGoodsBatchNoId { get; set; }

        /// <summary>
        /// 目标批号
        /// </summary>
        public string DestBatchNo { get; set; }

        /// <summary>
        /// 目标有效期
        /// </summary>
        public string DestExtDate { get; set; }

        /// <summary>
        /// 目标生产日期
        /// </summary>
        public string DestManuDate { get; set; }

        /// <summary>
        /// 目标数量
        /// </summary>
        public decimal DestQty { get; set; }

        /// <summary>
        /// 目标基数量
        /// </summary>
        public decimal DestBaseQty { get; set; }

        /// <summary>
        /// 目标基包装ID
        /// </summary>
        public int DestBasePackId { get; set; }

        /// <summary>
        /// 目标单价
        /// </summary>
        public decimal DestUnitPrice { get; set; }

        /// <summary>
        /// 作业人ID
        /// </summary>
        public int WorkById { get; set; }

        /// <summary>
        /// 作业时间
        /// </summary>
        public DateTime WorkAt { get; set; }

        /// <summary>
        /// 作业设备ID
        /// </summary>
        public int WorkDeviceId { get; set; }

        /// <summary>
        /// 附加字段
        /// </summary>
        public List<AttachItemValueClass> AttaItemList { get; set; }

        /// <summary>
        /// 作业ID
        /// </summary>
        public int WorkId { get; set; }

        /// <summary>
        /// 源账目ID【自增长序列】
        /// </summary>
        public int SourAccId { get; set; }

        /// <summary>
        /// 目标账目ID【自增长序列】
        /// </summary>
        public int DestAccId { get; set; }

        /// <summary>
        /// 订单明细ID
        /// </summary>
        public int OrderItemId { get; set; }

        /// <summary>
        /// 新批号ID
        /// </summary>
        public bool NewBatchNoId { get; set; }

        /// <summary>
        /// 新源账目ID
        /// </summary>
        public bool NewSourAccId { get; set; }

        /// <summary>
        /// 新目标账目ID
        /// </summary>
        public bool NewDestAccId { get; set; }

        /// <summary>
        /// 新作业ID
        /// </summary>
        public bool NewWorkId { get; set; }

        /// <summary>
        /// 新订单明细ID
        /// </summary>
        public bool NewOrderItemId { get; set; }

        /// <summary>
        /// 源包装数量
        /// </summary>
        public decimal SourPackUnitQty { get; set; }

        /// <summary>
        /// 目标包装数量
        /// </summary>
        public decimal DestPackUnitQty { get; set; }

        /// <summary>
        /// 是否打印
        /// </summary>
        public bool SendToPrint { get; set; }

        /// <summary>
        /// 打印格式
        /// </summary>
        public string SendPrintFormat { get; set; }

        /// <summary>
        /// 目标货位Code
        /// </summary>
        public string DestLocCode { get; set; }

        /// <summary>
        /// 源仓库ID
        /// </summary>
        public int SourWhId { get; set; }

        /// <summary>
        /// 目标仓库ID
        /// </summary>
        public int DestWhId { get; set; }

        /// <summary>
        /// 校验数量
        /// </summary>
        public decimal CheckQty { get; set; }

        /// <summary>
        /// 返回数量
        /// </summary>
        public decimal ReturnQty { get; set; }

        /// <summary>
        /// 返回原因
        /// </summary>
        public int ReturnReasonId { get; set; }

        /// <summary>
        /// 作业备注
        /// </summary>
        public string WorkNote { get; set; }

        public MobileWorkDataClass()
        {
            NewBatchNoId = false;
            NewSourAccId = false;
            NewDestAccId = false;
            NewWorkId = false;
            NewOrderItemId = false;
            SendToPrint = false;
            CheckQty = decimal.Zero;
            ReturnQty = decimal.Zero;
            ReturnReasonId = 0;
            WorkNote = "";
        }
    }
}