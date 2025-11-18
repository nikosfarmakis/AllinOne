using AllinOne.Configurations.Extensions;
using AllinOne.Data.Sqlite.Extensions;
using AllinOne.MemoryCache;
using AllinOne.Middlewares.Extensions;
using AllinOne.Redis.Extensions;
using AllinOne.Services.Extensions;
using AllinOne.Swagger.Extensions;
using AllinOne.Utils.Extensions;
using Microsoft.FeatureManagement;
using Serilog;
using System.Text.Json.Serialization;

try
{
    Log.Information("Starting up...");
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddConfigurations(builder.Configuration);

    builder.Services.AddSerilogWithFeatureFlags(builder.Configuration);

    // add host Serilog 
    builder.Host.UseSerilog();

    builder.Services.AddFeatureManagement();

    //builder.Services.AddControllers();
    builder.Services.AddControllers() //Enum information
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddCustomSwagger();

    builder.Services.AddSqliteDatabase(builder.Configuration);

    builder.Services.AddRedisCache(builder.Configuration);

    builder.Services.AddCustomCacheMemoryKeysHandler();

    builder.Services.AddProjectServices();

    var app = builder.Build();


    Log.Information("Application started in {Environment}", app.Environment.EnvironmentName);

    //Middlewares
    app.UsePipelineMiddlewares();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

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
