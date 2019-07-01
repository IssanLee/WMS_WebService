using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LY.WMS.WebService.Models
{
    public class ReqClass
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

        public List<ReqItemClass> ItemList
        {
            get;
            set;
        }

        public string ORG_ID
        {
            get;
            set;
        }

        public string CODE
        {
            get;
            set;
        }

        public string WH_CODE
        {
            get;
            set;
        }

        public string PARTNER_CODE
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

        public string ITEM_COUNT
        {
            get;
            set;
        }

        public decimal TOTAL_QTY
        {
            get;
            set;
        }

        public string TOTAL_AMOUNT
        {
            get;
            set;
        }

        public string IMP_STATUS_CODE
        {
            get;
            set;
        }

        public string EXP_STATUS_CODE
        {
            get;
            set;
        }

        public string FROM_OD_ID
        {
            get;
            set;
        }

        public string TO_OD_ID
        {
            get;
            set;
        }

        public string NOTE
        {
            get;
            set;
        }

        public string NOTE_1
        {
            get;
            set;
        }

        public string NOTE_2
        {
            get;
            set;
        }

        public string NOTE_3
        {
            get;
            set;
        }

        public string NOTE_4
        {
            get;
            set;
        }

        public string NOTE_5
        {
            get;
            set;
        }

        public string NOTE_6
        {
            get;
            set;
        }

        public string NOTE_7
        {
            get;
            set;
        }

        public string NOTE_8
        {
            get;
            set;
        }

        public string NOTE_9
        {
            get;
            set;
        }

        public string NOTE_10
        {
            get;
            set;
        }

        public string ERPID
        {
            get;
            set;
        }

        public ReqClass()
        {
            ItemList = new List<ReqItemClass>();
            ORG_ID = String.Empty;
            CODE = String.Empty;
            WH_CODE = String.Empty;
            PARTNER_CODE = String.Empty;
            STATUS_CODE = String.Empty;
            TYPE_CODE = String.Empty;
            ITEM_COUNT = String.Empty;
            TOTAL_AMOUNT = String.Empty;
            IMP_STATUS_CODE = String.Empty;
            EXP_STATUS_CODE = String.Empty;
            FROM_OD_ID = String.Empty;
            TO_OD_ID = String.Empty;
            NOTE = String.Empty;
            NOTE_1 = String.Empty;
            NOTE_2 = String.Empty;
            NOTE_3 = String.Empty;
            NOTE_4 = String.Empty;
            NOTE_5 = String.Empty;
            NOTE_6 = String.Empty;
            NOTE_7 = String.Empty;
            NOTE_8 = String.Empty;
            NOTE_9 = String.Empty;
            NOTE_10 = String.Empty;
            ERPID = String.Empty;
        }
    }
}