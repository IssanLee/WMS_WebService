using System;

namespace LY.WMS.WebService.Models.Base
{
    /// <summary>
    /// 订单作业明细
    /// </summary>
    public class OrderWorkItemClass
    {
        /// <summary>
        /// 作业数量
        /// </summary>
        public decimal WorkQty { get; set; }

        /// <summary>
        /// 作业单价
        /// </summary>
        public decimal WorkUnitPrice { get; set; }

        /// <summary>
        /// 作业日期
        /// </summary>
        public DateTime WorkDate { get; set; }

        /// <summary>
        /// 作业设备Code
        /// </summary>
        public string WorkDeviceCode { get; set; }

        /// <summary>
        /// 作业者
        /// </summary>
        public string WorkByName { get; set; }

        /// <summary>
        /// 货品包装
        /// </summary>
        public GoodsPackClass GoodsPack { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        public string ExpDate { get; set; }

        /// <summary>
        /// 原货位Code
        /// </summary>
        public string SourLocCode { get; set; }

        /// <summary>
        /// 目标货位Code
        /// </summary>
        public string DestLocCode { get; set; }
    }
}