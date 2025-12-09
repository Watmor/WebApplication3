using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApplication3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId =builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;

                options.Scope.Add("profile"); // tên, avatar
                options.Scope.Add("email");   // email
            });


            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSession(); // <-- bật session
            builder.Services.AddDistributedMemoryCache(); // cần cho session

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseSession(); // <-- thêm Middleware session
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseSession(); // <-- thêm Middleware session
            app.MapRazorPages();

            app.Run();
        }
    }
}
