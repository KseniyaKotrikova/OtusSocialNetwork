using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

public class ReplicationConnectionStringProvider : IDbConnectionStringProvider
{
    private readonly string _masterConnection;
    private readonly string[] _slaveConnections;
    private int _slaveIndex = 0;

    public ReplicationConnectionStringProvider(IConfiguration configuration)
    {
        // Безопасное чтение: .NET автоматически склеивает "ConnectionStrings:MasterConnection"
        // и переменную окружения "ConnectionStrings__MasterConnection"
        _masterConnection = configuration["ConnectionStrings:MasterConnection"] ?? string.Empty;
        
        var slavesRaw = configuration["ConnectionStrings:SlaveConnections"];
        
        // ФОЛБЭК ДЛЯ МИГРАЦИЙ (Design-Time): Если пустые, подставляем локальные креды хоста
        if (string.IsNullOrWhiteSpace(_masterConnection))
        {
            _masterConnection = "Host=localhost;Port=5432;Database=otus_db;Username=postgres;Password=my_pass";
            slavesRaw = "Host=localhost;Port=5433;Database=otus_db;Username=postgres;Password=my_pass|Host=localhost;Port=5434;Database=otus_db;Username=postgres;Password=my_pass";
            Console.WriteLine("[DB INIT] ⚠️ Запущен режим миграций (Design-Time). Применены фолбэк-строки подключения.");
        }
        _slaveConnections = !string.IsNullOrWhiteSpace(slavesRaw)
            ? slavesRaw.Split('|', StringSplitOptions.RemoveEmptyEntries)
            : Array.Empty<string>();

        // Логирование в консоль для отладки при старте (вы увидите, что считал докер)
        Console.WriteLine($"[DB INIT] Master connection string length: {_masterConnection.Length}");
        Console.WriteLine($"[DB INIT] Found {_slaveConnections.Length} slave connection strings.");
    }

    public string GetConnectionString()
    {
        // 1. Проверяем, запущен ли процесс утилитой миграций dotnet-ef
        var assemblyName = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name;
        bool isMigrationMode = assemblyName == "ef" || assemblyName?.StartsWith("dotnet-ef") == true;

        // 2. Если мы в режиме миграций ИЛИ роль не выставлена (первый старт) ИЛИ это Мастер
        if (isMigrationMode || DbConnectionContext.CurrentRole == DbNodeRole.Master || _slaveConnections.Length == 0)
        {
            if (string.IsNullOrEmpty(_masterConnection))
            {
                throw new InvalidOperationException("Строка подключения к Master БД не инициализирована!");
            }
            return _masterConnection;
        }

        // 3. Строго и только для GET-запросов, где Middleware явно выставил роль Slave, делаем Round-Robin
        var index = Interlocked.Increment(ref _slaveIndex) % _slaveConnections.Length;
        return _slaveConnections[Math.Abs(index)];
    }

}