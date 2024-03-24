namespace Utils;

/// <summary>
/// Represents a value that can be either a value or an error.
/// </summary>
public record Result<TOk, TErr>
{
    /// <summary>
    /// Represents a value.
    /// </summary>
    public sealed record Ok(TOk value) : Result<TOk, TErr>;

    /// <summary>
    /// Represents an error.
    /// </summary>
    public sealed record Err(TErr value) : Result<TOk, TErr>;

    /// <summary>
    /// Unwraps the value or throws an exception if the result is an error.
    /// </summary>
    public TOk Unwrap() =>
        this is Ok ok ? ok.value : throw new InvalidOperationException("Invalid result access");

    /// <summary>
    /// Unwraps the value or calls a function if the result is an error.
    /// </summary>
    public TOk UnwrapOrElse(Func<TErr, TOk> func) =>
        this is Ok ok ? ok.value : func((this as Err)!.value);

    /// <summary>
    /// Checks if the result is a value.
    /// </summary>
    public bool IsOk() => this is Ok;

    /// <summary>
    /// Checks if the result is an error.
    /// </summary>
    public bool IsErr() => this is Err;

    /// <summary>
    /// Maps the result to a new result.
    /// </summary>
    public Result<TNew, TErr> Map<TNew>(Func<TOk, TNew> func) =>
        this is Ok ok
            ? new Result<TNew, TErr>.Ok(func(ok.value))
            : new Result<TNew, TErr>.Err((this as Err)!.value);

    /// <summary>
    /// Tries to unwrap the value.
    /// </summary>
    public Option<TOk> TryUnwrap() => this is Ok ok ? ok.value : Option<TOk>.None;

    /// <summary>
    /// Unwraps the error or throws an exception if the result is a value.
    /// </summary>
    public TErr UnwrapErr() =>
        this is Err err ? err.value : throw new InvalidOperationException("Invalid result access");

    private Result() { }
}
