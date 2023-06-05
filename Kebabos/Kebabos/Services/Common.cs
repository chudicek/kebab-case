namespace Kebabos.Services;

using System.Data;
using Microsoft.AspNetCore.Mvc;

public class Common
{
    /// 
    public static async Task<T> AsTransaction<T>(IDbConnection connection, Func<IDbTransaction, Task<T>> func)
    {
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = await func(transaction);
                transaction.Commit();
                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw new Exception("transaction failed & fail handled", e);
            }
        }
    }

    public static async Task<T> WithConnectionOpening<T>(IDbConnection closedConnection, Func<IDbConnection, Task<T>> func)
    {
        using (var openConnection = closedConnection)
        {
            openConnection.Open();
            return await func(openConnection);
        }
    }



    public static IActionResult handle(ServiceError result) => result switch
    {
        ServiceError.NotFound => new NotFoundResult(),
        ServiceError.AlreadyExists => new ConflictResult(),
        ServiceError.InvalidInput => new BadRequestResult(),
        ServiceError.Unauthorized => new UnauthorizedResult(),
        ServiceError.Forbidden => new ForbidResult(),
        ServiceError.InternalError => new StatusCodeResult(500),
        _ => throw new InvalidOperationException($"Unknown error: {result}")
    };
}
