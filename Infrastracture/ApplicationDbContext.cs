using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationDbContext : DbContext
{
    // Конструктор передает настройки (включая интерцептор) в базовый класс EF Core
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Коллекция (таблица) пользователей для выполнения вашего ДЗ
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настраиваем индекс для эффективного поиска по префиксу (/user/search)
        modelBuilder.Entity<User>()
            .HasIndex(u => u.FirstName);
    }
}

// Модель пользователя, которая отображается в таблицу "Users"
[Table("users")]
public class User
{
    [Column("id")]
    public Guid Id { get; set; } // Оставляем и фиксируем GUID
    
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;
    
    [Column("second_name")]
    public string SecondName { get; set; } = string.Empty;
    
    [Column("birthdate")]
    public DateTime BirthDate { get; set; }
    
    [Column("biography")]
    public string Biography { get; set; } = string.Empty;
    
    [Column("city")]
    public string City { get; set; } = string.Empty;
    
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Column("gender")]
    public string Gender { get; set; } = string.Empty;
    
    [Column("email")]
    public string Email { get; set; } = string.Empty;
}