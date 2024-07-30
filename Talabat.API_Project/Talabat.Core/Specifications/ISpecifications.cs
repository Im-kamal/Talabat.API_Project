using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public interface ISpecifications<T> where T : BaseEntity
	{
        //_dbContext.Products.Where(X=>X.Id==id).Include(p=>p.ProductBrand).Include(p=>p.ProductType)

        //Signuture for property for Where Condition
        public Expression<Func<T, bool>> Criteria { get; set; }
        //Signuture for property for List of Include 
        public List<Expression<Func<T,object>>> Includes { get; set; }

        //property For OrderBy

        public Expression<Func<T,object>> OrderBy { get; set; }

		//property For OrderByDesc

		public Expression<Func<T, object>> OrderByDescending { get; set; }


        //Take()

        public int Take { get; set; }

		//Skip()

		public int Skip { get; set; }


        public bool IsPaginationEnable { get; set; }


    }
}
