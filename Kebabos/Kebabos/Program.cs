using Kebabos.Services.User;
using Kebabos.Services.Store;
using System.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
{
    dotenv.net.DotEnv.Load();
    string connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")!;
    builder.Services.AddControllers();
    // for the UserController constructor to work; when it requests an IUserService, it will get an instance of UserService
    builder.Services.AddScoped<IUserService, UserService>();
    // same for the StoreController
    builder.Services.AddScoped<IStoreService, StoreService>();
    builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

}
var app = builder.Build();
{
    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}