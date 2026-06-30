using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OtusSocialNetwork.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OTUS Highload Social Network", Version = "v1.2.0" });
});

// string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default") 
//                           ?? "Host=localhost;Database=otus_db;Username=postgres;Password=my_pass;";
//


// Рестрируем провайдер строк подключений
builder.Services.AddSingleton<IDbConnectionStringProvider, ReplicationConnectionStringProvider>();
builder.Services.AddTransient<ReadWriteCommandInterceptor>();

// Настраиваем EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    // Инициализируем дефолтной строкой (она всё равно подменится интерцептором)
    var provider = sp.GetRequiredService<IDbConnectionStringProvider>();
    options.UseNpgsql(provider.GetConnectionString());
    
    // Подключаем наш перехватчик
    options.AddInterceptors(sp.GetRequiredService<ReadWriteCommandInterceptor>());
});

builder.Services.AddScoped<UserRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// В самый верх файла Program.cs, сразу после var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("=== ПРИЛОЖЕНИЕ НАЧИНАЕТ СТАРТ ===");
Console.WriteLine($"СТРОКА ИЗ ДОКЕРА: {builder.Configuration["ConnectionStrings:MasterConnection"]}");


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Social Network API v1");
    c.RoutePrefix = "swagger"; 
});

app.UseDbRoleRouting();

app.MapControllers();

// Перед app.Run();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Принудительно выставляем роль Master для записи seed-данных
    DbConnectionContext.CurrentRole = DbNodeRole.Master;
    
    // Автоматически применяем миграции, если база еще не создана
    await db.Database.MigrateAsync();

    // Проверяем, если база пустая — генерируем пользователей
    if (!await db.Users.AnyAsync())
    {
        Console.WriteLine("Начало генерации тестовых данных...");
        var users = new List<User>();
        
        for (int i = 1; i <= 50000; i++)
        {
            users.Add(new User
            {
                FirstName = $"User_{i}_{Guid.NewGuid().ToString()[..8]}",
                Email = $"user{i}@example.com"
            });

            // Вставляем пачками по 5000 строк для скорости
            if (i % 5000 == 0)
            {
                await db.Users.AddRangeAsync(users);
                await db.SaveChangesAsync();
                users.Clear();
                Console.WriteLine($"Записано {i} пользователей...");
            }
        }
        Console.WriteLine("Генерация тестовых данных успешно завершена!");
    }
}

app.Run();