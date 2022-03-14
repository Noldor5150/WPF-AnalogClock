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
    public class AnalogClock : Control
    {
        private Line hourHand;
        private Line minuteHand;
        private Line secondHand;

        public static DependencyProperty ShowSecondsProperty = DependencyProperty.Register("ShowSeconds", typeof(bool), typeof(AnalogClock), new PropertyMetadata(true));

        public bool ShowSeconds
        {
            get => (bool)GetValue(ShowSecondsProperty);
            set => SetValue(ShowSecondsProperty, value);
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

            Binding showSecondHandBinding = new Binding
            {
                Path = new PropertyPath(nameof(ShowSeconds)),
                Source = this,
                Converter = new BooleanToVisibilityConverter()
            };
            secondHand.SetBinding(VisibilityProperty, showSecondHandBinding);
            UpdateHandAngles();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick +=(s,e)=> UpdateHandAngles();
            timer.Start();
            base.OnApplyTemplate();
        }

     /*   protected override void OnTimeChanged(DateTime time)
        {
            UpdateHandAngles(time);
            base.OnTimeChanged(time);
        }
     */
        private void UpdateHandAngles()
        {
            hourHand.RenderTransform = new RotateTransform((DateTime.Now.Hour / 12.0) * 360, 0.5, 0.5);
            minuteHand.RenderTransform = new RotateTransform((DateTime.Now.Minute / 60.0) * 360, 0.5, 0.5);
            secondHand.RenderTransform = new RotateTransform((DateTime.Now.Second / 60.0) * 360, 0.5, 0.5);
        }
    }
}
