using System;
using System.Runtime.CompilerServices;

public static class Math
{
    public const double E = System.Math.E;

    public const double PI = System.Math.PI;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Abs(decimal value)
    {
        return System.Math.Abs(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Abs(double value)
    {
        return System.Math.Abs(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Abs(short value)
    {
        return System.Math.Abs(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Abs(int value)
    {
        return System.Math.Abs(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Abs(long value)
    {
        return System.Math.Abs(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Abs(sbyte value)
    {
        return System.Math.Abs(value);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Abs(float value)
    {
        return System.Math.Abs(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Acos(double d)
    {
        return System.Math.Acos(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Acosh(double d)
    {
        return System.Math.Acosh(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Asin(double d)
    {
        return System.Math.Asin(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Asinh(double d)
    {
        return System.Math.Asinh(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atan(double d)
    {
        return System.Math.Atan(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atan2(double y, double x)
    {
        return System.Math.Atan2(y, x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Atanh(double d)
    {
        return System.Math.Atanh(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long BigMul(int a, int b)
    {
        return System.Math.BigMul(a, b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cbrt(double c)
    {
        return System.Math.Cbrt(c);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Ceiling(decimal d)
    {
        return System.Math.Ceiling(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Ceiling(double d)
    {
        return System.Math.Ceiling(d);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double RadianOf(double degree)
    {
        return PI / 180.0 * degree;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Clamp(ulong value, ulong min, ulong max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Clamp(uint value, uint min, uint max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Clamp(ushort value, ushort min, ushort max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
    {
        return System.Math.Clamp(value, min, value);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Clamp(short value, short min, short max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Clamp(double value, int min, int max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Clamp(decimal value, decimal min, decimal max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Clamp(byte value, byte min, byte max)
    {
        return System.Math.Clamp(value, min, max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Clamp(long value, long min, long max)
    {
        return System.Math.Clamp(value, min, max);
    }

    public static double Cos(double d)
    {
        return System.Math.Cos(d);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Cosh(double d)
    {
        return System.Math.Cosh(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DivRem(int a, int b, out int result)
    {
        return System.Math.DivRem(a, b, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DivRem(long a, long b, out long result)
    {
        return System.Math.DivRem(a, b, out result);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Exp(double d)
    {
        return System.Math.Exp(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Floor(double d)
    {
        return System.Math.Floor(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Floor(decimal d)
    {
        return System.Math.Floor(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double IEEERemainder(double x, double y)
    {
        return System.Math.IEEERemainder(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log(double d)
    {
        return System.Math.Log(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log(double a, double newBase)
    {
        return System.Math.Log(a, newBase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Log10(double d)
    {
        return System.Math.Log10(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Max(ulong x, ulong y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Max(uint x, uint y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Max(ushort x, ushort y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(float x, float y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Max(sbyte x, sbyte y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Max(decimal x, decimal y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(int x, int y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Max(short x, short y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Max(double x, double y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Max(byte x, byte y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Max(long x, long y)
    {
        return System.Math.Max(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Min(ulong val1, ulong val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Min(uint val1, uint val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort Min(ushort val1, ushort val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(float val1, float val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte Min(sbyte val1, sbyte val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Min(long val1, long val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Min(short val1, short val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Min(double val1, double val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Min(decimal val1, decimal val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Min(byte val1, byte val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int val1, int val2)
    {
        return System.Math.Min(val1, val2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Pow(double x, double y)
    {
        return System.Math.Pow(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(double value, MidpointRounding mode)
    {
        return System.Math.Round(value, mode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(double value, int digits, MidpointRounding mode)
    {
        return System.Math.Round(value, digits, mode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(double value, int digits)
    {
        return System.Math.Round(value, digits);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Round(double d)
    {
        return System.Math.Round(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(decimal value, int decimals, MidpointRounding mode)
    {
        return System.Math.Round(value, decimals, mode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(decimal value, int decimals)
    {
        return System.Math.Round(value, decimals);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(decimal value)
    {
        return System.Math.Round(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Round(decimal value, MidpointRounding mode)
    {
        return System.Math.Round(value, mode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(float value)
    {
        return System.Math.Sign(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(long value)
    {
        return System.Math.Sign(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(int value)
    {
        return System.Math.Sign(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(sbyte value)
    {
        return System.Math.Sign(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(double value)
    {
        return System.Math.Sign(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(decimal value)
    {
        return System.Math.Sign(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(short value)
    {
        return System.Math.Sign(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sin(double d)
    {
        return System.Math.Sin(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sinh(double d)
    {
        return System.Math.Sinh(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Sqrt(double d)
    {
        return System.Math.Sqrt(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tan(double d)
    {
        return System.Math.Tan(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Tanh(double d)
    {
        return System.Math.Tanh(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Truncate(decimal d)
    {
        return System.Math.Truncate(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Truncate(double d)
    {
        return System.Math.Truncate(d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RadianOf(float degree)
    {
        return MathF.RadianOf(degree);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double DegreeOf(double radian)
    {
        return radian * 180.0 / PI;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float DegreeOf(float radian)
    {
        return MathF.DegreeOf(radian);
    }
}
