using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	[Authorize]
	public class PaymentsController : APIBaseController
	{
		private readonly IPaymentServices _paymentServices;
		private readonly IMapper _mapper;
		const string endpointSecret = "whsec_f00046fc07ee922d83662ace15584c0147eece394189a5738bdca9131c3580bd";


		public PaymentsController(IPaymentServices paymentServices,IMapper mapper)
        {
			this._paymentServices = paymentServices;
			this._mapper = mapper;
		}
		[ProducesResponseType(typeof(CustomerBasketDto),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [HttpPost]
		public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var CustomerBasket =  await _paymentServices.CreateOrUpdatePaymentIntent(basketId);
			if (CustomerBasket is null) return BadRequest(new ApiResponse(400,"There is a Problem With Your Basket"));
			var MappedBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(CustomerBasket);
			return Ok(MappedBasket);
		}
		[HttpPost("webhook")]
		public async Task<IActionResult> StipeWebHook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			try
			{
				var stripeEvent = EventUtility.ConstructEvent(json,
					Request.Headers["Stripe-Signature"], endpointSecret);

				var PaymentIntent = stripeEvent.Data.Object as PaymentIntent; 
				// Handle the event
				if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
				{
					await _paymentServices.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, false);
				}
				else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
				{
					await _paymentServices.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, true);

				}
				return Ok();
			}
			catch (StripeException e)
			{
				return BadRequest();
			}
		}

	}
}
