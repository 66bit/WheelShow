﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using WheelShow.Views;
using WheelShow.Services;
using WheelShow.Models;

namespace WheelShow
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
