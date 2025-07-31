using MongoDbTesting.Components;
using MongoDbTesting.Data;
using MongoDbTesting.Services;
using Radzen;

namespace MongoDbTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddRadzenComponents();

            MongoDbConfig? mongoDbConfig = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();
            if (mongoDbConfig == null)
            {
                Console.WriteLine("MongoDbConf is NULL!");
                return;
            }

            builder.Services.AddScoped<IOpovDbAccessService>(s => new OpovDbAccessService(mongoDbConfig));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
