using ProyectoBD.Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DatabaseRepository>();
builder.Services.AddScoped<TableRepository>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = builder.Configuration.GetConnectionString("MasterConnection");
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(connectionString);

builder.Services.AddCors(options =>
{
    options.AddPolicy("http://localhost:4200/", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("http://localhost:4200/");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
