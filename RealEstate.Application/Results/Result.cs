
namespace RealEstate.Application.Results
{
    public class Result
    {
        public bool Succeeded { get; init; }
        public string? Error { get; init; }

        public static Result Ok() => new() { Succeeded = true };
        public static Result Fail(string error) => new() { Succeeded = false, Error = error };
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }

        public static Result<T> Ok(T value) => new() { Succeeded = true, Value = value };
        public new static Result<T> Fail(string error) => new() { Succeeded = false, Error = error };
    }

    public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
}
