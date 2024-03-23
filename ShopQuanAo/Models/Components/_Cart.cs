using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Data;

namespace ShopQuanAo.Models.Components
{
	public class _Cart: ViewComponent
	{
		private readonly ShopQuanAoContext _context;
		public _Cart (ShopQuanAoContext context)
		{
			_context = context;
		}
		public IViewComponentResult Invoke ()
		{
			return View();
		}
	}
}
