using Scalar.AspNetCore;

namespace Mathematics.Server;

public static class Program
{
	public static Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		builder.Services.AddOpenApi(options => options.AddScalarTransformers());
		builder.Services.AddEndpointsApiExplorer();

		var app = builder.Build();

		if (app.Environment.IsDevelopment()) 
		{
			app.MapOpenApi();
			
			app.MapScalarApiReference("/", scalar =>
			{
				scalar.Title = "Mathematics [Server]";
			});
			
			app.UseCors(policy => policy
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader());
		}

		app.UseStaticFiles();
		app.UseHttpsRedirection();
		app.UseRouting();
		
		app.MapStaticAssets();
		app.MapControllers();

		return app.RunAsync();
	}
}