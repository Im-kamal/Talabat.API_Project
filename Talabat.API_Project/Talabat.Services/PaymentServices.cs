using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.OrderSpecifications;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Services
{
	public class PaymentServices : IPaymentServices
	{
		private readonly IConfiguration _configuration;
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentServices(IConfiguration configuration, IBasketRepository basketRepo, IUnitOfWork unitOfWork)
		{
			this._configuration = configuration;
			this._basketRepo = basketRepo;
			this._unitOfWork = unitOfWork;
		}
		public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
		{
			//Secret Key
			StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];

			var Basket = await _basketRepo.GetBasketAsync(BasketId);

			if (Basket is null) return null;

			var ShippingPrice = 0M;
			if (Basket.DeliveryMethodId.HasValue)
			{
				var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsyn(Basket.DeliveryMethodId.Value);
				ShippingPrice = DeliveryMethod.Cost;
			}

			if (Basket.Items.Count > 0)
			{
				foreach (var item in Basket.Items)
				{
					var Product = await _unitOfWork.Repository<Product>().GetByIdAsyn(item.Id);
					if (item.Price != Product.Price)
						item.Price = Product.Price;

				}
			}

			var SubTotal = Basket.Items.Sum(item => item.Price * item.Quantity);

			var Services = new PaymentIntentService();
			PaymentIntent paymentIntent;
			if (string.IsNullOrEmpty(Basket.PaymentIntentId))  //Create
			{
				var Options = new PaymentIntentCreateOptions()
				{
					Amount = (long)(SubTotal * 100 + ShippingPrice * 100),
					Currency = "usd",
					PaymentMethodTypes = new List<string>() {"card"}
				};
				paymentIntent = await Services.CreateAsync(Options);
				Basket.PaymentIntentId = paymentIntent.Id;
				Basket.ClientSecret = paymentIntent.ClientSecret;
			}

			else    //Update
			{
				var Options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)(SubTotal * 100 + ShippingPrice * 100),
				};
				paymentIntent=await Services.UpdateAsync(Basket.PaymentIntentId, Options);
				Basket.PaymentIntentId = paymentIntent.Id;
				Basket.ClientSecret = paymentIntent.ClientSecret;
			}
			await _basketRepo.UpdateBasketAsync(Basket);
			return Basket;

		}

		public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentIntentId, bool flag)
		{
			var Spec = new OrderWithPaymentIntentSpec(PaymentIntentId);
			var Order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
			if (flag)
			{
				Order.Status = OrderStatus.PaymentRecived;
			}
			else
			{
				Order.Status = OrderStatus.PaymentFailed;
			}
			_unitOfWork.Repository<Order>().Update(Order);

			await _unitOfWork.CompleteAsync();
			return Order;

		}
	}
}
