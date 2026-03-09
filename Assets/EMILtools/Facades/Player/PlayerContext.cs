using EMILtools.Systems;

public interface IPlayerContextView : IContextViewImmutable
{
    public float SomeInt { get; }
}

public class PlayerContextData : ContextData, IPlayerContextView, IModuleUsabableContext
{
    public float SomeInt { get; set; }
}