using Microsoft.EntityFrameworkCore;
using MinimalAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SqlContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// method
async Task<List<Person>> GetAll(SqlContext context) =>
    await context.People.ToListAsync();

app.MapGet("/People", async (SqlContext context) => await context.People.ToListAsync());

app.MapGet("/Person/{id}", async (SqlContext context, int id) =>
    await context.People.FindAsync(id) is Person person ?
        Results.Ok(person) :
        Results.NotFound("Person not found."));

app.MapPost("/Person", async (SqlContext context, Person person) =>
{
    context.People.Add(person);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAll(context));
});

app.MapPut("Person/{id}", async (SqlContext context, Person person, int id) =>
{
    var dbPerson = await context.People.FindAsync(id);
    if (dbPerson == null) return Results.NotFound("Person not found.");

    dbPerson.FirstName = person.FirstName;
    dbPerson.LastName = person.LastName;
    dbPerson.City = person.City;
    await context.SaveChangesAsync();

    return Results.Ok(await GetAll(context));
});

app.MapDelete("Person/{id}", async (SqlContext context, int id) =>
{
    var dbPerson = await context.People.FindAsync(id);
    if (dbPerson == null) return Results.NotFound("Person not found.");

    context.People.Remove(dbPerson);
    await context.SaveChangesAsync();

    return Results.Ok(await GetAll(context));
});

app.Run();

