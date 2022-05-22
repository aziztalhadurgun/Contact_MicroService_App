using Contact.Report.BusinessLogic;
using Contact.Report.Config;
using Contact.Report.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IReportCreator, ReportCreator>();
builder.Services.AddOptions();
builder.Services.Configure<UserDataConfig>(builder.Configuration.GetSection("UserDataConfig"));

builder.Services.AddDbContext<ReportDbContext>(
    opt =>
    {
        opt.EnableSensitiveDataLogging();
        opt.EnableDetailedErrors();
        opt.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
    }, ServiceLifetime.Transient
);

var app = builder.Build();

app.MapGet("/report", async (IReportCreator reportCreator) =>
{
    await reportCreator.BuildReport();
    return Results.Ok();
});


app.Run();
