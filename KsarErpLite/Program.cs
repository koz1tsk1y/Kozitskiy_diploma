using KsarErpLite.Components;
using KsarErpLite.Data;
using KsarErpLite.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace KsarErpLite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseNetTopologySuite()));

            // Регистрация сервиса заявок
            builder.Services.AddScoped<IJobService, JobService>();

            builder.Services.AddScoped<IKsarParserService, KsarParserService>();

            // Встроенная система авторизации на базе Cookie
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login"; // Куда перенаправлять неавторизованных
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/access-denied";
                    options.Cookie.Name = "KsarErpAuth";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Сессия на неделю
                });

            builder.Services.AddAuthorization();
            builder.Services.AddCascadingAuthenticationState();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
