using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class ReqItemClass
    {
        public BaseIdClass BaseId
        {
            get;
            set;
        }

        public BaseVersionClass BaseVersion
        {
            get;
            set;
        }

        public string REQ_CODE
        {
            get;
            set;
        }

        public string G_CODE
        {
            get;
            set;
        }

        public string GP_CODE
        {
            get;
            set;
        }

        public string BatchNo
        {
            get;
            set;
        }

        public string LOT_NO
        {
            get;
            set;
        }

        public string WH_CODE
        {
            get;
            set;
        }

        public string LOC_CODE
        {
            get;
            set;
        }

        public string STATUS_CODE
        {
            get;
            set;
        }

        public string TYPE_CODE
        {
            get;
            set;
        }

        public string ROW_INDEX
        {
            get;
            set;
        }

        public string NOTE
        {
            get;
            set;
        }

        public decimal QTY
        {
            get;
            set;
        }

        public decimal UNIT_QTY
        {
            get;
            set;
        }

        public string UNIT_PRICE
        {
            get;
            set;
        }

        public string AMOUNT
        {
            get;
            set;
        }

        public string BASE_GP_CODE
        {
            get;
            set;
        }

        public decimal BASE_UNIT_QTY
        {
            get;
            set;
        }

        public decimal BASE_QTY
        {
            get;
            set;
        }

        public string ERPID
        {
            get;
            set;
        }

        public ReqItemClass()
        {
            REQ_CODE = String.Empty;
            G_CODE = String.Empty;
            GP_CODE = String.Empty;
            BatchNo = String.Empty;
            LOT_NO = String.Empty;
            WH_CODE = String.Empty;
            LOC_CODE = String.Empty;
            STATUS_CODE = String.Empty;
            TYPE_CODE = String.Empty;
            ROW_INDEX = String.Empty;
            NOTE = String.Empty;
            UNIT_PRICE = String.Empty;
            AMOUNT = String.Empty;
            BASE_GP_CODE = String.Empty;
            ERPID = String.Empty;
        }
    }
}