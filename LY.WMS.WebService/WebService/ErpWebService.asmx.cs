using LY.WMS.WebService.Business;
using LY.WMS.WebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace LY.WMS.WebService
{
    /// <summary>
    /// ErpWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ErpWebService : System.Web.Services.WebService
    {
        [WebMethod]
        public ResultMessage SubmitAccLocGbnToErp(List<ErpAccLocGbnClass> ParamErpAccLocGbnList)
        {
            return SubmitToErp.DoSubmitAccLocGbnToErp(ParamErpAccLocGbnList);
        }


        [WebMethod]
        public ResultMessage SubmitStockMoveToErp(StockMoveToClass ParamStockMoveTo)
        {
            return SubmitToErp.DoSubmitStockMoveToErp(ParamStockMoveTo);
        }

        [WebMethod]
        public ResultMessage SubmitSocToErp(List<SocClass> ParamSocList)
        {
            return SubmitToErp.DoSubmitSocToErp(ParamSocList);
        }

        [WebMethod]
        public ResultMessage OctCheckFinished(string ParamBillNo)
        {
            return SubmitToErp.OctCheckFinished(ParamBillNo);
        }


        [WebMethod]
        public ResultMessage SubmitWhWorkEcodeToErp(List<WhWorkItemEcode> ParamWorkItemEcodeList)
        {
            try
            {
                foreach (WhWorkItemEcode whWorkItemEcode in ParamWorkItemEcodeList)
                {
                    switch (whWorkItemEcode.EnumWorkType)
                    {
                        case EnumEcodeWorkType.OutCheck:
                            SubmitToErp.SaveOctCheckEcode(whWorkItemEcode);
                            break;
                        case EnumEcodeWorkType.ReceivedWork:
                            SubmitToErp.SaveReceivedEcode(whWorkItemEcode);
                            break;
                    }
                }
            }
            finally
            {
            }
            return new ResultMessage(true, "", "");
        }


        [WebMethod]
        public ResultMessage SubmitRglToErp(RglClass ParamRgl)
        {
            return SubmitToErp.DoSubmitRglToErp(ParamRgl);
        }

        [WebMethod]
        public ResultMessage SubmitSrToErp(SwapSrClass ParamSwapSr)
        {
            return SubmitToErp.DoSubmitSrToErp(ParamSwapSr);
        }

        [WebMethod]
        public ResultMessage SubmitWhWorkListToErp(List<WhWorkItem> ParamWhWorkItemList)
        {
            ResultMessage result;
            if (ParamWhWorkItemList == null)
            {
                result = new ResultMessage(false, "0000", "任务列表无效");
            }
            else if (ParamWhWorkItemList.Count == 0)
            {
                result = new ResultMessage(false, "0000", "任务列表无效");
            }
            else
            {
                EnumWorkType whWorkType = ParamWhWorkItemList[0].WhWorkType;
                if (whWorkType == EnumWorkType.ReceiveGoodsWork)
                {
                    return SubmitToErp.SaveReceiveItemListWork(ParamWhWorkItemList);
                }
                result = new ResultMessage(false, "0000", "指定的操作无效");
            }
            return result;
        }





        [WebMethod]
        public ResultMessage SubmitWhWorkToErp(WhWorkItem ParamWhWorkItem)
        {
            switch (ParamWhWorkItem.WhWorkType)
            {
                case EnumWorkType.ReceiveGoodsWork:
                    return SubmitToErp.SaveReceiveWork(ParamWhWorkItem);
                case EnumWorkType.UpGoodsWork:
                    return SubmitToErp.SaveReceiveUpWork(ParamWhWorkItem);
                case EnumWorkType.TakeWork:
                    return SubmitToErp.SaveStockTakeWork(ParamWhWorkItem);
                case EnumWorkType.MoveUpGoodsFromLoc:
                    return SubmitToErp.SaveMoveFromWhWork(ParamWhWorkItem);
                case EnumWorkType.MoveUpGoodsToLoc:
                    return SubmitToErp.SaveMoveToWhWork(ParamWhWorkItem);
                case EnumWorkType.OutCheckDiffWork:
                    return SubmitToErp.SaveOctDiffWork(ParamWhWorkItem);
                default:
                    return new ResultMessage(false, "0000", "指定的操作无效");
            }
        }
    }
}
