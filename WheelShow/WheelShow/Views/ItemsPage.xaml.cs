using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WheelShow.Models;
using WheelShow.Views;
using WheelShow.Services;
using System.Diagnostics;
using System.Windows.Input;

namespace WheelShow.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        readonly IPathService pathHelper = DependencyService.Get<IPathService>();
        readonly INotificationService notifications = DependencyService.Get<INotificationService>();
        readonly IMediaService media = DependencyService.Get<IMediaService>();
        readonly IFloatingService floating = DependencyService.Get<IFloatingService>();

        static bool starting = true;
        bool previousState = false;
        RideInfo Ride { get; set; }

        public ICommand OpenContextMenuCommand { get; }


        public ItemsPage()
        {
            if (starting)
            {
                var pathHelper = DependencyService.Get<IPathService>();
                pathHelper.ExecuteApplication(Defines.WheelLogPackage);
                starting = false;
            }
            InitializeComponent();
            SizeChanged += OnSizeChanged;

            OpenContextMenuCommand = new Command((e) =>
            {
                pathHelper.ExecuteApplication(Defines.WheelLogPackage);
            });

            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            lblInform.IsVisible = Height > 240;
        }

        private bool OnTimerTick()
        {
            var info = Models.RideInfo.GetCurrent();
            btnAddWP.IsEnabled = info != null;
            if (info == null) return true;

            Ride = info;
            BindingContext = Ride;
            UpdateNotification();

            return true;
        }

        private void UpdateNotification()
        {
            var srcTitle = $"{Ride.Speed:0} km/h, {Ride.BatteryLevel:0}%";
            var title = srcTitle;
            var content = $"Distance: {Ride.CurrentDistance:0.00} km. Total: {Ride.TotalDistance:0.00} km";
            bool isActiveState = !Ride.Time.IsLate();
            if (!isActiveState)
            {
                title = $"Connecting...";
                content = $"Last: {Ride.BatteryLevel:0}%. {content}";
            }

            notifications.ShowPermanentNotification(Defines.StatusChannel, Defines.StatusNotificationId, title, content);
            if (floating.IsFloatingStarted)
            {
                floating.SetFloatingMessage(Ride.Speed.ToString("0"), Ride.BatteryLevel.ToString("0") + "%", BatteryToColorConverter.Convert(Ride));
            }

            if (previousState && !isActiveState)
                notifications.ShowSimpleNotification(Defines.EventsChannel, Defines.EventsNotificationId, "WheelShow", "Connection lost!");
            previousState = isActiveState;
        }

        async void AddClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()
            {
                Latitude = Ride.Latitude,
                Longitude = Ride.Longitude
            }));
        }

        void OnTapped(object sender, EventArgs args)
        {
            media.PlayRingSound();
        }

        private void ShowFloating(object sender, EventArgs e)
        {
            floating.SwitchFloating();
        }

        private void ShowWheelLog(object sender, EventArgs e)
        {
            pathHelper.ExecuteApplication(Defines.WheelLogPackage);
        }
    }
}