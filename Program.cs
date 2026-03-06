using Microsoft.OpenApi.Models;
using OtusSocialNetwork.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS Highload Social Network", Version = "v1.2.0" });
});

string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default") 
                          ?? "Host=localhost;Database=otus_db;Username=postgres;Password=my_pass";

builder.Services.AddSingleton(new UserRepository(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Social Network API v1");
    c.RoutePrefix = "swagger"; 
});

app.MapControllers();
app.Run();