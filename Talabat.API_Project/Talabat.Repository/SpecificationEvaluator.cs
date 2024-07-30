using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
	public static class SpecificationEvaluator<T> where T : BaseEntity
	{
		//Fun To Build Query

		public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> Spec)
		{
			var query = inputQuery;

			if (Spec.Criteria is not null)
			{
				query = query.Where(Spec.Criteria);
			}

			if (Spec.OrderBy is not null)
			{
				query = query.OrderBy(Spec.OrderBy);
			}
			if (Spec.OrderByDescending is not null)
			{
				query = query.OrderByDescending(Spec.OrderByDescending);
			}

			if (Spec.IsPaginationEnable)
			{
				query=query.Skip(Spec.Skip).Take(Spec.Take);
			}

			query = Spec.Includes.Aggregate(query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));

			return query;
		}
	}
}
