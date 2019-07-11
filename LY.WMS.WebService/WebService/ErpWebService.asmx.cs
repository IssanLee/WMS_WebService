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
        public List<ReqClass> GetReqList(string ParamHeadStr, string ParamItemStr, DateTime ParamLmdate, int ParamRowIndex, int ParamRowNumber, DateTime ParamDownLoadDate)
        {
            return GetTrans.GetReqList(ParamHeadStr, ParamItemStr, ParamLmdate, ParamRowIndex, ParamRowNumber, ParamDownLoadDate);
        }
    }
}
