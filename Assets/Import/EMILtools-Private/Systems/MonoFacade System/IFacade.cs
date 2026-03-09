public interface IFacade<TContext>
    where TContext : struct, IModuleUsabableContext
{
    public FacadeComposition<TContext> comp { get; }
    public TBlackboardType API_Blackboard<TBlackboardType>() where TBlackboardType : IBlackboard
        => (TBlackboardType)comp.Blackboard;
    public TConfigType API_Config<TConfigType>() where TConfigType : IConfig
        => (TConfigType)comp.Config;
    public TFunctionalityType API_Functionality<TFunctionalityType>() where TFunctionalityType : IFunctionality
        => (TFunctionalityType)comp.Functionality;
    public TContext API_Context() => comp.Context;
}

public struct FacadeComposition<TContext>
    where TContext : struct, IModuleUsabableContext
{
    public readonly IBlackboard Blackboard;
    public readonly IConfig Config;
    public readonly IFunctionality Functionality;
    public TContext Context;

    
    public FacadeComposition(IBlackboard _blackboard, IConfig _config, IFunctionality _functionality)
    {
        Blackboard = _blackboard;
        Config = _config;
        Functionality = _functionality;
        Context = default;
    }
}