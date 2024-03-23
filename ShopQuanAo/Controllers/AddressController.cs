using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Controllers
{
	public class AddressController : Controller
	{
		private readonly ShopQuanAoContext _context;

		public AddressController(ShopQuanAoContext context)
		{
			_context = context;
		}
		public IActionResult Index()
		{
			return View();
		}

		public List<States> GetStates (int country_Id)
		{
			var listState = _context.states.Where(x => x.Country_id == country_Id).ToList();
			return listState;
		}
		public List<Cities> GetCities (int state_Id)
		{
			var listCity = _context.cities.Where(x => x.State_id == state_Id).ToList();
			return listCity;
		}
	}
}
