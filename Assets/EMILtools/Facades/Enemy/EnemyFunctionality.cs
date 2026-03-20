using EMILtools.Systems;

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
        
        
        return AddModule(new Idle(facade));
    }

    protected override void SetupTransitionsForFSM(StateMachine<IEnemyContextView> fsm, IEnemyContextView ctx)
    {
        // Add State Transitions here
        // fsm.AddAnyTransition<Jump>(new FuncPredicate(() => ctx.isJumping), "Jumping");
    }

    class Patrol : UnboundFunctionality<EnemyController, IEnemyContextView>
    {
        public Patrol(EnemyController facade) : base(facade) { }

        protected override void ExecutionImplementation(IEnemyContextView ctx)
        {
            
        }
    }
    
    class Idle : UnboundFunctionality<EnemyController, IEnemyContextView>,
        FSM_STATE_ENTER<IEnemyContextView>,
        UPDATE
    {
        public Idle(EnemyController facade) : base(facade) { }
        EnemyConfig cfg => facade.API_Config<EnemyConfig>(); EnemyBlackboard bb => facade.API_Blackboard<EnemyBlackboard>();
        protected override void ExecutionImplementation(IEnemyContextView ctx) { }
        public void OnEnterState(IEnemyContextView ctx)
        {
            cfg.animHandle.Play(bb.animator, EnemyConfig.EnemyAnims.Idle);
        }
    }
}