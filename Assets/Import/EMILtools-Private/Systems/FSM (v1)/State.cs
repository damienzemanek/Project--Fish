using EMILtools.Systems;

public abstract class State : IState
{
    
    public virtual void OnEnterState(IContextViewImmutable ctx)
    {
        
    }

    public virtual void OnExitState(IContextViewImmutable ctx)
    {
        
    }
}