using Microsoft.AspNetCore.Authentication.Cookies;

namespace SV21T1020218.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews()
                .AddMvcOptions(option =>
                {
                    option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(option =>
                            {
                                option.Cookie.Name = "AuthenticationCookie";         //Tên của cookie
                                option.LoginPath = "/Account/Login";                 //URL đến trang đăng nhập
                                option.AccessDeniedPath = "/Account/AccessDenined";  //URL đêbs trang trong trường bị cấm truy cấp
                                option.ExpireTimeSpan = TimeSpan.FromDays(360);      //Thời gian tồn tại của cookie
                            });
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            ApplicationContext.Configure
                (
                context: app.Services.GetRequiredService<IHttpContextAccessor>(),
                enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()
                );

            // Các chu?i k?tt n?i và khi t?o c?u hình cho BusinessLayer
            string connectionString = builder.Configuration.GetConnectionString("SV21T1020218");
            SV21T1020218.BusinessLayers.Configuration.Initialize(connectionString);
            app.Run();
        }
    }
}
