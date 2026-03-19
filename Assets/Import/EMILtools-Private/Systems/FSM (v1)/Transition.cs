
using Sirenix.OdinInspector;

public interface ITransition
{
    IState To { get; }
    IPredicate Condition { get; }
}

public class Transition : ITransition
{
    [ShowInInspector] readonly string toName;
    public IState To { get; }
    public IPredicate Condition { get; }
    
    public Transition(IState to, IPredicate condition)
    {
        To = to;
        Condition = condition;
        toName = to.GetType().Name;
    }
}