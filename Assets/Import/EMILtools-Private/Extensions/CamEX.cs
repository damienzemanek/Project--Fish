using System;
using EMILtools.Extensions;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Timers.TimerUtility;

public static class CamEX
{
    [Serializable]
    public class CurveValue : Timer
    {
        private const float NO_PAUSE = -1f;
        public enum Operation { Increase, Decrease }
        
        
        public AnimationCurve curve;
        [Min(0.01f)] public float operationScalar;

        
        [ReadOnly] public Operation operation;
        [ReadOnly] public float pauseProgress = NO_PAUSE;
        [ShowInInspector, ReadOnly] public virtual float Evaluate => (curve != null) ? curve.Evaluate(Progress) : 0;

        public CurveValue() : base(new Ref<float>(1f))
        {
            curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        }
        
        public CurveValue(float _initialTime, AnimationCurve curve) : base(_initialTime)
            =>  this.curve = curve;
        public CurveValue(Ref<float> _initialTime, AnimationCurve curve) : base(_initialTime) 
            =>  this.curve = curve;
        public CurveValue(AnimationCurve curve, float _initialTime, Action[] OnTimerStartCbs = null, Action[] OnTimerTickCbs = null, Action[] OnTimerStopCbs = null) : base(_initialTime, OnTimerStartCbs, OnTimerTickCbs, OnTimerStopCbs)
            => this.curve = curve;

        
        public override void TickImplementation(float deltaTime)
        {
            if (operation == Operation.Increase)
            {
                if (Time < initialTime)
                {
                    Time += deltaTime * operationScalar;
                    if (pauseProgress != NO_PAUSE && Progress > pauseProgress) Pause();
                }
                else Stop();
            }
            else if(operation == Operation.Decrease)
            {
                if (Time > 0)
                {
                    Time -= deltaTime * operationScalar;
                    if(pauseProgress != NO_PAUSE && Progress < pauseProgress) Pause();
                }
                else Stop();
            }
        }

        public override void InitializeTime() => Time = operation == Operation.Increase ? 0f : initialTime;

        public override void Start() =>
            throw new SystemException("Start() is not intended to be used in CurveValue, uss Start(Operation)");

        public void Start(Operation _operation)
        {
            operation = _operation;
            pauseProgress = NO_PAUSE;
            base.Start();
        }

        /// <summary>
        /// Start the curve either while its already active, or if its paused and you want to move it around
        /// </summary>
        /// <param name="_operation"></param>
        public void DynamicStart(Operation _operation)
        {
            if (initialTime == null) initialTime = new Ref<float>(1f);
            operation = _operation;
            pauseProgress = NO_PAUSE;
            StartCore();
        }
        
        /// <summary>
        /// Start the curve either while its already active, or if its paused and you want to move it around
        /// </summary>
        /// <param name="_operation"></param>
        public void DynamicStartAndPause(Operation _operation, float pauseProgress)
        {
            if (initialTime == null) initialTime = new Ref<float>(1f);
            operation = _operation;
            this.pauseProgress = pauseProgress;
            StartCore();
        }
        
        public void StartAndPauseAt(Operation _operation, float pauseProgress)
        {
            operation = _operation;
            this.pauseProgress = pauseProgress;
            base.Start();
        }
    }

}
