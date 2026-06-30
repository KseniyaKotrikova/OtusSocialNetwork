using Microsoft.EntityFrameworkCore.Diagnostics; // Нужен для ConnectionEventData
using Microsoft.EntityFrameworkCore.Storage;     // Нужен для DbConnectionInterceptor
using System.Data.Common;                        // Нужен для DbConnection


public class ReadWriteCommandInterceptor : DbConnectionInterceptor
{
    private readonly IDbConnectionStringProvider _connectionStringProvider;

    public ReadWriteCommandInterceptor(IDbConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public override InterceptionResult ConnectionOpening(
        DbConnection connection, 
        ConnectionEventData eventData, 
        InterceptionResult result)
    {
        connection.ConnectionString = _connectionStringProvider.GetConnectionString();
        return result;
    }

    public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection, 
        ConnectionEventData eventData, 
        InterceptionResult result, 
        CancellationToken cancellationToken = default)
    {
        connection.ConnectionString = _connectionStringProvider.GetConnectionString();
        return ValueTask.FromResult(result);
    }
}