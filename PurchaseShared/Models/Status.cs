using System;
using System.Collections.Generic;

namespace PurchaseShared.Models
{
    public static class Status
    {
        //list status
        public static List<string> PoStatus = new List<string>
        {
            "PR PENDING","PO CANCELED",
            "INQUERY SEND", "TECHNICAL CLEARANCE",
            "PO APPROVAL", "PO RELEASED",
            "WAITING PROFORMA", "DPI-AC", "COMPLETED"
        };
    }

    public enum OrderStatus
    {
        PR_PENDING ,
        PO_CANCELED,
        INQUERY_SEND,
        TECHNICAL_CLEARANCE,
        PO_APPROVAL,
        PO_RELEASED,
        WAITING_PROFORMA,
        DPI_AC,
        COMPLETED
    }
}
