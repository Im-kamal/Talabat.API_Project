using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Repository.Data.Configurations
{
	internal class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.Property(O => O.Status)
				.HasConversion(OS => OS.ToString(), OS => (OrderStatus) Enum.Parse(typeof(OrderStatus),OS));
			builder.Property(O => O.SubTotal)
				.HasColumnType("decimal(18,2)");

			builder.OwnsOne(O => O.ShippingAddress, SA => SA.WithOwner());

			builder.HasOne(O => O.DeliveryMethod)
				.WithMany()
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
