using DiplomBeb.Database;
using DiplomBeb.Components;
using DiplomBeb.Filters;
using DiplomBeb.Services;
using DiplomBeb.Services.Client;
using DiplomBeb.Services.Client.Components;
using DiplomBeb.Systems;
using DiplomBeb.Utils;
using DiplomBeb.Utils.Networking;
using Microsoft.EntityFrameworkCore;

namespace DiplomBeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddHttpContextAccessor();
            
            // adds web controllers
            builder.Services.AddControllers();
            builder.Services.AddScoped<CallbackManager>();
            builder.Services.AddScoped<MainService>();
            builder.Services.AddScoped<LibraryService>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ModerationService>();
            
            // swagger for debuggin web apis
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            // database with all data
            builder.Services.AddDbContext<MainDBContext>(
                options => options.UseMySql("server=127.0.0.1;Port=3306;user=digitalpassUser;password=4LcQ8N4Jl07jzfDy2c4H;database=digitalpass;",
                    new MySqlServerVersion(new Version(8, 0, 11) ),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
            
            // http client for client view
            builder.Services.AddScoped<BackendHttpClient>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAntiforgery();

            app.MapControllers();
            app.UseStaticFiles();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
