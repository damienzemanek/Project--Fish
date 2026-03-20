using EMILtools.Systems;
using Pathfinding;
using Sirenix.OdinInspector;

public interface IEnemyContextView : IContextViewImmutable
{
    public Path path { get; }
    public int currentWaypointIndex { get; }
    public bool reachedEndOfPath { get; }
    public bool isGrounded { get; }
}

public class EnemyContextData : ContextData, IEnemyContextView
{
    [field: ShowInInspector] public Path path { get; set; }
    [field: ShowInInspector] public int currentWaypointIndex { get; set; }
    [field: ShowInInspector] public bool reachedEndOfPath { get; set; }
    [field: ShowInInspector] public bool isGrounded { get; set; }

}