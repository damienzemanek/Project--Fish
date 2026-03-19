using EMILtools.Systems;

public interface IExecutionPathway { }

public interface UPDATE : IExecutionPathway { }

public interface FIXED_UPDATE : IExecutionPathway { }

public interface LATE_UPDATE : IExecutionPathway { }


public interface ON_SET : IExecutionPathway
{
    abstract void MutateUsingNewSetValues();
}


public interface FSM_AVALIABLE : IState { }

public interface FSM_STATE_ENTER<TViewCtx> : IExecutionPathway, FSM_AVALIABLE
    where TViewCtx : IContextViewImmutable
{
    abstract void OnEnterState(TViewCtx ctx);
    void IState.OnEnterState(IContextViewImmutable ctx) => OnEnterState((TViewCtx)ctx);
}

public interface FSM_STATE_EXIT<TViewCtx> : IExecutionPathway, FSM_AVALIABLE
    where TViewCtx : IContextViewImmutable

{
    abstract void OnExitState(TViewCtx ctx);
    void IState.OnExitState(IContextViewImmutable ctx) => OnEnterState((TViewCtx)ctx);
}