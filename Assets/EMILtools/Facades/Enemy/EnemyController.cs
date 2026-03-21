using System;
using UnityEngine;
using EMILtools.Systems;
using static EMILtools.Timers.TimerUtility;

public class EnemyController : MonoFacade<
    EnemyFunctionality,
    EnemyConfig,
    EnemyStructure,
    EnemyController.ActionMap>
    , ITimerUser, IBoundsCheckMsgReceiver<Collider2D>
{
    public class ActionMap : IActionMap
    {
        public readonly Publisher Idle = new();
        public readonly Publisher<Ref<bool>> CanSeeTarget = new ();
        
        internal readonly Ref<bool> canSeeTarget = new(false);
    }

    protected void Awake()
    {
        InitializeFacade();
    }

    void OnEnable() => Functionality.Bind();
    void OnDisable() => Functionality.Unbind();

    public void OnEnterBounds(Collider2D collidedWith, BoundsChecker sender)
    {
        Actions.CanSeeTarget.Publish(Actions.canSeeTarget.SetReturnThis(true)).Forget("Can See");
    }

    public void OnExitBounds(Collider2D collidedWith, BoundsChecker sender)
    {
        Actions.CanSeeTarget.Publish(Actions.canSeeTarget.SetReturnThis(false)).Forget("Can't See");
    }


}