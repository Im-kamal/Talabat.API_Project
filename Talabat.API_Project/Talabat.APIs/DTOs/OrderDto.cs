using System.ComponentModel.DataAnnotations;

namespace Talabat.APIs.DTOs
{
	public class OrderDto
	{
		[Required]
		public string BasketId { get; set; }
		public int DeliveryMethodId { get; set; }
		public AddressDto ShippingAddress { get; set; }
	}
}
