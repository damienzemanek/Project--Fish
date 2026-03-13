using EMILtools.Core;
using EMILtools.Extensions;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Timers.CurveValue;

public class PlayerFunctionality : Functionalities<
    PlayerController,
    IPlayerContextView>
{
    
    protected override void AddModulesHere()
    {
        // AddModule(new ExampleModule());
        AddModule(new Move(facade.Input.Move, facade));
        AddModule(new Jump(facade.Input.Jump, facade));
    }

    public class Jump : BoundSetFunctionality<PlayerController, IPlayerContextView,
            Jump.Setter>,
        FIXED_UPDATE,
        ON_SET
    {
        PlayerConfig cfg => facade.Config; PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>();
        public class Setter : DataSetter<bool>
        {
            [ShowInInspector] public bool isActive => Get;
        }
        public Jump(IPublisher publisher, PlayerController facade) : base(publisher, facade) { }
        
        public override PipelineBuilder<IPlayerContextView> InjectSteps(PipelineBuilder<IPlayerContextView> builder)
            => builder.Add_ShortCircuit(ctx => !SetContext.isActive)
                .Add_ShortCircuit(ctx => (ctx.jumps <= 0));

        protected override void Awake()
        {
            facade.API_Context<PlayerContextData>().jumps = cfg.jump.maxJumps;
        }
        
        public override bool ExecutionImplementation(IPlayerContextView ctx)
        {
            ctx.JumpCurve.Value += cfg.jump.jumpCurveRate;
            bb.rb.JumpScaled2D(bb.phys.jumpSettings, ctx.JumpCurve.Evaluate);
            return false;
        }

        public void OnSet()
        {
            if (SetContext.isActive)
            {
                facade.API_Context<PlayerContextData>().jumps -= 1;
                bb.jumpCurve.DynamicStart(Operation.Increase);
            }
            else
            {
                bb.jumpCurve.DynamicStart(Operation.Decrease);
                bb.jumpCurve.Value = 0;
            }
            
            if(bb.phys.isGrounded) Landed();
            void Landed() => facade.API_Context<PlayerContextData>().jumps = cfg.jump.maxJumps;
        }

    }
    
    public class Move : BoundSetFunctionality<PlayerController, IPlayerContextView,
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

        public override PipelineBuilder<IPlayerContextView> InjectSteps( PipelineBuilder<IPlayerContextView> builder)
            => builder.Add_ShortCircuit(ctx => !SetContext.isActive);
        
        public override bool ExecutionImplementation(IPlayerContextView ctx)
        {
            var targSpeed = SetContext.moveX * cfg.move.speedScalar;
            var speedDiff = targSpeed - bb.rb.linearVelocity.x;
            var accel = Mathf.Abs(targSpeed) > 0.01f ? cfg.move.acceleration : cfg.move.deceleration;
            var movement = Mathf.Pow(Mathf.Abs(speedDiff), 2) * accel * Mathf.Sign(speedDiff);
            bb.rb.AddForce(movement * Vector2.right, cfg.move.forceMode2d);
            bb.rb.linearVelocity = Vector2.ClampMagnitude(bb.rb.linearVelocity, cfg.move.maxVelocity);
            return true;
        }
    }
}