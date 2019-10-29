using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WheelShow.Models;
using System.IO;
using WheelShow.Services;

namespace WheelShow.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class NewItemPage : ContentPage
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        static readonly IPathService pathHelper = DependencyService.Get<IPathService>();

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            Name = Name.Replace(';', ',');
            var cult = System.Globalization.CultureInfo.InvariantCulture;
            string wp = $"{DateTime.Now};" + Latitude.ToString("0.00000000", cult) + ";" + Longitude.ToString("0.00000000", cult) + ";{Name}";
            string path = Path.Combine(pathHelper.DownloadsPath, "Waypoints.csv");
            using (var stream = File.AppendText(path))
            {
                stream.WriteLine(wp);
            }

            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}