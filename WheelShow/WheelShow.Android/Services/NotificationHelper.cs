using System;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;

namespace WheelShow.Droid.Services
{
    public class NotificationHelper : ContextWrapper
    {
        public const string PRIMARY_CHANNEL = "default";

        NotificationManager manager;
        NotificationManager Manager
        {
            get
            {
                if (manager == null)
                {
                    manager = (NotificationManager)GetSystemService(NotificationService);
                }
                return manager;
            }
        }

        int SmallIcon => Android.Resource.Drawable.StatNotifyChat;

        public NotificationHelper(Context context) : base(context)
        {
        }

        public void CreateChannel(string channelId, string channelName, NotificationImportance importance)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }
            var channel = new NotificationChannel(channelId, channelName, importance);
            channel.LightColor = Color.Green;
            channel.LockscreenVisibility = NotificationVisibility.Public;
            Manager.CreateNotificationChannel(channel);
        }

        public Notification.Builder GetNotification(string channelId, string title, string body)
        {
            return new Notification.Builder(ApplicationContext, channelId)
                     .SetContentTitle(title)
                     .SetContentText(body)
                     .SetSmallIcon(SmallIcon);
        }

        public void Notify(int id, Notification.Builder notification)
        {
            Manager.Notify(id, notification.Build());
        }

        public PendingIntent CreatePendingIntent()
        {
            return CreatePendingIntent(typeof(MainActivity));
        }

        public PendingIntent CreatePendingIntent(Type activityType)
        {
            Intent intent = new Intent(this, activityType);

            const int pendingIntentId = 0;
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
            return pendingIntent;
        }
    }
}
