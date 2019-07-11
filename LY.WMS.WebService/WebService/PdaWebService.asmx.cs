using LY.WMS.WebService.Business;
using LY.WMS.WebService.Business.Pda;
using LY.WMS.WebService.Models;
using LY.WMS.WebService.Models.Base;
using LY.WMS.WebService.Models.Pda;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace LY.WMS.WebService
{
    /// <summary>
    /// PdaWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class PdaWebService : System.Web.Services.WebService
    {
        #region Info
        [WebMethod]
        public SystemInformation GetSystemInformation()
        {
            return Info.GetSystemInformation();
        }

        [WebMethod]
        public List<SystemparamClass> GetSystemparamList()
        {
            return Info.GetSystemparamList();
        }

        [WebMethod]
        public static DateTime GetSystemDatetime()
        {
            return Info.GetSystemDatetime();
        }

        #endregion

        #region 登录权限部分[包括货位检查]

        [WebMethod]
        public DeviceClass GetDeviceInfo(string paramMac, string paramDeviceKeyCode)
        {
            return PdaBusiness.GetDeviceInfo(paramMac, paramDeviceKeyCode);
        }

        [WebMethod]
        public UserClass CardPwdLogin(string paramCardPwd, string paramDeviceMac)
        {
            return PdaBusiness.CardPwdLogin(paramCardPwd, paramDeviceMac);
        }

        [WebMethod]
        public List<UserRight> GetUserRightList(string paramUserFlag, string paramUsergid)
        {
            return PdaBusiness.GetUserRightList(paramUserFlag, paramUsergid);
        }

        [WebMethod]
        public OpResult CheckDestLoc(string paramUserFlag, MobileWorkDataClass paramWorkData, string paramLocCode)
        {
            if (!PdaBusiness.CheckUserSession(Convert.ToInt32(paramUserFlag)))
            {
                return new OpResult(false, "指定的用户无效!");
            }
            switch (paramWorkData.WorkType)
            {
                // 上架
                case EnumWorkType.UpGoodsWork:
                    return UpGoodsWork.CheckDestLoc(paramWorkData, paramLocCode);
                // 移库
                case EnumWorkType.MoveUpGoodsToLoc:
                    //m_WorkToDatabase = new MoveToLocWorkClass();
                    break;
                // 盘点
                case EnumWorkType.TakeWork:
                    //m_WorkToDatabase = new StockTakeWorkClass();
                    break;
                default:
                    return new OpResult(false, "未知的作业类型!");
            }
            return null;
        }

        #endregion

        [WebMethod]
        public List<OrderWorkItemClass> GetOrderWorkItemList(string paramUserFlag, MobileWorkDataClass paramWorkData)
        {
            if (!PdaBusiness.CheckUserSession(Convert.ToInt32(paramUserFlag)))
            {
                return null;
            }
            switch (paramWorkData.WorkType)
            {
                case EnumWorkType.UpGoodsWork:
                    return UpGoodsWork.GetOrderWorkList(paramWorkData);
                
                default:
                    return null;
            }
        }

        #region 上架

        [WebMethod]
        public List<UpGoodsBatchNoClass> GetUpGoodsBatchNoList(string paramUserFlag, string paramGoodsStr, int paramWhId)
        {
            return PdaBusiness.GetUpGoodsBatchNoList(paramUserFlag, paramGoodsStr, paramWhId);
        }

        #endregion

    }
}
