using System;
using System.Collections.Generic;
using System.Text;

namespace WheelShow.Services
{
    public interface IFloatingService
    {
        bool IsFloatingStarted { get; }
        void SwitchFloating();
        void SetFloatingMessage(string content, string extra, Xamarin.Forms.Color? color = null);
    }
}
