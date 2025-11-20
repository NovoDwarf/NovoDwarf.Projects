namespace Mathematics.Server;

public static class Program
{
	public static Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddOpenApi();

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
		}

		app.UseHttpsRedirection();
		
		return app.RunAsync();
	}
}
