using Serilog;
using SmartHome.Host.Extensions;

namespace SmartHome.Host;

public static partial class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Host.UseSerilog((ctx, services, cfg) =>
                cfg.ReadFrom.Configuration(ctx.Configuration)
                   .ReadFrom.Services(services)
                   .Enrich.FromLogContext());

            var haReady = builder.AddHomeAssistantIntegration();

            if (haReady)
            {
                builder.AddTelegramIntegration();
                builder.AddMaxIntegration();
                builder.AddVkIntegration();
            }
            else
            {
                Log.Warning("Bot integrations skipped because Home Assistant is not configured");
            }

            builder.Services.AddControllers();

            var app = builder.Build();
            
            app.UseStaticFiles();
            app.MapControllers();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
