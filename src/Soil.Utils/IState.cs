using System;

namespace Soil.Utils;

public interface IState<TStateType>
    where TStateType : struct, Enum
{
    public TStateType Type { get; }

    public void Enter();

    public void Execute();

    public void Exit();
}
