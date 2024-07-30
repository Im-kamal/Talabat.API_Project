using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Specifications.OrderSpecifications
{
	public class OrderWithPaymentIntentSpec : BaseSpecifications<Order>
	{
        public OrderWithPaymentIntentSpec(string PaymentInentId):base(O=>O.PaymentIntentId == PaymentInentId)
        {
            
        }
    }
}
