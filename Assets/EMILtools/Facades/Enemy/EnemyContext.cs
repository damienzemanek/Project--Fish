using EMILtools.Systems;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IEnemyContextView : IContextViewImmutable
{
    public Path path { get; }
    public int currentWaypointIndex { get; }
    public bool reachedEndOfPath { get; }
    public bool isGrounded { get; }
    public Vector2 pos { get; }
    public Vector2 nextWaypoint { get; }
    public Vector2 dirToNextWaypoint { get; }
    public Vector2 force { get; }
    public float distToNextWaypoint { get; }
    public float distToEndNode { get; }   
    public bool travelAngleTooCloseToVertical { get; }
    public DelayBuffer<bool> canSeeTarget { get; }
}

public class EnemyContextData : ContextData, IEnemyContextView
{
    [field: ShowInInspector] public bool reachedEndOfPath { get; set; }
    [field: ShowInInspector] public bool isGrounded { get; set; }
    [field: ShowInInspector] public bool travelAngleTooCloseToVertical { get; set; }
    [field: ShowInInspector] public DelayBuffer<bool> canSeeTarget { get; set; }

    [field: ShowInInspector] public Path path { get; set; }
    [field: ShowInInspector] public int currentWaypointIndex { get; set; }
    
    [field: ShowInInspector] public Vector2 pos { get; set; }
    [field: ShowInInspector] public Vector2 nextWaypoint { get; set; }
    [field: ShowInInspector] public Vector2 dirToNextWaypoint { get; set; }
    [field: ShowInInspector] public Vector2 force { get; set; }
    [field: ShowInInspector] public float distToNextWaypoint { get; set; }
    [field: ShowInInspector] public float distToEndNode { get; set; }
}
