using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
	
	public class AccountsController : APIBaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ITokenServices _tokenServices;
		private readonly IMapper _mapper;

		public AccountsController(UserManager<AppUser> userManager , SignInManager<AppUser> signInManager,ITokenServices tokenServices,IMapper mapper)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenServices = tokenServices;
			this._mapper = mapper;
		}
        //Register
        [HttpPost("Register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto model)
		{
			if ( CheckEmailExists(model.Email).Result.Value)
			{
				return BadRequest(new ApiResponse(400, "Email is Already Exist"));
			}
			var User = new AppUser()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				UserName = model.Email.Split('@')[0],
				PhoneNumber = model.PhoneNumber,
			};

			var Result = await _userManager.CreateAsync(User,model.Password);

			if(!Result.Succeeded) return BadRequest(new ApiResponse(400));

			var ReturnedUser = new UserDto()
			{
				DisplayName=User.DisplayName,
				Email=User.Email,
				Token =  await _tokenServices.CreateTokenAsync(User,_userManager)
			};
			return Ok(ReturnedUser);
		}

		//Login

		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto model)
		 {
			var User=await _userManager.FindByEmailAsync(model.Email);
			if(User is null) return Unauthorized(new ApiResponse(401));
			var Result= await _signInManager.CheckPasswordSignInAsync(User, model.Password,false);
			if(!Result.Succeeded) return Unauthorized(new ApiResponse(401));

			return Ok(new UserDto()
			{ 
				DisplayName = User.DisplayName,
				Email = User.Email,
				Token =  await _tokenServices.CreateTokenAsync(User,_userManager)

			});
		}

		[Authorize]
		[HttpGet("GetCurrentUser")]   // base /api / Accounts/GetCurrentUser
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var Email =  User.FindFirstValue(ClaimTypes.Email);              //User => from Claims This User Login    [Built in]

			var user = await _userManager.FindByEmailAsync(Email);

			var ReturnedObject = new UserDto()
			{
				DisplayName=user.DisplayName,
				Email = user.Email,
				Token = await _tokenServices.CreateTokenAsync(user,_userManager)
			};
			return Ok(ReturnedObject);
		}

		[Authorize]
		[HttpGet("Address")]
		public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
		{
			//var Email=User.FindFirstValue(ClaimTypes.Email);
			//var user=await _userManager.FindByEmailAsync(Email);
			var user = await _userManager.FindUserWithAddressAsync(User);
			var MappesAddress =	_mapper.Map<Address,AddressDto>(user?.Address);
			return Ok(MappesAddress);
		}

		[Authorize]
		[HttpPut("Address")]
		public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto UpdatedAddress)
		{
			
			var user = await _userManager.FindUserWithAddressAsync(User);
			var MappedAddress = _mapper.Map<AddressDto, Address>(UpdatedAddress);
			MappedAddress.Id=user.Address.Id;
			user.Address = MappedAddress;
			var Result=   await _userManager.UpdateAsync(user);
			if (!Result.Succeeded) return BadRequest(new ApiResponse(400));
			return Ok(UpdatedAddress);

		}

		[HttpGet("emailExists")]

		public async Task<ActionResult<bool>> CheckEmailExists(string Email)
		{
			//var User = await _userManager.FindByEmailAsync(Email);
			//if (User is null) return false;
			//else return true;
			return await _userManager.FindByEmailAsync(Email) is not null;
		}


	}
}
