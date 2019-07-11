using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models.Base
{
    public class GoodsPackTypeClass
    {
        /// <summary>
        /// 类型ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 类型Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; }

        public GoodsPackTypeClass()
        {
        }

        public GoodsPackTypeClass(int paramId, string paramCode, string paramName)
        {
            Id = paramId;
            Code = paramCode;
            Name = paramName;
        }
    }
}