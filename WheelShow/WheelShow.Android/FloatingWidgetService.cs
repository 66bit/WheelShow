using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using WheelShow.Services;
using Xamarin.Forms.Platform.Android;

namespace WheelShow.Droid
{
    [Service]
    public class FloatingWidgetService : Service, View.IOnTouchListener
    {
        private IWindowManager _windowManager;
        private WindowManagerLayoutParams _layoutParams;
        private View _floatingView;
        private TextView txtSpeed;
        private TextView txtKmh;
        private int _initialX;
        private int _initialY;
        private float _initialTouchX;
        private float _initialTouchY;
        private DateTime _startDown;

        //private int originalWidth;
        private float originalFontSize = 38;
        private bool expanded;
        private MainActivity mainActivity;

        readonly IMediaService media = Xamarin.Forms.DependencyService.Get<IMediaService>();

        public override void OnCreate()
        {
            base.OnCreate();
            Start();

            MainActivity.FloatingWidgetService = this;
        }

        public void SetMessage(string content, string extra, Xamarin.Forms.Color? color = null)
        {
            txtSpeed.Text = content;
            var second = System.Environment.NewLine + "km/h";
            if (expanded && !string.IsNullOrWhiteSpace(extra))
                second = extra + second;
            txtKmh.Text = second;
            if (color.HasValue)
            {
                txtSpeed.SetTextColor(color.Value.ToAndroid());
                txtKmh.SetTextColor(color.Value.ToAndroid());
            }
        }

        private void Start()
        {
            _floatingView = LayoutInflater.From(this).Inflate(Resource.Layout.layout_floating_widget, null);
            SetTouchListener();

            _layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent,
                WindowManagerTypes.ApplicationOverlay,
                WindowManagerFlags.NotFocusable,
                Format.Translucent)
            {
                Gravity = GravityFlags.Left | GravityFlags.Center
            };

            _windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            _windowManager.AddView(_floatingView, _layoutParams);
            //_expandedContainer = _floatingView.FindViewById(Resource.Id.flyout);
            txtSpeed = _floatingView.FindViewById(Resource.Id.txtSpeed) as TextView;
            txtKmh = _floatingView.FindViewById(Resource.Id.txtKmh) as TextView;
        }

        private void SetTouchListener()
        {
            var rootContainer = _floatingView.FindViewById<RelativeLayout>(Resource.Id.root);
            rootContainer.SetOnTouchListener(this);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            MainActivity.FloatingWidgetService = null;
            base.OnDestroy();

            if (_floatingView != null)
            {
                _windowManager.RemoveView(_floatingView);
            }
        }

        public bool OnTouch(View view, MotionEvent motion)
        {
            switch (motion.Action)
            {
                case MotionEventActions.Down:
                    //initial position
                    _initialX = _layoutParams.X;
                    _initialY = _layoutParams.Y;

                    //touch point
                    _initialTouchX = motion.RawX;
                    _initialTouchY = motion.RawY;

                    if (DateTime.Now - _startDown < TimeSpan.FromMilliseconds(300))
                    {
                        OnDblClick();
                    }
                    _startDown = DateTime.Now;
                    return true;

                case MotionEventActions.Up:
                    int offsetX = Math.Abs((int)motion.RawX - (int)_initialTouchX);
                    int offsetY = Math.Abs((int)motion.RawY - (int)_initialTouchY);

                    if (offsetX < 15 && offsetY < 15)
                    {
                        if (DateTime.Now - _startDown < TimeSpan.FromMilliseconds(500))
                        {
                            OnClick();
                        }
                        else
                        {
                            OnLongClock();
                        }
                    }
                    return true;

                case MotionEventActions.Move:
                    //Calculate the X and Y coordinates of the view.
                    _layoutParams.X = _initialX + (int)(motion.RawX - _initialTouchX);
                    _layoutParams.Y = _initialY + (int)(motion.RawY - _initialTouchY);

                    //Update the layout with new X & Y coordinate
                    _windowManager.UpdateViewLayout(_floatingView, _layoutParams);
                    return true;
            }

            return false;
        }

        private void OnLongClock()
        {
            Intent intent = this.PackageManager.GetLaunchIntentForPackage(this.BaseContext.PackageName);
            intent.SetFlags(ActivityFlags.NewTask);
            intent.SetFlags(ActivityFlags.ReceiverForeground);
            this.StartActivity(intent);
        }

        private void OnDblClick()
        {
            media.PlayRingSound();
            //Intent intent = this.PackageManager.GetLaunchIntentForPackage(this.BaseContext.PackageName);
            //intent.SetFlags(ActivityFlags.NewTask);
            //intent.SetFlags(ActivityFlags.ReceiverForeground);
            //this.StartActivity(intent);
        }

        private void OnClick()
        {
            if (originalFontSize == 0)
                originalFontSize = txtSpeed.TextSize;

            if (!expanded)
            {
                txtSpeed.SetTextSize(Android.Util.ComplexUnitType.Sp, originalFontSize * 2);
            }
            else
            {
                txtSpeed.SetTextSize(Android.Util.ComplexUnitType.Sp, originalFontSize);
            }
            expanded = !expanded;
        }
    }
}