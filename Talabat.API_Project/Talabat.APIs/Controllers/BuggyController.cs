﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
	public class BuggyController : APIBaseController
	{
		private readonly StoreContext _dbContext;

		public BuggyController(StoreContext dbContext) 
		{
			_dbContext = dbContext;
		}
		[HttpGet("NotFound")]   //Base/api/Buggy/NotFound 
		public ActionResult GetNotFoundRequest()
		{
			var Product=_dbContext.Products.Find(100);

			//if(Product is null) return NotFound();
			if(Product is null) return NotFound(new ApiResponse(404));
			return Ok(Product);
		}

		[HttpGet("ServerError")]
		public ActionResult GetServerError() 
		{
			var Product = _dbContext.Products.Find(100);
			var ProductToReturn = Product.ToString();   //Throw Exeption [Null Reference Exeption]
			//if (ProductToReturn is null) return NotFound(new ApiResponse(500));
			return Ok(ProductToReturn);
		}

		[HttpGet("BadRequest")]
		public ActionResult GetBadRequest() 
		{
			return BadRequest(new ApiResponse(400));
		}


		[HttpGet("BadRequest/{id}")]
		public ActionResult GetBadRequest(int id)
		{
			return Ok();
		}
	}
}
