using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
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
}