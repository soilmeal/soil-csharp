using System.Numerics;
using Soil.Math;

namespace Soil.Game;

public class Transform
{
    private Transform? _parent;

    private Vector3 _position;

    private Vector3 _scale;

    private Quaternion _rotation;

    private CoordinateSystem _coordinateSystem = CoordinateSystem.LeftHanded;

    public Transform? Parent
    {
        get
        {
            return _parent;
        }
        set
        {
            if (value == null)
            {
                return;
            }

            _parent = value;
            _coordinateSystem = value?._coordinateSystem ?? _coordinateSystem;
        }
    }

    public Transform(CoordinateSystem coordinateSystem)
    {
        _coordinateSystem = coordinateSystem;
    }

    public Transform(Transform? parent)
    {
        Parent = parent;
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

            Transform? parent = _parent;
            if (parent != null)
            {
                matrix *= parent.LocalMatrixInverted;
            }


            Matrix4x4.Decompose(matrix, out _scale, out _rotation, out _position);
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

    public Quaternion WorldRotation
    {
        get
        {
            Transform? parent = _parent;
            return parent != null ? parent.WorldRotation * _rotation : _rotation;
        }
        set
        {
            Transform? parent = _parent;
            _rotation = parent != null ? Quaternion.Inverse(parent.WorldRotation) * value : value;
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

    public Vector3 Up
    {
        get
        {
            return Vector3.Transform(Vector3.UnitY, WorldRotation);
        }
    }

    public Vector3 Right
    {
        get
        {
            return Vector3.Transform(Vector3.UnitX, WorldRotation);
        }
    }

    public Matrix4x4 LocalToWorldMatrix
    {
        get
        {
            Transform? parent = _parent;
            return parent != null ? parent.LocalMatrix * LocalMatrix : LocalMatrix;
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

    public Matrix4x4 LocalMatrix
    {
        get
        {
            return Matrix4x4.CreateScale(_scale)
                * Matrix4x4.CreateFromQuaternion(_rotation)
                * Matrix4x4.CreateTranslation(_position);
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

    public Vector3 WorldEulerAngles
    {
        get
        {
            return WorldRotation.ToEuler(_coordinateSystem);
        }
        set
        {
            WorldRotation = value.ToQuaternion(_coordinateSystem);
        }
    }

    public Vector3 LocalEulerAngles
    {
        get
        {
            return LocalRotation.ToEuler(_coordinateSystem);
        }
        set
        {
            LocalRotation = value.ToQuaternion(_coordinateSystem);
        }
    }
}
