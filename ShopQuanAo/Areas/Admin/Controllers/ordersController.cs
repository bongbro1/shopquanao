using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Areas.Admin.Models;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers
{
  public interface IOrderService
  {
    public List<int> getListCartIdByOrderId(int orderId);
    public Order getOrderById(int id);
  }
  [Area("Admin")]
  public class ordersController : Controller, IOrderService
  {
    private readonly ShopQuanAoContext _context;

    public ordersController(ShopQuanAoContext context)
    {
      _context = context;
    }

    // GET: Admin/Orders
    public async Task<IActionResult> Index()
    {
      return View(await _context.order.ToListAsync());
    }

    // GET: Admin/Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var order = await _context.order
          .FirstOrDefaultAsync(m => m.Id == id);
      if (order == null)
      {
        return NotFound();
      }

      return View(order);
    }

    // GET: Admin/Orders/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Admin/Orders/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,User_Id,totalPrice,order_date,order_approval,Country_address,State_address,City_Address,Specific_address,Status")] Order order)
    {
      if (ModelState.IsValid)
      {
        _context.Add(order);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(order);
    }

    // GET: Admin/Orders/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var order = await _context.order.FindAsync(id);
      if (order == null)
      {
        return NotFound();
      }
      return View(order);
    }

    // POST: Admin/Orders/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,User_Id,totalPrice,order_date,order_approval,Country_address,State_address,City_Address,Specific_address,Status")] Order order)
    {
      if (id != order.Id)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(order);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!OrderExists(order.Id))
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
      return View(order);
    }

    // GET: Admin/Orders/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var order = await _context.order
          .FirstOrDefaultAsync(m => m.Id == id);
      if (order == null)
      {
        return NotFound();
      }

      return View(order);
    }

    // POST: Admin/Orders/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var order = await _context.order.FindAsync(id);
      var orderItems = _context.order_Items.Where(x => x.Order_id == id).ToList();
      if (orderItems.Count > 0)
      {
        foreach (var item in orderItems)
        {
          _context.Remove(item);
          _context.SaveChanges();
        }
      }
      if (order != null)
      {
        _context.order.Remove(order);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(int id)
    {
      return _context.order.Any(e => e.Id == id);
    }

    [HttpGet]
    public IActionResult DuyetDon(int orderId)
    {
      var order = _context.order.Where(x => x.Id == orderId).FirstOrDefault();
      order.Status = true;
      order.order_approval = DateTime.Now;
      _context.Update(order);
      _context.SaveChanges();
      return Redirect("/admin/orders");
    }

    [HttpGet]
    public IActionResult HuyDon(int orderId)
    {
      var order = _context.order.Where(x => x.Id == orderId).FirstOrDefault();
      order.Status = false;
      order.order_approval = null;
      _context.Update(order);
      _context.SaveChanges();
      return Redirect("/admin/orders");
    }


    public Order getOrderById(int id)
    {
      Order order = _context.order.Where(x => x.Id == id).FirstOrDefault();
      return order;
    }
    public List<int> getListCartIdByOrderId(int orderId)
    {
      List<int> result = new List<int>();
      var listOrder_Item = _context.order_Items.Where(x => x.Order_id == orderId).ToList();
      foreach (var item in listOrder_Item)
      {
        result.Add(item.Cart_Id);
      }
      return result;
    }
  }
}
