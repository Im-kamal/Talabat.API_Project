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

namespace Talabat.Services
{
	public class OrderServices : IOrderServices
	{
		//private readonly IGenericRepository<Order> _orderRepo;
		//private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
		//private readonly IGenericRepository<Product> _productRepo;
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IPaymentServices _paymentServices;

		public OrderServices(IBasketRepository basketRepo, IUnitOfWork unitOfWork,IPaymentServices paymentServices)
			//IGenericRepository<Order> OrderRepo,
			//IGenericRepository<DeliveryMethod>	deliveryMethodRepo,
		    //IGenericRepository<Product> ProductRepo )
		{
			//_orderRepo = OrderRepo;
			//this._deliveryMethodRepo = deliveryMethodRepo;
			//_productRepo = ProductRepo;
			_basketRepo = basketRepo;
			this._unitOfWork = unitOfWork;
			this._paymentServices = paymentServices;
		}
		public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
		{
			var Basket = await _basketRepo.GetBasketAsync(basketId);
			var OrderItems = new List<OrderItem>();

			if (Basket?.Items.Count > 0)
			{
				foreach (var item in Basket.Items)
				{
					//var Product = await _productRepo.GetByIdAsyn(item.Id);
					var Product = await _unitOfWork.Repository<Product>().GetByIdAsyn(item.Id);
					var ProductItemOrdered = new ProductItemOrdered(item.Id,Product.Name,Product.PictureUrl);
					var OrderItem = new OrderItem(ProductItemOrdered,item.Quantity,Product.Price);
					OrderItems.Add(OrderItem);
				}
			}

			var SubTotal = OrderItems.Sum(item => item.Price * item.Quantity);

			//var DeliveryMethod = await _deliveryMethodRepo.GetByIdAsyn(deliveryMethodId);
			var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsyn(deliveryMethodId);

			var Spec = new OrderWithPaymentIntentSpec(Basket.PaymentIntentId);
			var ExOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
			if(ExOrder is not null)
			{
				_unitOfWork.Repository<Order>().Delete(ExOrder);
				await _paymentServices.CreateOrUpdatePaymentIntent(basketId);
			}

			var Order = new Order(buyerEmail , shippingAddress ,DeliveryMethod,OrderItems,SubTotal,Basket.PaymentIntentId);

			 //await _orderRepo.Add(Order);
			 await _unitOfWork.Repository<Order>().Add(Order);

			var Result = await _unitOfWork.CompleteAsync();

			if (Result <= 0) return null;

			return Order;
			
		}

		public async Task<Order> GetOrderByIdForSpecificUser(string buyerEmail, int orderId)
		{
			var Spec= new OrderSpecifications(buyerEmail,orderId);
			var Order= await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
			return Order;
		}

		public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
		{
			var spec = new OrderSpecifications(buyerEmail);	
			var Orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
			return Orders;	
		}
	}
}
