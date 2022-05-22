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

app.MapGet("/userById/{id}", async (Guid? id, UserDbContext db) =>
{
    if (!id.HasValue)
    {
        return Results.BadRequest("Please provide a valid user id!!!");
    }
    var results = await db.Users
        .FirstOrDefaultAsync(x => x.Id == id);

    return Results.Ok(results);
});

app.MapGet("/users", async (UserDbContext db) =>
{
    var results = await db.Users
        .ToListAsync();

    return Results.Ok(results);
});

app.MapPost("/userAdd", async (User user, UserDbContext db) =>
{
    await db.AddAsync(user);
    if(await db.SaveChangesAsync() > 0)
    {
        return Results.Ok(user);
    }
    return Results.BadRequest("User could not be added");

});

app.MapDelete("/userDelete/{id}", async (Guid id, UserDbContext db) =>
{
    var findUser = await db.Users.FindAsync(id);
    if (findUser is null)
    {
        return Results.BadRequest("User not found");
    }

    db.Users.Remove(findUser);
    await db.SaveChangesAsync();
    return Results.Ok($"{findUser.Name} was deleted");
});

app.MapPost("/userDetailAdd/{id}", async (Guid id, ContactInformation info, UserDbContext db) =>
{
    var findUser = await db.Users.FindAsync(id);
    if (findUser is null)
    {
        return Results.BadRequest("User not found");
    }

    info.UserId = id;
    await db.AddAsync(info);
    if (await db.SaveChangesAsync() > 0)
    {
        return Results.Ok(info);
    }
    return Results.BadRequest("Contact information could not be added");
});

app.MapGet("/userDetailById/{id}", async (Guid? id, UserDbContext db) =>
{
    if (!id.HasValue)
    {
        return Results.BadRequest("Please provide a valid user id!!!");
    }

    var results = await db.Users
        .Include(x => x.ContactInformations)
        .FirstOrDefaultAsync(x => x.Id == id);
        
    return Results.Ok(results);
});

app.MapDelete("/userDetailDelete/{id}", async (Guid id, UserDbContext db) =>
{
    var findInfo = await db.ContactInformations.FindAsync(id);
    if (findInfo is null)
    {
        return Results.BadRequest("Contact Information not found");
    }

    db.ContactInformations.Remove(findInfo);
    await db.SaveChangesAsync();
    return Results.Ok("Information was deleted");
});


app.Run();
