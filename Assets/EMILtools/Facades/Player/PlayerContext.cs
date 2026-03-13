using EMILtools.Systems;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IPlayerContextView : IModuleUsabableContext
{
    public ICurveValue JumpCurve { get; }
    public bool isGrounded { get; }
    public int jumps { get; }
}

[InlineProperty]
public class PlayerContextData : ContextData, IPlayerContextView
{
    [ShowInInspector] public ICurveValue JumpCurve => API_Blackboard<PlayerBlackboard>().jumpCurve;
    [ShowInInspector] public bool isGrounded => API_Blackboard<PlayerBlackboard>().phys.isGrounded;
    [ShowInInspector] public int jumps { get; set; }
    
}