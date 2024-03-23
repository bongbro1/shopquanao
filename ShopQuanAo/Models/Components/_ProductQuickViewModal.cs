using Microsoft.AspNetCore.Mvc;

namespace ShopQuanAo.Models.Components
{
	public class _ProductQuickViewModal : ViewComponent
	{
		public IViewComponentResult Invoke ()
		{
			return View();
		}
	}
}
