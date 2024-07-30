using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data
{
	public static class StoreContextSeed
	{
		//Seeding
		public static async Task SeedAsync(StoreContext dbContext)
		{

			//Seeding Brands
			if (!dbContext.ProductBrands.Any())
			{
				var BrandsData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/brands.json");
				var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);
				if (Brands?.Count > 0)
				{
					foreach (var Brand in Brands)
						await dbContext.Set<ProductBrand>().AddAsync(Brand);
					await dbContext.SaveChangesAsync();
				}
			}


			// Seeding Types

			if (!dbContext.ProductTypes.Any())
			{

				var TypesData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/types.json");
				var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);
				if (Types?.Count > 0)
				{
					foreach (var Type in Types)
						await dbContext.Set<ProductType>().AddAsync(Type);
					await dbContext.SaveChangesAsync();

				}
			}


			// Seeding Products
			if (!dbContext.Products.Any())
			{
				var ProductsData = File.ReadAllText("../Talabat.Repository/Data/DataSeeding/products.json");
				var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
				if (Products?.Count > 0)
				{
					foreach (var product in Products)
						await dbContext.Set<Product>().AddAsync(product);
					await dbContext.SaveChangesAsync();

				}
			}



		}
	}
}
