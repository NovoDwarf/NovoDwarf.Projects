namespace Mathematics.Core.Utilities;

using System.Numerics;

public static class NumericUtils<T> where T : INumber<T>
{
    public static bool ApproximatelySmart(T a, T b, T absoluteTolerance, T relativeTolerance)
    {
        if (a == b)
            return true;

        var diff = T.Abs(a - b);
        var max = T.Max(T.Abs(a), T.Abs(b));

        return diff <= T.Max(absoluteTolerance, max * relativeTolerance);
    }
    
    public static bool Approximately(T a, T b, T tolerance) => T.Abs(a - b) <= tolerance;

    public static bool ApproximatelyZero(T value, T tolerance) => T.Abs(value) <= tolerance;

    public static bool GreaterThan(T a, T b, T tolerance) => a > b + tolerance;

    public static bool LessThan(T a, T b, T tolerance) => a < b - tolerance;

    public static bool GreaterThanOrEqual(T a, T b, T tolerance) => !LessThan(a, b, tolerance);

    public static bool LessThanOrEqual(T a, T b, T tolerance) => !GreaterThan(a, b, tolerance);
}


public static class FloatUtils
{
    public const float AbsTol = 1e-6f;
    public const float RelTol = 1e-6f;

    public static bool Approximately(float a, float b)
        => NumericUtils<float>.ApproximatelySmart(a, b, AbsTol, RelTol);
}

public static class DoubleUtils
{
    public const double AbsTol = 1e-12;
    public const double RelTol = 1e-10;

    public static bool Approximately(double a, double b)
        => NumericUtils<double>.ApproximatelySmart(a, b, AbsTol, RelTol);
}
