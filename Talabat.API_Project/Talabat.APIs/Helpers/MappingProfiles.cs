using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.OrderAggregate;
using OrderAddress = Talabat.Core.Entities.OrderAggregate.Address;
using IdentityAddress = Talabat.Core.Entities.Identity.Address;
//using Address = Talabat.Core.Entities.Identity.Address;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles : Profile
	{
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDTO>()
                .ForMember(d=>d.ProductType , O=>O.MapFrom(S=>S.ProductType.Name))
                .ForMember(d=>d.ProductBrand,O=>O.MapFrom(S=>S.ProductBrand.Name))
                .ForMember(d=>d.PictureUrl ,O=>O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<IdentityAddress, AddressDto>().ReverseMap();
            CreateMap< AddressDto, OrderAddress>();
            CreateMap<CustomerBasketDto, CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

            CreateMap<Order,OrderToReturnDto>()
                .ForMember(d=>d.DeliveryMethod,o=>o.MapFrom(S=>S.DeliveryMethod.ShortName))
                .ForMember(d=>d.DeliveryMethodCost,o=>o.MapFrom(S=>S.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
                .ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());
        }
    }
}
