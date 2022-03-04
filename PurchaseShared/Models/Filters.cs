using System;
namespace PurchaseShared.Models
{
    public static class Filters
    {
        public static bool FilterFunction(Order order, string query)
        {
            if (string.IsNullOrWhiteSpace(query.ToLower()))
                return true;
            if (order.AgentName != null)
                if (order.AgentName.ToLower().Contains(query.ToLower()))
                    return true;
            if (order.Commentaires != null)
                if (order.Commentaires.ToLower().Contains(query.ToLower()))
                    return true;
            if (order.Description != null)
                if (order.Description.ToLower().Contains(query.ToLower()))
                    return true;
            if (order.Devise != null)
                if (order.Devise.ToLower().Contains(query.ToLower()))
                    return true;
            if (order.PurchaseOrder != null)
                if (order.PurchaseOrder.ToLower().Contains(query.ToLower()))
                    return true;
            if (order.ReqNumber != 0)
                if (order.ReqNumber.ToString().ToLower().Contains(query.ToLower()))
                    return true;
            if (order.Status != null)
                if (order.Status.ToLower().Contains(query.ToLower()))
                    return true;
            if (order.Amount != 0)
                if (order.Amount.ToString().Contains(query.ToLower()))
                    return true;
            if (order.Vendor.Name != null)
                if (order.Vendor.Name.ToLower().Contains(query.ToLower()))
                    return true;
            return false;
        }
    }
}
