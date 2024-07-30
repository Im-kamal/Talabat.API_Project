using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications; 
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
	public class ProductsController : APIBaseController
	{
		
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
	

		public ProductsController(IMapper mapper,
		IUnitOfWork unitOfWork)
        {
			_mapper = mapper;
			this._unitOfWork = unitOfWork;
		}
		[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
		//Get All Products
		[HttpGet]  //BaseUrl/api/Products=====Get
		public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetProducts([FromQuery]ProductSpecParams Params) 
		 {
			var Spec = new ProductWithBrandAndTypeSpecifications(Params);
			var Products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
			var MappedProducts=_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(Products);
			//OkObjectResult Result=new OkObjectResult(Products);
			//return Result;
			var CountSpec = new ProductWithFilerationForCountAsync(Params);
			var Count= await _unitOfWork.Repository<Product>().GetCountIdWithSpecAsync(CountSpec);

			var ReturnedObject = new Pagination<ProductToReturnDTO>()
			{
				PageIndex = Params.PageIndex,
				PageSize = Params.PageSize,
				Count=Count,
				Data = MappedProducts
			};
			return Ok(ReturnedObject);   //Helper method
		}

		//Get Products By Id
		[HttpGet("{id}")]  //BaseUrl/api/Products/{id}
		[ProducesResponseType(typeof(ProductToReturnDTO), StatusCodes.Status200OK /*200*/)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound/*404*/)]
		public async Task<ActionResult<ProductToReturnDTO>> GetProduct(int id)
		{
			var Spec=new ProductWithBrandAndTypeSpecifications(id);
			var Product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(Spec);
			if(Product is null) return NotFound(new ApiResponse(404));
			var MappedProduct=_mapper.Map<Product, ProductToReturnDTO>(Product);

			return Ok(MappedProduct); 
		}


		[HttpGet("Types")]  // Base/api/Products/Types
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
		{
			var Types =await _unitOfWork.Repository<ProductType>().GetAllAsyn();

			return Ok(Types);
		}

		[HttpGet("Brands")]  // Base/api/Products/Types
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var Brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsyn();

			return Ok(Brands);
		}

	}
}
