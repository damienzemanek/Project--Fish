using System.Threading.Tasks;
using UnityEngine;

namespace EMILtools.Systems
{
    public interface IResolvable
    {
        public virtual void Reset() { }
        public bool Resolve();
    }

    /// <summary>
    /// Context for ResolveContexts, used to pass data and control the flow of the pipeline
    /// Resolve "BEFORE" and "AFTER" the execution of the pipeline step
    /// (!) NOTE: "AFTER" resolves will NOT run if the pipeline step short circuits
    /// (!) NOTE: "BEFORE" resolves will run even if the pipeline step short circuits
    /// (!) NOTE: The main method will NOT run if ANY pipeline step short circuits
    /// </summary>
    public interface IResolvableWithContext : IResolvable
    {
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
}
