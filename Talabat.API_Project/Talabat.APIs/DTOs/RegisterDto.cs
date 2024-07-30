using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
	public class RegisterDto
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		public string DisplayName { get; set; } = null!;
		[Required]
		[Phone]
		public string? PhoneNumber { get; set; } = null!;
		[Required]
		[RegularExpression("(?=^.{6,10}$)(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*()_+]).*$",
			ErrorMessage = "Password Must contain 1 Uppercase , 1 Lowercase ,1 Digit ,1 Special Chatracters,At Least 6 Characters")]
		public string Password { get; set; } = null!;
	}
}
