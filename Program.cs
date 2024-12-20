using PizzaStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";
builder.Services.AddEndpointsApiExplorer();
// Impl�mentation de la base de donn�es en m�moire actuelle
//builder.Services.AddDbContext<PizzaDb>(options =>
//options.UseInMemoryDatabase("items"));

// Une base de donn�es persistante.
builder.Services.AddSqlite<PizzaDb>(connectionString);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = " API PizzaStore ",
        Description = " Faire les pizzas que vous aimez ",
        Version = "v1"
    });
});
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});
app.MapGet("/", () => "Bonjour S�n�gal!");
app.MapGet("/pizzas", async (PizzaDb db) => await db.Pizzas.ToListAsync());
//Cr�er des �l�ments
app.MapPost("/pizza", async (PizzaDb db, Pizza pizza) =>
{
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});
//Obtenir un �l�ment
app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id));
//Mettre � jour un �l�ment 
app.MapPut("/pizza/{id}", async (PizzaDb db, Pizza updatepizza, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();
    pizza.Nom = updatepizza.Nom;
    pizza.Description = updatepizza.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
//Supprimer un �l�ment 
app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null)
    {
        return Results.NotFound();
    }
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok();
});
app.Run();