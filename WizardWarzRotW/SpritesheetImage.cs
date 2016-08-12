using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace WizardWarzRotW
{
    class SpritesheetImage : Control
    {
        private const double Default_FrameRate = 1000 / 30.0;

        static SpritesheetImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpritesheetImage), new FrameworkPropertyMetadata(typeof(SpritesheetImage)));
        }

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(SpritesheetImage), new UIPropertyMetadata(null, UpdatePreCalculations));

        public int FrameMaxX
        {
            get { return (int)GetValue(FrameMaxXProperty); }
            set { SetValue(FrameMaxXProperty, value); }
        }

        public static readonly DependencyProperty FrameMaxXProperty =
            DependencyProperty.Register("FrameMaxX", typeof(int), typeof(SpritesheetImage), new UIPropertyMetadata(0, UpdatePreCalculations));

        public int FrameMaxY
        {
            get { return (int)GetValue(FrameMaxYProperty); }
            set { SetValue(FrameMaxYProperty, value); }
        }

        public static readonly DependencyProperty FrameMaxYProperty =
            DependencyProperty.Register("FrameMaxY", typeof(int), typeof(SpritesheetImage), new UIPropertyMetadata(0, UpdatePreCalculations));

        public static void UpdatePreCalculations(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var instance = sender as SpritesheetImage;
            if (instance.Source != null && instance.FrameMaxX != 0 && instance.FrameMaxY != 0)
            {
                instance.frameWidth = instance.Source.Width / instance.FrameMaxX;
                instance.frameHeight = instance.Source.Height / instance.FrameMaxY;
                if (double.IsNaN(instance.Width))
                    instance.Width = instance.frameWidth;
                if (double.IsNaN(instance.Height))
                    instance.Height = instance.frameHeight;
                instance.RefreshViewbox();
            }
        }

        public bool LoopForever
        {
            get { return (bool)GetValue(LoopForeverProperty); }
            set { SetValue(LoopForeverProperty, value); }
        }

        public static readonly DependencyProperty LoopForeverProperty =
            DependencyProperty.Register("LoopForever", typeof(bool), typeof(SpritesheetImage), new UIPropertyMetadata(true, OnPlaysRemainingChanged));

        public int PlaysRemaining
        {
            get { return (int)GetValue(PlaysRemainingProperty); }
            set { SetValue(PlaysRemainingProperty, value); }
        }

        public static readonly DependencyProperty PlaysRemainingProperty =
            DependencyProperty.Register("PlaysRemaining", typeof(int), typeof(SpritesheetImage), new UIPropertyMetadata(1, OnPlaysRemainingChanged));

        private static void OnPlaysRemainingChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpritesheetImage self = o as SpritesheetImage;
            if (!self.timer.IsEnabled && (self.PlaysRemaining > 0 || self.LoopForever))
            {
                self.timer.Start();
            }
            else if (self.timer.IsEnabled && self.LoopForever == false && self.PlaysRemaining == 0)
            {
                self.timer.Stop();
            }
        }

        private void RefreshViewbox()
        {
            Viewbox = new Rect(currentFrameX * frameWidth, currentFrameY * frameHeight, frameWidth, frameHeight);
        }

        private static readonly DependencyPropertyKey ViewboxPropertyKey =
            DependencyProperty.RegisterReadOnly("Viewbox", typeof(Rect), typeof(SpritesheetImage), new UIPropertyMetadata(null));

        public Rect Viewbox
        {
            get { return (Rect)GetValue(ViewboxProperty); }
            private set { SetValue(ViewboxPropertyKey, value); }
        }

        public static readonly DependencyProperty ViewboxProperty = ViewboxPropertyKey.DependencyProperty;

        public double FrameRate
        {
            get { return (double)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }

        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.Register("FrameRate", typeof(double), typeof(SpritesheetImage), new UIPropertyMetadata(Default_FrameRate, OnFrameRateChanged));

        private static void OnFrameRateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpritesheetImage self = o as SpritesheetImage;
            if (self != null)
            {
                if (e.NewValue is double && (double)e.NewValue > 0)
                {
                    self.timer.Interval = TimeSpan.FromMilliseconds(1000 / (double)e.NewValue);
                }
                else
                {
                    self.timer.Interval = TimeSpan.FromMilliseconds(Default_FrameRate);

                }
            }
        }

        public static readonly RoutedEvent AnimationCompleteEvent = EventManager.RegisterRoutedEvent("AnimationComplete", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SpritesheetImage));
        public event RoutedEventHandler AnimationComplete
        {
            add { AddHandler(AnimationCompleteEvent, value); }
            remove { RemoveHandler(AnimationCompleteEvent, value); }
        }

        private double frameWidth;
        private double frameHeight;
        private int currentFrameX;
        private int currentFrameY;
        private DispatcherTimer timer;

        public SpritesheetImage()
        {
            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(FrameRate), DispatcherPriority.Normal, new EventHandler(OnTimerTick), this.Dispatcher);
            this.Unloaded += new RoutedEventHandler(SpritesheetImage_Unloaded);
        }

        void SpritesheetImage_Unloaded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void OnTimerTick(object sender, EventArgs args)
        {
            if (LoopForever || PlaysRemaining > 0)
            {
                currentFrameX++;
                if (currentFrameX >= FrameMaxX)
                {
                    currentFrameX = 0;
                    currentFrameY++;
                    if (currentFrameY >= FrameMaxY)
                    {
                        currentFrameY = 0;
                        if (!LoopForever)
                        {
                            PlaysRemaining--;
                            if (PlaysRemaining == 0)
                            {
                                timer.Stop();
                                RaiseEvent(new RoutedEventArgs(AnimationCompleteEvent));
                            }
                        }
                    }
                }
                RefreshViewbox();
            }
        }
    }
}
