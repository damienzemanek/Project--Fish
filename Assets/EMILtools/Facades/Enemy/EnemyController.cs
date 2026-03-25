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
        IBoundsCheckMsgReceiver<Collider2D, AttackCtx>,
        IEntityFacade
{
    Transform IFacade.transform => gameObject.transform;
    public class ActionMap : IActionMap
    {
        public readonly Publisher Idle = new();
        public readonly Publisher<bool> CanSeeTarget = new ();
        public readonly Publisher<AttackCtx> TakeDamage = new();
    }

    protected void Awake()
    {
        InitializeFacade();
    }

    void OnEnable() => Functionality.Bind();
    void OnDisable() => Functionality.Unbind();

    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<CanSeeContext> sender, CanSeeContext ctx)
    {
        Debug.Log("Entered View Range : " + ctx.canSeeTarget);
        CanSee(ctx.canSeeTarget);
    }

    public void OnExitBounds(Collider2D collidedWith, BoundsChecker<CanSeeContext> sender, CanSeeContext ctx)
    {
        Debug.Log("Exited View Range : " + ctx.canSeeTarget);
        CanSee(ctx.canSeeTarget);
    }
    void CanSee(bool canSee) => Actions.CanSeeTarget.Publish(canSee).Forget("Can See");
    
    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<AttackCtx> sender, AttackCtx ctx)
    {
        Debug.Log("Hit");
        Actions.TakeDamage.Publish(ctx);
        Debug.Log("Hit Compelte");
    }
}