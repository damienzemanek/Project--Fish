using System;
using EMILtools.Core;
using UnityEngine;
using EMILtools.Systems;
using EMILtools.Timers;
using static AttackingBoundsChecker;
using static CanSeeBoundsChecker;
using static EnemyController;
using static HookFinisherBoundsChecker;

public class EnemyController : MonoFacade<
    EnemyFunctionality,
    EnemyConfig,
    EnemyStructure,
    ActionMap>,
        IBoundsCheckMsgReceiver<Collider2D, CanSeeContext>, 
        IBoundsCheckMsgReceiver<Collider2D, AttackCtx>,
        ISignalReceiverTaggedContext<(bool, FinisherChoice)>,
    IEntityFacade
{
    Transform IFacade.transform => gameObject.transform;
    public class ActionMap : IActionMap
    {
        public readonly Publisher Idle = new();
        public readonly Publisher<bool> CanSeeTarget = new ();
        public readonly Publisher<AttackCtx> TakeDamage = new();
        public readonly Publisher<bool> Stun = new();
        public readonly Publisher<(bool isHooked, Publisher<Hook.FinisherContext>)> isHookedBySomething = new();
        public readonly Publisher<bool> FinisherInputAvaliable = new();
        public IContextViewImmutable ctx { get; }
    }
    protected void Awake() => InitializeFacade();
    void OnEnable() => Functionality.Bind();
    void OnDisable() => Functionality.Unbind();

    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<CanSeeContext> sender, CanSeeContext ctx) => CanSee(ctx.canSeeTarget);
    public void OnExitBounds(Collider2D collidedWith, BoundsChecker<CanSeeContext> sender, CanSeeContext ctx) => CanSee(ctx.canSeeTarget);
    void CanSee(bool canSee) => Actions.CanSeeTarget.Publish(canSee).Forget("Can See");
    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<AttackCtx> sender, AttackCtx ctx)
     => Actions.TakeDamage.Publish(ctx);
    
    public void ReceiveSignal(string tag, (bool, FinisherChoice) ctx)
    {
        if (tag != "FINISHER") return;
        Actions.FinisherInputAvaliable.Publish(ctx.Item1);
    }
}