using EMILtools.Core;
using EMILtools.Systems;
using EMILtools.Timers;
using Pathfinding;
using UnityEngine;

public class EnemyFunctionality : Functionalities<
    EnemyController,
    IEnemyContextView>
{
    protected override IState AddModulesHere()
    {
        // Add Modules like this
        // AddModule(new ExampleModule(...));

        // Return the module that is the starting state, ex:
        // return AddModule(new Idle(...))

        AddModule(new Jump(facade));
        AddModule(new ClampLateralMovement(facade));
        AddModule(new InAir(facade));
        return AddModule(new Follow(facade));
    }

    protected override void SetupTransitionsForFSM(StateMachine<IEnemyContextView> fsm, IEnemyContextView ctx)
    {
        // Add State Transitions here
        // fsm.AddAnyTransition<Jump>(new FuncPredicate(() => ctx.isJumping), "Jumping");
    }


    class Jump : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FIXED_UPDATE
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>();
        public Jump(EnemyController facade) : base(facade) { }

        protected override void Awake()
        {
            bb.jumpTimer = new CountdownTimer(cfg.jump.jumpDelay);
            facade.InitTimer(bb.jumpTimer, true);
        }

        public override PipelineBuilder<IEnemyContextView> InjectSteps(PipelineBuilder<IEnemyContextView> builder) => builder
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ctx => !ctx.travelAngleTooCloseToVertical))
                .Add_ShortCircuit(new FuncPredicate(() => bb.jumpTimer.isRunning));
        
        
        protected override void ExecutionImplementation(IEnemyContextView ctx)
        {
            bb.jumpTimer.StartAndReset();
            bb.rb.AddForce(Vector2.up * cfg.jump.jumpForceScalar, ForceMode2D.Impulse);
            Debug.Log("ENEMY Jump");
        }
    }
    

    class ClampLateralMovement : UnboundFunctionality<EnemyController, IEnemyContextView>,
        UPDATE
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>();
        EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public ClampLateralMovement(EnemyController facade) : base(facade) { }
        protected override void ExecutionImplementation(IEnemyContextView ctx)
        {
            float clampedX = Mathf.Clamp(bb.rb.linearVelocity.x, -cfg.clampLateralMove.maxVelocity, cfg.clampLateralMove.maxVelocity);
            float currentY = bb.rb.linearVelocity.y;
            bb.rb.linearVelocity = new Vector2(clampedX, currentY);
        }
    }
    
    class InAir : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FIXED_UPDATE,
        FSM_STATE_ENTER<IPlayerContextView>
    {
        EnemyConfig cfg => facade.Config; EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>();
        public InAir(EnemyController facade) : base(facade) { }
        protected override void ExecutionImplementation(IEnemyContextView ctx)
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

        public void OnEnterState(IPlayerContextView ctx)
        {
            Debug.Log("Entetered State: InAir");
        }
    }

    class Follow : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FIXED_UPDATE,
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
                .Add_Middleware(_ => bb.computePath.RateLimitedUpdateTick())
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ctx => ctx.path == null))
                .Add_Middleware(GrabVariables)
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(ReachedEndOfPath))
                .Add_ShortCircuit(new FuncCtxPredicate<IEnemyContextView>(TravelAngleTooCloseToVertical));

        
        protected override void ExecutionImplementation(IEnemyContextView ctx)
        {
            Debug.Log("Follow");
            if (ctx.distToNextWaypoint < cfg.follow.nextWaypointDistance)
                if (ctx.currentWaypointIndex + 1 < ctx.path.vectorPath.Count) mutateCtx.currentWaypointIndex++;
            
            bb.rb.AddForce(ctx.force);
            /// TO DO: If the enemy is in the same position for a long period of time (3 seconds) and the
            /// player is ABOUVE the enemy, it will try jumping very high 3 times, if they fail, it will lose interest
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
        }
    }
    
    class Idle : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FSM_STATE_ENTER<IEnemyContextView>,
        UPDATE
    {
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>();
        EnemyContextData mutateCtx => facade.API_Context<EnemyContextData>();
        public Idle(EnemyController facade) : base(facade) { }
        protected override void ExecutionImplementation(IEnemyContextView ctx) { }
        public void OnEnterState(IEnemyContextView ctx)
        {
            Debug.Log("Entered Idle State");
            cfg.animHandle.Play(bb.animator, EnemyConfig.EnemyAnims.Idle);
        }
    }
}