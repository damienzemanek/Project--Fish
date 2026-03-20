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
        return null;
    }

    protected override void SetupTransitionsForFSM(StateMachine<IEnemyContextView> fsm, IEnemyContextView ctx)
    {
        // Add State Transitions here
        // fsm.AddAnyTransition<Jump>(new FuncPredicate(() => ctx.isJumping), "Jumping");
    }
    
}