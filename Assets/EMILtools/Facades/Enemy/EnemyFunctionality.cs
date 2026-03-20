using EMILtools.Core;
using EMILtools.Systems;
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

        AddModule(new InAir(facade));
        return AddModule(new Follow(facade));
    }

    protected override void SetupTransitionsForFSM(StateMachine<IEnemyContextView> fsm, IEnemyContextView ctx)
    {
        // Add State Transitions here
        // fsm.AddAnyTransition<Jump>(new FuncPredicate(() => ctx.isJumping), "Jumping");
    }
    
    class InAir : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FIXED_UPDATE,
        FSM_STATE_ENTER<IPlayerContextView>
    {
        EnemyConfig cfg => facade.Config; EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>();
        public InAir(EnemyController facade) : base(facade) { }
        protected override void ExecutionImplementation(IEnemyContextView ctx)
        {
            facade.API_Context<EnemyContextData>().isGrounded = isGrounded();
            if(!ctx.isGrounded) bb.rb.AddForce(-facade.transform.up * cfg.inAir.fallScalar, cfg.inAir.forceMode);
        }
        
        bool isGrounded() => Physics2D.Raycast
            ( bb.feetPoint.position, -facade.transform.up, cfg.inAir.checkDist, cfg.inAir.mask);

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

        protected override void ExecutionImplementation(IEnemyContextView ctx)
        {
            bb.computePath.RateLimitedUpdateTick();
            if (ctx.path == null) return;
            
            var pos = bb.rb.position;
            float distToEndNode = Mathf.Abs((pos - (Vector2)ctx.path.vectorPath[^1]) .magnitude);

            if (distToEndNode < cfg.follow.stoppingDistToTarget)
            {
                Debug.Log("Reached end of path");
                mutateCtx.reachedEndOfPath = true; return;
            }
            else 
                mutateCtx.reachedEndOfPath = false;
            
            Debug.Log("Follow");
            Vector2 nextWaypoint = ctx.path.vectorPath[ctx.currentWaypointIndex]; // ths line errors
            Vector3 dir = (nextWaypoint - pos).normalized;
            Vector2 force = dir * cfg.follow.speedScalar * Time.deltaTime;
            float distToNextWaypoint = Vector2.Distance(pos, nextWaypoint);

            if (distToNextWaypoint < cfg.follow.nextWaypointDistance)
                if (ctx.currentWaypointIndex + 1 < ctx.path.vectorPath.Count) mutateCtx.currentWaypointIndex++;
            
            bb.rb.AddForce(force);
            
            
            /// TO DO: If the enemy is in the same position for a long period of time (3 seconds) and the
            /// player is ABOUVE the enemy, it will try jumping very high 3 times, if they fail, it will lose interest
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
        public Idle(EnemyController facade) : base(facade) { }
        protected override void ExecutionImplementation(IEnemyContextView ctx) { }
        public void OnEnterState(IEnemyContextView ctx)
        {
            Debug.Log("Entered Idle State");
            cfg.animHandle.Play(bb.animator, EnemyConfig.EnemyAnims.Idle);
        }
    }
}