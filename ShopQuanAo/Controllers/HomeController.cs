using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Areas.Admin.Models;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using System.Diagnostics;

namespace ShopQuanAo.Controllers
{
	public class LabelList
	{
		private List<product> products_best_seller;
		private List<product> products_featured;
		private List<product> products_sale;
		private List<product> products_top_rate;

		public List<product> Products_best_seller { get => products_best_seller; set => products_best_seller = value; }
		public List<product> Products_featured { get => products_featured; set => products_featured = value; }
		public List<product> Products_sale { get => products_sale; set => products_sale = value; }
		public List<product> Products_top_rate { get => products_top_rate; set => products_top_rate = value; }
	}
	public class HomeController : Controller
	{
		private readonly ShopQuanAoContext _context;

		public HomeController(ShopQuanAoContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			var items = new LabelList()
			{
				Products_best_seller = _context.product.Include(x => x.category).Where(x => x.Label == "best-seller").ToList(),
				Products_featured = _context.product.Include(x => x.category).Where(x => x.Label == "featured").ToList(),
				Products_sale = _context.product.Include(x => x.category).Where(x => x.Label == "sale").ToList(),
				Products_top_rate = _context.product.Include(x => x.category).Where(x => x.Label == "top-rate").ToList()
			};
			ViewData["Products"] = items;
			return View(items);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View();
		}

		public IActionResult About ()
		{
			return View();
		}

		public IActionResult Contact()
		{
			return View();
		}

		public IActionResult Blog ()
		{
			var blogs = _context.blog.ToList();
			return View(blogs);
		}
		public IActionResult BlogDetail (int? id)
		{
			var blog = _context.blog.FirstOrDefault(x => x.Id == id);
			return View(blog);
		}
	}
}
