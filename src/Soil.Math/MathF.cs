using System;
using System.Runtime.CompilerServices;

public static class MathF
{
    public const float E = System.MathF.E;

    public const float PI = System.MathF.PI;

    private const float DegToRad = PI / 180f;

    private const float RadToDeg = 180f / PI;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(float x)
    {
        return System.MathF.Abs(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acos(float x)
    {
        return System.MathF.Acos(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Acosh(float x)
    {
        return System.MathF.Acosh(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asin(float x)
    {
        return System.MathF.Asin(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Asinh(float x)
    {
        return System.MathF.Asinh(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan(float x)
    {
        return System.MathF.Atan(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atan2(float y, float x)
    {
        return System.MathF.Atan2(y, x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Atanh(float x)
    {
        return System.MathF.Atanh(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cbrt(float x)
    {
        return System.MathF.Cbrt(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Ceiling(float x)
    {
        return System.MathF.Ceiling(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cos(float x)
    {
        return System.MathF.Cos(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cosh(float x)
    {
        return System.MathF.Cosh(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Exp(float x)
    {
        return System.MathF.Exp(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Floor(float x)
    {
        return System.MathF.Floor(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float IEEERemainder(float x, float y)
    {
        return System.MathF.IEEERemainder(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(float x)
    {
        return System.MathF.Log(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log(float x, float y)
    {
        return System.MathF.Log(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Log10(float x)
    {
        return System.MathF.Log10(x);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(float x, float y)
    {
        return System.MathF.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(float x, float y)
    {
        return System.MathF.Min(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Pow(float x, float y)
    {
        return System.MathF.Pow(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(float x, MidpointRounding mode)
    {
        return System.MathF.Round(x, mode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(float x, int digits, MidpointRounding mode)
    {
        return System.MathF.Round(x, digits, mode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(float x)
    {
        return System.MathF.Round(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Round(float x, int digits)
    {
        return System.MathF.Round(x, digits);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(float x)
    {
        return System.MathF.Sign(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sin(float x)
    {
        return System.MathF.Sin(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sinh(float x)
    {
        return System.MathF.Sinh(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sqrt(float x)
    {
        return System.MathF.Sqrt(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tan(float x)
    {
        return System.MathF.Tan(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Tanh(float x)
    {
        return System.MathF.Tanh(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Truncate(float x)
    {
        return System.MathF.Truncate(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RadianOf(float degree)
    {
        return DegToRad * degree;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DegreeOf(float radian)
    {
        return radian * RadToDeg;
    }
}
