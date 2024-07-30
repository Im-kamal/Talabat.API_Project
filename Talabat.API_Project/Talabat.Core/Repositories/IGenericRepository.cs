using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repositories
{
	public interface IGenericRepository<T> where T : BaseEntity
	{
		#region Without Specifications
		//Get All
		Task<IReadOnlyList<T>> GetAllAsyn();

		//Get By Id
		Task<T> GetByIdAsyn(int id);
		#endregion

		#region With Specifications
		Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec);
		Task<T> GetEntityWithSpecAsync(ISpecifications<T> Spec);
		#endregion

		Task<int> GetCountIdWithSpecAsync(ISpecifications<T> Spec);

		Task Add(T entity);

		void Delete(T entity);
		void Update(T entity);
	}
}
