using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using WheelShow.Services;

[assembly: Xamarin.Forms.Dependency(typeof(WheelShow.Droid.Services.DroidNotificationService))]

namespace WheelShow.Droid.Services
{
    class DroidNotificationService : INotificationService
    {
        public void ShowSimpleNotification(string channelId, int notificationId, string title, string content)
        {
            var activity = CrossCurrentActivity.Current.Activity as MainActivity;
            var notificationHelper = activity.NotificationHelper;

            var note = notificationHelper.GetNotification(channelId, title, content)
                .SetContentIntent(notificationHelper.CreatePendingIntent())
                .SetAutoCancel(true);
            notificationHelper.Notify(notificationId, note);
        }

        public void ShowPermanentNotification(string channelId, int notificationId, string title, string content)
        {
            var activity = CrossCurrentActivity.Current.Activity as MainActivity;
            var notificationHelper = activity.NotificationHelper;

            var note = notificationHelper.GetNotification(channelId, title, content)
                .SetContentIntent(notificationHelper.CreatePendingIntent())
                .SetAutoCancel(false)
                .SetOngoing(true)
                .SetOnlyAlertOnce(true);

            notificationHelper.Notify(notificationId, note);
        }
    }
}