using TaskApi.Contracts.Result;

namespace TaskApi.Extensions;

public static class ResultExtensions
{
    public static IResult ToResult<T>(
        this Result<T> result,
        Func<T, IResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        return ToErrorResult(
            result.Status,
            result.Error);
    }
    
    private static IResult ToErrorResult(
        ResultStatus status,
        string? error)
    {
        return status switch
        {
            ResultStatus.ValidationError =>
                Results.BadRequest(new
                {
                    error
                }),

            ResultStatus.NotFound =>
                Results.NotFound(new
                {
                    error
                }),

            ResultStatus.Conflict =>
                Results.Conflict(new
                {
                    error
                }),

            _ =>
                Results.StatusCode(
                    StatusCodes.Status500InternalServerError)
        };
    }
}