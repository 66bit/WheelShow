using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace WheelShow.ViewModels
{
    public class AboutViewModel
    {
        public AboutViewModel()
        {
            Title = "About";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("http://66bit.ru")));
        }

        public string Title { get; }
        public ICommand OpenWebCommand { get; }
    }
}