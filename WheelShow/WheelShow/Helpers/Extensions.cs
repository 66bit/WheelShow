using System;
using System.Collections.Generic;
using System.Text;

namespace WheelShow
{
    public static class Extensions
    {
        public static readonly TimeSpan MaxTime = TimeSpan.FromSeconds(5);

        public static float SafeParseFloat(this string val)
        {
            return float.Parse(val, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
        }
        public static double SafeParseDouble(this string val)
        {
            return double.Parse(val, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo);
        }
        public static bool IsLate(this DateTime time)
        {
            return DateTime.Now - time > MaxTime;
        }
        public static BetteryLevel RangeBetteryLevel(this float battery)
        {
            if (battery < 15)
                return BetteryLevel.Low;
            else if (battery < 30)
                return BetteryLevel.Medium;
            else
                return BetteryLevel.High;
        }
    }

    public enum BetteryLevel
    {
        Low,
        Medium,
        High
    }
}
