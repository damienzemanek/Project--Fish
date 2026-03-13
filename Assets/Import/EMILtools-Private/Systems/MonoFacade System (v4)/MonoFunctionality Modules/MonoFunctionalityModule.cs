using Sirenix.OdinInspector;


namespace EMILtools.Systems
{
    public interface IMonoFunctionalityModule
    {
        public abstract ISubscriber Subscriber { get; }
        protected virtual void Awake() { }
        public abstract void SetupModule();
    }
    
    public abstract class MonoFunctionalityModule<TFacade>
        : IMonoFunctionalityModule
        where TFacade : class, IFacade
    {
        [Title("$Name"), PropertyOrder(-1)]
        [ShowInInspector] public string Name => "Module: " + this.GetType().Name;
    
    
        // ---------- Variables ----------
        public TFacade facade { get; set; }

        // ---------- Ctor ----------
        public MonoFunctionalityModule(TFacade facade) => this.facade = facade;
    
    
        // ---------- Abstracts ----------
        public abstract ISubscriber Subscriber { get; }

        /// <summary>
        /// Set up the module, called from Monobehaviour's Awake
        /// </summary>
        protected virtual void Awake() { }
    
        /// <summary>
        /// "Template Method Pattern" For Awake
        /// </summary>
        public abstract void SetupModule();

    }
}
