using EMILtools.Systems;

public interface IExecutionPathway { }

public interface UPDATE : IExecutionPathway
{
    void OnUpdateTick(IContextViewImmutable ctx);
}

public interface FIXED_UPDATE : IExecutionPathway
{
    void OnFixedTick(IContextViewImmutable ctx);
}

public interface LATE_UPDATE : IExecutionPathway
{
    void OnLateTick(IContextViewImmutable ctx);
}

public interface ON_SET : IExecutionPathway
{
    abstract void OnSet();
}