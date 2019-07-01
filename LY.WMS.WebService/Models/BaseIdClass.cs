using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class BaseIdClass
    {
        public string Id
        {
            get;
            set;
        }

        public string GID
        {
            get;
            set;
        }

        public BaseIdClass()
        {
        }

        public BaseIdClass(string ParamId, string ParamGuid)
        {
            Id = ParamId;
            GID = ParamGuid;
        }
    }
}