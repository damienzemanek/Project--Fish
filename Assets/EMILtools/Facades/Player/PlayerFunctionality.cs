using EMILtools.Core;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerFunctionality : Functionalities<
    PlayerController,
    PlayerStructure>
{
    protected override void AddModulesHere()
    {
        // AddModule(new ExampleModule());
        AddModule(new Move(facade.Input.Move, facade));
    }


    public class Move : BoundSetFunctionality<
        PlayerController,
        PlayerStructure,
        Context<PlayerContextData, IPlayerContextView>,
        Move.Setter>,
        FIXED_UPDATE
    {
        PlayerConfig cfg => facade.Config;
        PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>();
        
        
        public class Setter : SettableTemplate<bool, Vector2> 
        { [ShowInInspector] public Vector2 move => unnamedStoredValue2; }

        public Move(IPersistentDelegate _action, PlayerController facade) : base(_action, facade) { }

        public override PipelineBuilder<Context<PlayerContextData, IPlayerContextView>> InjectSteps(
            PipelineBuilder<Context<PlayerContextData, IPlayerContextView>> builder)
        {
            builder.Add_ShortCircuit(ctx => !isActive);
            return builder;
        }

        public override bool ExecutionImplementation(Context<PlayerContextData, IPlayerContextView> ctx)
        {
            Vector2 moveDir = SetContext.move * cfg.move.speedScalar;
            bb.rb.AddForce(moveDir);
            return true;
        }

        public void OnFixedTick() => Execute<Context<PlayerContextData, IPlayerContextView>>();
    }
        
        
}