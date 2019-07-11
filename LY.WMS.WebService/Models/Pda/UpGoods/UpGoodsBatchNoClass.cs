namespace LY.WMS.WebService.Models.Pda
{
    /// <summary>
    /// 上架货品信息
    /// </summary>
    public class UpGoodsBatchNoClass
    {
        /// <summary>
        /// 货品编码
        /// </summary>
        public string GoodsCode { get; set; }

        /// <summary>
        /// 货品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 货品规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Prod { get; set; }

        /// <summary>
        /// 生产商
        /// </summary>
        public string Manu { get; set; }

        /// <summary>
        /// 准字号
        /// </summary>
        public string ApprNo { get; set; }

        /// <summary>
        /// 69码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 货品号id
        /// </summary>
        public int GoodsId { get; set; }

        /// <summary>
        /// 货品包装id
        /// </summary>
        public int GoodsPackId { get; set; }

        /// <summary>
        /// 货品批号id
        /// </summary>
        public int GoodsBatchNoId { get; set; }

        /// <summary>
        /// 包装编码
        /// </summary>
        public string PackCode { get; set; }

        /// <summary>
        /// 包装名
        /// </summary>
        public string PackName { get; set; }

        /// <summary>
        /// 包装数量
        /// </summary>
        public decimal PackUnitQty { get; set; }

        /// <summary>
        /// 箱包装数量
        /// </summary>
        public decimal BoxPackUnitQty { get; set; }

        /// <summary>
        /// 箱包装类型
        /// </summary>
        public string BoxPackCode { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public string ExpDate { get; set; }

        /// <summary>
        /// 生产日期
        /// </summary>
        public string ManuDate { get; set; }

        /// <summary>
        /// 仓库名
        /// </summary>
        public string ToWhAreaName { get; set; }

        /// <summary>
        /// Erp仓库名
        /// </summary>
        public string ErpWhName { get; set; }

        /// <summary>
        /// 目标库位号
        /// </summary>
        public string ToLocCode { get; set; }

        /// <summary>
        /// 数量(散件)
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string TranCode { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 目标仓库区域id
        /// </summary>
        public int ToWhAreaId { get; set; }

        /// <summary>
        /// 目标库位id
        /// </summary>
        public int ToLocId { get; set; }

        /// <summary>
        /// 订单id
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 订单明细id
        /// </summary>
        public int OrderItemId { get; set; }

        /// <summary>
        /// 来源类型
        /// </summary>
        public string Reqtype { get; set; }

        /// <summary>
        /// 贮藏条件
        /// </summary>
        public string StorageCondition { get; set; }

        public UpGoodsBatchNoClass()
        {
            BoxPackCode = "";
        }
    }
}