using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
	{ 
	
        public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria=criteriaExpression;
		}
        public BaseSpecifications()
        {
            
        }
        public Expression<Func<T, bool>> Criteria { get; set; }
		
		public List<Expression<Func<T, object>>> Includes { get ; set ; } = new List<Expression<Func<T, object>>>();
		public Expression<Func<T, object>> OrderBy { get ; set; }
		public Expression<Func<T, object>> OrderByDescending { get; set; }
		public int Take { get ; set ; }
		public int Skip { get; set; }
		public bool IsPaginationEnable { get ; set ; }

		public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
		{
			OrderBy = orderByExpression;
		}

		public void AddOrderByDescendig(Expression<Func<T, object>> orderByDescExpression)
		{
			OrderByDescending = orderByDescExpression;
		}


		public void ApplyPagination (int skip,int take)
		{
			IsPaginationEnable = true;
			Skip = skip;
			Take = take;
		}



	}
}
