using System.Threading.Tasks;

namespace EMILtools.Systems
{
    public interface ISubscriber<TContext>
        where TContext : class, IContext
    {
        bool isActive { get; set; }
        Task Execute(TContext ctx);
        Task Execute();
    }
}