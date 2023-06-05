namespace Kebabos.Services.User;

using Kebabos.Models;
using Kebabos.Contracts.User;
using System.Data;
using Dapper;
using Kebabos.Services;

public class UserService : IUserService
{
    private readonly IDbConnection _connection;

    public UserService(IDbConnection dbConnection)
    {
        _connection = dbConnection;
    }

    private class DatabaseUserResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public User ToEntity()
        {
            return new User(Id!, Name!);
        }
    }

    private static async Task<User?> GetUserByName(IDbConnection conn, string name)
    {
        var query = "SELECT id as Id, name as Name FROM \"user\" WHERE name = @Name";
        var parameters = new { Name = name };
        var dbResponse = await conn.QuerySingleOrDefaultAsync<DatabaseUserResponse>(query, parameters);
        return dbResponse?.ToEntity() ?? null;
    }

    public async Task<ServiceResult<User>> CreateUser(UserCreateRequest request)
    => await Common.WithConnectionOpening(_connection, async openedConnection =>
        await Common.AsTransaction(openedConnection, async _ =>
        {
            var maybePresent = await GetUserByName(openedConnection, request.Username);
            if (maybePresent != null) return ServiceResult<User>.Err(ServiceError.AlreadyExists);
            var query = "INSERT INTO \"user\" (name) VALUES (@Username) RETURNING id as Id, name as Name";
            var parameters = new { Username = request.Username };
            var dbResponse = await openedConnection.QuerySingleOrDefaultAsync<DatabaseUserResponse>(query, parameters);
            return ServiceResult<User>.FromOr(dbResponse?.ToEntity(), ServiceError.AlreadyExists);
        }
        ));

    public async Task<ServiceResult<List<User>>> GetUsers()
    {
        var query = "SELECT id as Id, name as Name FROM \"user\";";
        var dbResponse = await _connection.QueryAsync<DatabaseUserResponse>(query);
        return ServiceResult<List<User>>.FromOr(dbResponse?.Select(dbUser => dbUser.ToEntity())?.ToList(), ServiceError.InternalError);
    }

    public async Task<ServiceResult<User>> GetUserById(Guid id)
    {
        var query = "SELECT id as Id, name as Name FROM \"user\" WHERE id = @Id";
        var parameters = new { Id = id };
        var dbResponse = await _connection.QuerySingleOrDefaultAsync<DatabaseUserResponse>(query, parameters);
        return ServiceResult<User>.FromOr(dbResponse?.ToEntity(), ServiceError.NotFound);
    }
}
