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
            PlayerContextData,
            Move.Setter>,
        FIXED_UPDATE
    {
        PlayerConfig cfg => facade.Config;
        PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>();
        
        
        public class Setter : SettableTemplate<bool, Vector2> 
        { [ShowInInspector] public Vector2 move {get => unnamedStoredValue2; set => unnamedStoredValue2 = value;} }

        public Move(IPersistentDelegate _action, PlayerController facade) : base(_action, facade) { }

        public override PipelineBuilder<PlayerContextData> InjectSteps(
            PipelineBuilder<PlayerContextData> builder)
        => builder.Add_ShortCircuit(ctx => !isActive, 
            shortCircuited: new IResolveContext[] { new Callback(ResetMove) });

        public override bool ExecutionImplementation(PlayerContextData ctx)
        {
            Vector2 moveDir = SetContext.move * cfg.move.speedScalar;
            bb.rb.AddForce(moveDir);
            return true;
        }
        
        public void ResetMove()
        {
            SetContext.move = Vector2.zero;
        }

        public void OnFixedTick() => Execute();
    }
}