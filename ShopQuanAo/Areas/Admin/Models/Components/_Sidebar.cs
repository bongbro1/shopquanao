using Microsoft.AspNetCore.Mvc;

namespace ShopQuanAo.Areas.Admin.Models.Components
{
  public class _Sidebar : ViewComponent
  {
    public IViewComponentResult Invoke ()
    {
      return View();
    }
  }
}
