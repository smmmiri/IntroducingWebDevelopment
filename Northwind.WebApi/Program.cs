using Microsoft.AspNetCore.Mvc.Formatters; // To use IOutputFormatter.
using Microsoft.Extensions.Caching.Memory; // To use IMemoryCache and so on.
using Northwind.EntityModels; // To use AddNorthwindContext method.
using Northwind.WebApi.Repositories; // To use ICustomerRepository.
using Swashbuckle.AspNetCore.SwaggerUI; // To use SubmitMethod.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(new MemoryCacheOptions()));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddNorthwindContext();
builder.Services
    .AddControllers(options =>
    {
        Console.WriteLine("Default output formatters:");
        foreach (IOutputFormatter formatter in options.OutputFormatters)
        {
            OutputFormatter? mediaFormatter = formatter as OutputFormatter;
            if (mediaFormatter is null)
            {
                Console.WriteLine($" {formatter.GetType().Name}");
            }
            else // OutputFormatter class has SupportedMediaTypes.
            {
                Console.WriteLine(" {0}, Media types: {1}",
                    arg0: mediaFormatter.GetType().Name,
                    arg1: string.Join(", ",
                    mediaFormatter.SupportedMediaTypes));
            }
        }
    })
    .AddXmlDataContractSerializerFormatters()
    .AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind Service API Version 1");
        c.SupportedSubmitMethods(new[]
        {
            SubmitMethod.Get,
            SubmitMethod.Post,
            SubmitMethod.Put,
            SubmitMethod.Delete
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
