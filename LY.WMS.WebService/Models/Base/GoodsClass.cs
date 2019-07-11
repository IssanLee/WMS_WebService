namespace LY.WMS.WebService.Models.Base
{
    public class GoodsClass
    {
        /// <summary>
        /// 货品Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 货品码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 货品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 货品规格
        /// </summary>
        public string Spec { get; set; }

        /// <summary>
        /// 生产地
        /// </summary>
        public string Prod { get; set; }

        /// <summary>
        /// 生产厂家
        /// </summary>
        public string Manu { get; set; }

        /// <summary>
        /// 69码
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 准字号
        /// </summary>
        public string ApprNo { get; set; }

        /// <summary>
        /// EcodeYn
        /// </summary>
        public bool EcodeYn { get; set; }

        /// <summary>
        /// IsEnable
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 作业等级Id
        /// </summary>
        public string WorkLevelId { get; set; }

        public GoodsClass()
        {
            Code = "";
            Name = "";
            Spec = "";
            Prod = "";
            Manu = "";
            Barcode = "";
            Note = "";
            ApprNo = "";
            WorkLevelId = "";
        }

        public GoodsClass(int paramId, string paramCode, string paramName, string paramSpec, string paramProd, string paramManu, string paramApprNo, int paramEcodeYn, bool paramIsEnable)
        {
            Code = "";
            Name = "";
            Spec = "";
            Prod = "";
            Manu = "";
            Barcode = "";
            Note = "";
            ApprNo = "";
            WorkLevelId = "";
            Id = paramId;
            Code = paramCode;
            Name = paramName;
            Spec = paramSpec;
            Prod = paramProd;
            Manu = paramManu;
            ApprNo = paramApprNo;
            EcodeYn = (paramEcodeYn != 0);
            IsEnable = paramIsEnable;
        }
    }
}