using DataLayers.SQLServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using SV21T1020218.BusinessLayers;
using SV21T1020218.DataLayers;

using SV21T1020218.DomainModels;

namespace SV21T1020218.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Lấy connection string và kiểm tra null
            string connectionString = builder.Configuration.GetConnectionString("SV21T1020218") 
                ?? throw new InvalidOperationException("Connection string 'SV21T1020218' not found.");

            // Khởi tạo Business Layer configuration
            Configuration.Initialize(connectionString);

            // Đăng ký các DAL
            builder.Services.AddScoped<ICommonDAL<Customer>>(provider => new CustomerDAL(connectionString));            

            // Add services 
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(option =>
                            {
                                option.Cookie.Name = "AuthenticationCookie";
                                option.LoginPath = "/Account/Login";
                                option.ExpireTimeSpan = TimeSpan.FromDays(360);
                            });

            var app = builder.Build();

            // Cấu hình ApplicationContext
            ApplicationContext.Configure(
                app.Services.GetRequiredService<IHttpContextAccessor>(),
                app.Environment
            );

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
          
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
