using Contact.Users.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDbContext>(
    opt =>
    {
        opt.EnableSensitiveDataLogging();
        opt.EnableDetailedErrors();
        opt.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
    }, ServiceLifetime.Transient
);

var app = builder.Build();



app.Run();
