public interface IExecutionPathway { }

public interface UPDATE : IExecutionPathway
{
    void OnUpdateTick();
}

public interface FIXED_UPDATE : IExecutionPathway
{
    void OnFixedTick();
}

public interface LATE_UPDATE : IExecutionPathway
{
    void OnLateTick();
}

public interface ON_SET : IExecutionPathway
{
    abstract void OnSet();
}