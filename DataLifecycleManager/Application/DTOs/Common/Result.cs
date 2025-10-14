using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DataLifecycleManager.Application.DTOs.Common
{
    /// <summary>
    /// Represents the result of an operation that can succeed or fail
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    public class Result<T>
    {
        protected internal Result(bool succeeded, T? value, IEnumerable<string>? errors, string? message = null)
        {
            Succeeded = succeeded;
            Value = value;
            Errors = errors?.ToArray() ?? new string[0];
            Message = message ?? string.Empty;
        }

        public bool Succeeded { get; set; }
        public T? Value { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }

        public static Result<T> Success(T value, string? message = null)
        {
            return new Result<T>(true, value, null, message);
        }

        public static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, default, errors, string.Join("; ", errors));
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, new[] { error }, error);
        }
    }

    /// <summary>
    /// Represents the result of an operation without a return value
    /// </summary>
    public class Result
    {
        protected internal Result(bool succeeded, IEnumerable<string>? errors)
        {
            Succeeded = succeeded;
            Errors = errors?.ToArray() ?? new string[0];
        }

        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }

        public static Result Success()
        {
            return new Result(true, null);
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }

        public static Result Failure(string error)
        {
            return new Result(false, new[] { error });
        }
    }
}
