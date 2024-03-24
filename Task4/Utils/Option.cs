namespace Utils;

/// <summary>
/// Represents an optional value.
/// </summary>
public record Option<T>
{
    /// <summary>
    /// Represents a value.
    /// </summary>
    public sealed record Some(T value) : Option<T>;

    /// <summary>
    /// Represents no value.
    /// </summary>
    public sealed record _None : Option<T>;

    /// <summary>
    /// Represents no value.
    /// </summary>
    public static readonly _None None = new _None();

    /// <summary>
    /// Creates a new option from a nullable value.
    /// </summary>
    public static Option<T> From(T? value) => value != null ? new Some(value) : None;

    /// <summary>
    /// Implicitly converts a value to an option.
    /// </summary>
    public static implicit operator Option<T>(T value) => new Some(value);

    /// <summary>
    /// Checks if the option is a value.
    /// </summary>
    public bool IsSome() => this is Some;

    /// <summary>
    /// Maps the option to a new option.
    /// </summary>
    public Option<TNew> Map<TNew>(Func<T, TNew> func) =>
        this is Some some ? new Option<TNew>.Some(func(some.value)) : Option<TNew>.None;

    /// <summary>
    /// Maps the option to a new option.
    /// </summary>
    public async Task<TNew> MapOr<TNew>(Func<T, Task<TNew>> func, TNew onNone) =>
        this is Some some ? await func(some.value) : onNone;


    /// <summary>
    /// Maps the option to a new option.
    /// </summary>
    public Option<TNew> AndThen<TNew>(Func<T, Option<TNew>> func) =>
        this is Some some ? func(some.value) : Option<TNew>.None;

    /// <summary>
    public T Unwrap() =>
        this is Some some
            ? some.value
            : throw new InvalidOperationException("Invalid option access");

    /// <summary>
    public static Option<IEnumerable<T>> Collect(IEnumerable<Option<T>> list) =>
        list.Aggregate(
                new Option<List<T>>.Some(new List<T>()),
                (Option<List<T>> acc, Option<T> x) =>
                    acc.AndThen<List<T>>(
                        (List<T> list) =>
                            x.Map(value =>
                            {
                                list.Add(value);
                                return list;
                            })
                    )
            )
            .AndThen(x => new Option<IEnumerable<T>>.Some(x));

    private Option() { }
}
