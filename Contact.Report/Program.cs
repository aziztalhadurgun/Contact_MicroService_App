using Contact.Report.BusinessLogic;
using Contact.Report.Config;
using Contact.Report.Consumers;
using Contact.Report.DataAccess;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IReportCreator, ReportCreator>();
builder.Services.AddOptions();
builder.Services.Configure<UserDataConfig>(builder.Configuration.GetSection("UserDataConfig"));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UpdatingReportStatus>();
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, rabbit) =>
    {
        rabbit.UseMessageRetry(x => x.Interval(2, 1000));

        rabbit.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        rabbit.ReceiveEndpoint("create-report-queue", endpoint =>
        {
            endpoint.ConfigureConsumer<UpdatingReportStatus>(context);
        });
    });
});

builder.Services.AddDbContext<ReportDbContext>(
    opt =>
    {
        opt.EnableSensitiveDataLogging();
        opt.EnableDetailedErrors();
        opt.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
    }, ServiceLifetime.Transient
);

var app = builder.Build();

app.MapPost("/createReport", async (ReportDbContext db, ISendEndpointProvider sendEndpointProvider) =>
{
    var report = new Reports
    {
        Id = Guid.NewGuid(),
        CreatedOn = DateTime.UtcNow,
        Status = "Hazýrlanýyor"
    };

    await db.AddAsync(report);
    await db.SaveChangesAsync();

    var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-report-queue"));
    await endpoint.Send<Reports>(report);

    return Results.Ok();
});

app.MapGet("/reports", async (ReportDbContext db) =>
{
    var results = await db.Reports
        .ToListAsync();

    return Results.Ok(results);
});

app.Run();
