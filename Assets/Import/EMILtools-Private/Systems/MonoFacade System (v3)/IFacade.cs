
namespace EMILtools.Systems
{
    public interface IFacade
    {
        public TMonoStructureType API_Structure<TMonoStructureType>() where TMonoStructureType : IMonoStructure;
        public TContextType API_Context<TContextType>() where TContextType : IContext;
        public TBlackboardType API_Blackboard<TBlackboardType>() where TBlackboardType : IBlackboard;
        public TConfigType API_Config<TConfigType>() where TConfigType : IConfig;
        public TFunctionalityType API_Functionality<TFunctionalityType>() where TFunctionalityType : IFunctionality;
    }

}
