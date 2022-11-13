using System;
using System.Collections.Generic;

namespace PurchaseShared.Models
{
    public static class Status
    {
        //list status
        public static List<string> PoStatus = new List<string>
        {
            "PR PENDING",
            "PO CANCELED",
            "INQUERY SEND",
            "TECHNICAL CLEARANCE",
            "PO APPROVAL",
            "PO RELEASED",
            "WAITING PROFORMA",
            "DPI-AC",
            "COMPLETED"
        };
    }

    public enum EOrderStatus
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

    public static class OrderStatus
    {
        public static string PO_APPROVAL { get; set; } = "PO APPROVAL";
        public static string PO_CANCELED { get; set; } = "PO CANCELED";
        public static string PO_INQUERY_SEND { get; set; } = "INQUERY SEND";
        public static string TECHNICAL_CLEARANCE { get; set; } = "TECHNICAL CLEARANCE";
        public static string PR_PENDING { get; set; } = "PR PENDING";
        public static string PO_RELEASED { get; set; } = "PO RELEASED";
        public static string WAITING_PROFORMA { get; set; } = "WAITING PROFORMA";
        public static string DPI_AC { get; set; } = "DPI-AC";
        public static string PO_COMPLETED { get; set; } = "PO COMPLETED";


    }

}
