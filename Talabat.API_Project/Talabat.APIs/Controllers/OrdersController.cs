using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Services;
using Talabat.Services;

namespace Talabat.APIs.Controllers
{
	
	public class OrdersController : APIBaseController
	{
		private readonly IMapper _mapper;
		private readonly IOrderServices _orderServices;
		private readonly IUnitOfWork _unitOfWork;

		public OrdersController(IMapper mapper,IOrderServices orderServices,IUnitOfWork unitOfWork)
        {
			this._mapper = mapper;
			this._orderServices = orderServices;
			this._unitOfWork = unitOfWork;
		}
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost]
		[Authorize]
		public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var MappedAddress = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
			var Order = await _orderServices.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, MappedAddress);
			if (Order is null) return BadRequest(new ApiResponse(400));
			return Ok(Order);
		}

		[ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForSpecificUser()
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var Orders = await _orderServices.GetOrdersForSpecificUserAsync(buyerEmail);
			if (Orders is null) return NotFound(new ApiResponse(404,"No Orders For This User"));
			var MappedOrders = _mapper.Map<IReadOnlyList< Order>,IReadOnlyList< OrderToReturnDto>>(Orders); 
			return Ok(MappedOrders);
		}
		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[Authorize]
		[HttpGet("{id}")]
		public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
		{
			var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var Order = await _orderServices.GetOrderByIdForSpecificUser(BuyerEmail, id);
			if (Order is null) return NotFound(new ApiResponse(404,$"No Order With Id = {id} for this user"));
			var MappedOrder = _mapper.Map<Order, OrderToReturnDto>(Order);
			return Ok(MappedOrder);
		}

		[HttpGet("DeliveryMethods")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
		{
			var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsyn();
			return Ok(DeliveryMethods);
		}

	}
}
