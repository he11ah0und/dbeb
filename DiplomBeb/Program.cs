using DiplomBeb.Components;
using DiplomBeb.Services.Client;
using DiplomBeb.Utils.Networking;

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
            
            builder.Services.AddDistributedMemoryCache();
            
            builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("api"));
            builder.Services.AddScoped<ApiService>();
            
            // http client for client view
            builder.Services.AddScoped<BackendHttpClient>();
            
            // adds web controllers
            builder.Services.AddScoped<SyncRenderService>();
            builder.Services.AddScoped<ThemeService>();
            builder.Services.AddScoped<LibraryService>();
            builder.Services.AddScoped<HomeService>();
            builder.Services.AddScoped<AccountService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ModerationService>();
            
            // swagger for debuggin web apis
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
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
            
            app.UseStaticFiles();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
