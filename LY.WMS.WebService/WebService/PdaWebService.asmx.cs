using LY.WMS.WebService.Business;
using LY.WMS.WebService.Models;
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

        #region 登录权限部分

        [WebMethod]
        public DeviceClass GetDeviceInfo(string paramMac, string paramDeviceKeyCode)
        {
            return Info.GetDeviceInfo(paramMac, paramDeviceKeyCode);
        }

        [WebMethod]
        public UserClass CardPwdLogin(string paramCardPwd, string paramDeviceMac)
        {
            return Info.CardPwdLogin(paramCardPwd, paramDeviceMac);
        }

        [WebMethod]
        public List<UserRight> GetUserRightList(string paramUserFlag, string paramUsergid)
        {
            return Info.GetUserRightList(paramUserFlag, paramUsergid);
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
                    //m_WorkToDatabase = new UpGoodsWorkClass();
                    break;
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
            //return m_WorkToDatabase.CheckDestLoc(paramWorkData, paramLocCode);
        }

        #endregion

        #region 上架

        [WebMethod]
        public List<UpGoodsBatchNoClass> GetUpGoodsBatchNoList(string paramUserFlag, string paramGoodsStr, int paramWhId)
        {
            return PdaBusiness.GetUpGoodsBatchNoList(paramUserFlag, paramGoodsStr, paramWhId);
        }

        #endregion

    }
}
