using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models.Base
{
    /// <summary>
    /// 货品包装
    /// </summary>
    public class GoodsPackClass
    {
        /// <summary>
        /// 货品类型
        /// </summary>
        public GoodsClass Goods { get; set; }

        /// <summary>
        /// 货品包装类型
        /// </summary>
        public GoodsPackTypeClass PackType { get; set; }

        /// <summary>
        /// 包装货品Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 包装货品码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 包装货品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 包装货品件数规格
        /// </summary>
        public decimal UnitQty { get; set; }

        /// <summary>
        /// 包装69码
        /// </summary>
        public string PackBcode { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 包装长
        /// </summary>
        public decimal Lenght { get; set; }

        /// <summary>
        /// 包装宽
        /// </summary>
        public decimal Width { get; set; }

        /// <summary>
        /// 包装高
        /// </summary>
        public decimal Height { get; set; }

        /// <summary>
        /// 包装体积
        /// </summary>
        public decimal Vol { get; set; }

        /// <summary>
        /// 包装重量
        /// </summary>
        public decimal G_Weight { get; set; }

        /// <summary>
        /// 包装重量
        /// </summary>
        public decimal Net_Weigth { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        public string Level { get; set; }

        public GoodsPackClass()
        {
            PackType = new GoodsPackTypeClass();
        }

        public GoodsPackClass(int paramId, string paramCode, string paramName, decimal paramPackQty, string parampackBcode, GoodsClass paramGoods, GoodsPackTypeClass paramPackType, bool paramIsEnable)
        {
            PackType = new GoodsPackTypeClass();
            Id = paramId;
            Code = paramCode;
            Name = paramName;
            UnitQty = paramPackQty;
            PackBcode = parampackBcode;
            Goods = paramGoods;
            PackType = paramPackType;
            IsEnable = paramIsEnable;
        }
    }
}