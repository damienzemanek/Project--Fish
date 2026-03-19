using EMILtools.Systems;

public interface IExecutionPathway { }

public interface UPDATE : IExecutionPathway { }

public interface FIXED_UPDATE : IExecutionPathway { }

public interface LATE_UPDATE : IExecutionPathway { }


public interface ON_SET : IExecutionPathway
{
    abstract void MutateUsingNewSetValues();
}



public interface FSM_AVALIABLE
{
    
}

public interface FSM_STATE_ENTER : IExecutionPathway, FSM_AVALIABLE
{
    abstract void OnEnterState(IContextViewImmutable ctx);
}

public interface FSM_STATE_EXIT : IExecutionPathway, FSM_AVALIABLE
{
    abstract void OnExitState(IContextViewImmutable ctx);
}