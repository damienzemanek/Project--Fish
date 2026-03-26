using EMILtools.Systems;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IEnemyContextView : IEntityCtx
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
    public bool invulnerable { get; }
    public float hp { get; }
    public LivingEntity.BasicHealthThresholds currentHealthState { get; }
    public IEntityCtx.FaceDirection facingDirection { get; set; }
}

public class EnemyContextData : ContextData, IEnemyContextView
{
    [field: ShowInInspector] public bool reachedEndOfPath { get; set; }
    [field: ShowInInspector] public bool isGrounded { get; set; }
    [field: ShowInInspector] public bool travelAngleTooCloseToVertical { get; set; }
    [field: ShowInInspector] public DelayBuffer<bool> canSeeTarget { get; set; }
    [field: ShowInInspector] public bool invulnerable { get; set; }
    [field: ShowInInspector] public float hp { get; set; }
    [field: ShowInInspector] public LivingEntity.BasicHealthThresholds currentHealthState { get; set; }
    [field: ShowInInspector] public IEntityCtx.FaceDirection facingDirection { get; set; }

    [field: BoxGroup("A Star")] [field: ShowInInspector] public Path path { get; set; }
    [field: BoxGroup("A Star")] [field: ShowInInspector] public int currentWaypointIndex { get; set; }
    [field: BoxGroup("A Star")] [field: ShowInInspector] public Vector2 pos { get; set; }
    [field: BoxGroup("A Star")] [field: ShowInInspector] public Vector2 nextWaypoint { get; set; }
    [field: BoxGroup("A Star")] [field: ShowInInspector] public Vector2 dirToNextWaypoint { get; set; }
    [field: BoxGroup("A Star")] [field: ShowInInspector] public Vector2 force { get; set; }
    [field: BoxGroup("A Star")] [field: ShowInInspector] public float distToNextWaypoint { get; set; }
    [field: BoxGroup("A Star")] [field: ShowInInspector] public float distToEndNode { get; set; }
}
