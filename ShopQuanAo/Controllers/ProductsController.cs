using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Areas.Admin.Models;
using ShopQuanAo.Data;
using X.PagedList;

namespace ShopQuanAo.Controllers
{
	public interface IProductService
	{
		public product getProductById (int id);
	}
	public class ProductsController : Controller, IProductService
	{
		private readonly ShopQuanAoContext _context;
		private readonly INotyfService _notyf;


		public ProductsController(ShopQuanAoContext context, INotyfService notyf)
		{
			_context = context;
			_notyf = notyf;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public IActionResult Shop(string? keySearch, int? page, int? pageSize, string? sortby, string? priceZone, string? color, string? cateName)
		{
			pageSize = pageSize ?? 8;
			int pageNumber = (page ?? 1);
			ViewBag.pageSize = pageSize;
			ViewBag.pageNumber = pageNumber;
			ViewBag.sortby = sortby;
			ViewBag.priceZone = priceZone;
			ViewBag.color = color;

      // Get all products initially
      var products = _context.product.Include(x => x.category).AsQueryable();
			if (cateName != null)
			{
				products = products.Where(x => x.category.Name.ToLower() == cateName);
			}
      // Apply search filter if keySearch is provided
      if (!string.IsNullOrEmpty(keySearch))
      {
        products = products.Where(x => x.Name.ToLower().Contains(keySearch.Trim().ToLower()));
      }

      // Apply sorting
      switch (sortby)
      {
        case "price_increase":
          products = products.OrderBy(x => x.Price);
          break;
        case "price_decrease":
          products = products.OrderByDescending(x => x.Price);
          break;
        default:
          products = products.OrderBy(x => x.Id);
          break;
      }

      // Apply price filter
      if (priceZone != "all" && priceZone != "moreThan200" && priceZone != null)
      {
        int minPrice = int.Parse(priceZone.Split("_")[0]);
        int maxPrice = int.Parse(priceZone.Split("_")[1]);
        products = products.Where(x => x.Price >= minPrice && x.Price < maxPrice);
      }
      else if (priceZone == "moreThan200")
      {
        products = products.Where(x => x.Price >= 200);
      }

      
      // color
      if (color != null && color != "all")
			{
        products = products.Where(x => x.Color == color);
      }
      return View(products.ToPagedList(pageNumber, (int)pageSize));
    }
		public IActionResult Fillter(string? keySearch, string? sortby, string? priceZone, string? color, int? page, int? pageSize)
		{
      var url = "";
			if (keySearch == null)
			{
				url = "/products/shop";
				if (sortby != null)
				{
					url = $"/products/shop?sortby={sortby}&priceZone={priceZone}&color={color}&page={page}&pageSize={pageSize}";
				}
			}
			else
			{
				url = $"/products/shop/?keySearch={keySearch}";
			}
			return Json(new { redirectToUrl = url });
		}

		[HttpPost]
		public IActionResult ShowProductQuickViewModal(int productId)
		{
			product product = _context.product.Where(x => x.Id == productId).FirstOrDefault();
			// Xử lý dữ liệu ở đây
			return PartialView("_ProductQuickViewModal", product);
		}

		public IActionResult Details (int id)
		{
			var product = _context.product.Include(x => x.category).Where(x => x.Id == id).FirstOrDefault();
			var items = _context.product.Include(x => x.category).Where(x => x.CategoryId == product.CategoryId && x.Id != id).ToList();
			ViewData["RelatedProducts"] = items;
			return View(product);
		}


		public product getProductById(int id)
		{
			var product = _context.product.Include(x => x.category).Where(x => x.Id == id).FirstOrDefault();
			return product;
		}
	}
}
