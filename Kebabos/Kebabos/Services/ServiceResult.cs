namespace Kebabos.Services;

public class ServiceResult<T>
{
    private ServiceResult(T result)
    {
        Result = result;
    }

    private ServiceResult(ServiceError error)
    {
        Error = error;
    }

    public static ServiceResult<T> Ok(T result) => new(result);
    public static ServiceResult<T> Err(ServiceError error) => new(error);

    public static ServiceResult<T> FromOr(T? maybeValue, ServiceError error)
        => maybeValue == null ? Err(error) : Ok(maybeValue);

    public T? Result { get; }
    public ServiceError? Error { get; }

    public T Unwrap() => Result ?? throw new InvalidOperationException($"Cannot unwrap error: {Error}");

    public ServiceError UnwrapErr() => Error ?? throw new InvalidOperationException($"Cannot unwrap result: {Result}");

    public bool IsOk() => Result != null;

    public bool IsErr() => Error != null;

    public U process<U>(
        Func<T, U> okFunc,
        Func<ServiceError, U> errFunc
    ) => IsOk() ? okFunc(Unwrap()) : errFunc(UnwrapErr());
}

public enum ServiceError
{
    NotFound,
    AlreadyExists,
    InvalidInput,
    Unauthorized,
    Forbidden,
    InternalError
}