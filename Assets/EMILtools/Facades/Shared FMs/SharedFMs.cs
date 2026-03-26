using System;
using EMILtools.Core;
using EMILtools.Systems;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static EMILtools.Timers.TimerUtility;


public interface IEntityFacade : IFacade, ITimerUser { }

public interface IEntityCtx : IContextViewImmutable
{
    public enum FaceDirection { Left, Right }
    
    public float hp { get; set; }
    public bool invulnerable { get; set; }
    public LivingEntity.BasicHealthThresholds currentHealthState { get; set; }
    public FaceDirection facingDirection { get; set; }
}

public interface IEntityBlackboard : IBlackboard
{
    public CountdownTimer invulnerableTimer { get; set; }
    public LivingEntity livingEntity { get; }
    public Rigidbody2D rb { get; }
    public AttackingBoundsChecker[] attackingBoundsCheckers { get; }
}

public interface IEntityConfig : IConfig
{
    [Serializable]
    public struct TakeDmg
    {
        [field: SerializeField] public float invulnerablePeriod { get; private set; }
        [field: FormerlySerializedAs("<pushForce>k__BackingField")] [field: SerializeField] public float pushForceX { get; set; }
        [field: SerializeField] public float pushForceY { get; private set; }
    }
    [Serializable]
    public struct HitStop
    {
        [field: SerializeField] [field: Range(0, 1)] public float scalar { get; private set; }
        [field: SerializeField] public float period { get; private set; }
    }
    
    public HitStop hitStop { get; set; }
    public TakeDmg takeDmg { get; set; }
}


public class SharedFMs
{
    public class InjectCtxIntoBoundsChecker<TFacade> : UnboundFunctionality<TFacade, IEntityCtx>
        where TFacade : IEntityFacade
    {
        public InjectCtxIntoBoundsChecker(TFacade facade) : base(facade) { }
        IEntityConfig cfg => facade.API_Config<IEntityConfig>(); IEntityBlackboard bb => facade.API_Blackboard<IEntityBlackboard>(); IEntityCtx mutateCtx => facade.API_Context<IEntityCtx>();

        protected override void Awake()
        {
            for (int i = 0; i < bb.attackingBoundsCheckers.Length; i++)
            {
                if(bb.attackingBoundsCheckers[i].enter) bb.attackingBoundsCheckers[i].enterContext.attackerEntityCtx = mutateCtx;
                if(bb.attackingBoundsCheckers[i].exit) bb.attackingBoundsCheckers[i].exitContext.attackerEntityCtx = mutateCtx;
                if(bb.attackingBoundsCheckers[i].stay) bb.attackingBoundsCheckers[i].stayContext.attackerEntityCtx = mutateCtx;
            }
        }
    }
    
    
    public class TakeDmg<TFacade> : BoundSetFunctionality<TFacade, IEntityCtx, TakeDmg<TFacade>.Setter>,
        ON_SET
            where TFacade : IEntityFacade
    {
        IEntityConfig cfg => facade.API_Config<IEntityConfig>(); IEntityBlackboard bb => facade.API_Blackboard<IEntityBlackboard>(); IEntityCtx mutateCtx => facade.API_Context<IEntityCtx>();

        public class Setter : DataSetter<AttackingBoundsChecker.AttackCtx>
        {
            [ShowInInspector] public AttackingBoundsChecker.AttackCtx AttackCtx => Get;
        }

        PersistentAction postCb;
        
        public TakeDmg(IPublisher publisher, TFacade facade, PersistentAction postCb) : base(publisher, facade)
            => this.postCb = postCb;

        protected override void Awake()
        {
            bb.invulnerableTimer = new CountdownTimer(cfg.takeDmg.invulnerablePeriod);
            facade.InitTimer(bb.invulnerableTimer, true);
            bb.invulnerableTimer.OnTimerStop.Add(InvulnerablitityEnd);
            
            PersistentAction<LivingEntity.BasicHealthThresholds> newHealthState = new PersistentAction<LivingEntity.BasicHealthThresholds>(NewHealthState);
            bb.livingEntity.healthThresholds.SetAllDelegates(newHealthState);
        }

        public void MutateUsingNewSetValues()
        {
            if (mutateCtx.invulnerable) return;
            if (mutateCtx.currentHealthState == LivingEntity.BasicHealthThresholds.Dying) return;
            SetContext.AttackCtx.hitMsgReceiver.Value.InjectContext(SimpleMsg.SimpleAttackHit);
            
            bb.invulnerableTimer.StartAndReset();
            mutateCtx.invulnerable = true;
            mutateCtx.hp = bb.livingEntity.TakeDamageCaller.Invoke(SetContext.AttackCtx.damageInfo);
            
            bb.rb.linearVelocity = new Vector2(0f, bb.rb.linearVelocity.y);
            Vector2 pushDir = SetContext.AttackCtx.attackerEntityCtx.facingDirection == IEntityCtx.FaceDirection.Right ? Vector2.right : Vector2.left;
            Vector2 lateral = pushDir * cfg.takeDmg.pushForceX;
            Vector2 upwards = Vector2.up * cfg.takeDmg.pushForceY;
            
            bb.rb.linearVelocity = lateral + upwards;
            
            //TimeManager.Instance.SlowTimeForSeconds(cfg.hitStop.period, cfg.hitStop.scalar);
            postCb?.Invoke();
            
            Debug.Log("[TakeDamage] push vec : " + pushDir + " combined: " + (pushDir * cfg.takeDmg.pushForceX));
            Debug.Log("[TakeDamage] velocity after force: " + bb.rb.linearVelocity);
        }

        void NewHealthState(LivingEntity.BasicHealthThresholds newHealthState)
        {
            Debug.Log("New Health State: " + newHealthState + "");
            mutateCtx.currentHealthState = newHealthState;
        }
        
        void InvulnerablitityEnd() => mutateCtx.invulnerable = false;
    }
}
