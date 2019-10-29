using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using WheelShow.Models;

namespace WheelShow
{
    public class BatteryToColorConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ride = value as RideInfo;
            return Convert(ride);
        }

        public static Color Convert(RideInfo source)
        {
            if (source == null)
                return Color.Gray;

            var battery = source.BatteryLevel;

            if (source.Time.IsLate())
                return Color.Gray;

            if (battery.RangeBetteryLevel() == BetteryLevel.Low)
                return Color.Red;
            if (battery.RangeBetteryLevel() == BetteryLevel.Medium)
                return Color.Orange;

            return Color.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class StateOnTimeToColorConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Color.Gray;

            var v = (DateTime)value;

            if (v.IsLate())
                return Color.Gray;

            return Color.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
