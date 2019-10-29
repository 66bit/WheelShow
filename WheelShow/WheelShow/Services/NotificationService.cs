using System;
using System.Collections.Generic;
using System.Text;

namespace WheelShow.Services
{
    public interface INotificationService
    {
        void ShowSimpleNotification(string channelId, int notificationId, string title, string content);
        void ShowPermanentNotification(string channelId, int notificationId, string title, string content);
    }
}
