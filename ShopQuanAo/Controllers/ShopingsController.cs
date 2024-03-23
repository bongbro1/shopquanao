using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using ShopQuanAo.Areas.Admin.Controllers;
using ShopQuanAo.Areas.Admin.Models;
using ShopQuanAo.Data;
using System.Security.Claims;
using System.Net.Http.Headers;
using Azure.Core;
using ShopQuanAo.Models;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.RenderTree;

namespace ShopQuanAo.Controllers
{
	public class Province
	{
		public string Code { get; set; }
		public string Name { get; set; }
	}
	public class CartIdListRequest
	{
		public string CartIdList { get; set; }
	}
	public class ShopingsController : Controller
  {
		private readonly ShopQuanAoContext _context;
		private readonly IProductService _productService;
		private readonly ICartService _cartService;
		private readonly IUserService _userService;
		private readonly INotyfService _notyf;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public ShopingsController(ShopQuanAoContext context, INotyfService notyf, IProductService productService, IUserService userService, ICartService cartService, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_notyf = notyf;
			_productService = productService;
			_userService = userService;
			_cartService = cartService;
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task<IActionResult> ShopingCart()
		{
			return View();
		}

		[HttpPost("/save-cart-ids")]
		public IActionResult SaveCartIds([FromBody] CartIdListRequest request)
		{
			// Lưu cartIdList vào Session
			_httpContextAccessor.HttpContext.Session.SetString("cartIdListChecked", request.CartIdList);
			return Ok("");
		}

		[HttpPost]
		public IActionResult AddToCart(int productIdOrder, string? sizeOrder, string? colorOrder, int? quantityOrder)
		{
			string referer = "";
			int quantity = quantityOrder ?? 1;
			string size = sizeOrder ?? "S";
			string color = colorOrder ?? "Xanh";
			if (User.Identity.IsAuthenticated)
			{
				if (size != null && color != null && quantity != 0)
				{
					var product = _productService.getProductById(productIdOrder);
					int userId = int.Parse(User.FindFirstValue("Id"));
					cart cartOld = _context.cart.Where(x => x.UserId == userId && x.ProductId == product.Id && x.ColorOrder == colorOrder && x.SizeOrder == sizeOrder && x.Status == false).FirstOrDefault();
					if (cartOld != null)
					{
						cartOld.Quantity += quantity;
						cartOld.Total += (float)(product.Price * quantity);
						_context.Update(cartOld);
						_context.SaveChanges();
					}
					else
					{
						cart cart = new cart()
						{
							ProductId = product.Id,
							UserId = userId,
							Quantity = quantity,
							ColorOrder = color,
							SizeOrder = size,
							Total = (float)(product.Price * quantity),
							Status = false
						};
						_context.Add(cart);
						_context.SaveChanges();
					}
					_notyf.Success("Thêm vào giỏ hàng thành công!");
				}
				else
				{
					_notyf.Error("Vui lòng chọn size và color!");
				}
			}
			else
			{
				_notyf.Error("Vui lòng đăng nhập để sử dụng!");
			}
			referer = Request.Headers["Referer"].ToString();
			// Chuyển hướng đến trang trước đó
			return Redirect(referer);
		}

		[HttpPost]
		public IActionResult UpdateCart (int cartId, int productId, int quantity, string? type)
		{
			var product = _productService.getProductById(productId);
			cart cartOld = _cartService.getCartById(cartId);
			if (cartOld != null)
			{
				if (type == "WriteQuantity")
				{
					cartOld.Quantity = quantity;
					cartOld.Total = (float)(product.Price * quantity);
				}
				else
				{
					cartOld.Quantity += quantity;
					cartOld.Total += (float)(product.Price * quantity);
				}
				
				_context.Update(cartOld);
				_context.SaveChanges();
			}
			string referer = Request.Headers["Referer"].ToString();
			// Chuyển hướng đến trang trước đó
			return Redirect(referer);
		}

		[HttpGet]
		public IActionResult RemoveFromCart(int id)
		{
			cart cart = _cartService.getCartById(id);
			_context.Remove(cart);
			_context.SaveChanges();

			string referer = Request.Headers["Referer"].ToString();
			// Chuyển hướng đến trang trước đó
			return Redirect(referer);
		}


		[HttpGet]
		public IActionResult Checkout()
		{
			// Lấy cartIdList từ Session
			var cartIdListJson = _httpContextAccessor.HttpContext.Session.GetString("cartIdListChecked");

			// Chuyển đổi chuỗi JSON thành danh sách cartIdList
			var cartIdList = new List<string>();
			if (cartIdListJson != null)
			{
				cartIdList = JsonConvert.DeserializeObject<List<string>>(cartIdListJson);
			}
			if (cartIdList.Count == 0)
			{
				_notyf.Information("Vui lòng chọn sản phẩm!");
				string referer = Request.Headers["Referer"].ToString();

				// Chuyển hướng đến trang trước đó
				return Redirect(referer);
			}
			// Truyền cartIdList qua view thông qua model hoặc ViewBag
			ViewBag.CartIdList = cartIdList;

			var listCountry = _context.countries.ToList();
			ViewData["Countries"] = new SelectList(listCountry, "Id", "Country_name");
			return View();
		}

		[HttpPost]
		public IActionResult Checkout(string Fullname, string Phone, int Country, int State, int City, string? Specific_address, double total_Price)
		{
			if (User.Identity.IsAuthenticated)
			{
				if (Country != 0 && City != 0 && State != 0)
				{
					int userId = int.Parse(User.FindFirstValue("Id"));
					user user = _userService.getUserByUserId(userId);

					Order order = new Order()
					{
						User_Id = userId,
						totalPrice = total_Price,
						order_date = DateTime.Now,
						order_approval = null,
						Fullname = Fullname,
						Phone = Phone,
						City_Address = _context.cities.Where(x => x.Id == City).FirstOrDefault().City_name,
						Country_address = _context.countries.Where(x => x.Id == Country).FirstOrDefault().Country_name,
						State_address = _context.states.Where(x => x.Id == State).FirstOrDefault().State_name,
						Specific_address = Specific_address,
						Status = false
					};
					_context.Add(order);
					_context.SaveChanges();
					int order_Id = _context.order.Where(x => x.User_Id == userId).OrderByDescending(x => x.Id).FirstOrDefault().Id;

					var cartIdListJson = _httpContextAccessor.HttpContext.Session.GetString("cartIdListChecked");
					var cartIdList = new List<string>();
					cartIdList = JsonConvert.DeserializeObject<List<string>>(cartIdListJson);
					foreach (var cartId in cartIdList)
					{
						var cart = _cartService.getCartById(int.Parse(cartId));
						Order_Items order_items = new Order_Items()
						{
							Cart_Id = cart.Id,
							Order_id = order_Id
						};
						_context.Add(order_items);
						_context.SaveChanges();
					}
					_userService.SendVerificationLinkEmail("dtc21h4802010193@ictu.edu.vn", "", "NewOrder", order_Id);

          return Redirect("/Shopings/OrderNotification");
				}
				else
				{
					_notyf.Error("Vui lòng điền đầy đủ thông tin nhận hàng!");

					string referer = Request.Headers["Referer"].ToString();

					// Chuyển hướng đến trang trước đó
					return Redirect(referer);
				}
			}
			else if (!User.Identity.IsAuthenticated)
			{
				_notyf.Error("Vui lòng đăng nhập để sử dụng");
				return Redirect("/users/login");
			}

			return Redirect("/home/error");
		}


    public IActionResult OrderNotification ()
		{
			return View();
		}
	}
}
