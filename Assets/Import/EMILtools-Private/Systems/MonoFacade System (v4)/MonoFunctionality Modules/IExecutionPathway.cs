using EMILtools.Systems;

public interface IExecutionPathway { }

public interface UPDATE : IExecutionPathway { }

public interface FIXED_UPDATE : IExecutionPathway { }

public interface LATE_UPDATE : IExecutionPathway { }

public interface ON_SET : IExecutionPathway
{
    abstract void OnSet();
}