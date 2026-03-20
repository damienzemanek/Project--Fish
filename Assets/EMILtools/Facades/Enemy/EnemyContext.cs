using EMILtools.Systems;

public interface IEnemyContextView : IContextViewImmutable
{
    public float SomeInt { get; }
}

public class EnemyContextData : ContextData, IEnemyContextView
{
    public float SomeInt { get; set; }
}