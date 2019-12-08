using UnityEngine;

namespace Core.Common.Extensions.Math
{
    public static class MathExtensions
    {
        public static bool IsBetweenRange(this float thisValue, float value1, float value2)
        {
            return thisValue >= Mathf.Min(value1, value2) && thisValue <= Mathf.Max(value1, value2);
        }

        public static bool IsBetweenRange(this int thisValue, int value1, int value2)
        {
            return thisValue >= Mathf.Min(value1, value2) && thisValue <= Mathf.Max(value1, value2);
        }

        public static bool IsBetweenRange(this double thisValue, double value1, double value2)
        {
            return thisValue >= System.Math.Min(value1, value2) && thisValue <= System.Math.Max(value1, value2);
        }
    }
}