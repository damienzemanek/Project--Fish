using System;
using EMILtools.Systems;
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
        IEntityFacade
{
    Transform IFacade.transform => gameObject.transform;

    public class ActionMap : IActionMap
    {
        public readonly Publisher<AttackCtx> TakeDamage = new();
        public readonly Publisher<IPlayerContextView> HookAttack = new();
    }

    public class PlayerInputMap : InputMap
    {
        public readonly Publisher<(bool, float)> Move = new();
        public readonly Publisher<bool> Jump = new();
        public readonly Publisher<Vector2> Look = new();
        public readonly Publisher<bool> Attack = new();
        public readonly Publisher<bool> Hook = new();
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
    
}