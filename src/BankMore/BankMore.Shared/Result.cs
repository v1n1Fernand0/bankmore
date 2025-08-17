﻿namespace BankMore.Shared;

public class Result<T>
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Ok(T value) => new(true, value, null);
    public static Result<T> Fail(string error) => new(false, default, error);
}
