using System;
using EMILtools_Private.Core;
using UnityEngine;
using EMILtools.Systems;
using static AttackingBoundsChecker;
using static CanSeeBoundsChecker;
using static EnemyController;

public class EnemyController : MonoFacade<
    EnemyFunctionality,
    EnemyConfig,
    EnemyStructure,
    ActionMap>,
        IBoundsCheckMsgReceiver<Collider2D, CanSeeContext>, 
        IBoundsCheckMsgReceiver<Collider2D, AttackCtx>,
        ISignalReceiverTC<(bool, FinisherChoice)>,
        ISignalReceiverTC<BoolInt>,
        ISignalReceiverT<LivingEntity.PhasedHealthThresholdEnum>,
        IBoundsCheckMsgReceiver<Collider2D, InRangeBoundsChecker.InRangeContext>,
IEntityFacade
{
    
    
    Transform IFacade.transform => gameObject.transform;
    public class ActionMap : IActionMap
    {
        public readonly Publisher<bool> AttackInRange = new();
        public readonly Publisher<bool> ToggleHyperArmor = new();
        public readonly Publisher Idle = new();
        public readonly Publisher<bool> CanSeeTarget = new ();
        public readonly Publisher<AttackCtx> TakeDamage = new();
        public readonly Publisher<bool> Stun = new();
        public readonly Publisher<(bool isHooked, Publisher<Hook.FinisherContext>)> isHookedBySomething = new();
        public readonly Publisher<bool> FinisherInputAvaliable = new();
        public readonly Publisher<BoolInt> AttackColliderSetActive = new();

        public IContextViewImmutable ctx { get; }
    }
    protected void Awake() => InitializeFacade();
    void OnEnable() => Functionality.Bind();
    void OnDisable() => Functionality.Unbind();

    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<CanSeeContext> sender, CanSeeContext ctx) => CanSee(ctx.canSeeTarget);
    public void OnExitBounds(Collider2D collidedWith, BoundsChecker<CanSeeContext> sender, CanSeeContext ctx) => CanSee(ctx.canSeeTarget);
    void CanSee(bool canSee) => Actions.CanSeeTarget.Publish(canSee).Forget("Can See");
    
    // Receive Attack DI
    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<AttackCtx> sender, AttackCtx ctx)
    {
        Debug.Log("DMG : to " + gameObject.name);
        Actions.TakeDamage.Publish(ctx);
    }
    
    
    // In Range DI
    public void OnStayBounds(Collider2D collidedWith, BoundsChecker<InRangeBoundsChecker.InRangeContext> sender, InRangeBoundsChecker.InRangeContext ctx)
    {
        if (ctx.inRange) Actions.AttackInRange.Publish(true);
    }

    public void OnExitBounds(Collider2D collidedWith, BoundsChecker<InRangeBoundsChecker.InRangeContext> sender,
        InRangeBoundsChecker.InRangeContext ctx) =>
        Actions.AttackInRange.Publish(false);


    public void ReceiveSignal(string tag, (bool, FinisherChoice) ctx)
    {
        if (tag == "FINISHER") Actions.FinisherInputAvaliable.Publish(ctx.Item1);
    }
    
    public void ReceiveSignal(string tag, BoolInt ctx)
    {
        if (tag == "ATTACKANIM")
        {

            Actions.AttackColliderSetActive.Publish(ctx);
        }
    }

    public void ReceiveSignal(LivingEntity.PhasedHealthThresholdEnum t)
    {
        Debug.Log("PHASE CHANGE STATE RECEIVED: " + t);
        
        switch (t)
        {
            case LivingEntity.PhasedHealthThresholdEnum.PhaseFour:
                
                break;
            case LivingEntity.PhasedHealthThresholdEnum.PhaseThree:
                
                break;
            case LivingEntity.PhasedHealthThresholdEnum.PhaseTwo: 
                
                API_Context<EnemyContextData>().hyperArmorUsableInState = true;
                
                Debug.Log("PHASE CHANGE STATE COMPLETE: " + t);
                
                EnemyFunctionality.EnemyDescisions CanNowBlock = new EnemyFunctionality.EnemyDescisions { blockingAllowed = true };
                GetFunctionality<EnemyFunctionality.IAPI_EnemyDescisionMaker>().InvokeAndSendDepdancies(CanNowBlock);
                
                break;
            case LivingEntity.PhasedHealthThresholdEnum.PhaseOne:
                
                break;
        }
    }
}











