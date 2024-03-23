using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopQuanAo.Areas.Admin.Controllers;
using ShopQuanAo.Controllers;
using ShopQuanAo.Data;
namespace ShopQuanAo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddDbContext<ShopQuanAoContext>(options =>
			    options.UseSqlServer(builder.Configuration.GetConnectionString("ShopQuanAoContext") ?? throw new InvalidOperationException("Connection string 'ShopQuanAoContext' not found.")));

			// Add services to the container.
			builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

      builder.Services.AddScoped<IProductService, ProductsController>();
			builder.Services.AddScoped<IUserService, UsersController>();
			builder.Services.AddScoped<ICartService, cartsController>();
			builder.Services.AddScoped<IOrderService, ordersController>(); // phia ADMIN

			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
		.AddCookie(options =>
		{
			options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
			options.SlidingExpiration = true;
			options.AccessDeniedPath = "/Forbidden/";
		});
			builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddSession();
			


			var app = builder.Build();
			app.UseSession();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
      app.UseRouting();
			app.UseStaticFiles();

			app.UseAuthentication();
			app.UseAuthorization();


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "areas",
					pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
				);
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");

			});

			

			app.Run();
		}
	}
}
