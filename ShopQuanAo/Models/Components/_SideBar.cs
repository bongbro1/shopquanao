using Microsoft.AspNetCore.Mvc;

namespace ShopQuanAo.Models.Components
{
	public class _SideBar : ViewComponent
	{
		public IViewComponentResult Invoke ()
		{
			return View();
		}
	}
}
