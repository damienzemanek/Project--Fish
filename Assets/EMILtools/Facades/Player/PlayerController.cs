using System;
using EMILtools.Core;
using EMILtools.Extensions;
using EMILtools.Systems;
using EMILtools.Timers;
using UnityEngine;
using Sirenix.OdinInspector;
using static AttackingBoundsChecker;
using static PrimaryInputAuthority;
using static EMILtools.Systems.IInputSubordinate<PlayerController.PlayerInputMap,PrimaryInputAuthority.Subordinates>;
using static HookBoundsChecker;


public class PlayerController : MonoFacade<
    PlayerFunctionality,
    PlayerConfig,
    PlayerStructure,
    PlayerController.ActionMap>,
        IInputSubordinate<PlayerController.PlayerInputMap, Subordinates>,
        IBoundsCheckMsgReceiver<Collider2D, AttackCtx>,
        IBoundsCheckMsgReceiver<Collider2D, HookContext>,

IEntityFacade
{
    Transform IFacade.transform => gameObject.transform;

    public class ActionMap : IActionMap
    {
        public readonly Publisher<AttackCtx> TakeDamage = new();
        public readonly Publisher<IPlayerContextView> IvunrabilityVisualization = new();
        public readonly Publisher<IPlayerContextView> HookAttack = new();
        
        public readonly Publisher<(
            bool finisherActive,
            CountdownTimer finisherTimer,
            PersistentAction HookedBreakout,
            Ref<bool> finisherInputAvaliable, 
            IDamageable damageable)> 
        Finisher = new();
        public IContextViewImmutable ctx { get; }
    }

    public class PlayerInputMap : InputMap
    {
        public readonly Publisher<(bool, float)> Move = new();
        public readonly Publisher<bool> Jump = new();
        public readonly Publisher<Vector2> Look = new();
        public readonly Publisher<bool> Attack = new();
        public readonly Publisher<bool> Hook = new();
        public readonly Publisher<bool> FinishInput = new();
    }

    public PlayerInputMap Input { get; set; }
    [field: SerializeField] [field: PropertyOrder(-1)]
    public SubordinateContext inputSubordinateContext { get; set; }
    public PlayerInputMap InjectInputMap() => new PlayerInputMap();
    public void InitSubordinate() =>  InitializeFacade();
    public void OnAuthorityReceived() => Functionality.Bind();
    public void OnAuthorityLost() => Functionality.Unbind();


    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<AttackCtx> sender, AttackCtx ctx)
    {
        Debug.Log($"Player took damage: {ctx.damageInfo.dmg}");
        Actions.TakeDamage.Publish(ctx);
    }

    public void OnStayBounds(Collider2D collidedWith, BoundsChecker<AttackCtx> sender, AttackCtx ctx)
    {
        Debug.Log($"Player try receive damage: {ctx.damageInfo.dmg}");
        Actions.TakeDamage.Publish(ctx);
    }

    /// <summary>
    /// When the player hooks onto a target
    /// </summary>
    /// <param name="collidedWith"></param>
    /// <param name="sender"></param>
    /// <param name="ctx"></param>
    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<HookContext> sender,
        HookContext ctx)
    {
        if(collidedWith == null) return;
        if (!collidedWith.TryGetComponent<EnemyController>(out var targetActions)) return;
        var map = targetActions.API_Actions<EnemyController.ActionMap>();
        API_Context<PlayerContextData>().targetStunPublisher = map.Stun;
        API_Context<PlayerContextData>().isHookLatchedOntoTarget = true;
        map.isHookedBySomething.Publish((true, Actions.Finisher));
    }

    /// <summary>
    /// When the player unhooks from a target
    /// </summary>
    /// <param name="collidedWith"></param>
    /// <param name="sender"></param>
    /// <param name="ctx"></param>
    public void OnExitBounds(Collider2D collidedWith, BoundsChecker<HookContext> sender,
        HookContext ctx)
    {
        API_Context<PlayerContextData>().targetStunPublisher = null;
        API_Context<PlayerContextData>().isHookLatchedOntoTarget = false;
        if(collidedWith == null) return;
        var hasCont = collidedWith.TryGetComponent<EnemyController>(out var targetActions);
        if (hasCont)
            targetActions.API_Actions<EnemyController.ActionMap>().isHookedBySomething.Publish((false, Actions.Finisher));
    }


}






























