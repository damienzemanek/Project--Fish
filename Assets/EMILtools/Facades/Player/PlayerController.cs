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


public class PlayerController : MonoFacade<
    PlayerFunctionality,
    PlayerConfig,
    PlayerStructure,
    PlayerController.ActionMap>,
        IInputSubordinate<PlayerController.PlayerInputMap, Subordinates>,
        IBoundsCheckMsgReceiver<Collider2D, AttackCtx>,
        IBoundsCheckMsgReceiver<Collider2D, HookBoundsChecker.HookContext>,
    IEntityFacade
{
    Transform IFacade.transform => gameObject.transform;

    public class ActionMap : IActionMap
    {
        public readonly Publisher<AttackCtx> TakeDamage = new();
        public readonly Publisher<IPlayerContextView> HookAttack = new();
        public readonly Publisher<(bool, CountdownTimer, PersistentAction)> Finisher = new();
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
    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker<HookBoundsChecker.HookContext> sender,
        HookBoundsChecker.HookContext ctx)
    {
        var targetActions = collidedWith.Get<EnemyController>().API_Actions<EnemyController.ActionMap>();
        API_Context<PlayerContextData>().targetStunPublisher = targetActions.Stun;
        API_Context<PlayerContextData>().isHookLatchedOntoTarget = true;
        targetActions.isHookedBySomething.Publish((true, Actions.Finisher));
    }

    public void OnExitBounds(Collider2D collidedWith, BoundsChecker<HookBoundsChecker.HookContext> sender,
        HookBoundsChecker.HookContext ctx)
    {
        var targetActions = collidedWith.Get<EnemyController>().API_Actions<EnemyController.ActionMap>();
        API_Context<PlayerContextData>().targetStunPublisher = null;
        API_Context<PlayerContextData>().isHookLatchedOntoTarget = false;
        targetActions.isHookedBySomething.Publish((true, null));

        Debug.Log("HOOK UNATTACHED");
    }
}