using System;
using System.Numerics;

namespace Soil.Math;

// based on https://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/
// and https://stackoverflow.com/questions/12088610/conversion-between-euler-quaternion-like-in-unity3d-engine
public static class QuaternionExtensions
{
    private const float SingularityNorthPole = 0.499f;

    private const float SingularitySouthPole = -.499f;

    public static Vector3 ToEuler(
        this Quaternion q,
        CoordinateSystem coord = CoordinateSystem.RightHandled)
    {
        switch (coord)
        {
            case CoordinateSystem.LeftHanded:
            {
                return q.ToEulerInLeftHandled();
            }
            case CoordinateSystem.RightHandled:
            {
                return q.ToEulerInRightHandled();
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(coord), coord, null);
            }
        }
    }

    private static Vector3 ToEulerInRightHandled(this Quaternion q)
    {

        float sqw = q.W * q.W;
        float sqx = q.X * q.X;
        float sqy = q.Y * q.Y;
        float sqz = q.Z * q.Z;
        float unit = sqx + sqy + sqz + sqw;
        float test = (q.X * q.Y) + (q.Z * q.W);

        Vector3 euler;
        if (test > SingularityNorthPole * unit)
        {
            euler.Z = 2f * MathF.Atan2(q.X, q.W);
            euler.X = MathF.PI / 2;
            euler.Y = 0;
            return euler.NormalizeDegree();
        }
        if (test < SingularitySouthPole * unit)
        {
            euler.Z = -2f * MathF.Atan2(q.X, q.W);
            euler.X = -MathF.PI / 2;
            euler.Y = 0;
            return euler.NormalizeDegree();
        }

        euler.Z = MathF.Atan2((2f * q.Y * q.W) - (2f * q.X * q.Z), sqx - sqy - sqz + sqw);
        euler.X = MathF.Asin(2 * test / unit);
        euler.Y = MathF.Atan2((2f * q.X * q.W) - (2f * q.Y * q.Z), -sqx + sqy - sqz + sqw);
        return euler.NormalizeDegree();
    }

    private static Vector3 ToEulerInLeftHandled(this Quaternion q)
    {
        float sqw = q.W * q.W;
        float sqx = q.X * q.X;
        float sqy = q.Y * q.Y;
        float sqz = q.Z * q.Z;
        float unit = sqx + sqy + sqz + sqw;
        float test = (q.X * q.W) - (q.Y * q.Z);

        Vector3 euler;
        if (test > SingularityNorthPole * unit)
        {
            euler.Y = 2f * MathF.Atan2(q.Y, q.X);
            euler.X = MathF.PI / 2;
            euler.Z = 0;
            return euler.NormalizeDegree();
        }
        if (test < SingularitySouthPole * unit)
        {
            euler.Y = -2f * MathF.Atan2(q.Y, q.X);
            euler.X = -MathF.PI / 2;
            euler.Z = 0;
            return euler.NormalizeDegree();
        }

        euler.Y = MathF.Atan2((2f * q.W * q.Y) + (2f * q.Z * q.X), 1 - (2f * ((q.X * q.X) + (q.Y * q.Y))));
        euler.X = MathF.Asin(2f * ((q.W * q.X) - (q.Y * q.Z)));
        euler.Z = MathF.Atan2((2f * q.W * q.Z) + (2f * q.X * q.Y), 1 - (2f * ((q.Z * q.Z) + (q.X * q.X))));
        return euler.NormalizeDegree();
    }
}
