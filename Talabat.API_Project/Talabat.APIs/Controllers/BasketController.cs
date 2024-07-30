using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Repository;

namespace Talabat.APIs.Controllers
{

	public class BasketController : APIBaseController
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;

		public BasketController(IBasketRepository basketRepository,IMapper mapper)
		{
			_basketRepository = basketRepository;
			_mapper = mapper;
		}
		//Get Or ReCreate Basket
		[HttpGet("{BasketId}")]
		public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string BasketId)
		{
			var Basket = await _basketRepository.GetBasketAsync(BasketId);
			if (Basket is null) return new CustomerBasket(BasketId);
			return Ok(Basket);
		}

		//Update Or Create new Basket
		[HttpPost]
		public async Task<ActionResult<CustomerBasket>> UpdateOrCreateBasket(CustomerBasketDto Basket)
		{
			var MappedAddress = _mapper.Map<CustomerBasketDto, CustomerBasket>(Basket);
			var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(MappedAddress);
			if (CreatedOrUpdatedBasket is null) return BadRequest(new ApiResponse(400));
			return Ok(CreatedOrUpdatedBasket);
		}

		//Delete Basket

		[HttpDelete("{BasketId}")]
		public async Task<ActionResult<bool>> DeleteBasket (string BasketId)
		{
			return await _basketRepository.DeleteBasketAsync(BasketId);	
		}
	}
}
