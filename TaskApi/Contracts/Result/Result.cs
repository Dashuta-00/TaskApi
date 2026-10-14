namespace TaskApi.Contracts.Result;

public class Result<T>
{
    public T? Value { get; }
    public ResultStatus Status { get; }
    public string? Error { get; }
    
    public bool IsSuccess => Status == ResultStatus.Success;

    private Result(
        ResultStatus status, 
        T? value = default,
        string? error = null)
    {
        Value = value;
        Status = status;
        Error = error;
    }

    public static Result<T> Success(T? value = default)
    {
        return new Result<T>(ResultStatus.Success, value);
    }

    public static Result<T> ValidationError(string error)
    {
        return new Result<T>(ResultStatus.ValidationError, error: error);
    }
    
    public static Result<T> NotFound(string error)
    {
        return new Result<T>(ResultStatus.NotFound, error: error);
    }
    
    public static Result<T> Conflict(string error)
    {
        return new Result<T>(ResultStatus.Conflict, error: error);
    }
}