using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Areas.Admin.Models;
using ShopQuanAo.Data;

namespace ShopQuanAo.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class usersController : Controller
	{
		private readonly ShopQuanAoContext _context;

		public usersController(ShopQuanAoContext context)
		{
			_context = context;
		}

		// GET: Admin/users
		public async Task<IActionResult> Index()
		{
			return View(await _context.user.ToListAsync());
		}

		// GET: Admin/users/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var user = await _context.user
					.FirstOrDefaultAsync(m => m.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			return View(user);
		}

		// GET: Admin/users/Create
		public IActionResult Create()
		{
			var items = new List<SelectListItem>
		{
				new SelectListItem { Value = "QTV", Text = "Quản trị viên" },
				new SelectListItem { Value = "NV", Text = "Nhân viên" },
				new SelectListItem { Value = "KH", Text = "Khách hàng" }
		};

			ViewData["Role"] = items;
			return View();
		}

		// POST: Admin/users/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Role,Phone,Address,ResetPassword,VerifyAccount")] user user)
		{
			if (ModelState.IsValid)
			{
				_context.Add(user);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(user);
		}

		// GET: Admin/users/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var user = await _context.user.FindAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			var items = new List<SelectListItem>
		{
				new SelectListItem { Value = "QTV", Text = "Quản trị viên" },
				new SelectListItem { Value = "NV", Text = "Nhân viên" },
				new SelectListItem { Value = "KH", Text = "Khách hàng" }
		};

			ViewData["Role"] = items;
			return View(user);
		}

		// POST: Admin/users/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,Role,Phone,Address,ResetPassword,VerifyAccount")] user user)
		{
			if (id != user.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(user);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!userExists(user.Id))
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
			var items = new List<SelectListItem>
		{
				new SelectListItem { Value = "QTV", Text = "Quản trị viên" },
				new SelectListItem { Value = "NV", Text = "Nhân viên" },
				new SelectListItem { Value = "KH", Text = "Khách hàng" }
		};

			ViewData["Role"] = items;
			return View(user);
		}

		// GET: Admin/users/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var user = await _context.user
					.FirstOrDefaultAsync(m => m.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			return View(user);
		}

		// POST: Admin/users/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var user = await _context.user.FindAsync(id);
			if (user != null)
			{
				_context.user.Remove(user);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool userExists(int id)
		{
			return _context.user.Any(e => e.Id == id);
		}
	}
}
