namespace Mathematics.Core.Utilities;

public class FloatUtils
{
    // Стандартная точность для большинства случаев
    public const float DefaultFloatTolerance = 1e-6f;
    public const double DefaultDoubleTolerance = 1e-12;
    public const decimal DefaultDecimalTolerance = 1e-12m;

    #region Float сравнения
    
    public static bool Approximately(float a, float b, float tolerance = DefaultFloatTolerance) => Math.Abs(a - b) <= tolerance;

    public static bool ApproximatelyZero(float value, float tolerance = DefaultFloatTolerance) => Math.Abs(value) <= tolerance;

    public static bool GreaterThan(float a, float b, float tolerance = DefaultFloatTolerance) => a > b + tolerance;

    public static bool GreaterThanOrEqual(float a, float b, float tolerance = DefaultFloatTolerance) => a > b - tolerance;

    public static bool LessThan(float a, float b, float tolerance = DefaultFloatTolerance) => a < b - tolerance;

    public static bool LessThanOrEqual(float a, float b, float tolerance = DefaultFloatTolerance) => a < b + tolerance;

    #endregion

    #region Double сравнения

    public static bool Approximately(double a, double b, double tolerance = DefaultDoubleTolerance) => Math.Abs(a - b) <= tolerance;

    public static bool ApproximatelyZero(double value, double tolerance = DefaultDoubleTolerance) => Math.Abs(value) <= tolerance;

    public static bool GreaterThan(double a, double b, double tolerance = DefaultDoubleTolerance) => a > b + tolerance;

    public static bool GreaterThanOrEqual(double a, double b, double tolerance = DefaultDoubleTolerance) => a > b - tolerance;

    public static bool LessThan(double a, double b, double tolerance = DefaultDoubleTolerance) => a < b - tolerance;

    public static bool LessThanOrEqual(double a, double b, double tolerance = DefaultDoubleTolerance) => a < b + tolerance;

    #endregion

    #region Decimal сравнения

    public static bool Approximately(decimal a, decimal b, decimal tolerance = DefaultDecimalTolerance)
    {
        return Math.Abs(a - b) <= tolerance;
    }

    public static bool ApproximatelyZero(decimal value, decimal tolerance = DefaultDecimalTolerance)
    {
        return Math.Abs(value) <= tolerance;
    }

    public static bool GreaterThan(decimal a, decimal b, decimal tolerance = DefaultDecimalTolerance)
    {
        return a > b + tolerance;
    }

    public static bool GreaterThanOrEqual(decimal a, decimal b, decimal tolerance = DefaultDecimalTolerance)
    {
        return a > b - tolerance;
    }

    public static bool LessThan(decimal a, decimal b, decimal tolerance = DefaultDecimalTolerance)
    {
        return a < b - tolerance;
    }

    public static bool LessThanOrEqual(decimal a, decimal b, decimal tolerance = DefaultDecimalTolerance)
    {
        return a < b + tolerance;
    }

    #endregion

    #region Сравнения с относительной точностью (для очень больших/малых чисел)

    public static bool ApproximatelyRelative(double a, double b, double relativeTolerance = 1e-10)
    {
        if (a == b) return true;
        
        var diff = Math.Abs(a - b);
        var max = Math.Max(Math.Abs(a), Math.Abs(b));
        
        return diff <= max * relativeTolerance;
    }

    public static bool ApproximatelyRelative(float a, float b, float relativeTolerance = 1e-6f)
    {
        if (a == b) 
            return true;
        
        var diff = Math.Abs(a - b);
        var max = Math.Max(Math.Abs(a), Math.Abs(b));
        
        return diff <= max * relativeTolerance;
    }

    #endregion

    #region Утилиты для выбора точности

    public static float GetMachineEpsilonFloat()
    {
        return 1.1920929E-07f; // Machine epsilon для float
    }

    public static double GetMachineEpsilonDouble()
    {
        return 1.1102230246251565E-16; // Machine epsilon для double
    }

    // Автоматический подбор точности на основе масштаба чисел
    public static double GetAdaptiveTolerance(double a, double b)
    {
        var max = Math.Max(Math.Abs(a), Math.Abs(b));
        return max * 1e-10;
    }

    public static float GetAdaptiveTolerance(float a, float b)
    {
        var max = Math.Max(Math.Abs(a), Math.Abs(b));
        return max * 1e-6f;
    }

    #endregion
}