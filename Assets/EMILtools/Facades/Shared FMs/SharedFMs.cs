using System;
using EMILtools.Core;
using EMILtools.Systems;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Timers.TimerUtility;


public interface IEntityFacade : IFacade, ITimerUser { }

public interface IEntityCtx : IContextViewImmutable
{
    public float hp { get; set; }
    public bool invulnerable { get; set; }
    public LivingEntity.BasicHealthThresholds currentHealthState { get; set; }
}

public interface IEntityBlackboard : IBlackboard
{
    public CountdownTimer invulnerableTimer { get; set; }
    public LivingEntity livingEntity { get; }
}

public interface IEntityConfig : IConfig
{
    [Serializable]
    public struct TakeDmg
    {
        [field: SerializeField] public float invulnerablePeriod { get; private set; }
    }
    
    public TakeDmg takeDmg { get; set; }
    public float pushForce { get; set; }
}


public class SharedFMs
{
    public class TakeDmg<TFacade> : BoundSetFunctionality<TFacade, IEntityCtx, TakeDmg<TFacade>.Setter>,
        ON_SET
            where TFacade : IEntityFacade
    {
        IEntityConfig cfg => facade.API_Config<IEntityConfig>(); IEntityBlackboard bb => facade.API_Blackboard<IEntityBlackboard>(); IEntityCtx mutateCtx => facade.API_Context<IEntityCtx>();

        public class Setter : DataSetter<AttackingBoundsChecker.AttackCtx>
        {
            [ShowInInspector] public AttackingBoundsChecker.AttackCtx AttackCtx => Get;
        }

        public TakeDmg(IPublisher publisher, TFacade facade) : base(publisher, facade) { }

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
            bb.invulnerableTimer.StartAndReset();
            mutateCtx.invulnerable = true;
            mutateCtx.hp = bb.livingEntity.TakeDamageCaller.Invoke(SetContext.AttackCtx.damageInfo);
            
            Vector3 pushDir = SetContext.AttackCtx.attackerTransform.position - facade.transform.position;
            SetContext.AttackCtx.attackerRb.AddForce(pushDir.normalized * cfg.takeDmg.pushForce, ForceMode.Impulse);
        }

        void NewHealthState(LivingEntity.BasicHealthThresholds newHealthState)
        {
            Debug.Log("New Health State: " + newHealthState + "");
            mutateCtx.currentHealthState = newHealthState;
        }
        
        void InvulnerablitityEnd() => mutateCtx.invulnerable = false;
    }
}
