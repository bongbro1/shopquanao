using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using NuGet.Common;
using ShopQuanAo.Areas.Admin.Models;
using ShopQuanAo.Data;
using File = System.IO.File;

namespace ShopQuanAo.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class productsController : Controller
	{
		private readonly ShopQuanAoContext _context;
		private readonly IWebHostEnvironment _hostingEnvironment;

		public productsController(ShopQuanAoContext context, IWebHostEnvironment hostingEnvironment)
		{
			_context = context;
			_hostingEnvironment = hostingEnvironment;
		}

		// GET: Admin/products
		public async Task<IActionResult> Index()
		{
			var shopQuanAoContext = _context.product.Include(p => p.category);
			return View(await shopQuanAoContext.ToListAsync());
		}

		// GET: Admin/products/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.product
					.Include(p => p.category)
					.FirstOrDefaultAsync(m => m.Id == id);
			if (product == null)
			{
				return NotFound();
			}

			return View(product);
		}

		// GET: Admin/products/Create
		public IActionResult Create()
		{
			ViewData["CategoryId"] = new SelectList(_context.category, "Id", "Name");
			var labels = new List<SelectListItem>
			{
					new SelectListItem { Value = "best-seller", Text = "Best Seller" },
					new SelectListItem { Value = "featured", Text = "Featured" },
					new SelectListItem { Value = "sale", Text = "Sale" },
					new SelectListItem { Value = "top-rate", Text = "Top Rate" }
			};
			ViewData["Label"] = labels;
			var sizes = new List<SelectListItem>
			{
				new SelectListItem {Value = "S", Text = "S"},
				new SelectListItem {Value = "M", Text = "M"},
				new SelectListItem {Value = "L", Text = "L"},
				new SelectListItem {Value = "XL", Text = "XL"},
			};
			ViewData["Size"] = sizes;

			var colors = new List<SelectListItem>
			{
        new SelectListItem { Text = "Xanh lá cây", Value = "green" },
        new SelectListItem { Text = "Đỏ", Value = "red" },
        new SelectListItem { Text = "Xanh dương", Value = "blue" },
        new SelectListItem { Text = "Vàng", Value = "yellow" },
        new SelectListItem { Text = "Tím", Value = "purple" },
        new SelectListItem { Text = "Cam", Value = "orange" },
        new SelectListItem { Text = "Hồng", Value = "pink" }
      };
      ViewData["Color"] = colors;

      return View();
		}

		// POST: Admin/products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> Create([Bind("Id,Name,Description,Images,Price,Quantity,CategoryId,Size,Color,Label")] product product, IFormFile[] fileInputs)
		{
			if (ModelState.IsValid)
			{
				if (fileInputs != null && fileInputs.Length > 0)
				{
					// Xử lý tệp đã tải lên
					foreach (var fileInput in fileInputs)
					{
						var extension = Path.GetExtension(fileInput.FileName);
						var fileName = Guid.NewGuid().ToString().Substring(0, 10 - extension.Length) + extension;
						var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
						var fileOld = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileInput.FileName);
						// Kiểm tra xem tệp có tồn tại trong thư mục images hay không
						if (System.IO.File.Exists(fileOld))
						{
							// Nếu tệp đã tồn tại, không cần tạo tên mới
							product.Images += (fileInput.FileName + ", ");
						}
						else
						{
							// Nếu tệp không tồn tại, tạo một tên file mới và lưu tệp vào thư mục
							using (var stream = new FileStream(filePath, FileMode.Create))
							{
								await fileInput.CopyToAsync(stream);
							}
							// Lưu đường dẫn của tệp vào model
							product.Images += (fileName + ", ");
						}
					}
				}
				product.Images = product.Images.Substring(0, product.Images.Length - 2);
				_context.Add(product);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
      return View(product);
		}

		// GET: Admin/products/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.product.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.category, "Id", "Name", product.CategoryId);
      var labels = new List<SelectListItem>
      {
          new SelectListItem { Value = "best-seller", Text = "Best Seller" },
          new SelectListItem { Value = "featured", Text = "Featured" },
          new SelectListItem { Value = "sale", Text = "Sale" },
          new SelectListItem { Value = "top-rate", Text = "Top Rate" }
      };
      ViewData["Label"] = labels;
      var sizes = new List<SelectListItem>
      {
        new SelectListItem {Value = "S", Text = "S"},
        new SelectListItem {Value = "M", Text = "M"},
        new SelectListItem {Value = "L", Text = "L"},
        new SelectListItem {Value = "XL", Text = "XL"},
      };
      ViewData["Size"] = sizes;

      var colors = new List<SelectListItem>
      {
        new SelectListItem { Text = "Xanh lá cây", Value = "green" },
        new SelectListItem { Text = "Đỏ", Value = "red" },
        new SelectListItem { Text = "Xanh dương", Value = "blue" },
        new SelectListItem { Text = "Vàng", Value = "yellow" },
        new SelectListItem { Text = "Tím", Value = "purple" },
        new SelectListItem { Text = "Cam", Value = "orange" },
        new SelectListItem { Text = "Hồng", Value = "pink" }
      };
      ViewData["Color"] = colors;
      return View(product);
		}

		// POST: Admin/products/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Images,Price,Quantity,CategoryId,Size,Color,Label")] product product, IFormFile[] fileInputs)
		{
			if (id != product.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					if (fileInputs.Count() > 0)
					{
						product.Images = "";
						foreach (var fileInput in fileInputs)
						{
							var extension = Path.GetExtension(fileInput.FileName);
							var fileName = Guid.NewGuid().ToString().Substring(0, 10 - extension.Length) + extension;
							var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileName);
							var fileOld = Path.Combine(_hostingEnvironment.WebRootPath, "images", fileInput.FileName);
							// Kiểm tra xem tệp có tồn tại trong thư mục images hay không
							if (System.IO.File.Exists(fileOld))
							{
								// Nếu tệp đã tồn tại, không cần tạo tên mới
								product.Images += (fileInput.FileName + ", ");
							}
							else
							{
								// Nếu tệp không tồn tại, tạo một tên file mới và lưu tệp vào thư mục
								using (var stream = new FileStream(filePath, FileMode.Create))
								{
									await fileInput.CopyToAsync(stream);
								}
								// Lưu đường dẫn của tệp vào model
								product.Images += (fileName + ", ");
							}
						}
						product.Images = product.Images.Substring(0, product.Images.Length - 2);
					}
					_context.Update(product);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!productExists(product.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
      
      return View(product);
		}

		// GET: Admin/products/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.product
					.Include(p => p.category)
					.FirstOrDefaultAsync(m => m.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.category, "Id", "Name", product.CategoryId);
      var labels = new List<SelectListItem>
      {
          new SelectListItem { Value = "best-seller", Text = "Best Seller" },
          new SelectListItem { Value = "featured", Text = "Featured" },
          new SelectListItem { Value = "sale", Text = "Sale" },
          new SelectListItem { Value = "top-rate", Text = "Top Rate" }
      };
      ViewData["Label"] = labels;
      var sizes = new List<SelectListItem>
      {
        new SelectListItem {Value = "S", Text = "S"},
        new SelectListItem {Value = "M", Text = "M"},
        new SelectListItem {Value = "L", Text = "L"},
        new SelectListItem {Value = "XL", Text = "XL"},
      };
      ViewData["Size"] = sizes;

      var colors = new List<SelectListItem>
      {
        new SelectListItem { Text = "Xanh lá cây", Value = "green" },
        new SelectListItem { Text = "Đỏ", Value = "red" },
        new SelectListItem { Text = "Xanh dương", Value = "blue" },
        new SelectListItem { Text = "Vàng", Value = "yellow" },
        new SelectListItem { Text = "Tím", Value = "purple" },
        new SelectListItem { Text = "Cam", Value = "orange" },
        new SelectListItem { Text = "Hồng", Value = "pink" }
      };
      ViewData["Color"] = colors;
      return View(product);
		}

		// POST: Admin/products/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var product = await _context.product.FindAsync(id);
			if (product != null)
			{
				_context.product.Remove(product);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool productExists(int id)
		{
			return _context.product.Any(e => e.Id == id);
		}
	}
}
