using EMILtools.Systems;

public interface IExecutionPathway { }

public interface UPDATE : IExecutionPathway
{
    void OnUpdateTick<TContext>(TContext ctx);
}

public interface FIXED_UPDATE : IExecutionPathway
{
    void OnFixedTick<TContext>(TContext ctx);
}

public interface LATE_UPDATE : IExecutionPathway
{
    void OnLateTick<TContext>(TContext ctx);
}

public interface ON_SET : IExecutionPathway
{
    abstract void OnSet();
}