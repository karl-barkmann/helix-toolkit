using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class MultiSegmentModel2D : ShapeModel2D
    {
        public static readonly DependencyProperty SegmentAngleProperty =
            DependencyProperty.Register("SegmentAngle", typeof(double), typeof(MultiSegmentModel2D),
                new PropertyMetadata(0d, OnSegmentAngleChanged));

        public static readonly DependencyProperty SegmentNumProperty =
            DependencyProperty.Register("SegmentNum", typeof(int), typeof(MultiSegmentModel2D),
                new PropertyMetadata(0, OnSegmentNumChanged));

        private readonly Storyboard _storyboard;

        public MultiSegmentModel2D()
        {
            this.Transform = new RotateTransform() {Angle = 0d};
            AnimationDuration = new Duration(TimeSpan.FromSeconds(3));
            AnimationInterval = TimeSpan.FromSeconds(0);

            _storyboard = new Storyboard() {RepeatBehavior = RepeatBehavior.Forever};
            Timeline.SetDesiredFrameRate(this._storyboard, 30);
            var animation = CreateDoubleAnimation();
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Transform.Angle"));
            _storyboard.Children.Add(animation);
        }

        private DoubleAnimation CreateDoubleAnimation()
        {
            var animation = new DoubleAnimation() {From = 0, To = 360};

            var bindingInterval = new Binding("AnimationInterval")
            {
                Mode = BindingMode.OneWay,
                Source = this
            };
            BindingOperations.SetBinding(animation, Timeline.BeginTimeProperty, bindingInterval);
            var bindingDuration = new Binding("AnimationDuration")
            {
                Mode = BindingMode.OneWay,
                Source = this
            };
            BindingOperations.SetBinding(animation, Timeline.DurationProperty, bindingDuration);
            return animation;
        }

        public bool EnableAnimation
        {
            get { return (bool) GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(MultiSegmentModel2D),
                new PropertyMetadata(false, OnEnableAnimationChanged));

        private static void OnEnableAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSegmentModel2D model2D))
            {
                return;
            }

            var newValue = (bool) e.NewValue;
            if (newValue)
            {
                model2D._storyboard.Begin();
            }
            else
            {
                model2D._storyboard.Stop();
            }
        }

        public Duration AnimationDuration
        {
            get { return (Duration) GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(MultiSegmentModel2D),
                new PropertyMetadata(default(Duration), OnAnimationChanged));

        public TimeSpan AnimationInterval
        {
            get { return (TimeSpan) GetValue(AnimationIntervalProperty); }
            set { SetValue(AnimationIntervalProperty, value); }
        }

        public static readonly DependencyProperty AnimationIntervalProperty =
            DependencyProperty.Register("AnimationInterval", typeof(TimeSpan), typeof(MultiSegmentModel2D),
                new PropertyMetadata(default(TimeSpan), OnAnimationChanged));

        public bool IsReverse
        {
            get { return (bool) GetValue(IsReverseProperty); }
            set { SetValue(IsReverseProperty, value); }
        }

        public static readonly DependencyProperty IsReverseProperty =
            DependencyProperty.Register("IsReverse", typeof(bool), typeof(MultiSegmentModel2D),
                new PropertyMetadata(false, OnIsReverseChanged));

        private static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSegmentModel2D model2D))
            {
                return;
            }

            model2D.ReStartAnimation();
        }

        private static void OnIsReverseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSegmentModel2D model2D))
            {
                return;
            }
            var newValue = (bool) e.NewValue;
            var animation = model2D._storyboard.Children[0] as DoubleAnimation;
            if (animation == null)
            {
                return;
            }
            if (newValue)
            {
                animation.From = 360d;
                animation.To = 0d;
            }
            else
            {
                animation.From = 0d;
                animation.To = 360;
            }

            model2D.ReStartAnimation();
        }

        private void ReStartAnimation()
        {
            if (_storyboard == null || _storyboard.Children.Count < 1)
            {
                return;
            }
            if (EnableAnimation)
            {
                _storyboard.Pause();
                _storyboard.Begin();
            }
            else
            {
                _storyboard.Stop();
            }
        }

        public double SegmentAngle
        {
            get => (double) GetValue(SegmentAngleProperty);
            set => SetValue(SegmentAngleProperty, value);
        }

        public int SegmentNum
        {
            get => (int) GetValue(SegmentNumProperty);
            set => SetValue(SegmentNumProperty, value);
        }

        private static void OnSegmentAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model2D = d as MultiSegmentModel2D;
            if (model2D?.SceneNode is MultiSegmentNode2D node2D)
                node2D.SegmentAngle = (double) e.NewValue;
        }

        private static void OnSegmentNumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var model2D = d as MultiSegmentModel2D;
            if (model2D?.SceneNode is MultiSegmentNode2D node2D)
                node2D.SegmentNum = (int) e.NewValue;
        }


        protected override SceneNode2D OnCreateSceneNode()
        {
            return new MultiSegmentNode2D();
        }


        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnUpdate(RenderContext2D context)
        {
            base.OnUpdate(context);
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode2D node)
        {
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}