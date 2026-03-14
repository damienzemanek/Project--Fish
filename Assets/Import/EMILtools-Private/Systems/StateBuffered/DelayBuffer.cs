using System.Collections.Generic;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Timers.TimerUtility;

namespace EMILtools.Systems
{
    public class DelayBuffer<TVal> : ITimerUser
    {
        [ShowInInspector] public TVal Value
        {
            get => isBuffered ? bufferVal : actualVal;
            set
            {
                if (EqualityComparer<TVal>.Default.Equals(actualVal, value)) return;
                actualVal = value;
                StateChanged();
            }
        }

        TVal actualVal;
        TVal bufferVal;
    
        bool isBuffered => bufferTime.isRunning;
        readonly CountdownTimer bufferTime;
    
        public DelayBuffer(TVal value, Ref<float> _bufferTime)
        {
            bufferTime = new CountdownTimer(_bufferTime);
            bufferTime.OnTimerStop.Add(BufferEndsThenReset);
            this.InitTimer(bufferTime, true);
            actualVal = bufferVal = value;
        }

        void StateChanged()
        {
            bufferTime.Start();
        }

        void BufferEndsThenReset()
        {
            bufferVal = actualVal;
        }


        public static implicit operator TVal(DelayBuffer<TVal> delayBuffer) => delayBuffer.Value;
    }

}
