using Configurator.serialog;
using Serilog;
using Hangfire.PostgreSql;
using Hangfire;
using Parse;
using Parse.Core.Hangfire;
using Microsoft.AspNetCore.Diagnostics;
using DataBase.Contexts;
using Microsoft.EntityFrameworkCore;


serialogConfig.ConfigureLogger();
Log.Information("Starting web application");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ParseFull>();
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer(options =>
{
    options.Queues = new[] { "default" };
    options.WorkerCount = 1;  // только 1 задача одновременно
});
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<Serilog.Extensions.Hosting.DiagnosticContext>();
builder.Services.AddSingleton(Log.Logger);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    db.Database.EnsureCreated();
}


app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            var errorResponse = new
            {
                StatusCode = context.Response.StatusCode,
                Message = contextFeature.Error.Message,
                StackTrace = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                              ? contextFeature.Error.StackTrace
                              : null
            };
        await context.Response.WriteAsJsonAsync(errorResponse);
        }
    });
});


app.UseHangfireDashboard("/dashboard");
app.LaunchCategPars();
app.LaunchProductsPars();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });
app.MapControllers();
app.UseSerilogRequestLogging();

app.Run();
