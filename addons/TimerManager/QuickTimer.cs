using System;
using Godot;

/// <summary>
/// private class hiding the implementation of ITimer
/// </summary>
internal class QuickTimer : ITimer
{
    public Node Owner { get; set; }

    private float _timeInSeconds;
    private bool _repeats;
    private Action<ITimer> _onTime;
    private bool _isDone;
    private float _elapsedTime;


    public void Stop()
    {
        _isDone = true;
    }

    public void Reset()
    {
        _elapsedTime = 0f;
    }

    public Node GetOwner()
    {
        return Owner;
    }

    internal bool Tick(float delta)
    {
        // if stop was called before the tick then isDone will be true and we should not tick again no matter what
        if (!_isDone && _elapsedTime > _timeInSeconds)
        {
            _elapsedTime -= _timeInSeconds;
            _onTime(this);

            if (!_isDone && !_repeats)
            {
                _isDone = true;
            }
        }

        _elapsedTime += delta;

        return _isDone;
    }

    internal void Initialize(float timeInSeconds, bool repeats, Node owner, Action<ITimer> onTime)
    {
        _timeInSeconds = timeInSeconds;
        _repeats = repeats;
        Owner = owner;
        _onTime = onTime;
        
    }

    /// <summary>
    /// nulls out the object references so the GC can pick them up if needed
    /// </summary>
    internal void Unload()
    {
        Owner = null;
        _onTime = null;
    }
}