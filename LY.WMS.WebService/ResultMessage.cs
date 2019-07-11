namespace LY.WMS.WebService
{
    /// <summary>
    /// 返回消息结果
    /// </summary>
    public struct ResultMessage
    {
        /// <summary>
        /// 返回结果成功与否
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 错误Code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorStr { get; set; }

        public ResultMessage(bool ParamResult, string ParamErrorStr)
        {
            this = default;
            Result = ParamResult;
            ErrorCode = "999";
            ErrorStr = ParamErrorStr;
        }

        public ResultMessage(bool ParamResult, string ParamErrorCode, string ParamErrorStr)
        {
            this = default;
            Result = ParamResult;
            ErrorCode = ParamErrorCode;
            ErrorStr = ParamErrorStr;
        }
    }
}