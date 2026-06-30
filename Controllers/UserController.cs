using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using OtusSocialNetwork.DTO;
using OtusSocialNetwork.Repositories;
using Microsoft.EntityFrameworkCore;

namespace OtusSocialNetwork.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly UserRepository _repository;
    private static readonly ConcurrentDictionary<string, string> _sessions = new();

    public UserController(UserRepository repository) => _repository = repository;

    [HttpPost("user/register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest req)
    {
        string hash = BCrypt.Net.BCrypt.HashPassword(req.password);
        var id = await _repository.Register(req, hash);
        return Ok(new { user_id = id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(AppLoginRequest req)
    {
        if (!Guid.TryParse(req.id, out var guid)) return BadRequest();
        var hash = await _repository.GetPasswordHash(guid);
        
        if (hash == null || !BCrypt.Net.BCrypt.Verify(req.password, hash))
            return Unauthorized();

        var token = Guid.NewGuid().ToString();
        _sessions[token] = req.id;
        return Ok(new { token });
    }

    [HttpGet("user/get/{id}")]
    public async Task<IActionResult> GetUser(string id, ApplicationDbContext db)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            return BadRequest(new { message = "Невалидный формат GUID" });
        }

        var user = await _repository.GetUserById(guid);

        if (user == null)
        {
            return NotFound(new { message = "Пользователь не найден" });
        }

        return Ok(user);
    }

    [HttpGet("/user/search")]
    public async Task<IActionResult> Search([FromQuery] string firstName, [FromQuery] string lastName)
    {
        // Валидация: если параметры пустые, лучше не нагружать базу полным сканированием
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return BadRequest("firstName and lastName are required");
        }

        try
        { 
            var users = await _repository.Search(firstName, lastName);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error during search");
        }
    }
    
    [HttpPost("/user/create")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto dto, [FromServices]ApplicationDbContext db)
    {
        // Запрос уйдет на Master
        var user = new User
        {
            FirstName =  dto.FirstName, 
            SecondName = dto.SecondName,
            Biography =  dto.Biography,
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Ok(user);
    }

}
