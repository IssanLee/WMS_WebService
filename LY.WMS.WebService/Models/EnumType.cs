namespace LY.WMS.WebService.Models
{
    /// <summary>
    /// 作业类型
    /// </summary>
    public enum EnumWorkType
    {
        ReceiveGoodsWork = 1,
        UpGoodsWork = 4,
        PickGoodsWork = 5,
        OutCheckWork = 6,
        TakeWork = 7,
        MoveUpGoodsFromLoc = 9,
        MoveUpGoodsToLoc = 10,
        OutCheckDiffWork = 11
    }

    /// <summary>
    /// 事务状态
    /// </summary>
    public enum EnumTranStatus
    {
        /// <summary>
        /// 已审核
        /// </summary>
        Audited = 1,
        /// <summary>
        /// 已取消
        /// </summary>
        Canceled,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed,
        /// <summary>
        /// 已确认
        /// </summary>
        Confirmed,
        /// <summary>
        /// 新建
        /// </summary>
        Created,
        /// <summary>
        /// 编辑中
        /// </summary>
        Editing,
        /// <summary>
        /// 已传送
        /// </summary>
        Posted,
        /// <summary>
        /// 已提交
        /// </summary>
        Submited,
        /// <summary>
        /// 正在验收
        /// </summary>
        InCheck,
        /// <summary>
        /// 验货完成
        /// </summary>
        InCheckFinished,
        /// <summary>
        /// 收货完成
        /// </summary>
        ReceiveFinished,
        /// <summary>
        /// 正在上货
        /// </summary>
        UpGoods,
        /// <summary>
        /// 上货完成
        /// </summary>
        UpGoodsFinished,
        /// <summary>
        /// 正在出库复核
        /// </summary>
        InOutCheck,
        /// <summary>
        /// 正在拣货
        /// </summary>
        InPick,
        /// <summary>
        /// 拣货完成
        /// </summary>
        PickFinished,
        /// <summary>
        /// 出库复核完成
        /// </summary>
        OutCheckFinished
    }
}