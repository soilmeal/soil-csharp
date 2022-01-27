using System;
using System.Collections.Generic;
using System.Linq;

namespace Soil.Utils;

public class StateMachine<TStateType>
    where TStateType : struct, Enum

{
    private static readonly NoneState _none = new();

    private readonly Dictionary<TStateType, IState<TStateType>> _states;

    private IState<TStateType> _lastState;

    private IState<TStateType> _currentState;

    public IState<TStateType> CurrentState
    {
        get
        {
            return _currentState;
        }
    }

    public IState<TStateType> LastState
    {
        get
        {
            return _lastState;
        }
    }

    public StateMachine(params IState<TStateType>[] states)
    {
        _states = states.ToDictionary(state => state.Type);

        _lastState = _none;
        _currentState = _none;
    }

    public void ChangeState(TStateType state)
    {
        ChangeState(state, false);
    }

    public void SetState(TStateType state)
    {
        ChangeState(state, true);
    }

    private void ChangeState(TStateType state, bool overwrite)
    {
        var newState = _states[state];

        if (!overwrite && _currentState != _none)
        {
            _currentState.Exit();
        }

        _lastState = _currentState;
        _currentState = newState;
        _currentState.Enter();
    }

    public void Update()
    {
        if (_currentState == _none)
        {
            return;
        }

        _currentState.Execute();
    }

    private class NoneState : IState<TStateType>
    {
        public TStateType Type
        {
            get
            {
                return default;
            }
        }

        public void Enter()
        {
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}
