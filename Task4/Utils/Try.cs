namespace Utils;

/// <summary>
/// Class for trying to call a function that may throw an exception.
/// </summary>
public static class Try<TException>
    where TException : Exception
{
    /// <summary>
    /// Calls a function that may throw an exception.
    /// </summary>
    public static Result<TResult, TException> Call<TResult>(Func<TResult> func)
    {
        try
        {
            return new Result<TResult, TException>.Ok(func());
        }
        catch (TException exception)
        {
            return new Result<TResult, TException>.Err(exception);
        }
    }

    /// <summary>
    /// Calls an async function that may throw an exception.
    /// </summary>
    public static async Task<Result<TResult, TException>> CallAsync<TResult>(
        Func<Task<TResult>> func
    )
    {
        try
        {
            return new Result<TResult, TException>.Ok(await func());
        }
        catch (TException exception)
        {
            return new Result<TResult, TException>.Err(exception);
        }
    }
}
