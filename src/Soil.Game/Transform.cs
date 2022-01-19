using System;
using System.Numerics;
using Soil.Math;

namespace Soil.Game;

public class Transform : Component
{
    private Vector3 _position;

    private Vector3 _scale;

    private Quaternion _rotation;

    public Transform? Parent
    {
        get
        {
            return GameObject?.Parent?.Transform;
        }
        set
        {
            GameObject? gameObject = GameObject;
            if (gameObject == null)
            {
                return;
            }

            gameObject.Parent = value?.GameObject;
        }
    }

    public Transform? parent
    {
        get
        {
            return Parent;
        }
        set
        {
            Parent = value;
        }
    }

    public CoordinateSystem CoordinateSystem
    {
        get
        {
            return GameObject.World.CoordinateSystem;
        }
    }

    public CoordinateSystem coordinateSystem
    {
        get
        {
            return CoordinateSystem;
        }
    }

    public Vector3 LocalPosition
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
        }
    }

    public Vector3 localPosition
    {
        get
        {
            return LocalPosition;
        }
        set
        {
            LocalEulerAngles = value;
        }
    }

    public Vector3 WorldPosition
    {
        get
        {
            Matrix4x4.Decompose(LocalToWorldMatrix, out _, out _, out var pos);
            return pos;
        }
        set
        {
            Matrix4x4 matrix = LocalToWorldMatrix;
            matrix.Translation = value;

            Transform? parent = Parent;
            if (parent != null)
            {
                matrix *= parent.LocalMatrixInverted;
            }


            Matrix4x4.Decompose(matrix, out _scale, out _rotation, out _position);
        }
    }

    public Vector3 position
    {
        get
        {
            return WorldPosition;
        }
        set
        {
            WorldPosition = value;
        }
    }

    public Quaternion LocalRotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            _rotation = value;
        }
    }

    public Quaternion localRotation
    {
        get
        {
            return LocalRotation;
        }
        set
        {
            LocalRotation = localRotation;
        }
    }

    public Quaternion WorldRotation
    {
        get
        {
            Transform? parent = Parent;
            return parent != null ? parent.WorldRotation * _rotation : _rotation;
        }
        set
        {
            Transform? parent = Parent;
            _rotation = parent != null ? Quaternion.Inverse(parent.WorldRotation) * value : value;
        }
    }

    public Quaternion rotation
    {
        get
        {
            return WorldRotation;
        }
        set
        {
            WorldRotation = value;
        }
    }

    public Vector3 LocalScale
    {
        get
        {
            return _scale;
        }
        set
        {
            _scale = value;
        }
    }

    public Vector3 Forward
    {
        get
        {
            return Vector3.Transform(Vector3.UnitZ, WorldRotation);
        }
    }

    public Vector3 forward
    {
        get
        {
            return Forward;
        }
    }

    public Vector3 Up
    {
        get
        {
            return Vector3.Transform(Vector3.UnitY, WorldRotation);
        }
    }

    public Vector3 up
    {
        get
        {
            return Up;
        }
    }

    public Vector3 Right
    {
        get
        {
            return Vector3.Transform(Vector3.UnitX, WorldRotation);
        }
    }

    public Vector3 right
    {
        get
        {
            return Right;
        }
    }

    public Matrix4x4 LocalScaleMatrix
    {
        get
        {
            return Matrix4x4.CreateScale(_scale);
        }
    }

    public Matrix4x4 localScaleMatrix
    {
        get
        {
            return LocalScaleMatrix;
        }
    }

    public Matrix4x4 WorldScaleMatrix
    {
        get
        {
            Transform? parent = Parent;
            return parent != null ? parent.WorldScaleMatrix * LocalScaleMatrix : LocalScaleMatrix;
        }
    }

    public Matrix4x4 scaleMatrix
    {
        get
        {
            return WorldScaleMatrix;
        }
    }

    public Matrix4x4 LocalRotationMatrix
    {
        get
        {
            return Matrix4x4.CreateFromQuaternion(_rotation);
        }
    }

    public Matrix4x4 localRotationMatrix
    {
        get
        {
            return localRotationMatrix;
        }
    }

    public Matrix4x4 WorldRotationMatrix
    {
        get
        {
            Transform? parent = Parent;
            return parent != null
                ? parent.WorldRotationMatrix * LocalRotationMatrix
                : LocalRotationMatrix;
        }
    }

    public Matrix4x4 rotationMatrix
    {
        get
        {
            return WorldRotationMatrix;
        }
    }

    public Matrix4x4 LocalTranslationMatrix
    {
        get
        {
            return Matrix4x4.CreateTranslation(_position);
        }
    }

    public Matrix4x4 localTranslationMatrix
    {
        get
        {
            return LocalTranslationMatrix;
        }
    }

    public Matrix4x4 LocalMatrix
    {
        get
        {
            return LocalScaleMatrix * LocalRotationMatrix * LocalTranslationMatrix;
        }
    }

    public Matrix4x4 localMatrix
    {
        get
        {
            return LocalMatrix;
        }
    }

    public Matrix4x4 LocalMatrixInverted
    {
        get
        {
            Matrix4x4.Invert(LocalMatrix, out var inverted);
            return inverted;
        }
    }

    public Matrix4x4 localMatrixInverted
    {
        get
        {
            return LocalMatrixInverted;
        }
    }

    public Matrix4x4 LocalToWorldMatrix
    {
        get
        {
            Transform? parent = Parent;
            return parent != null ? LocalMatrix * parent.LocalToWorldMatrix : LocalMatrix;
        }
    }

    public Matrix4x4 localToWorldMatrix
    {
        get
        {
            return localToWorldMatrix;
        }
    }

    public Matrix4x4 WorldToLocalMatrix
    {
        get
        {
            Matrix4x4.Invert(LocalToWorldMatrix, out var inverted);
            return inverted;
        }
    }

    public Matrix4x4 worldToLocalMatrix
    {
        get
        {
            return WorldToLocalMatrix;
        }
    }

    public Vector3 WorldEulerAngles
    {
        get
        {
            return WorldRotation.ToEuler(CoordinateSystem);
        }
        set
        {
            WorldRotation = value.ToQuaternion(CoordinateSystem);
        }
    }

    public Vector3 eulerAngles
    {
        get
        {
            return WorldEulerAngles;
        }
        set
        {
            WorldEulerAngles = value;
        }
    }

    public Vector3 LocalEulerAngles
    {
        get
        {
            return LocalRotation.ToEuler(CoordinateSystem);
        }
        set
        {
            LocalRotation = value.ToQuaternion(CoordinateSystem);
        }
    }

    public Vector3 localEulerAngles
    {
        get
        {
            return LocalEulerAngles;
        }
        set
        {
            LocalEulerAngles = value;
        }
    }

    public Transform()
    {
    }

    public Vector3 TransformDirection(Vector3 direction)
    {
        return Vector3.Transform(direction, WorldRotation);
    }

    public Vector3 TransformDirection(float x, float y, float z)
    {
        return TransformDirection(new Vector3(x, y, z));
    }

    public Vector3 TransformVector(Vector3 vector)
    {
        return Vector3.Transform(vector, WorldScaleMatrix * WorldRotationMatrix);
    }

    public Vector3 TransformVector(float x, float y, float z)
    {
        return TransformVector(new Vector3(x, y, z));
    }

    public Vector3 TransformPoint(Vector3 point)
    {
        return TransformVector(point) + WorldPosition;
    }

    public Vector3 TransformPoint(float x, float y, float z)
    {
        return TransformPoint(new Vector3(x, y, z));
    }

    public Vector3 InverseTransformDirection(Vector3 direction)
    {
        return Vector3.Transform(direction, Quaternion.Inverse(WorldRotation));
    }

    public Vector3 InverseTransformDirection(float x, float y, float z)
    {
        return InverseTransformDirection(new Vector3(x, y, z));
    }

    public Vector3 InverseTransformVector(Vector3 vector)
    {
        Matrix4x4.Invert(WorldScaleMatrix * WorldRotationMatrix, out var inverted);
        return Vector3.Transform(vector, inverted);
    }

    public Vector3 InverseTransformVector(float x, float y, float z)
    {
        return InverseTransformVector(new Vector3(x, y, z));
    }

    public Vector3 InverseTransformPoint(Vector3 point)
    {
        return InverseTransformVector(point - WorldPosition);
    }

    public Vector3 InverseTransformPoint(float x, float y, float z)
    {
        return InverseTransformPoint(new Vector3(x, y, z));
    }

    public void Rotate(Vector3 eulers, Space relativeTo = Space.Self)
    {
        Quaternion quaternion = eulers.ToQuaternion(CoordinateSystem);
        switch (relativeTo)
        {
            case Space.Self:
            {
                LocalRotation *= quaternion;
                break;
            }
            case Space.World:
            {
                WorldRotation *= Quaternion.Inverse(WorldRotation) * quaternion * WorldRotation;
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(relativeTo), relativeTo, null);
            }
        }
    }

    public void Rotate(Vector3 axis, float angle, Space relativeTo = Space.Self)
    {
        switch (relativeTo)
        {
            case Space.Self:
            {
                WorldRotation *= Quaternion.CreateFromAxisAngle(
                    TransformDirection(axis),
                    MathF.RadianOf(angle));
                break;
            }
            case Space.World:
            {
                WorldRotation *= Quaternion.CreateFromAxisAngle(axis, MathF.RadianOf(angle));
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(relativeTo), relativeTo, null);
            }
        }
    }

    public void RotationAround(Vector3 point, Vector3 axis, float angle)
    {
        Vector3 currWorldPos = WorldPosition;
        Quaternion quaternion = Quaternion.CreateFromAxisAngle(axis, MathF.RadianOf(angle));
        Vector3 diff = currWorldPos - point;
        diff = Vector3.Transform(diff, quaternion);
        _ = WorldPosition = point + diff;

        Rotate(axis, angle, Space.World);
    }

    public void LookAt(Transform target)
    {
        LookAt(target, Vector3.UnitY);
    }

    public void LookAt(Transform target, Vector3 worldUp)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        LookAt(target.WorldPosition, worldUp);
    }

    public void LookAt(Vector3 targetWorldPosition)
    {
        LookAt(targetWorldPosition, Vector3.UnitY);
    }

    public void LookAt(Vector3 targetWorldPosition, Vector3 worldUp)
    {
        Matrix4x4 lookAtMat4x4 = Matrix4x4Helper.CreateLookAt(
            WorldPosition,
            targetWorldPosition,
            worldUp,
            CoordinateSystem);

        Matrix4x4.Decompose(
            lookAtMat4x4,
            out _,
            out var newWorldRotation,
            out _
        );

        WorldRotation = newWorldRotation;
    }

    public void Rotate(float x, float y, float z, Space relativeTo = Space.Self)
    {
        Rotate(new Vector3(x, y, z), relativeTo);
    }

    public void Translate(Vector3 translation, Space space = Space.Self)
    {
        switch (space)
        {
            case Space.World:
            {
                WorldPosition += translation;
                break;
            }
            case Space.Self:
            {
                WorldPosition += TransformDirection(translation);
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }
    }

    public void Translate(float x, float y, float z, Space space = Space.Self)
    {
        Translate(new Vector3(x, y, z), space);
    }

    public void Translate(Vector3 translation, Transform relativeTo)
    {
        WorldPosition += relativeTo != null
            ? relativeTo.TransformDirection(translation)
            : translation;
    }

    public void Translate(float x, float y, float z, Transform relativeTo)
    {
        Translate(new Vector3(x, y, z), relativeTo);
    }
}
