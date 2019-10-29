using System;
using System.Collections.Generic;
using System.Text;
using Android.Content.PM;

namespace WheelShow.Services
{
    public enum RequestPermissionsState
    {
        Unknown,
        Requested,
        Guaranteed,
        Deny
    }

    public interface IPathService
    {
        RequestPermissionsState AccessPermissions { get; }
        string DownloadsPath { get; }
        string ExternalSDCardPath { get; }

        void RequestPermissions();
        bool ExecuteApplication(string packageName);
    }
}
