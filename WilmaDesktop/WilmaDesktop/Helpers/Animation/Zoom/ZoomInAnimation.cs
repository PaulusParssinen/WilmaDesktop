using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WilmaDesktop.Helpers.Animation
{
    public static class ZoomInAnimation
    {
        private const string ZoomInAnimationKey = "ZoomInAnimation";
        private const string ZoomInAnimationKeyTimeKey = "ZoomInAnimationKeyTime";
        private const string ZoomInAnimationEasingModeKey = "ZoomInAnimationEasingMode";

        #region IsEnabled Property
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled", typeof(bool), typeof(ZoomInAnimation), new PropertyMetadata(false, null, CoerceIsEnabled));

        private static object CoerceIsEnabled(DependencyObject element, object value)
        {
            var frameworkElement = element as FrameworkElement;
            var resources = Application.Current.Resources;

            if (frameworkElement == null || resources == null)
                return value;

            frameworkElement.RenderTransformOrigin = new Point(.5,.5);

            if ((bool)value)
            {
                frameworkElement.RenderTransform = new ScaleTransform(0, 0);
                frameworkElement.Visibility = Visibility.Visible;

                if (frameworkElement.Resources.Contains(ZoomInAnimationKey))
                    return value;

                var delay = GetStartDelay(frameworkElement);
                var storyboard = CreateStoryboard(resources, element, delay.TimeSpan);

                frameworkElement.Resources.Add(ZoomInAnimationKey, storyboard);

                storyboard.Begin();
            }
            else
            {
                if (!frameworkElement.Resources.Contains(ZoomInAnimationKey))
                    return value;

                frameworkElement.Resources.Remove(ZoomInAnimationKey);
            }

            return value;
        }

        public static void SetIsEnabled(DependencyObject element, object value)
        {
            element.SetValue(IsEnabledProperty, (bool)value);
        }

        public static bool GetIsEnabled(DependencyObject element)
            => (bool) element.GetValue(IsEnabledProperty);
        #endregion

        #region StartDelay Property
        public static readonly DependencyProperty StartDelayProperty = DependencyProperty.RegisterAttached(
            "StartDelay", typeof(KeyTime), typeof(ZoomInAnimation), new PropertyMetadata(KeyTime.FromTimeSpan(new TimeSpan(0))));

        public static void SetStartDelay(DependencyObject element, KeyTime value)
        {
            element.SetValue(StartDelayProperty, value);
        }

        public static KeyTime GetStartDelay(DependencyObject element)
            => (KeyTime) element.GetValue(StartDelayProperty);

        #endregion

        private static Storyboard CreateStoryboard(ResourceDictionary resources, DependencyObject element, TimeSpan delayTime)
        {
            DoubleAnimationUsingKeyFrames GetScaleAnimation()
                => new DoubleAnimationUsingKeyFrames
                {
                    BeginTime = delayTime,
                    KeyFrames = new DoubleKeyFrameCollection
                    {
                        new EasingDoubleKeyFrame(0),
                        new EasingDoubleKeyFrame(1, (KeyTime)resources[ZoomInAnimationKeyTimeKey])
                        {
                            EasingFunction = new CircleEase
                            {
                                EasingMode = (EasingMode)resources[ZoomInAnimationEasingModeKey]
                            }
                        }
                    }
                };

            var storyboard = new Storyboard
            {
                Children = new TimelineCollection
                {
                    GetScaleAnimation(),
                    GetScaleAnimation()
                }
            };

            Storyboard.SetTargetProperty(storyboard.Children[0], new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(storyboard.Children[1], new PropertyPath("RenderTransform.ScaleY"));

            Storyboard.SetTarget(storyboard, element);

            return storyboard;
        }
    }
}
