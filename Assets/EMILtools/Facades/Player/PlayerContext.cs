using EMILtools.Systems;

public interface IPlayerContextView : IContextViewImmutable
{
    // Readonly properties
}

public class PlayerContextData : ContextData<IPlayerContextView>, IPlayerContextView, IModuleUsabableContext
{
    // Mutable state
}