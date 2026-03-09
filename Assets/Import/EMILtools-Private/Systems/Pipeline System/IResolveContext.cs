using System;
using System.Threading.Tasks;
using EMILtools.Timers;
using UnityEngine;
using static EMILtools.Timers.TimerUtility;


/// <summary>
/// Context for ResolveContexts, used to pass data and control the flow of the pipeline
/// Resolves happen BEFORE execution
/// </summary>
public interface IResolveContext
{
    public virtual void Reset() { }
    public bool Resolve<TContext>(in TContext ctx) where TContext : class;
}

/// <summary>
/// Represents an interface for waitable resolve operations in the pipeline.
/// This interface enables components to await asynchronous resolution
/// while maintaining the non-blocking nature of the resolve process.
/// </summary>
public interface IResolveWaitable
{
    bool waiting { get; set; }
    public Task WaitUntilResolved(bool reenacting = false)
    {
        Debug.Log("WaitUntilResolved Called");
        if (reenacting) waiting = true;
        return cachedWaitTask;
    }
    public Task cachedWaitTask { get; set; }
}

/// <summary>
/// Represents a callback mechanism that can be invoked before pipeline step execution
/// </summary>
public class Callback : IResolveContext
{
    static readonly bool ContinueResolving = true;
    public readonly Action Action;

    public Callback(Action _action)
    {
        Action = _action;
    }
    public bool resolveBeforeExecution { get; set; }

    public bool Resolve<TContext>(in TContext ctx) where TContext : class
    {
        //Debug.Log("CALLBACK");
        Action?.Invoke();
        return ContinueResolving;
    }
}

/// <summary>
/// Timed resolving context that integrates with a pipeline execution flow.
/// Will ShortCircuit if the timer is not finished (Only If the StepType is a ShortCircuit)
/// </summary>
public class Timed : IResolveContext, ITimerUser
{
    // Is not intended to be read as ShortCircuit FALSE, used just for readability in the Resolve()
    bool ShortCircuitIfNotFinished => false; 
    bool ContinueResolving => true;
    public CountdownTimer Timer => timer;
    CountdownTimer timer;
    public Timed(float sec)
    {
        timer = new CountdownTimer(sec);
        this.InitTimer(timer, isFixed: true); }
    
    public bool Resolve<TContext>(in TContext ctx) where TContext : class
    {
        if(!timer.isRunning && !timer.isFinished()) timer.Start();
        Debug.Log($"Timer called, isRunning: {timer.isRunning}, isFinished: {timer.isFinished()}");
        return timer.isFinished() ? ContinueResolving : ShortCircuitIfNotFinished;
    }
}

/// <summary>
/// Represents a waitable component used in the pipeline that incorporates a countdown timer.
/// Provides functionality to wait asynchronously until the timer completes (Stays in UnityTime)
/// Used when delays are necessary before progressing within the pipeline.
/// </summary>
public class Wait : IResolveContext, ITimerUser, IResolveWaitable
{
    // --- static ----
    static bool ContinueResolving = true;
    
    // --- Privates ----
    CountdownTimer timer;
    TaskCompletionSource<bool> tcs;
    
    // --- API ----
    public bool waiting { get; set; } = false;
    public Task cachedWaitTask { get; set; }
    public CountdownTimer Timer => timer;
    public bool resolveBeforeExecution { get; set; }
    // --- Ctor ----
    public Wait(float sec)
    {
        timer = new CountdownTimer(sec);
        this.InitTimer(timer, isFixed: true);
        tcs = new();
        cachedWaitTask = tcs.Task;
        timer.OnTimerStop.Add(TimerStopped);
        Debug.Log("Wait Timer Initialized");
    }
    
    void TimerStopped()
    {
        tcs.TrySetResult(true);
        waiting = false;
        Debug.Log("Wait Timer Finished");
    }


    public void Reset()
    {
        tcs = new();
        cachedWaitTask = tcs.Task;
        timer.Reset();
    }
    
    public bool Resolve<TContext>(in TContext ctx) where TContext : class
    {
        if (!timer.isRunning && !timer.isFinished())
        {
            timer.Start();
            Debug.Log("Started Wait Timer");
        }
        return ContinueResolving;
    }
}

