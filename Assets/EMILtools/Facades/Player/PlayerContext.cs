using EMILtools.Core;
using EMILtools.Systems;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IPlayerContextView : IModuleUsabableContext
{
    public ICountdownTimer CoyoteTimer { get; }
    public ICurveValue JumpCurve { get; }
    public bool IsGrounded { get; }
    public int jumps { get; }
    public bool FallingWithoutJumpingFirst { get; }
    public bool jumpInProgress { get; }
    public bool didJumpCoyoteInput { get; }
}

[InlineProperty]
public class PlayerContextData : ContextData, IPlayerContextView
{
    [ShowInInspector] CountdownTimer coyoteTimer;
    [ShowInInspector] public ICurveValue JumpCurve => API_Blackboard<PlayerBlackboard>().jumpCurve;
    [ShowInInspector] public ReactiveIntercept<bool> isGrounded = new ReactiveIntercept<bool>();
    [ShowInInspector] public int jumps { get; set; }
    [ShowInInspector] public DelayBuffer<bool> fallingWithoutJumpingFirst { get; set; }
    [ShowInInspector] public bool jumpInProgress { get; set; }
    [ShowInInspector] public bool didJumpCoyoteInput { get; set; }
    
    // API Distinct
    public ICountdownTimer CoyoteTimer => coyoteTimer;
    public bool IsGrounded => isGrounded;
    public bool FallingWithoutJumpingFirst => fallingWithoutJumpingFirst;
}