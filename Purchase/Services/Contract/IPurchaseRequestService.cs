using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PurchaseShared.Models;

namespace Purchase.Services.Contract
{
	public interface IPurchaseRequestService
	{
		Task<IEnumerable<PurchaseRequestLine>> GetPurchaseRequestsAsync();
		Task<PurchaseRequestLine> GetPurchaseRequestByIdAsync();
		Task AddPurchaseRequestAsync(PurchaseRequestLine purchaseRequestLine);
		Task UpdatePurchaseRequestAsync(PurchaseRequestLine purchaseRequestLine);
		Task DeletePurchaseRequestAsync(string id);
		Task ImportPurchaseRequestAsync(IReadOnlyList<PurchaseRequestLine> purchaseRequestLines);
	}
}

