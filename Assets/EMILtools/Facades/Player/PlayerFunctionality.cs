using System;
using System.Collections;
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
        AddModule(new Friction(facade));
        AddModule(new Fall(facade));
    }


    class Fall : UnboundFunctionality<PlayerController, IPlayerContextView>,
        FIXED_UPDATE
    {
        PlayerConfig cfg => facade.Config; PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>();
        public Fall(PlayerController facade) : base(facade) { }

        public override PipelineBuilder<IPlayerContextView> InjectSteps(PipelineBuilder<IPlayerContextView> builder) => builder;
        public override bool ExecutionImplementation(IPlayerContextView ctx)
        {
            facade.API_Context<PlayerContextData>().isGrounded.Value = isGrounded();
            if(!ctx.IsGrounded) bb.rb.AddForce(-facade.transform.up * cfg.fall.scalar, ForceMode2D.Force);
            return false;
        }
        
        
        bool isGrounded()
        {
            var transform = facade.transform;
            var fall = cfg.fall;
            
            if (!bb.feetPoint)
            {
                var newFeetPoint = new GameObject("Feet Point Auto-Generated");
                newFeetPoint.transform.parent = transform;
                newFeetPoint.transform.localPosition = transform.position.With(y: transform.position.y + 0.02f);
                bb.feetPoint = newFeetPoint.transform;
            }

            if (fall.checkDist == 0) fall.checkDist = 0.08f;
            
            return Physics2D.Raycast(
                bb.feetPoint.position,
                -facade.transform.up,
                cfg.fall.checkDist,
                cfg.fall.mask);
        }
    }

    class Friction : UnboundFunctionality<PlayerController, IPlayerContextView>,
        FIXED_UPDATE
    {
        PlayerConfig cfg => facade.Config; PlayerBlackboard bb => facade.API_Blackboard<PlayerBlackboard>();
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
            UseBuffer(() => SetContext.isActive && !jumpInProgress, 1f, () => enableBufferHandle);
        }

        [ShowInInspector, ReadOnly] bool jumpInProgress = false;
        [ShowInInspector] bool enableBufferHandle => true;
        
        public override bool ExecutionImplementation(IPlayerContextView ctx)
        {
            if (!jumpInProgress) return false;
            ctx.JumpCurve.Value += cfg.jump.jumpCurveRate;
            var mult = ctx.JumpCurve.Evaluate;
            bb.rb.AddForce(new Vector2(0, cfg.jump.scalar) * mult, cfg.jump.forceMode);
            Debug.Log("Jumping");
            return false;
        }

        public void OnSet()
        {
            if (SetContext.isActive)
            {
                if (!ctx.isGrounded) return;
                jumpInProgress = true;
                bb.jumpCurve.DynamicStart(Operation.Increase);
                Debug.Log("jump started");
            }
            else
            { 
                jumpInProgress = false;
                bb.jumpCurve.DynamicStart(Operation.Decrease);
                //bb.jumpCurve.Value = 0;
            }
        }

        void Grounded(bool v)
        {
            if (v) {
                jumpInProgress = false;
                bb.jumpCurve.Value = 0; }
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