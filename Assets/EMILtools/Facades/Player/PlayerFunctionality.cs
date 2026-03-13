using EMILtools.Core;
using EMILtools.Extensions;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerFunctionality : Functionalities<
    PlayerController,
    PlayerContextData>
{
    
    protected override void AddModulesHere()
    {
        // AddModule(new ExampleModule());
        AddModule(new Move(facade.Input.Move, facade));
    }
    
    public class Move : BoundSetFunctionality<PlayerController, PlayerContextData,
            Move.Setter>,
        FIXED_UPDATE
    {
        PlayerConfig cfg => facade.Config; PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>();
        public class Setter : DataSetter<(bool, float)>
        {
            [ShowInInspector] public bool isActive => Get.Item1;
            [ShowInInspector] public float moveX => isActive ? base.Get.Item2 : NumEX.ZeroF;
        }
        public Move(IPublisher publisher, PlayerController facade) : base(publisher, facade) { }

        public override PipelineBuilder<PlayerContextData> InjectSteps( PipelineBuilder<PlayerContextData> builder)
            => builder.Add_ShortCircuit(ctx => !SetContext.isActive);
        
        public override bool ExecutionImplementation(PlayerContextData ctx)
        {
            Vector2 moveDir = new Vector2(x: SetContext.moveX * cfg.move.speedScalar, y: 0);
            bb.rb.AddForce(moveDir, cfg.move.forceMode2d);
            return true;
        }
    }
}