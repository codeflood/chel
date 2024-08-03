using System;

namespace Chel.Abstractions.Results;

/// <summary>
/// A successful <see cref="CommandResult"/> with no return value.
/// </summary>
public class SuccessResult : CommandResult
{
    private static SuccessResult s_instance = new SuccessResult();

    public static SuccessResult Instance => s_instance;

    public SuccessResult()
    {
        Success = true;
    }

    public override string ToString()
    {
        return string.Empty;
    }
}
