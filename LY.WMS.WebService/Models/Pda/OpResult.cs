using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    /// <summary>
    /// 操作结果
    /// </summary>
    public class OpResult
    {
        public string Data { get; }

        public bool Result { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorDetail { get; set; }

        public OpResult()
        {
        }

        public OpResult(bool paramResult, string paramErrorMessage, string paramErrorCode = "", string paramErrorDetail = "")
        {
            Result = paramResult;
            ErrorCode = paramErrorCode;
            ErrorMessage = paramErrorMessage;
            ErrorDetail = paramErrorDetail;
        }

        public OpResult(bool paramResult, string paramErrorMessage, string paramErrorCode, string paramErrorDetail, string paramData)
        {
            Result = paramResult;
            Data = paramData;
            ErrorCode = paramErrorCode;
            ErrorMessage = paramErrorMessage;
            ErrorDetail = paramErrorDetail;
        }

        public OpResult(string paramData)
        {
            Data = paramData;
            Result = true;
            ErrorCode = "";
            ErrorMessage = "";
            ErrorDetail = "";
        }
    }
}