using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Saca_Common;
using SACA_Common.Configurations;
using SACA_Infra.Context;
using SACA_Infra.SeedData;
using SACA_Infra.Utils;
using SACA_Service;
using SACA_Service.SignalR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var startup = new BaseStartUp(builder.Configuration);
startup.ConfigurationService(builder.Services);
AppSettings.Instance.SetConfiguration(builder.Configuration);
var logger = new LoggerConfiguration().ReadFrom
                                  .Configuration(builder.Configuration)
                                  .Enrich
                                  .FromLogContext()
                                  .CreateLogger();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // ← QUAN TRỌNG
});
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Services.AddServices();
builder.Services.AddScoped<XmlHelper>();
var connectionString = builder.Configuration.GetConnectionString("DBContext");
builder.Services.AddDbContext<SACA_Context>(optBuilder =>
{
    optBuilder.UseMySQL(connectionString ?? "");
    //optBuilder.EnableSensitiveDataLogging();
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SACA_Context>();

    try
    {
        dbContext.Database.Migrate();
        SeedData.Seed(scope.ServiceProvider);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
    }
}
startup.Configure(app, app.Environment);
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<LeaderBoardHub>("/leaderboardHub");
app.Run();
