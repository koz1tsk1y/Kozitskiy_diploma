using KsarErpLite.Components;
using KsarErpLite.Data;
using Microsoft.EntityFrameworkCore;
using KsarErpLite.Services;

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

            // Регистрация ApplicationDbContext с подключением Npgsql и NetTopologySuite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    o => o.UseNetTopologySuite()));

            // Регистрация сервиса заявок
            builder.Services.AddScoped<IJobService, JobService>();

            builder.Services.AddScoped<IKsarParserService, KsarParserService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
