using System.Threading.Tasks;

namespace EMILtools.Systems
{
    public interface ISubscriber
    {
        bool isActive { get; set; }
        Task Execute();

    }
    
    public interface ISubscriber<TContext> : ISubscriber
    {
        Task Execute(TContext ctx);
    }
    
    public interface ISubscriber<T1, T2> : ISubscriber
    {
        Task Execute(T1 ctx1, T2 ctx2);
    }
    

}