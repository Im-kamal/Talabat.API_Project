using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Services
{
	public interface IOrderServices
	{
		public Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress);
		Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail);

		Task<Order> GetOrderByIdForSpecificUser(string buyerEmail,int orderId);
	}
}
