namespace Soil.Game;

public interface ITimer
{

    public float Scale { get; set; }

    public float scale { get; set; }

    public float Time { get; }

    public float time { get; }

    public double TimeAsDouble { get; }

    public double timeAsDouble { get; }

    public float DeltaTime { get; }

    public float deltaTime { get; }

    public double DeltaTimeAsDouble { get; }

    public double deltaTimeAsDouble { get; }

    public float UnscaledTime { get; }

    public float unscaledTime { get; }

    public double UnscaledTimeAsDouble { get; }

    public double unscaledTimeAsDouble { get; }

    public float UnscaledDeltaTime { get; }

    public float unscaledDeltaTime { get; }

    public double UnscaledDeltaTimeAsDouble { get; }

    public double unscaledDeltaTimeAsDouble { get; }

    public float SmoothDeltaTime { get; }

    public float smoothDeltaTime { get; }

    public float FixedTime { get; }

    public float fixedTime { get; }

    public double FixedTimeAsDouble { get; }

    public double fixedTimeAsDouble { get; }

    public float FixedDeltaTime { get; }

    public float fixedDeltaTime { get; }

    public double FixedDeltaTimeAsDouble { get; }

    public double fixedDeltaTimeAsDouble { get; }

    public float FixedUnscaledTime { get; }

    public float fixedUnscaledTime { get; }

    public double FixedUnscaledTimeAsDouble { get; }

    public double fixedUnscaledTimeAsDouble { get; }

    public float FixedUnscaledDeltaTime { get; }

    public float fixedUnscaledDeltaTime { get; }

    public double FixedUnscaledDeltaTimeAsDouble { get; }

    public double fixedUnscaledDeltaTimeAsDouble { get; }
}
