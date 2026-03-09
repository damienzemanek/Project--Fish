using EMILtools.Systems;

public interface IPlayerContextView : IContextViewImmutable
{
    // Readonly properties
}

public class PlayerContextData : ContextData, IPlayerContextView, IModuleUsabableContext
{
    // Mutable state
}