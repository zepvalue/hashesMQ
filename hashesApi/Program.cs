using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Initial Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false);

// Db Configuration
var connectionString = builder.Configuration.GetConnectionString("LocalDbConnStr");
builder.Services.AddDbContext<HashContext>(options =>
{
    options.UseSqlServer(connectionString);
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database
var initializer = new DatabaseInitializer(app.Services);
initializer.Initialize();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
