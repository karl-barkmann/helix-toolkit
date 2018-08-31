using System;
using System.Windows;
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

        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(MultiSegmentModel2D),
                new PropertyMetadata(false, OnEnableAnimationChanged));

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(TimeSpan), typeof(MultiSegmentModel2D),
                new PropertyMetadata(default(TimeSpan), OnAnimationChanged));

        public static readonly DependencyProperty AnimationIntervalProperty =
            DependencyProperty.Register("AnimationInterval", typeof(TimeSpan), typeof(MultiSegmentModel2D),
                new PropertyMetadata(default(TimeSpan), OnAnimationChanged));

        public static readonly DependencyProperty IsReverseProperty =
            DependencyProperty.Register("IsReverse", typeof(bool), typeof(MultiSegmentModel2D),
                new PropertyMetadata(false, OnIsReverseChanged));

        private readonly Storyboard _storyboard;

        public MultiSegmentModel2D()
        {
            Transform = new RotateTransform {Angle = 0d};
            this.RenderTransformOrigin=new Point(0.5,0.5);
            _storyboard = new Storyboard {RepeatBehavior = RepeatBehavior.Forever};
            Timeline.SetDesiredFrameRate(_storyboard, 30);
            var animation = CreateDoubleAnimation();
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Transform.Angle"));
            _storyboard.Children.Add(animation);
        }

        public bool EnableAnimation
        {
            get => (bool) GetValue(EnableAnimationProperty);
            set => SetValue(EnableAnimationProperty, value);
        }

        public TimeSpan AnimationDuration
        {
            get => (TimeSpan) GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public TimeSpan AnimationInterval
        {
            get => (TimeSpan) GetValue(AnimationIntervalProperty);
            set => SetValue(AnimationIntervalProperty, value);
        }

        public bool IsReverse
        {
            get => (bool) GetValue(IsReverseProperty);
            set => SetValue(IsReverseProperty, value);
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

        private DoubleAnimation CreateDoubleAnimation()
        {
            var animation = new DoubleAnimation {From = 0, To = 360};

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

        private static void OnEnableAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSegmentModel2D model2D))
                return;

            var newValue = (bool) e.NewValue;
            if (newValue)
                model2D._storyboard.Begin();
            else
                model2D._storyboard.Stop();
        }

        private static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSegmentModel2D model2D))
                return;

            model2D.ReStartAnimation();
        }

        private static void OnIsReverseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MultiSegmentModel2D model2D))
                return;
            var newValue = (bool) e.NewValue;
            var animation = model2D._storyboard.Children[0] as DoubleAnimation;
            if (animation == null)
                return;
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
                return;
            if (EnableAnimation)
            {
                _storyboard.Stop();
                _storyboard.Begin();
            }
            else
            {
                _storyboard.Stop();
            }
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