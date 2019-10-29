using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android;
using WheelShow.Models;
using Xamarin.Forms;
using Plugin.CurrentActivity;
using WheelShow.Droid.Services;
using WheelShow.Services;
using Android.Content;

namespace WheelShow.Droid
{
    [Activity(Label = "WheelShow", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static FloatingWidgetService FloatingWidgetService { get; internal set; }
        public NotificationHelper NotificationHelper { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            NotificationHelper = new NotificationHelper(this);
            NotificationHelper.CreateChannel(Defines.StatusChannel, GetString(Resource.String.status_channel_name), NotificationImportance.Low);
            NotificationHelper.CreateChannel(Defines.EventsChannel, GetString(Resource.String.events_channel_name), NotificationImportance.High);

            global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental", "FastRenderers_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public void StartFloatingService()
        {
            StartService(new Intent(this, typeof(FloatingWidgetService)));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            var droidPathHelper = DependencyService.Get<IPathService>() as DroidPathService;
            if (droidPathHelper != null)
            {
                if (droidPathHelper.OnRequestPermissionsResult(requestCode, permissions, grantResults))
                    return;
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == DroidFloatingService.RequestOverlayId)
            {
                if (Android.Provider.Settings.CanDrawOverlays(BaseContext))
                    StartFloatingService();
            }
        }
    }
}