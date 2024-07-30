using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly StoreContext _dbContext;

		public GenericRepository(StoreContext dbContext)   //Ask CLR for Creating Object from StoreContext Implicitly
        {
			_dbContext = dbContext;
		}
		#region Without Specifications
		public async Task<IReadOnlyList<T>> GetAllAsyn()
		{
			//if (typeof(T) == typeof(Product))
			//{
			//	return (IReadOnlyList<T>)await _dbContext.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).ToListAsync();
			//}
			return await _dbContext.Set<T>().ToListAsync();
		}


		public async Task<T> GetByIdAsyn(int id)

			//return await _dbContext.Set<T>().Where(X=>X.Id==id).FirstOrDefaultAsync();
			=> await _dbContext.Set<T>().FindAsync(id);  // Find  => To Search Localy The First If not Found Localy then Search In Database

		#endregion
		#region With Specifications
		public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecifications(Spec).ToListAsync();
		}
		public async Task<T> GetEntityWithSpecAsync(ISpecifications<T> Spec)
		{
			return  await ApplySpecifications(Spec).FirstOrDefaultAsync();
		} 
		#endregion 

		private IQueryable<T> ApplySpecifications(ISpecifications<T> Spec)
		{
			return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), Spec);
		}

		public async Task<int> GetCountIdWithSpecAsync(ISpecifications<T> Spec)
		{
			return await ApplySpecifications(Spec).CountAsync();
		}

		public async Task Add(T entity)
		{
			await _dbContext.Set<T>().AddAsync(entity);	
		}

		public void Delete(T entity)
			=>_dbContext.Set<T>().Remove(entity);
		
		

		public void Update(T entity)
		=>_dbContext.Set<T>().Update(entity);
	}
}
