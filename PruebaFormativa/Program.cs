using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDb>(opt => opt.UseInMemoryDatabase("User"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/user", async(User incUser, UserDb db)=>
{
    var user1 = await db.User.Where(u => u.Rut == incUser.Rut).FirstOrDefaultAsync();
    if(user1 != null)
    {
        return Results.Conflict();
    }
     var user2 = await db.User.Where(u => u.Correo == incUser.Correo).FirstOrDefaultAsync();
    if(user2 != null)
    {
        return Results.Conflict();
    }
    
    db.User.Add(incUser);
    await db.SaveChangesAsync();
    return Results.Created($"/user/{incUser.Id}", incUser);
});

app.MapGet("/user", async(UserDb db)=>
{
    
    return Results.Ok(await db.User.ToListAsync());
});

app.MapGet("/user/active", async(UserDb db)=>
{
    var list = await db.User.Where(u => u.Activo == true).ToListAsync();
    return Results.Ok(list);
});

app.MapGet("user/{rut}", async(string rut, UserDb db)=>
{
     var user = await db.User.Where(u => u.Rut == rut).FirstOrDefaultAsync();
    if(user == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(user);
});

app.MapPut("user/{id}", async(int id, User inputUser, UserDb db)=>
{
    var user = await db.User.FindAsync(id);
    if(user == null)
    {
        return Results.NotFound();
    }

    user.Nombre = inputUser.Nombre;
    user.Correo = inputUser.Correo;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPut("user/{id}/state", async(int id, UserDb db)=>
{
    var user = await db.User.FindAsync(id);
    if(user == null)
    {
        return Results.NotFound();
    }

    if(user.Activo == true)
    {
        user.Activo = false;
    }
    else
    {
        user.Activo = true;
    }

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("user/{id}", async(int id, UserDb db)=>
{
    var user = await db.User.FindAsync(id);
    if(user == null)
    {
        return Results.NotFound();
    }
    db.User.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();


app.Run();
