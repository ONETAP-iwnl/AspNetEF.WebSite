using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using System.Configuration;
using WebSiteCollege.Identity;

namespace WebSiteCollege
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                    .AddNegotiate();

            var epplusLicenseContext = builder.Configuration.GetSection("EPPlus:LicenseContext").Value;
            if (!string.IsNullOrEmpty(epplusLicenseContext))
            {
                ExcelPackage.LicenseContext = Enum.Parse<LicenseContext>(epplusLicenseContext);
            }

            // Configure the HTTP request pipeline.
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

    }
}
