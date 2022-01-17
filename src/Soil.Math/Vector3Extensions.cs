using System;
using System.Numerics;

namespace Soil.Math;

public static class Vector3Extensions
{
    public static Quaternion ToQuaternion(
        this Vector3 v,
        CoordinateSystem coord = CoordinateSystem.RightHandled)
    {
        switch (coord)
        {
            case CoordinateSystem.LeftHanded:
            {
                return Quaternion.CreateFromYawPitchRoll(
                    MathF.RadianOf(v.Y),
                    MathF.RadianOf(v.X),
                    MathF.RadianOf(v.Z));
            }
            case CoordinateSystem.RightHandled:
            {
                return Quaternion.CreateFromYawPitchRoll(
                    MathF.RadianOf(v.Z),
                    MathF.RadianOf(v.X),
                    MathF.RadianOf(v.Y));
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(coord), coord, null);
            }
        }
    }

    public static Vector3 NormalizeDegree(this Vector3 v)
    {
        v.X = NormalizeDegree(v.X);
        v.Y = NormalizeDegree(v.Y);
        v.Z = NormalizeDegree(v.Z);
        return v;
    }

    private static float NormalizeDegree(float angle)
    {
        while (angle > 360f)
        {
            angle -= 360f;
        }

        while (angle < 0)
        {
            angle += 360;
        }

        return angle;
    }
}
