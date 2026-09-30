using System;
using System.Collections.Generic;

namespace AcademIQ.Shared.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? Error { get; }
    public List<string> Errors { get; } = new();

    private Result(bool isSuccess, T? value, string? error, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        if (errors != null)
        {
            Errors.AddRange(errors);
        }
        else if (!string.IsNullOrWhiteSpace(error))
        {
            Errors.Add(error);
        }
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, string.Join("; ", errors), errors);
}

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public List<string> Errors { get; } = new();

    private Result(bool isSuccess, string? error, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        if (errors != null)
        {
            Errors.AddRange(errors);
        }
        else if (!string.IsNullOrWhiteSpace(error))
        {
            Errors.Add(error);
        }
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(IEnumerable<string> errors) => new(false, string.Join("; ", errors), errors);
}

