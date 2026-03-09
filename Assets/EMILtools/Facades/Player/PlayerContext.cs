using EMILtools.Systems;
using Sirenix.OdinInspector;

public interface IPlayerContextView : IContextViewImmutable
{
    // Readonly properties
    public float SomeInt { get; }
}

public class PlayerContextData : ContextData<PlayerBlackboard>, IPlayerContextView, IModuleUsabableContext
{
    // Mutable state
    [field: ShowInInspector] public float SomeInt { get; set; }
}