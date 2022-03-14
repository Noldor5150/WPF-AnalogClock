﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AnalogClock.CustomControls
{
    public delegate void TimeChangedEventHandler(object sender, TimeChangedEventArgs args);
    public class AnalogClock : Control
    {
        private Line hourHand;
        private Line minuteHand;
        private Line secondHand;

        public static DependencyProperty ShowSecondsProperty = DependencyProperty.Register("ShowSeconds", typeof(bool), typeof(AnalogClock), new PropertyMetadata(true));

        public static RoutedEvent TimeChangedEvent = EventManager.RegisterRoutedEvent("TimeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<DateTime>), typeof(AnalogClock));
        public bool ShowSeconds
        {
            get => (bool)GetValue(ShowSecondsProperty);
            set => SetValue(ShowSecondsProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<DateTime> TimeChanged
        {
            add
            {
                AddHandler(TimeChangedEvent, value);
            }
            remove
            {
                RemoveHandler(TimeChangedEvent, value);
            }
        }

        static AnalogClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnalogClock), new FrameworkPropertyMetadata(typeof(AnalogClock)));
        }

        public override void OnApplyTemplate()
        {
            hourHand = Template.FindName("PART_HourHand", this) as Line;
            minuteHand = Template.FindName("PART_MinuteHand", this) as Line;
            secondHand = Template.FindName("PART_SecondHand", this) as Line;

            /*  Binding showSecondHandBinding = new Binding
              {
                  Path = new PropertyPath(nameof(ShowSeconds)),
                  Source = this,
                  Converter = new BooleanToVisibilityConverter()
              };
              secondHand.SetBinding(VisibilityProperty, showSecondHandBinding);
            */
            UpdateHandAngles(DateTime.Now);
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick +=(s,e)=> OnTimeChanged(DateTime.Now);
            timer.Start();
            base.OnApplyTemplate();
        }

  
        protected virtual void OnTimeChanged(DateTime time)
        {
            UpdateHandAngles(time);
            UpdateTimeState(time);
            RaiseEvent( new RoutedPropertyChangedEventArgs<DateTime>(DateTime.Now.AddSeconds(-1),DateTime.Now, TimeChangedEvent ));
        }

        private void UpdateTimeState(DateTime time)
        {
            if (time.Hour >6 && time.Hour<18)
            {
                VisualStateManager.GoToState(this, "Day", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "Night", false);
            }
        }

        private void UpdateHandAngles(DateTime time)
        {
            hourHand.RenderTransform = new RotateTransform((time.Hour / 12.0) * 360, 0.5, 0.5);
            minuteHand.RenderTransform = new RotateTransform((time.Minute / 60.0) * 360, 0.5, 0.5);
            secondHand.RenderTransform = new RotateTransform((time.Second / 60.0) * 360, 0.5, 0.5);
        }
    }
}
