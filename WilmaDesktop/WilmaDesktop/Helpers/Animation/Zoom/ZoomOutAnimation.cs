using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WilmaDesktop.Helpers.Animation
{
    public static class ZoomOutAnimation
    {
        private const string ZoomOutAnimationKey = "ZoomOutAnimation";
        private const string ZoomOutAnimationKeyTimeKey = "ZoomOutAnimationKeyTime";
        private const string ZoomOutAnimationEasingModeKey = "ZoomOutAnimationEasingMode";

        #region IsEnabled Property
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled", typeof(bool), typeof(ZoomOutAnimation), new PropertyMetadata(false, null, CoerceIsEnabled));

        private static object CoerceIsEnabled(DependencyObject element, object value)
        {
            var frameworkElement = element as FrameworkElement;
            var resources = Application.Current.Resources;

            if (frameworkElement == null || resources == null)
                return value;

            if (frameworkElement.Visibility == Visibility.Hidden)
                return value;

            frameworkElement.RenderTransformOrigin = new Point(.5, .5);

            if ((bool)value)
            {
                if (frameworkElement.Resources.Contains(ZoomOutAnimationKey))
                    return value;

                frameworkElement.RenderTransform = new ScaleTransform(1, 1);

                var delay = GetStartDelay(frameworkElement);
                var storyboard = CreateStoryboard(resources, element, delay.TimeSpan);

                frameworkElement.Resources.Add(ZoomOutAnimationKey, storyboard);

                storyboard.Begin();
            }
            else
            {
                if (!frameworkElement.Resources.Contains(ZoomOutAnimationKey))
                    return value;

                frameworkElement.Resources.Remove(ZoomOutAnimationKey);
            }

            return value;
        }

        public static void SetIsEnabled(DependencyObject element, object value)
        {
            element.SetValue(IsEnabledProperty, (bool)value);
        }

        public static bool GetIsEnabled(DependencyObject element)
            => (bool)element.GetValue(IsEnabledProperty);
        #endregion

        #region StartDelay Property
        public static readonly DependencyProperty StartDelayProperty = DependencyProperty.RegisterAttached(
            "StartDelay", typeof(KeyTime), typeof(ZoomOutAnimation), new PropertyMetadata(KeyTime.FromTimeSpan(new TimeSpan(0))));

        public static void SetStartDelay(DependencyObject element, Duration value)
        {
            element.SetValue(StartDelayProperty, value);
        }

        public static KeyTime GetStartDelay(DependencyObject element)
            => (KeyTime)element.GetValue(StartDelayProperty);

        #endregion

        private static Storyboard CreateStoryboard(ResourceDictionary resources, DependencyObject element, TimeSpan delayTime)
        {
            DoubleAnimationUsingKeyFrames GetScaleAnimation()
                => new DoubleAnimationUsingKeyFrames {
                    BeginTime = delayTime,
                    KeyFrames = new DoubleKeyFrameCollection
                    {
                        new EasingDoubleKeyFrame(1),
                        new EasingDoubleKeyFrame(0, (KeyTime)resources[ZoomOutAnimationKeyTimeKey])
                        {
                            EasingFunction = new CircleEase
                            {
                                EasingMode = (EasingMode)resources[ZoomOutAnimationEasingModeKey]
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
