public interface IFacade<TMonoStructureType>
    where TMonoStructureType : IMonoStructure
{
    public TMonoStructureType API_Structure();
    public TBlackboardType API_Blackboard<TBlackboardType>() where TBlackboardType : IBlackboard;
    public TConfigType API_Config<TConfigType>() where TConfigType : IConfig;
    public TFunctionalityType API_Functionality<TFunctionalityType>() where TFunctionalityType : IFunctionality;
}
