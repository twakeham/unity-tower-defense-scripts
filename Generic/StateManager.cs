using UnityEngine;
using System;
using System.Collections.Generic;


// Simple state machine

public sealed class StateManager<T> {

    public T context;
    public event Action onStateChanged;

    public State<T> currentState {
        get {
            return _state;
        }
    }
    public State<T> previousState;

    private State<T> _state;
    private Dictionary<System.Type, State<T>> _states = new Dictionary<System.Type, State<T>>();

    public StateManager(T context, State<T> initialState) {
        context = context;
        addState(initialState);
        _state = initialState;
        _state.begin();
    }

    public void addState(State<T> state) {
        state.setManager(this, context);
        _states[state.GetType()] = state;
    }

    public void update(float deltaTime) {
        _state.update(deltaTime);
    }

    public R changeState<R>() where R : State<T> {
        System.Type type = typeof(R);
        if (_state.GetType() == type) {
            return _state as R;
        }

        if (_state != null) {
            _state.end();
        }

        previousState = _state;
        _state = _states[type];
        _state.begin();

        if (onStateChanged != null) {
            onStateChanged();
        }

        return _state as R;
    }
}


public class State<T> {

    public event Action onStateEnter;
    public event Action onStateLeave;

    protected StateManager<T> _manager;
    protected T _context;

    protected float deltaT;

    public State() { }

    internal void setManager(StateManager<T> manager, T context) {
        _manager = manager;
        _context = context;
        init();
    }

    public virtual void init() { }

    public virtual void begin() { }

    public virtual void update(float deltaTime) {
        deltaT = deltaTime;
    }

    public virtual void end() { }

}