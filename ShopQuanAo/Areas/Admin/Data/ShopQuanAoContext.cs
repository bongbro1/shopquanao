using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Areas.Admin.Models;

namespace ShopQuanAo.Data
{
	public class ShopQuanAoContext : DbContext
	{
		public ShopQuanAoContext(DbContextOptions<ShopQuanAoContext> options)
				: base(options)
		{
		}

		public DbSet<ShopQuanAo.Areas.Admin.Models.user> user { get; set; }
		public DbSet<ShopQuanAo.Areas.Admin.Models.category> category { get; set; }
		public DbSet<ShopQuanAo.Areas.Admin.Models.product> product { get; set; }
		public DbSet<ShopQuanAo.Areas.Admin.Models.cart> cart { get; set; }
		public DbSet<ShopQuanAo.Models.Countries> countries { get; set; }
		public DbSet<ShopQuanAo.Models.States> states { get; set; }
		public DbSet<ShopQuanAo.Models.Cities> cities { get; set; }
		public DbSet<ShopQuanAo.Models.Order> order { get; set; }
		public DbSet<ShopQuanAo.Models.Order_Items> order_Items { get; set; }
	    public DbSet<ShopQuanAo.Areas.Admin.Models.blog> blog { get; set; } = default!;
	}
}
