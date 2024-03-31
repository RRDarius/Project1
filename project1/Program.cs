using Microsoft.EntityFrameworkCore;
using NSwag.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ClothesDb>(opt => opt.UseInMemoryDatabase("ClothesDB"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "Clothes Inventory Management API";
    config.Title = "ClothesAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "ClothesAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapGet("/clothes", handler: async (ClothesDb db) =>
    await db.Clothes.ToListAsync());

app.MapGet(pattern: "/clothes/givenAway", handler: async (ClothesDb db) =>
    await db.Clothes.Where(predicate: t => !t.IsPresent).ToListAsync());

app.MapGet("/clothes/{id}", handler: async (int id, ClothesDb db) =>
    await db.Clothes.FindAsync(id)
        is ClothingArticle clothing
            ? Results.Ok(clothing)
            : Results.NotFound());

app.MapPost("/clothes", handler: async (ClothingArticle clothing, ClothesDb db) =>
{
    db.Clothes.Add(clothing);
    await db.SaveChangesAsync();

    return Results.Created($"/clothes/{clothing.Id}", clothing);
});

app.MapPut("/clothes/{id}", async (int id, ClothingArticle inputClothing, ClothesDb db) =>
{
    var cloth = await db.Clothes.FindAsync(id);

    if (cloth is null) return Results.NotFound();

    cloth.Name = inputClothing.Name;
    cloth.IsPresent = inputClothing.IsPresent;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/clothes/{id}", async (int id, ClothesDb db) =>
{
    if (await db.Clothes.FindAsync(id) is ClothingArticle clothing)
    {
        db.Clothes.Remove(entity: clothing);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();