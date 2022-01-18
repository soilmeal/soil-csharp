using System;
using System.Numerics;

namespace Soil.Math;

public static class Matrix4x4Helper
{
    public static Matrix4x4 CreateLookAt(
        Vector3 position,
        Vector3 target,
        Vector3 up,
        CoordinateSystem coord = CoordinateSystem.LeftHanded)
    {
        switch (coord)
        {
            case CoordinateSystem.LeftHanded:
            {
                return CreateLookAtForLeftHanded(position, target, up);
            }
            case CoordinateSystem.RightHandled:
            {
                return CreateLookAtForRightHanded(position, target, up);
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(coord), coord, null);
            }
        }
    }

    // based on https://github.com/g-truc/glm/blob/6ad79aae3eb5bf809c30bf1168171e9e55857e45/glm/ext/matrix_transform.inl#L122
    public static Matrix4x4 CreateLookAtForLeftHanded(
        Vector3 cameraPosition,
        Vector3 cameraTarget,
        Vector3 cameraUpVector)
    {
        Vector3 forward = Vector3.Normalize(cameraTarget - cameraPosition);
        Vector3 side = Vector3.Normalize(Vector3.Cross(cameraUpVector, forward));
        Vector3 up = Vector3.Cross(forward, side);

        Matrix4x4 result = Matrix4x4.Identity;

        result.M11 = side.X;
        result.M12 = side.Y;
        result.M13 = side.Z;

        result.M21 = up.X;
        result.M22 = up.Y;
        result.M23 = up.Z;

        result.M31 = forward.X;
        result.M32 = forward.Y;
        result.M33 = forward.Z;

        result.M41 = Vector3.Dot(side, cameraPosition);
        result.M42 = Vector3.Dot(up, cameraPosition);
        result.M43 = Vector3.Dot(forward, cameraPosition);

        return result;
    }

    //https://github.com/g-truc/glm/blob/6ad79aae3eb5bf809c30bf1168171e9e55857e45/glm/ext/matrix_transform.inl#L99
    public static Matrix4x4 CreateLookAtForRightHanded(
        Vector3 cameraPosition,
        Vector3 cameraTarget,
        Vector3 cameraUpVector)
    {
        Vector3 forward = Vector3.Normalize(cameraTarget - cameraPosition);
        Vector3 side = Vector3.Normalize(Vector3.Cross(forward, cameraUpVector));
        Vector3 up = Vector3.Cross(side, forward);

        Matrix4x4 result = Matrix4x4.Identity;

        result.M11 = side.X;
        result.M12 = side.Y;
        result.M13 = side.Z;

        result.M21 = up.X;
        result.M22 = up.Y;
        result.M23 = up.Z;

        result.M31 = -forward.X;
        result.M32 = -forward.Y;
        result.M33 = -forward.Z;

        result.M41 = -Vector3.Dot(side, cameraPosition);
        result.M42 = -Vector3.Dot(up, cameraPosition);
        result.M43 = Vector3.Dot(forward, cameraPosition);

        return result;
    }
}
