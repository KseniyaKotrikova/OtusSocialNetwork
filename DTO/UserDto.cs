public class UserDto
{
    // Оставляем для обратной совместимости
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Добавляем новые поля под структуру таблицы users
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }

}