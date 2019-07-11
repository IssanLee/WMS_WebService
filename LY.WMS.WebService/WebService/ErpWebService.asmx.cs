using LY.WMS.WebService.Business.Erp;
using LY.WMS.WebService.Models.Erp;
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
        #region 上架作业

        [WebMethod]
        public ResultMessage SubmitRglToErp(RglClass ParamRgl)
        {
            return ErpBusiness.SubmitRglToErp(ParamRgl);
        }

        [WebMethod]
        public ResultMessage SubmitSrToErp(SwapSrClass ParamSwapSr)
        {
            return ErpBusiness.SubmitSrToErp(ParamSwapSr);
        }
        #endregion

    }
}
