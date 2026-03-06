using OtusSocialNetwork.DTO;

namespace OtusSocialNetwork.Repositories;
using Npgsql;
using System.Data;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString) => _connectionString = connectionString;

    public async Task<string?> Register(UserRegisterRequest req, string passwordHash)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var sql = @"INSERT INTO users (first_name, second_name, birthdate, biography, city, password_hash, gender) 
                    VALUES (@f, @s, @b, @bio, @c, @ph, @g) RETURNING id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("f", req.first_name);
        cmd.Parameters.AddWithValue("s", req.second_name);
        cmd.Parameters.AddWithValue("b", req.birthdate);
        cmd.Parameters.AddWithValue("bio", req.biography ?? "");
        cmd.Parameters.AddWithValue("c", req.city ?? "");
        cmd.Parameters.AddWithValue("ph", passwordHash);
        cmd.Parameters.AddWithValue("g", req.gender ?? "");
        return (await cmd.ExecuteScalarAsync())?.ToString();
    }

    public async Task<string?> GetPasswordHash(Guid id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        using var cmd = new NpgsqlCommand("SELECT password_hash FROM users WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return await cmd.ExecuteScalarAsync() as string;
    }

    public async Task<UserResponse?> GetUserById(Guid id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand("SELECT id, first_name, second_name, birthdate, biography, city, gender FROM users WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new UserResponse
            (reader.GetGuid(0).ToString(),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDateTime(3),
                reader.IsDBNull(4) ? "" : reader.GetString(4),
                reader.IsDBNull(5) ? "" : reader.GetString(5),
                reader.IsDBNull(6) ? "" : reader.GetString(6)
            );
        }

        return null;
    }
    // Здесь же будет метод Search с вашим LIKE запросом
}

