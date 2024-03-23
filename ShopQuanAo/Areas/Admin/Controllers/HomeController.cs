using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Data;

namespace ShopQuanAo.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class HomeController : Controller
  {
    private readonly ShopQuanAoContext _context;

    public HomeController(ShopQuanAoContext context)
    {
      _context = context;
    }
    public IActionResult Index()
    {
      return View();
    }
  }
}
