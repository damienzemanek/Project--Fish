using EMILtools.Core;
using EMILtools.Systems;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IPlayerContextView : IEntityCtx
{
    public ICountdownTimer CoyoteTimer { get; }
    public ICurveValue JumpCurve { get; }
    public bool IsGrounded { get; }
    public int jumps { get; }
    public bool FallingWithoutJumpingFirst { get; }
    public bool isJumping { get; }
    public bool canJumpCoyote { get; }
    public FaceDirection facingDirection { get; }
    public PlayerBlackboard.AttackDir attackDir { get; }
    public bool landing { get; }
    public bool isAttacking { get; }
    public bool isMoving { get; }   
    bool isHooking { get; }
    Vector2 mousePos { get; }
}

[InlineProperty]
public class PlayerContextData : ContextData, IPlayerContextView
{
    [ShowInInspector] CountdownTimer coyoteTimer;
    [ShowInInspector] public ICurveValue JumpCurve => API_Blackboard<PlayerBlackboard>().jumpCurve;
    [ShowInInspector] public ReactiveIntercept<bool> isGrounded = new ReactiveIntercept<bool>();
    [ShowInInspector] public int jumps { get; set; }
    [ShowInInspector] public Vector2 mousePos { get; set; }
    
    [ShowInInspector] public DelayBuffer<bool> fallingWithoutJumpingFirst { get; set; }
    [ShowInInspector] public IEntityCtx.FaceDirection facingDirection { get; set; }
    [ShowInInspector] public PlayerBlackboard.AttackDir attackDir { get; set; }
    [ShowInInspector] public bool landing { get; set; }
    [ShowInInspector] public bool isJumping { get; set; }
    [ShowInInspector] public bool canJumpCoyote { get; set; }
    [ShowInInspector] public bool isAttacking { get; set; }
    [ShowInInspector] public bool isMoving { get; set; }
    [ShowInInspector] public bool isHooking { get; set; }
    
    
    // API Distinct
    public ICountdownTimer CoyoteTimer => coyoteTimer;
    public bool IsGrounded => isGrounded;
    [field:ShowInInspector] public bool FallingWithoutJumpingFirst => fallingWithoutJumpingFirst;
    [field:ShowInInspector] public float hp { get; set; }
    [field:ShowInInspector] public bool invulnerable { get; set; }
    [field:ShowInInspector] public LivingEntity.BasicHealthThresholds currentHealthState { get; set; }
}