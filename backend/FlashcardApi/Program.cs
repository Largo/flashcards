using Microsoft.EntityFrameworkCore;
using FlashcardApi.Data;
using FlashcardApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add Entity Framework and SQLite
builder.Services.AddDbContext<FlashcardApi.Data.FlashcardDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=flashcards.db"));

// Register the SpacedRepetitionService
builder.Services.AddScoped<FlashcardApi.Services.SpacedRepetitionService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:5175") // Vue.js dev server ports
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowVueApp");
app.UseAuthorization();
app.MapControllers();

app.Run();
