using System;
using UnityEngine;

namespace EMILtools.Extensions
{
    public static class NumEX
    {
        public const float ZeroF = 0f;
        public static readonly Index LastIndex = ^1;
        private const float NinetyF = 90f;

        /// <summary>
        /// Ensure Tolerance is between 0 and 1
        /// </summary>
        /// <param name="set"></param>
        /// <param name="compare"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static float ToleranceSet(float value, float target, float tolerance)
        {
            tolerance = Mathf.Clamp01(tolerance);
            float range = Mathf.Abs(target) * tolerance;
            return Mathf.Abs(value - target) <= range ? target : value;
        }

        /// <summary>
        /// Flips 0 to 1 and 1 to 0
        /// or 0.25 to 0.75
        /// or 0.5 to 0.5
        /// or 0.75 to 0.25
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float Flip01(float val)
        => Mathf.Abs(val - 1f);
        
        public static float Scaled(
            this float preScaled,
            Vector2 range,     // e.g. 0.8f
            float approachStart,  // e.g. verticalOffsetScalar (3)
            float approachEnd     // e.g. 1f
        )
        {
            // If outside the range, just return base value (or change this if needed)
            if (preScaled <= range.x)
                return preScaled;

            // Normalize prog within the range
            float prog = Mathf.InverseLerp(range.x, range.y, preScaled); // 0 -> 1

            // Lerp the scaling factor
            float approach = Mathf.Lerp(approachStart, approachEnd, prog);
            //approach = Mathf.Clamp(approach, Mathf.Min(approachStart, approachEnd), Mathf.Max(approachStart, approachEnd));

            return preScaled * approach;
        }
        
        
        public static float ProgressWhenApproaching(
            this float value,
            float start,
            float target,
            bool increasing // true = going up, false = going down
        )
        {
            if (Mathf.Approximately(start, target))
                return 1f;

            float t;

            if (increasing)
            {
                // value moves from start → target (ascending)
                t = (value - start) / (target - start);
            }
            else
            {
                // value moves from start → target (descending)
                t = (start - value) / (start - target);
            }

            return Mathf.Clamp01(t);
        }
        
        
    }
}