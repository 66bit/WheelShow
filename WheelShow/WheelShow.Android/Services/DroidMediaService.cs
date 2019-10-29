using System;
using System.Collections.Generic;
using System.Text;
using Android.Media;
using WheelShow.Services;

[assembly: Xamarin.Forms.Dependency(typeof(WheelShow.Droid.Services.DroidMediaService))]

namespace WheelShow.Droid.Services
{
    public class DroidMediaService : IMediaService
    {
        private MediaPlayer mediaPlayer;

        public void PlayRingSound()
        {
            mediaPlayer = MediaPlayer.Create(global::Android.App.Application.Context, Resource.Raw.ring);
            mediaPlayer.Start();
        }
    }
}
