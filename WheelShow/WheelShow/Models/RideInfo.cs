using Android;
using Android.Support.V4.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WheelShow.Services;
using Xamarin.Forms;

namespace WheelShow.Models
{
    public class RideInfo
    {
        static readonly IPathService pathHelper = DependencyService.Get<IPathService>();
        static readonly string LogFolder = "WheelLog Logs";

        public float Speed { get; set; }
        public float BatteryLevel { get; set; }
        public float CurrentDistance { get; set; }

        public float Voltage { get; set; }
        public float TotalDistance { get; set; }

        public DateTime Time { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }


        public static RideInfo GetCurrent()
        {
            var path = GetCurrentFile();
            if (path == null) return null;

            const int max = 512;
            string data;
            if (new FileInfo(path).Length <= max)
                data = File.ReadAllText(path.ToString());
            else
            {
                byte[] buffer = new byte[max];
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    stream.Seek(-max, SeekOrigin.End);
                    stream.Read(buffer, 0, buffer.Length);
                    data = Encoding.UTF8.GetString(buffer);
                }
            }

            return ExtractData(data);
        }

        private static RideInfo ExtractData(string data)
        {
            var p = data.Length;
            string[] items;
            do
            {
                p = data.LastIndexOf('\n', p - 1);
                if (p == -1) return null;
                var p2 = data.LastIndexOf('\n', p - 1);
                if (p2 == -1) return null;

                var line = data.Substring(p).Trim();
                items = line.Split(',');
            } while (items.Length != 21);

            try
            {
                var res = new RideInfo();
                res.Speed = items[8].SafeParseFloat();
                res.BatteryLevel = items[12].SafeParseFloat();
                res.CurrentDistance = items[13].SafeParseFloat() / 1000;

                res.Voltage = items[9].SafeParseFloat();
                res.TotalDistance = items[14].SafeParseFloat() / 1000;

                res.Time = DateTime.Parse(items[0] + "T" + items[1]);
                res.Latitude = items[2].SafeParseDouble();
                res.Longitude = items[3].SafeParseDouble();
                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        private static string GetCurrentFile()
        {
            if (pathHelper.AccessPermissions == RequestPermissionsState.Unknown)
                pathHelper.RequestPermissions();
            if (pathHelper.AccessPermissions != RequestPermissionsState.Guaranteed)
                return null; // Should wait

            var personal = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (!Directory.Exists(pathHelper.DownloadsPath))
                return null;

            var path = Path.Combine(pathHelper.DownloadsPath, LogFolder);
            if (!Directory.Exists(path))
                path = pathHelper.ExternalSDCardPath;
            if (!Directory.Exists(path))
                return null;


            var files = Directory.EnumerateFiles(path, "*.csv", SearchOption.TopDirectoryOnly);
            files = files.OrderByDescending(x => File.GetLastWriteTimeUtc(x));
            return files.FirstOrDefault();
        }
    }
}
