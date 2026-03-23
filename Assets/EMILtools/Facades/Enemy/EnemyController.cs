using System;
using UnityEngine;
using EMILtools.Systems;
using static AttackingBoundsChecker;
using static CanSeeBoundsChecker;
using static EMILtools.Timers.TimerUtility;
using static EnemyController;

public class EnemyController : MonoFacade<
    EnemyFunctionality,
    EnemyConfig,
    EnemyStructure,
    ActionMap>, 
        ITimerUser, 
        IBoundsCheckMsgReceiver<Collider2D, CanSeeContext>, 
        IBoundsCheckMsgReceiver<Collider2D, AttackingCtx>
{
    public class ActionMap : IActionMap
    {
        public readonly Publisher Idle = new();
        public readonly Publisher<Ref<bool>> CanSeeTarget = new ();
        public readonly Publisher<IEnemyContextView> TakeDamage = new();
        
        internal readonly Ref<bool> canSeeTarget = new(false);
    }

    protected void Awake()
    {
        InitializeFacade();
    }

    void OnEnable() => Functionality.Bind();
    void OnDisable() => Functionality.Unbind();
    
    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<CanSeeContext> sender, CanSeeContext ctx) => CanSee(ctx.canSeeTarget);
    public void OnExitBounds(Collider2D collidedWith, BoundsChecker<CanSeeContext> sender, CanSeeContext ctx) => CanSee(ctx.canSeeTarget);
    void CanSee(bool canSee) => Actions.CanSeeTarget.Publish(Actions.canSeeTarget.SetReturnThis(canSee)).Forget("Can See");


    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<AttackingCtx> sender, AttackingCtx ctx)
        => Actions.TakeDamage.Publish(API_Context<EnemyContextData>());
}