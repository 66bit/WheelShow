using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using WheelShow.Services;

[assembly: Xamarin.Forms.Dependency(typeof(WheelShow.Droid.Services.DroidFloatingService))]

namespace WheelShow.Droid.Services
{
    class DroidFloatingService : IFloatingService
    {
        public const int RequestOverlayId = 5000;

        public bool RequestFloatingPermissions()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.M) return false;
            var activity = CrossCurrentActivity.Current.Activity as MainActivity;
            var context = activity.BaseContext;
            if (Android.Provider.Settings.CanDrawOverlays(context)) return true;

            var uri = Android.Net.Uri.Parse("package:" + activity.PackageName);
            var intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission, uri);
            activity.StartActivityForResult(intent, RequestOverlayId);
            return false;
        }

        public void SwitchFloating()
        {
            var activity = CrossCurrentActivity.Current.Activity as MainActivity;

            if (IsFloatingStarted)
            {
                MainActivity.FloatingWidgetService.StopSelf();
                return;
            }

            var ok = RequestFloatingPermissions();
            if (ok)
                activity.StartFloatingService();
        }

        public bool IsFloatingStarted => MainActivity.FloatingWidgetService != null;

        public void SetFloatingMessage(string content, string extra, Xamarin.Forms.Color? color = null)
        {
            //var activity = CrossCurrentActivity.Current.Activity as MainActivity;
            MainActivity.FloatingWidgetService.SetMessage(content, extra, color);
        }
    }
}