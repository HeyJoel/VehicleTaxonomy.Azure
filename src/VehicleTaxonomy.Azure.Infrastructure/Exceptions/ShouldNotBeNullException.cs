using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VehicleTaxonomy.Azure.Infrastructure;

/// <summary>
/// Similar to a guard clause or NullArgumentException, but can be used
/// within code blocks where you know a variable is not null but the compiler
/// is unable to infer a not-null state.
/// </summary>
public class ShouldNotBeNullException : Exception
{
    public ShouldNotBeNullException()
        : base("A variable has unexpectedly been found null")
    {
    }

    public ShouldNotBeNullException(string message)
        : base(message)
    {
    }

    public static void ThrowIfNull([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string varName = "")
    {
        if (value == null)
        {
            throw new ShouldNotBeNullException($"The variable '{varName}' is not expected to be null.");
        }
    }
}
