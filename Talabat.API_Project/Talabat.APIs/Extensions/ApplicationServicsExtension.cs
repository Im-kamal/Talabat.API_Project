using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Services;

namespace Talabat.APIs.Extensions
{
	public static class ApplicationServicsExtension
	{

		public static IServiceCollection AddApplicationServices(this IServiceCollection Servises)
		{
			//Servises.AddScoped<IBasketRepository, BasketRepository>();
			Servises.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));

			//builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();

			//Servises.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			Servises.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
			Servises.AddScoped(typeof(IPaymentServices), typeof(PaymentServices));
			Servises.AddScoped(typeof(IOrderServices), typeof(OrderServices));
			//builder.Services.AddAutoMapper(M=>M.AddProfile(new MappingProfiles()));
			Servises.AddAutoMapper(typeof(MappingProfiles));
			Servises.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
														 .SelectMany(P => P.Value.Errors)
														 .Select(E => E.ErrorMessage)
														 .ToList();
					var ValidationErrorResponse = new ApiValidationErroeResponse()
					{
						Errors = errors
					};
					return new BadRequestObjectResult(ValidationErrorResponse);
				};
			});

			return Servises;


		}
	}
}
