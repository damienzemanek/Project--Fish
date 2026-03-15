using EMILtools.Core;
using EMILtools.Extensions;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Timers.CurveValue;
using static EMILtools.Core.RectEX<PlayerConfig.MouseZones>;

public class PlayerFunctionality : Functionalities<
    PlayerController,
    IPlayerContextView>
{
    
    protected override void AddModulesHere()
    {
        // AddModule(new ExampleModule());
        AddModule(new Move(facade.Input.Move, facade));
        AddModule(new Jump(facade.Input.Jump, facade));
        AddModule(new Friction(facade));
        AddModule(new Fall(facade));
        AddModule(new Look(facade.Input.Look, facade));
    }


    class Look : BoundSetFunctionality<PlayerController, IPlayerContextView, Look.Setter>,
        UPDATE
    {
        PlayerConfig cfg => facade.Config; PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>(); PlayerContextData ctx => facade.API_Context<PlayerContextData>();
        public class Setter : DataSetter<Vector2>
        {
            [ShowInInspector] public Vector2 mousePos => Get;
        }
        public Look(IPublisher publisher, PlayerController facade) : base(publisher, facade) { }
        public override PipelineBuilder<IPlayerContextView> InjectSteps(PipelineBuilder<IPlayerContextView> builder) => builder;

        public override bool ExecutionImplementation(IPlayerContextView ctx)
        {
            var dir = CheckIfAnyContains(cfg.facing.callbackZones, SetContext.mousePos);
            if(dir == PlayerConfig.MouseZones.LeftScreen) bb.facingBody.rotation = Quaternion.Euler(0, 0, 0);
            else if(dir == PlayerConfig.MouseZones.RightScreen) bb.facingBody.rotation = Quaternion.Euler(0, 180, 0);
            return false;
        }
    }

    class Fall : UnboundFunctionality<PlayerController, IPlayerContextView>,
        FIXED_UPDATE
    {
        PlayerConfig cfg => facade.Config; PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>();
        public Fall(PlayerController facade) : base(facade) { }

        public override PipelineBuilder<IPlayerContextView> InjectSteps(PipelineBuilder<IPlayerContextView> builder) => builder;

        protected override void Awake()
        {
            facade.API_Context<PlayerContextData>().fallingWithoutJumpingFirst = new DelayBuffer<bool>(false, cfg.fall.fallingBufferWindow);
        }

        public override bool ExecutionImplementation(IPlayerContextView ctx)
        {
            facade.API_Context<PlayerContextData>().isGrounded.Value = isGrounded();
            if(!ctx.IsGrounded) bb.rb.AddForce(-facade.transform.up * cfg.fall.scalar, cfg.fall.forceMode);
            facade.API_Context<PlayerContextData>().fallingWithoutJumpingFirst
                .SetBuffered(!ctx.IsGrounded && !ctx.isJumping && ctx.canJumpCoyote);
            return false;
        }
        
        
        bool isGrounded() => Physics2D.Raycast
            ( bb.feetPoint.position, -facade.transform.up, cfg.fall.checkDist, cfg.fall.mask);
    }

    class Friction : UnboundFunctionality<PlayerController, IPlayerContextView>,
        FIXED_UPDATE
    {
        PlayerConfig cfg => facade.Config; PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>(); PlayerContextData ctx => facade.API_Context<PlayerContextData>();
        public Friction(PlayerController facade) : base(facade) { }

        public override PipelineBuilder<IPlayerContextView> InjectSteps(PipelineBuilder<IPlayerContextView> builder)
            => builder.Add_ShortCircuit(ctx => !ctx.IsGrounded);

        public override bool ExecutionImplementation(IPlayerContextView ctx)
        {
            float amount = Mathf.Min(Mathf.Abs(bb.rb.linearVelocityX), cfg.friction.frictionScalar);
            amount *= Mathf.Sign(bb.rb.linearVelocityX);
            bb.rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            return false;
        }
    }

    class Jump : BoundSetFunctionality<PlayerController, IPlayerContextView, Jump.Setter>,
        FIXED_UPDATE,
        ON_SET
    {
        PlayerConfig cfg => facade.Config; PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>(); PlayerContextData ctx => facade.API_Context<PlayerContextData>();
        public class Setter : DataSetter<bool>
        {
            [ShowInInspector] public bool isActive => Get;
        }
        public Jump(IPublisher publisher, PlayerController facade) : base(publisher, facade) { }

        public override PipelineBuilder<IPlayerContextView> InjectSteps(PipelineBuilder<IPlayerContextView> builder)
            => builder.Add_ShortCircuit(ctx => !SetContext.isActive);

        protected override void Awake()
        {
            facade.API_Context<PlayerContextData>().isGrounded.Reactions.Add(Grounded);
            UseBuffer(() => ctx.canJumpCoyote && !ctx.FallingWithoutJumpingFirst, cfg.jump.coyoteInputWindow);
            SetContext.OnSetEvent.Add(ResetBuffer);
        }
        
        public override bool ExecutionImplementation(IPlayerContextView ctx)
        {
            ctx.JumpCurve.Value += cfg.jump.jumpCurveRate;
            var mult = ctx.JumpCurve.Evaluate;
            bb.rb.AddForce(new Vector2(0, cfg.jump.scalar) * mult, cfg.jump.forceMode);
            facade.API_Context<PlayerContextData>().isJumping = true;
            
            return false;
        }

        public void MutateUsingNewSetValues()   
        {
            if (SetContext.isActive)
            {
                if (bb.rb.linearVelocityY < 0f)
                {
                    if (!ctx.FallingWithoutJumpingFirst && !ctx.isJumping && ctx.canJumpCoyote) 
                        bb.rb.linearVelocityY = 0;
                }    
                bb.jumpCurve.DynamicStart(Operation.Increase);
            }
            else { 
                if(ctx.isJumping) facade.API_Context<PlayerContextData>().canJumpCoyote = false;
                facade.API_Context<PlayerContextData>().isJumping = false;
                bb.jumpCurve.DynamicStart(Operation.Decrease);
                bb.jumpCurve.Value = 0;
            }
            
        }
        

        void Grounded(bool v)
        {
            if (v) {
                bb.jumpCurve.Value = 0; 
                facade.API_Context<PlayerContextData>().isJumping = false;
                facade.API_Context<PlayerContextData>().canJumpCoyote = true;
                facade.API_Context<PlayerContextData>().fallingWithoutJumpingFirst.SetNotBuffered(false); 
            }
        }
        
        

    }
    
    class Move : BoundSetFunctionality<PlayerController, IPlayerContextView, Move.Setter>,
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
            
            float clampedX = Mathf.Clamp(bb.rb.linearVelocity.x, -cfg.move.maxVelocity, cfg.move.maxVelocity);
            float currentY = bb.rb.linearVelocity.y;
            bb.rb.linearVelocity = new Vector2(clampedX, currentY); return true;
        }
    }
}