using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Android.Content.PM;
using WheelShow.Services;
using Android;
using Android.App;
using Plugin.CurrentActivity;
using Android.Content;

[assembly: Xamarin.Forms.Dependency(typeof(WheelShow.Droid.Services.DroidPathService))]

namespace WheelShow.Droid.Services
{
    internal class DroidPathService : IPathService
    {
        internal const int STORAGE_REQUEST_CODE = 10;

        public RequestPermissionsState AccessPermissions { get; private set; }

        string IPathService.DownloadsPath
        {
            get
            {
                return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
            }
        }

        public string ExternalSDCardPath
        {
            get
            {
                // Virtual SD card
                try
                {
                    var context = Application.Context;
                    Java.IO.File[] dirs = context.GetExternalFilesDirs(null);

                    foreach (Java.IO.File folder in dirs)
                    {
                        bool IsRemovable = Android.OS.Environment.InvokeIsExternalStorageRemovable(folder);
                        bool IsEmulated = Android.OS.Environment.InvokeIsExternalStorageEmulated(folder);

                        if (IsRemovable && !IsEmulated)
                        {
                            var path = folder.Path;
                            int p = path.IndexOf("/Android/data");
                            if (p != -1)
                                return path.Substring(0, p);
                            else
                                return null;
                        }
                    }
                }
                catch (Exception)
                {
                }

                //var path = Directory.GetDirectories("/storage").FirstOrDefault(x => x.Contains('-'));
                //if (path == null)
                //    return null;
                return null;
            }
        }

        public void RequestPermissions()
        {
            if (AccessPermissions == RequestPermissionsState.Requested) return;
            CrossCurrentActivity.Current.Activity.RequestPermissions(new String[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, STORAGE_REQUEST_CODE);
            AccessPermissions = RequestPermissionsState.Requested;
        }

        public bool OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            if (requestCode == STORAGE_REQUEST_CODE)
            {
                // Check if the only required permission has been granted
                var idRead = CheckPermissionState(permissions, Manifest.Permission.ReadExternalStorage, grantResults);
                var idWrite = CheckPermissionState(permissions, Manifest.Permission.WriteExternalStorage, grantResults);

                if (idRead == Permission.Granted && idWrite == Permission.Granted)
                {
                    AccessPermissions = RequestPermissionsState.Guaranteed;
                }
                else
                {
                    AccessPermissions = RequestPermissionsState.Deny;
                }
                return true;
            }
            return false;
        }

        private static Permission? CheckPermissionState(string[] permissions, string requested, Permission[] grantResults)
        {
            var id = permissions.ToList().IndexOf(requested);
            if (id == -1)
                return null;
            if (id > grantResults.Length - 1)
                return null;
            return grantResults[id];
        }

        public bool ExecuteApplication(string packageName)
        {
            var activity = CrossCurrentActivity.Current.Activity;

            Intent intent = activity.PackageManager.GetLaunchIntentForPackage(packageName);
            if (intent == null)
                return false;
            activity.StartActivity(intent);
            return true;
        }
    }
}
