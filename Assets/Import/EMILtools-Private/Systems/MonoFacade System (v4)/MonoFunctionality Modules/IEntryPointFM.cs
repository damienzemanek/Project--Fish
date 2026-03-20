using EMILtools.Systems;

public interface IEntryPointFM { }

public interface UPDATE : IEntryPointFM { }

public interface FIXED_UPDATE : IEntryPointFM { }

public interface LATE_UPDATE : IEntryPointFM { }


public interface ON_SET : IEntryPointFM
{
    abstract void MutateUsingNewSetValues();
}

public interface FSM_AVALIABLE : IState { }

public interface FSM_STATE_ENTER<TViewCtx> : IEntryPointFM, FSM_AVALIABLE
    where TViewCtx : IContextViewImmutable
{
    abstract void OnEnterState(TViewCtx ctx);
    void IState.OnEnterState(IContextViewImmutable ctx) => OnEnterState((TViewCtx)ctx);
}

public interface FSM_STATE_EXIT<TViewCtx> : IEntryPointFM, FSM_AVALIABLE
    where TViewCtx : IContextViewImmutable

{
    abstract void OnExitState(TViewCtx ctx);
    void IState.OnExitState(IContextViewImmutable ctx) => OnEnterState((TViewCtx)ctx);
}