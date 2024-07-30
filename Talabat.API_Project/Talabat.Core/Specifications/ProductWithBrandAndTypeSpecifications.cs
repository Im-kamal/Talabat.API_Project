using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class ProductWithBrandAndTypeSpecifications : BaseSpecifications<Product>
	{

        //CTOR  Is Used For Get All Products
        public ProductWithBrandAndTypeSpecifications(ProductSpecParams Params)
            :base(P=>
                (string.IsNullOrEmpty(Params.Search) ||P.Name.ToLower().Contains(Params.Search))
                &&
                (!Params.BrandId.HasValue || P.ProductBrandId == Params.BrandId)
                &&
                (!Params.TypeId.HasValue || P.ProductTypeId == Params.TypeId)
                 )
        {
            Includes.Add(p => p.ProductType);
            Includes.Add(p => p.ProductBrand);
            if (!string.IsNullOrEmpty(Params.Sort))
            {
                switch (Params.Sort)
                {
                    case "PriceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDescendig(p => p.Price);
                        break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }

            ApplyPagination(Params.PageSize * (Params.PageIndex - 1), Params.PageSize);
        }

        //CTOR Is Used For Get Product By Id
        public ProductWithBrandAndTypeSpecifications(int id) :base(P=>P.Id==id)
        {
			Includes.Add(p => p.ProductType);
			Includes.Add(p => p.ProductBrand);
		}
    }
}
