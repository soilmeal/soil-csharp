using System;
using System.Runtime.CompilerServices;

namespace Soil.Game;

public class Timer : ITimer
{
    public delegate void UpdateHandler();

    public delegate void FixedUpdateHandler();

    private readonly TimeSpan _fixedTimeStep;

    private TimeSpan _lastTime;

    private float _scale;

    private TimeSpan _time;

    private TimeSpan _deltaTime;

    private TimeSpan _unscaledTime;

    private TimeSpan _unscaledDeltaTime;

    private TimeSpan _smoothDeltaTime;

    private TimeSpan _fixedTime;

    private TimeSpan _fixedUnscaledTime;

    private UpdateHandler? _onUpdate;

    private FixedUpdateHandler? _onFixedUpdate;

    public float Scale
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

    public float scale
    {
        get
        {
            return Scale;
        }
        set
        {
            Scale = value;
        }
    }

    public float Time
    {
        get
        {
            return (float)TimeAsDouble;
        }
    }

    public float time
    {
        get
        {
            return Time;
        }
    }

    public double TimeAsDouble
    {
        get
        {
            return _time.TotalSeconds;
        }
    }

    public double timeAsDouble
    {
        get
        {
            return TimeAsDouble;
        }
    }


    public float DeltaTime
    {
        get
        {
            return (float)DeltaTimeAsDouble;
        }
    }

    public float deltaTime
    {
        get
        {
            return DeltaTime;
        }
    }

    public double DeltaTimeAsDouble
    {
        get
        {
            return _deltaTime.TotalSeconds;
        }
    }

    public double deltaTimeAsDouble
    {
        get
        {
            return DeltaTimeAsDouble;
        }
    }

    public float UnscaledTime
    {
        get
        {
            return (float)UnscaledTimeAsDouble;
        }
    }

    public float unscaledTime
    {
        get
        {
            return UnscaledTime;
        }
    }

    public double UnscaledTimeAsDouble
    {
        get
        {
            return _unscaledTime.TotalSeconds;
        }
    }

    public double unscaledTimeAsDouble
    {
        get
        {
            return UnscaledTimeAsDouble;
        }
    }

    public float UnscaledDeltaTime
    {
        get
        {
            return (float)UnscaledDeltaTimeAsDouble;
        }
    }

    public float unscaledDeltaTime
    {
        get
        {
            return UnscaledDeltaTime;
        }
    }

    public double UnscaledDeltaTimeAsDouble
    {
        get
        {
            return _unscaledDeltaTime.TotalSeconds;
        }
    }

    public double unscaledDeltaTimeAsDouble
    {
        get
        {
            return UnscaledDeltaTimeAsDouble;
        }
    }

    public float SmoothDeltaTime
    {
        get
        {
            return (float)_smoothDeltaTime.TotalSeconds;
        }
    }

    public float smoothDeltaTime
    {
        get
        {
            return SmoothDeltaTime;
        }
    }

    public float FixedTime
    {
        get
        {
            return (float)FixedTimeAsDouble;
        }
    }

    public float fixedTime
    {
        get
        {
            return FixedTime;
        }
    }

    public double FixedTimeAsDouble
    {
        get
        {
            return _fixedTime.TotalSeconds;
        }
    }

    public double fixedTimeAsDouble
    {
        get
        {
            return FixedTimeAsDouble;
        }
    }

    public float FixedDeltaTime
    {
        get
        {
            return (float)FixedDeltaTimeAsDouble;
        }
    }

    public float fixedDeltaTime
    {
        get
        {
            return FixedDeltaTime;
        }
    }

    public double FixedDeltaTimeAsDouble
    {
        get
        {
            return _fixedTimeStep.TotalSeconds;
        }
    }

    public double fixedDeltaTimeAsDouble
    {
        get
        {
            return FixedDeltaTimeAsDouble;
        }
    }

    public float FixedUnscaledTime
    {
        get
        {
            return (float)FixedUnscaledTimeAsDouble;
        }
    }

    public float fixedUnscaledTime
    {
        get
        {
            return FixedUnscaledTime;
        }
    }

    public double FixedUnscaledTimeAsDouble
    {
        get
        {
            return _unscaledDeltaTime.TotalSeconds;
        }
    }

    public double fixedUnscaledTimeAsDouble
    {
        get
        {
            return FixedUnscaledTimeAsDouble;
        }
    }

    public float FixedUnscaledDeltaTime
    {
        get
        {
            return (float)FixedUnscaledDeltaTimeAsDouble;
        }
    }

    public float fixedUnscaledDeltaTime
    {
        get
        {
            return FixedUnscaledDeltaTime;
        }
    }

    public double FixedUnscaledDeltaTimeAsDouble
    {
        get
        {
            TimeSpan fixedUnscaledDeltaTime = _scale >= 0f ? _fixedTimeStep / _scale : _fixedTimeStep;

            return fixedUnscaledDeltaTime.TotalSeconds;
        }
    }

    public double fixedUnscaledDeltaTimeAsDouble
    {
        get
        {
            return FixedUnscaledDeltaTimeAsDouble;
        }
    }

    public UpdateHandler? OnUpdate
    {
        get
        {
            return _onUpdate;
        }
        set
        {
            _onUpdate = value;
        }
    }

    public FixedUpdateHandler? OnFixedUpdate
    {
        get
        {
            return _onFixedUpdate;
        }
        set
        {
            _onFixedUpdate = value;
        }
    }

    public Timer(TimeSpan fixedTimeStep)
        : this(fixedTimeStep, DateTime.UtcNow.TimeOfDay)
    {
    }

    public Timer(TimeSpan fixedTimeStep, TimeSpan initialTime)
    {
        _fixedTimeStep = fixedTimeStep;
        Reset(initialTime);
    }

    public void Update(TimeSpan currTime)
    {
        TimeSpan lastTimeGap = currTime - _lastTime;
        _unscaledDeltaTime = currTime - _lastTime;
        _unscaledTime += _unscaledDeltaTime;

        _deltaTime = _unscaledDeltaTime * _scale;
        _time += _deltaTime;

        // smoothing based on https://forum.unity.com/threads/time-smoothdeltatime.10253/
        _smoothDeltaTime = _smoothDeltaTime > TimeSpan.Zero
            ? (_smoothDeltaTime * 0.8) + (_deltaTime * 0.2)
            : _deltaTime;

        TimeSpan gap = _time - _fixedTime;
        while (gap >= _fixedTimeStep)
        {
            _fixedTime += _fixedTimeStep;
            _fixedUnscaledTime += _scale > 0f ? _fixedTimeStep / _scale : TimeSpan.Zero;

            _onFixedUpdate?.Invoke();
        }

        _onUpdate?.Invoke();
        _lastTime = currTime;
    }

    public void Reset()
    {
        Reset(DateTime.UtcNow.TimeOfDay);
    }

    public void Reset(TimeSpan resetTime)
    {
        _lastTime = resetTime;

        _unscaledDeltaTime = TimeSpan.Zero;
        _smoothDeltaTime = TimeSpan.Zero;
    }
}
