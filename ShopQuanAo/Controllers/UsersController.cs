using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopQuanAo.Areas.Admin.Models;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using ShopQuanAo.Areas.Admin.Controllers;

namespace ShopQuanAo.Controllers
{
  public interface IUserService
  {
    public user getUserByUserId(int userId);
    public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor, int? orderId);

  }
  public class UsersController : Controller, IUserService
  {
    private readonly ShopQuanAoContext _context;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    public UsersController(ShopQuanAoContext context, ICartService cartService, IProductService productService, IOrderService orderService)
    {
      _context = context;
      _cartService = cartService;
      _productService = productService;
      _orderService = orderService;
    }
    public IActionResult Index()
    {
      return View();
    }
    public IActionResult MyAccount()
    {
      if (User.Identity.IsAuthenticated)
      {
        int userId = int.Parse(User.FindFirstValue("Id"));
        user user = getUserByUserId(userId);
        return View(user);
      }
      return Redirect("/users/login");
    }

    public IActionResult Login()
    {
      return View();
    }
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
      var user = _context.user.Where(x => x.Email == email && x.Password == password).FirstOrDefault();
      if (user != null)
      {
        var claims = new List<Claim>
        {
            new Claim("Id", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));
        return Redirect("/");

      }
      string message = "Tài khoản, mật khẩu không hợp lệ!";
      ViewBag.Message = message;
      return View();
    }

    public IActionResult LogOut()
    {
      HttpContext.SignOutAsync(
      CookieAuthenticationDefaults.AuthenticationScheme);
      return Redirect("/");
    }

    public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor, int? orderId)
    {
      var link = "";
      var fromEmail = new MailAddress("bongtham01@gmail.com", "BongBaby");
      var verifyUrl = "/Users/" + emailFor + "/" + activationCode;

      var toEmail = new MailAddress(emailID);
      var fromEmailPassword = "qzit tnwi oczu rsne"; // mã xác thực 2 bước
      if (emailFor != "NewOrder")
      {
        var uriBuilder = new UriBuilder
        {
          Scheme = Request.Scheme, // Lấy scheme của yêu cầu (http hoặc https)
          Host = Request.Host.Host, // Lấy hostname của yêu cầu (localhost hoặc tên miền)
          Port = Request.Host.Port ?? 80,
          Path = verifyUrl
        };

        link = uriBuilder.Uri.AbsoluteUri;
      }

      string subject = "";
      string body = "";
      if (emailFor == "Register")
      {
        subject = "Your account is successfully created!";
        body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
            " successfully created. Please click on the below link to verify your account" +
            " <br/><br/><a href='" + link + "'>" + link + "</a> ";
      }
      else if (emailFor == "ResetPassword")
      {
        subject = "Reset Password";
        body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
            "<br/><br/><a href=" + link + ">Reset Password link</a>";
      }
      else if (emailFor == "NewOrder")
      {
        Order order = _orderService.getOrderById((int)orderId);

        user us = getUserByUserId(order.User_Id);
        List<int> listCartId = _orderService.getListCartIdByOrderId((int)orderId);
        subject = "New Order";
        string s = "";
        foreach (var item in listCartId)
        {
          cart cart = _cartService.getCartById(item);
          product pr = _productService.getProductById(cart.ProductId);

          s += "   - <b>" + pr.Name.ToString() + " (x" + cart.Quantity + ")" + "</b><br/>";
        }
        body = "Kính gửi <b>Quản trị viên</b>,<br/><br/>" +
              "Tôi viết thư này để thông báo rằng chúng ta đã nhận được một đơn hàng mới trên trang web Shop Quan Ao. Chi tiết đơn hàng như sau: <br/>" +
              "Mã đơn hàng: <b>" + order.Id + "</b><br/>" + "Tên Khách hàng: <b>" + us.Name + "</b><br/>" + "Ngày đặt hàng: <b>" + order.order_date + "</b><br/>" + "Tổng đơn hàng: <b>" + @String.Format("${0:#,##0.00}", order.totalPrice) + "</b><br/>" + "Sản phẩm đã đặt mua: <br/>" + s + "<br/>" + "Cảm ơn!<br/><br/>";
      }


      var smtp = new SmtpClient
      {
        Host = "smtp.gmail.com",
        Port = 587,
        EnableSsl = true,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
      };

      using (var message = new MailMessage(fromEmail, toEmail)
      {
        Subject = subject,
        Body = body,
        IsBodyHtml = true
      })
        smtp.Send(message);
    }

    public IActionResult Register()
    {
      return View();
    }

    [HttpPost]
    public IActionResult Register(string name, string email, string password, string repassword)
    {
      var user = _context.user.Where(u => u.Email == email).FirstOrDefault();
      bool checkEmail = user == null;
      if (ModelState.IsValid && password == repassword && checkEmail)
      {
        user us = new user
        {
          Name = name,
          Email = email,
          Password = password,
          Role = "KH",
          Phone = "",
          Address = ""
        };

        string resetCode = Guid.NewGuid().ToString();// tạo chuỗi code random để gửi kèm link reset password
        SendVerificationLinkEmail(email, resetCode, "Register", null);
        us.VerifyAccount = resetCode;
        _context.Add(us);
        _context.SaveChanges();
        ViewBag.Message = "Chúng tôi đã gửi email xác nhận tới bạn. Vui lòng kiểm tra để đăng nhập";
        ViewBag.Status = true;
        return View();
      }
      ViewBag.Status = false;
      if (checkEmail == false)
      {
        ViewBag.Message = "Email này đã đăng ký tài khoản vui lòng thử lại!";
      }
      else
      {
        ViewBag.Message = "Vui lòng nhập mật khẩu trùng nhau!";
      }
      return View();
    }

    [HttpGet]
    public IActionResult Register(string? id)
    {
      user us = _context.user.Where(x => x.VerifyAccount == id).FirstOrDefault();
      if (us != null)
      {
        if (id != null)
        {
          us.VerifyAccount = "";
          _context.SaveChanges();
          return View(us);
        }
      }
      if (id == null)
        return View();
      return Redirect("/home/error");
    }


    // QUÊN MẬT KHẨU

    // GET: /Account/ForgotPassword: Hiển thị view để người dùng nhập email
    public IActionResult ForgotPassword()
    {
      return View();
    }
    // POST: /Account/ForgotPassword: Nhận email từ form người dùng nhập

    [HttpPost]
    public IActionResult ForgotPassword(string EmailID)
    {
      //Verify Email ID
      //Generate Reset password link 
      //Send Email 
      string message = "";
      bool status = false;

      var account = _context.user.Where(a => a.Email == EmailID).FirstOrDefault();
      if (account != null)
      {
        //Send email for reset password
        string resetCode = Guid.NewGuid().ToString();// tạo chuỗi code random để gửi kèm link reset password
        SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword", null);
        account.ResetPassword = resetCode;
        //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
        //in our model class in part 1
        _context.SaveChanges();
        message = "Reset password link has been sent to your email id.";
      }
      else
      {
        message = "Account not found";
      }

      ViewBag.Message = message;
      return View();
    }

    // lấy dữ liệu từ link người dùng bấm ở gmail. Id là chuỗi sau Users/ResetPassword/...
    public IActionResult ResetPassword(string id)
    {
      //Verify the reset password link
      //Find account associated with this link
      //redirect to reset password page
      if (string.IsNullOrWhiteSpace(id))
      {
        return Redirect("/home/error");
      }

      var user = _context.user.Where(a => a.ResetPassword == id).FirstOrDefault();
      if (user != null)
      {
        ResetPasswordModel model = new ResetPasswordModel();
        model.Email = user.Email;
        model.ResetCode = id;
        return View(model);
      }
      else
      {
        return Redirect("/home/error");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ResetPassword(ResetPasswordModel model)
    {
      var message = "";
      var status = false;
      if (ModelState.IsValid)
      {
        var user = _context.user.Where(a => a.ResetPassword == model.ResetCode).FirstOrDefault();
        model.Email = user.Email;
        if (user != null)
        {
          user.Password = model.NewPassword;
          user.ResetPassword = "";
          _context.SaveChanges();
          message = "New password updated successfully";
          status = true;
        }
      }
      else
      {
        message = "Something invalid";
        status = false;
      }
      ViewBag.Message = message;
      ViewBag.Status = status;
      return View(model);
    }


    public user getUserByUserId(int userId)
    {
      var user = _context.user.Where(x => x.Id == userId).FirstOrDefault();
      return user;
    }
  }
}
