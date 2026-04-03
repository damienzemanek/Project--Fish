using System;
using EMILtools.Core;
using EMILtools.Systems;
using EMILtools.Timers;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using static AttackingBoundsChecker;
using static LivingEntity;

public class EnemyFunctionality : Functionalities<
    EnemyController,
    IEnemyContextView>
{
    
    protected override IState AddModulesHere(EnemyController f)
    {
        // Add Modules like this
        // AddModule(new ExampleModule(...));

        // Return the module that is the starting state, ex:
        // return AddModule(new Idle(...))

        AddModule(new Stunnable(facade.Actions.Stun, f));
        AddModule(new SharedFMs.InjectCtxIntoBoundsChecker<EnemyController>(f));
        AddModule(new DyingState(f));
        AddModule(new SharedFMs.TakeDmg<EnemyController>(facade.Actions.TakeDamage, f, null));
        AddModule(new FaceDir(f));
        AddModule(new Idle(f));
        AddModule(new ViewRange(facade.Actions.CanSeeTarget, f));
        AddModule(new Jump(f));
        AddModule(new SharedFMs.ClampLateralMovement<EnemyController>(f));
        AddModule(new InAir(f));
        return AddModule(new Follow(f));
    }
    protected override void SetupTransitionsForFSM(StateMachine<IEnemyContextView> fsm, IEnemyContextView ctx)
    {
        // Add State Transitions here
        // fsm.AddAnyTransition<Jump>(new FuncPredicate(() => ctx.isJumping), "Jumping");
        
        fsm.AddTransition<Idle, Follow>(new FuncCtxPredicate<IEnemyContextView>(ctx => ctx.canSeeTarget), "Can See Target");
        fsm.AddTransition<Follow, Idle>(new FuncCtxPredicate<IEnemyContextView>(ctx => !ctx.canSeeTarget), "Can't See Target");
        
        fsm.AddAnyTransition<DyingState>(new FuncCtxPredicate<IEnemyContextView>(ctx => ctx.currentHealthState == BasicHealthThresholds.Dying), "Dying");
        fsm.AddTransition<DyingState, Follow>(new FuncCtxPredicate<IEnemyContextView>(ctx => ctx.currentHealthState != BasicHealthThresholds.Dying), "Not Dying");
        
        fsm.AddAnyTransition<Stunnable>(new FuncCtxPredicate<IEnemyContextView>(ctx => ctx.isStunned), "Stunned");
        fsm.AddTransition<Stunnable, Follow>(new FuncCtxPredicate<IEnemyContextView>(ctx => !ctx.isStunned), "Not Stunned");
    }
    
    class Stunnable : BoundSetFunctionality<EnemyController, IEnemyContextView, Stunnable.Setter>,
        FSM_STATE_ENTER<IEnemyContextView>,
        ON_SET
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>(); EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public class Setter : DataSetter<bool>
        {
            [ShowInInspector] public bool isStunned => Get;
        }

        public Stunnable(IPublisher publisher, EnemyController facade) : base(publisher, facade) { }

        protected override void Awake()
        {
            bb.stunnedTimer = new CountdownTimer(cfg.stunned.stunnedPeriod);
            bb.stunnedTimer.OnTimerStop.Add(() => mutateCtx.isStunned = false);
            facade.InitTimer(bb.stunnedTimer, true);
        }
        
        public void OnEnterState(IEnemyContextView ctx)
        {
            Debug.Log("Entered Stun State");
            bb.stunnedTimer.StartAndReset();
            mutateCtx.isStunned = true;
            bb.rb.linearVelocity = new Vector2(0, bb.rb.linearVelocity.y);
            bb.damageFlasher.Flash(DamageFlasher.FlashType.Stun);
        }
        public void MutateUsingNewSetValues() => mutateCtx.isStunned = SetContext.isStunned;

    }
    
    class DyingState : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FSM_STATE_ENTER<IEnemyContextView>
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>(); EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public DyingState(EnemyController facade) : base(facade) { }

        protected override void Awake()
        {
            bb.dyingStateTimer = new CountdownTimer(cfg.dyingState.dyingStatePeriod);
            facade.InitTimer(bb.dyingStateTimer, true);
            bb.dyingStateTimer.OnTimerStop.Add(StopDyingAfterCertainTime);
        }

        public void OnEnterState(IEnemyContextView ctx)
        {
            Debug.Log("Dying");
            cfg.animHandle.Play(bb.animator, EnemyConfig.EnemyAnims.Dying);
            bb.dyingStateTimer.StartAndReset();
            bb.viewRange.enabled = false;
        }

        void StopDyingAfterCertainTime()
        {
            Debug.Log("Stopped Dying");
            bb.livingEntity.Heal(cfg.dyingState.outOfDeathStateHealAmount, out var newState);
            mutateCtx.currentHealthState = newState;
            bb.viewRange.enabled = true;
        }
    }
    

    class FaceDir : UnboundFunctionality<EnemyController, IEnemyContextView>,
        UPDATE<IEnemyContextView>
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>(); EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public FaceDir(EnemyController facade) : base(facade) { }

        public override PipelineBuilder<IEnemyContextView> InjectSteps(PipelineBuilder<IEnemyContextView> builder) => builder
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ctx => !ctx.canSeeTarget));

        public override void Execute(IEnemyContextView ctx)
        {
            float targX = bb.target.position.x;
            float myX = facade.transform.position.x;
            float dif = targX - myX;
            if (dif < 0)
            {
                bb.faceDirTransform.rotation = Quaternion.Euler(0, 0, 0);
                mutateCtx.facingDirection = IEntityCtx.FaceDirection.Left;
            }
            else
            {
                bb.faceDirTransform.rotation = Quaternion.Euler(0, 180, 0);
                mutateCtx.facingDirection = IEntityCtx.FaceDirection.Right;
            }
            //Debug.Log("facing");
        }
    }
    
    class Idle : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FSM_STATE_ENTER<IEnemyContextView>
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>(); EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public Idle(EnemyController facade) : base(facade) { }

        public override PipelineBuilder<IEnemyContextView> InjectSteps(PipelineBuilder<IEnemyContextView> builder) => builder
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ctx => ctx.canSeeTarget));

        public void OnEnterState(IEnemyContextView ctx)
        {
            Debug.Log("Entered Idle State");
            cfg.animHandle.Play(bb.animator, EnemyConfig.EnemyAnims.Idle);
        }
    }


    class ViewRange : BoundSetFunctionality<EnemyController, IEnemyContextView, ViewRange.Setter>, 
        ON_SET
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>(); EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public class Setter : DataSetter<bool>
        {
            [ShowInInspector] public bool canSeeTarget => Get;
        }

        protected override void Awake()
        {
            mutateCtx.canSeeTarget = new DelayBuffer<bool>(false, cfg.viewRange.delayToStopChasing);
        }

        public ViewRange(IPublisher publisher, EnemyController facade) : base(publisher, facade) { }

        public void MutateUsingNewSetValues()
        {
            Debug.Log("View Range: NEW " + SetContext.canSeeTarget);
            if(!SetContext.canSeeTarget)
                mutateCtx.canSeeTarget.SetBuffered(SetContext.canSeeTarget);
            else 
                mutateCtx.canSeeTarget.SetNotBuffered(SetContext.canSeeTarget);
        }
    }


    class Jump : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FIXED_UPDATE<IEnemyContextView>
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>(); EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public Jump(EnemyController facade) : base(facade) { }

        protected override void Awake()
        {
            bb.jumpTimer = new CountdownTimer(cfg.jump.jumpDelay);
            facade.InitTimer(bb.jumpTimer, true);
        }

        public override PipelineBuilder<IEnemyContextView> InjectSteps(PipelineBuilder<IEnemyContextView> builder) => builder
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ctx => !ctx.canSeeTarget))
                .Add_ShortCircuit(new FuncPredicate(() => facade.FSM.CurrentStateType == typeof(DyingState)))
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ctx => !ctx.travelAngleTooCloseToVertical))
                .Add_ShortCircuit(new FuncPredicate(() => bb.jumpTimer.isRunning));
        
        
        public override void Execute(IEnemyContextView ctx)
        {
            bb.jumpTimer.StartAndReset();
            bb.rb.AddForce(Vector2.up * cfg.jump.jumpForceScalar, ForceMode2D.Impulse);
            Debug.Log("ENEMY Jump");
        }
    }
    
    
    class InAir : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FIXED_UPDATE<IEnemyContextView>
    {
        EnemyConfig cfg => facade.Config; EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>();
        public InAir(EnemyController facade) : base(facade) { }
        public override void Execute(IEnemyContextView ctx)
        {
            facade.API_Context<EnemyContextData>().isGrounded = IsGrounded();
            if(!ctx.isGrounded) bb.rb.AddForce(-facade.transform.up * cfg.inAir.fallScalar, cfg.inAir.forceMode);
        }

        bool IsGrounded()
        {
            for (int i = 0; i < bb.feetPoints.Length; i++)
                if (Physics2D.Raycast(bb.feetPoints[i].position, -facade.transform.up, cfg.inAir.checkDist, cfg.inAir.mask)) return true;
            return false;
        }
    }

    class Follow : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FIXED_UPDATE<IEnemyContextView>,
        FSM_STATE_ENTER<IEnemyContextView>
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>();
        EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public Follow(EnemyController facade) : base(facade) { }

        protected override void Awake()
        {
            PersistentAction computePath = new PersistentAction(() => bb.seeker.StartPath(bb.rb.position, bb.target.position, OnPathComplete));
            bb.computePath.InjectMethod(computePath);
            
            
            
        }
        
        public override PipelineBuilder<IEnemyContextView> InjectSteps(PipelineBuilder<IEnemyContextView> builder) => builder
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ctx => !ctx.canSeeTarget))
                .Add_ShortCircuit(new FuncPredicate(() => facade.FSM.CurrentStateType == typeof(DyingState)))
                .Add_ShortCircuit(new FuncPredicate(() => facade.FSM.CurrentStateType == typeof(Stunnable)))
                .Add_Middleware(_ => bb.computePath.RateLimitedUpdateTick())
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ctx => ctx.path == null))
                .Add_Middleware(GrabVariables)
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ReachedEndOfPath))
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(TravelAngleTooCloseToVertical));

        
        public override void Execute(IEnemyContextView ctx)
        {
            //Debug.Log("Follow");
            if (ctx.distToNextWaypoint < cfg.follow.nextWaypointDistance)
                if (ctx.currentWaypointIndex + 1 < ctx.path.vectorPath.Count) mutateCtx.currentWaypointIndex++;
            
            bb.rb.AddForce(ctx.force);
        }

        void GrabVariables(IEnemyContextView ctx)
        {
            mutateCtx.pos = bb.rb.position; 
            mutateCtx.nextWaypoint = ctx.path.vectorPath[ctx.currentWaypointIndex];
            mutateCtx.dirToNextWaypoint = (ctx.nextWaypoint - ctx.pos).normalized;
            mutateCtx.force = ctx.dirToNextWaypoint * (cfg.follow.speedScalar * Time.deltaTime);
            mutateCtx.distToNextWaypoint = Vector2.Distance(ctx.pos, ctx.nextWaypoint);
            mutateCtx.distToEndNode = Mathf.Abs((bb.rb.position - (Vector2)ctx.path.vectorPath[^1]) .magnitude);
        }
        
        bool TravelAngleTooCloseToVertical(IEnemyContextView ctx)
        {
            float dot = Vector2.Dot(ctx.dirToNextWaypoint.normalized, Vector2.up); // dot: 1 = up, 0 = horizontal, -1 = down
            float angleToUp = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg;
            //Debug.Log($"angleToUp: {angleToUp} | dot: {dot}");
            if (dot < 0f) return false;  // If moving downward, always allow
            float threshold = Mathf.Cos(cfg.follow.minHorizAngleToFollow * Mathf.Deg2Rad); // Only restrict upward movement
            bool isTooVertical = dot > threshold;
            return mutateCtx.travelAngleTooCloseToVertical = isTooVertical;
        }


        bool ReachedEndOfPath(IEnemyContextView ctx)
        {
            if (ctx.distToEndNode < cfg.follow.stoppingDistToTarget)
                { mutateCtx.reachedEndOfPath = true; return true; }
            else
                { mutateCtx.reachedEndOfPath = false; return false; }
        }

        void OnPathComplete(Path path)
        {
            if (path.error) return;
            mutateCtx.path = path;
            mutateCtx.currentWaypointIndex = 0;
        }

        public void OnEnterState(IEnemyContextView ctx)
        {
            Debug.Log("Entered Follow State");
            cfg.animHandle.Play(bb.animator, EnemyConfig.EnemyAnims.Walk);
        }
    }
    
    
}