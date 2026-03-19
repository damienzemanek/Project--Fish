using System;
using System.Threading.Tasks;
using EMILtools.Timers;
using UnityEngine;
using static EMILtools.Timers.TimerUtility;


namespace EMILtools.Systems
{

    public abstract class Resolvable : IResolvable
    {
        public readonly bool executeOnce;
        public bool consumed { get; set; }
        protected Resolvable(bool executeOnce = false) => this.executeOnce = executeOnce;
        public virtual void ResetWait()
        {
            consumed = false;
        }
        public abstract bool Resolve(object ctx);
    }
    
    
    
    /// <summary>
    /// Represents a callback mechanism that can be invoked before pipeline step execution
    /// </summary>
    public class Callback : Resolvable
    {
        readonly bool ContinueResolving = true;
        readonly Action Action;

        public Callback(Action _action, bool executeOnce = false) : base(executeOnce)
            => Action = _action;

        public override bool Resolve(object ctx)
        {
            Action?.Invoke();
            return ContinueResolving;
        }
    }
    
    /// <summary>
    /// Represents a callback mechanism that can be invoked before pipeline step execution
    /// </summary>
    public class Callback<TCtx> : Resolvable
    {
        readonly bool ContinueResolving = true;
        readonly Action<TCtx> Action;
        [NonSerialized] TCtx cached;
        
        public Callback(Action<TCtx> _action, bool executeOnce = false) : base(executeOnce)
            => Action = _action;

        
        public bool Resolve<TContext1>(TContext1 ctx)
        {
            if (ctx is TCtx typed) cached = typed;
            else throw new InvalidCastException("Wrong Context Type given to Callback");
            Action?.Invoke(cached);
            return ContinueResolving;
        }

        public override bool Resolve(object ctx)
        {
            cached = (TCtx)ctx;
            Action?.Invoke(cached);
            return ContinueResolving;
        }
    }
    

    /// <summary>
    /// Short Circuits if time is not finished
    /// Re-accesses the timer to check if it is finished
    /// If its finished it will continue
    /// </summary>
    public class TimedGate : Resolvable, ITimerUser
    {
        // Is not intended to be read as ShortCircuit FALSE, used just for readability in the Resolve()
        readonly bool ShortCircuitIfNotFinished = false; 
        readonly bool ContinueResolving = true;
        readonly bool resetWhenAccessedAndDone;
        readonly CountdownTimer timer;

        bool open => timer.IsFinished();
        
        public TimedGate(float sec, bool resetWhenAccessedAndDone, out Action resetHandle, bool executeOnce = false) : base(executeOnce)
        {
            timer = new CountdownTimer(sec);
            this.InitTimer(timer, isFixed: true); 
            this.resetWhenAccessedAndDone = resetWhenAccessedAndDone;
            resetHandle = () => timer.StartAndReset();
        }
        
        public override bool Resolve(object ctx)
        {
            if(!timer.isRunning && !timer.IsFinished()) timer.StartAndReset();
            var _open = open;
            if(open && resetWhenAccessedAndDone) timer.Reset();
            return _open ? ContinueResolving : ShortCircuitIfNotFinished;
        }
    }

    /// <summary>
    /// Blocks until complete
    /// </summary>
    public class Wait : Resolvable, ITimerUser, IResolveWaitable
    {
        // --- static ----
        readonly bool ContinueResolving = true;
        
        // --- Privates ----
        readonly CountdownTimer timer;
        TaskCompletionSource<bool> blockingTcs;
        
        // --- API ----
        public bool waiting { get; set; } = false;
        public Task cachedWaitTask { get; set; }

        // --- Ctor ----
        public Wait(float sec, bool executeOnce = false) : base(executeOnce)
        {
            timer = new CountdownTimer(sec);
            this.InitTimer(timer, isFixed: true);
            blockingTcs = new();
            cachedWaitTask = blockingTcs.Task;
            timer.OnTimerStop.Add(TimerStopped);
        }
        
        void TimerStopped()
        {
            blockingTcs.TrySetResult(true);
            waiting = false;
            Debug.Log("Wait Timer Finished");
            ResetWait();
        }


        public override void ResetWait()
        {
            blockingTcs = new();
            cachedWaitTask = blockingTcs.Task;
            timer.Reset();
        }
        

        public override bool Resolve(object ctx)
        {
            Debug.Log("Wait Timer Called");
            if (!timer.isRunning && !timer.IsFinished())
            {
                timer.StartAndReset();
                Debug.Log("Started Wait Timer");
            }
            return ContinueResolving;
            
        }
    }
    
}



