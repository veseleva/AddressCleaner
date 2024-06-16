
using System.Net.Http.Headers;

namespace AddressCleanerAM
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddCors();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var remoteServiceUrl = builder.Configuration["AddressCleaner:RemoteServiceUrl"];
			var token = builder.Configuration["AddressCleaner:Token"];
			var secret = builder.Configuration["AddressCleaner:Secret"];

			builder.Services.AddHttpClient<AddressCleanerService>(httpClient => {
				httpClient.BaseAddress = new Uri(remoteServiceUrl);
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				httpClient.DefaultRequestHeaders.Add("X-Secret", secret);
			});

			builder.Services.AddProblemDetails();
			builder.Services.AddAutoMapper(typeof(AddressProfile));

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				//app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseExceptionHandler(exceptionHandlerApp =>
			{
				exceptionHandlerApp.Run(async httpContext =>
				{
					app.Logger.LogError("Request failed. Request: {request}", 
						httpContext.Request.Path.ToString() + httpContext.Request.QueryString.ToString());

					var pds = httpContext.RequestServices.GetService<IProblemDetailsService>();
					if (pds == null
						|| !await pds.TryWriteAsync(new() { HttpContext = httpContext }))
					{
						await httpContext.Response.WriteAsync("An error occurred.");
					}
				});
			});

			app.UseStatusCodePages(statusCodeHandlerApp =>
			{
				statusCodeHandlerApp.Run(async httpContext =>
				{
					app.Logger.LogError("Request failed with statusCode {statusCode}. Request: {request}", 
						httpContext.Response.StatusCode, httpContext.Request.Path.ToString() + httpContext.Request.QueryString.ToString());

					var pds = httpContext.RequestServices.GetService<IProblemDetailsService>();
					if (pds == null
						|| !await pds.TryWriteAsync(new() { HttpContext = httpContext }))
					{
						await httpContext.Response.WriteAsync("An error occurred.");
					}
				});
			});

			app.UseCors(builder => builder.AllowAnyOrigin());

			app.MapGet("/clean", async (string address, AddressCleanerService addressCleanerService) => {

				var cleanAddress = await addressCleanerService.GetCleanAddressAsync(address, default);

				if (cleanAddress == null)
				{
					app.Logger.LogError("Failed to get data. Request: {address}", address);
					return Results.Problem("Error getting data.");
				}

				app.Logger.LogInformation("Successful get data. Request: {address}", address);
				return Results.Json(cleanAddress);
			});
			app.MapGet("/", (HttpContext context) => context.Response.WriteAsync("Address cleaner"));

			app.Run();
		}
	}
}
