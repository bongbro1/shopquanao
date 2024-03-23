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
	public interface ICartService
	{
		public List<cart> getListCartByUserId(int userId);
		public int getCartCountByUserId(int userId);
		public cart getCartById(int id);
	}
	[Area("Admin")]
	
	public class cartsController : Controller, ICartService
	{
		private readonly ShopQuanAoContext _context;

		public cartsController(ShopQuanAoContext context)
		{
			_context = context;
		}

		// GET: Admin/carts
		public async Task<IActionResult> Index()
		{
			return View(await _context.cart.ToListAsync());
		}

		// GET: Admin/carts/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var cart = await _context.cart
					.FirstOrDefaultAsync(m => m.Id == id);
			if (cart == null)
			{
				return NotFound();
			}

			return View(cart);
		}

		// GET: Admin/carts/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Admin/carts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,ProductId,UserId,Quantity,Total,Status")] cart cart)
		{
			if (ModelState.IsValid)
			{
				_context.Add(cart);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(cart);
		}

		// GET: Admin/carts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var cart = await _context.cart.FindAsync(id);
			if (cart == null)
			{
				return NotFound();
			}
			return View(cart);
		}

		// POST: Admin/carts/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,UserId,Quantity,Total,Status")] cart cart)
		{
			if (id != cart.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(cart);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!cartExists(cart.Id))
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
			return View(cart);
		}

		// GET: Admin/carts/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var cart = await _context.cart
					.FirstOrDefaultAsync(m => m.Id == id);
			if (cart == null)
			{
				return NotFound();
			}

			return View(cart);
		}

		// POST: Admin/carts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var cart = await _context.cart.FindAsync(id);
			if (cart != null)
			{
				_context.cart.Remove(cart);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool cartExists(int id)
		{
			return _context.cart.Any(e => e.Id == id);
		}

		public List<cart> getListCartByUserId(int userId)
		{
			var listCart = _context.cart.Where(x => x.UserId == userId && x.Status == false).ToList();
			return listCart;
		}
		public int getCartCountByUserId(int userId)
		{
			if (userId == 0)
				return 0;
			else
			{
				int count = _context.cart.Where(x => x.UserId == userId && x.Status == false).Count();
				return count;
			}
		}
		public cart getCartById(int id)
		{
			var cart = _context.cart.Where(x => x.Id == id).FirstOrDefault();
			return cart;
		}
	}
}
