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

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string GetSystemparamList()
        {
            string sql = string.Format(@"select * from V_SYS_param_LIST where SYS_param_GP_CODE = 'MOBILEPARAM'");
            DataTable dataTable = Common.OracleDB.GetDataTableBySql(sql); 
            return "";
        }

        [WebMethod]
        public string GetDbVersion()
        {
            return Common.OracleDB.GetStringBySql("SELECT NAME FROM V$DATABASE");
        }


    }
}
