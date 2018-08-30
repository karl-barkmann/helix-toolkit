using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Orientation = System.Windows.Controls.Orientation;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class ProgressBar2D : Element2D
    {
        #region Background
        public static readonly DependencyProperty BackgroundProperty
           = DependencyProperty.Register("Background", typeof(Brush), typeof(ProgressBar2D),
               new PropertyMetadata(new SolidColorBrush(Colors.Transparent),
               (d, e) =>
               {
                   var m = d as ProgressBar2D;
                   m.backgroundChanged = true;
                   m.InvalidateRender();
               }));

        public Brush Background
        {
            set
            {
                SetValue(BackgroundProperty, value);
            }
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }
        }

        #endregion

        #region Fill

        public static DependencyProperty FillProperty
           = DependencyProperty.Register("Fill", typeof(Brush), typeof(ProgressBar2D), new PropertyMetadata(new SolidColorBrush(Colors.Black),
               (d, e) =>
               {
                   (d as ProgressBar2D).fillChanged = true;
               }));

        public Brush Fill
        {
            set
            {
                SetValue(FillProperty, value);
            }
            get
            {
                return (Brush)GetValue(FillProperty);
            }
        }

        #endregion

        #region Maximum

        /// <summary>
        /// Gets or sets the <see cref="Maximum"/> value.
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                                nameof(Maximum),
                                typeof(double),
                                typeof(ProgressBar2D),
                                new PropertyMetadata(100d, MaximumPropertyChanged, CoerceMaximum));

        private static object CoerceMaximum(DependencyObject d, object baseValue)
        {
            ProgressBar2D ctrl = (ProgressBar2D)d;
            double min = ctrl.Minimum;
            if ((double)baseValue < min)
            {
                return min;
            }
            return baseValue;
        }

        private static void MaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBar2D)d).OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Maximum"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Maximum"/>.</param>
        /// <param name="newValue">New value of <see cref="Maximum"/>.</param>
        protected virtual void OnMaximumChanged(double oldValue, double newValue)
        {
            maximumChanged = true;
        }

        #endregion

        #region Minimum

        /// <summary>
        /// Gets or sets the <see cref="Minimum"/> value.
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                                nameof(Minimum),
                                typeof(double),
                                typeof(ProgressBar2D),
                                new PropertyMetadata(0d, MinimumPropertyChanged));
        private static void MinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBar2D)d).OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Minimum"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Minimum"/>.</param>
        /// <param name="newValue">New value of <see cref="Minimum"/>.</param>
        protected virtual void OnMinimumChanged(double oldValue, double newValue)
        {
            minimumChanged = true;
        }

        #endregion

        #region Value

        /// <summary>
        /// Gets or sets the <see cref="Value"/> value.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Value"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                                nameof(Value),
                                typeof(double),
                                typeof(ProgressBar2D),
                                new PropertyMetadata(0d, ValuePropertyChanged, CoerceValue));

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            ProgressBar2D ctrl = (ProgressBar2D)d;
            double min = ctrl.Minimum;
            double v = (double)baseValue;
            if (v < min)
            {
                return min;
            }

            double max = ctrl.Maximum;
            if (v > max)
            {
                return max;
            }

            return baseValue;
        }

        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBar2D)d).OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Value"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Value"/>.</param>
        /// <param name="newValue">New value of <see cref="Value"/>.</param>
        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            if (_doubleAnimation != null)
            {
                BeginAnimation(AnimatedValueProperty, null);
                _doubleAnimation = null;
            }

            if (IsAnimationEnabled)
            {
                _doubleAnimation = new DoubleAnimation(oldValue, newValue, AnimationDuration);
                _doubleAnimation.FillBehavior = FillBehavior.Stop;
                _doubleAnimation.Completed += _doubleAnimation_Completed;

                BeginAnimation(AnimatedValueProperty, _doubleAnimation, HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                AnimatedValue = newValue;
            }
        }

        private void _doubleAnimation_Completed(object sender, EventArgs e)
        {
            BeginAnimation(AnimatedValueProperty, null);
            AnimatedValue = Value;
        }

        #endregion

        #region IsAnimationEnabled

        /// <summary>
        /// Gets or sets the <see cref="IsAnimationEnabled"/> value.
        /// </summary>
        public bool IsAnimationEnabled
        {
            get { return (bool)GetValue(IsAnimationEnabledProperty); }
            set { SetValue(IsAnimationEnabledProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="IsAnimationEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty IsAnimationEnabledProperty =
            DependencyProperty.Register(
                                nameof(IsAnimationEnabled),
                                typeof(bool),
                                typeof(ProgressBar2D),
                                new PropertyMetadata(true, IsAnimationEnabledPropertyChanged));

        private static void IsAnimationEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBar2D)d).OnIsAnimationEnabledChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="IsAnimationEnabled"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="IsAnimationEnabled"/>.</param>
        /// <param name="newValue">New value of <see cref="IsAnimationEnabled"/>.</param>
        protected virtual void OnIsAnimationEnabledChanged(bool oldValue, bool newValue)
        {

        }

        #endregion

        #region AnimationDuration

        /// <summary>
        /// Gets or sets the <see cref="AnimationDuration"/> value.
        /// </summary>
        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="AnimationDuration"/> property.
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(
                                nameof(AnimationDuration),
                                typeof(Duration),
                                typeof(ProgressBar2D),
                                new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(300)), AnimationDurationPropertyChanged));

        private static void AnimationDurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBar2D)d).OnAnimationDurationChanged((Duration)e.OldValue, (Duration)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="AnimationDuration"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="AnimationDuration"/>.</param>
        /// <param name="newValue">New value of <see cref="AnimationDuration"/>.</param>
        protected virtual void OnAnimationDurationChanged(Duration oldValue, Duration newValue)
        {

        }

        #endregion

        #region AnimatedValue

        /// <summary>
        /// Gets or sets the <see cref="AnimatedValue"/> value.
        /// </summary>
        private double AnimatedValue
        {
            get { return (double)GetValue(AnimatedValueProperty); }
            set { SetValue(AnimatedValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="AnimatedValue"/> property.
        /// </summary>
        private static readonly DependencyProperty AnimatedValueProperty =
            DependencyProperty.Register(
                                nameof(AnimatedValue),
                                typeof(double),
                                typeof(ProgressBar2D),
                                new PropertyMetadata(0d, AnimatedValuePropertyChanged));

        private static void AnimatedValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBar2D)d).OnAnimatedValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="AnimatedValue"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="AnimatedValue"/>.</param>
        /// <param name="newValue">New value of <see cref="AnimatedValue"/>.</param>
        protected virtual void OnAnimatedValueChanged(double oldValue, double newValue)
        {
            (SceneNode as ProgressBarNode2D).Value = newValue;
        }

        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets the <see cref="Orientation"/> value.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Orientation"/> property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                                nameof(Orientation),
                                typeof(Orientation),
                                typeof(ProgressBar2D),
                                new PropertyMetadata(Orientation.Vertical, OrientationPropertyChanged));

        private static void OrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressBar2D)d).OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="Orientation"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="Orientation"/>.</param>
        /// <param name="newValue">New value of <see cref="Orientation"/>.</param>
        protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue)
        {
            orientationChanged = true;
        }

        #endregion

        #region Stroke properties
        public static DependencyProperty StrokeProperty
            = DependencyProperty.Register("Stroke", typeof(Brush), typeof(ProgressBar2D), new PropertyMetadata(new SolidColorBrush(Colors.Black),
                (d, e) =>
                {
                    (d as ProgressBar2D).strokeChanged = true;
                }));

        public Brush Stroke
        {
            set
            {
                SetValue(StrokeProperty, value);
            }
            get
            {
                return (Brush)GetValue(StrokeProperty);
            }
        }

        public static DependencyProperty StrokeDashCapProperty
        = DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(ProgressBar2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeDashCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
            }));

        public PenLineCap StrokeDashCap
        {
            set
            {
                SetValue(StrokeDashCapProperty, value);
            }
            get
            {
                return (PenLineCap)GetValue(StrokeDashCapProperty);
            }
        }

        public static DependencyProperty StrokeStartLineCapProperty
            = DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(ProgressBar2D), new PropertyMetadata(PenLineCap.Flat,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeStartLineCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
                }));

        public PenLineCap StrokeStartLineCap
        {
            set
            {
                SetValue(StrokeStartLineCapProperty, value);
            }
            get
            {
                return (PenLineCap)GetValue(StrokeStartLineCapProperty);
            }
        }

        public static DependencyProperty StrokeEndLineCapProperty
        = DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(ProgressBar2D), new PropertyMetadata(PenLineCap.Flat,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeEndLineCap = ((PenLineCap)e.NewValue).ToD2DCapStyle();
            }));

        public PenLineCap StrokeEndLineCap
        {
            set
            {
                SetValue(StrokeEndLineCapProperty, value);
            }
            get
            {
                return (PenLineCap)GetValue(StrokeEndLineCapProperty);
            }
        }

        public static DependencyProperty StrokeDashArrayProperty
            = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(ProgressBar2D), new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeDashArray = e.NewValue == null ? new float[0] : (e.NewValue as DoubleCollection).Select(x => (float)x).ToArray();
                }));

        public DoubleCollection StrokeDashArray
        {
            set
            {
                SetValue(StrokeDashArrayProperty, value);
            }
            get
            {
                return (DoubleCollection)GetValue(StrokeDashArrayProperty);
            }
        }

        public static DependencyProperty StrokeDashOffsetProperty
            = DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(ProgressBar2D), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeDashOffset = (float)(double)e.NewValue;
                }));

        public double StrokeDashOffset
        {
            set
            {
                SetValue(StrokeDashOffsetProperty, value);
            }
            get
            {
                return (double)GetValue(StrokeDashOffsetProperty);
            }
        }

        public static DependencyProperty StrokeLineJoinProperty
        = DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(ProgressBar2D), new PropertyMetadata(PenLineJoin.Bevel,
            (d, e) =>
            {
                ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeLineJoin = ((PenLineJoin)e.NewValue).ToD2DLineJoin();
            }));


        public PenLineJoin StrokeLineJoin
        {
            set
            {
                SetValue(StrokeLineJoinProperty, value);
            }
            get
            {
                return (PenLineJoin)GetValue(StrokeLineJoinProperty);
            }
        }

        public static DependencyProperty StrokeMiterLimitProperty
            = DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(ProgressBar2D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeMiterLimit = (float)(double)e.NewValue;
                }));

        public double StrokeMiterLimit
        {
            set
            {
                SetValue(StrokeMiterLimitProperty, value);
            }
            get
            {
                return (double)GetValue(StrokeMiterLimitProperty);
            }
        }

        public static DependencyProperty StrokeThicknessProperty
            = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ProgressBar2D), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeThickness = (float)(double)e.NewValue;
                }));

        public double StrokeThickness
        {
            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
            get
            {
                return (double)GetValue(StrokeThicknessProperty);
            }
        }


        public DashStyle DashStyle
        {
            get { return (DashStyle)GetValue(DashStyleProperty); }
            set { SetValue(DashStyleProperty, value); }
        }

        public static readonly DependencyProperty DashStyleProperty =
            DependencyProperty.Register("DashStyle", typeof(DashStyle), typeof(ProgressBar2D), new PropertyMetadata(DashStyles.Solid,
                (d, e) => {
                    ((d as Element2DCore).SceneNode as ProgressBarNode2D).StrokeDashStyle = (e.NewValue as DashStyle).ToD2DDashStyle();
                }));


        #endregion

        private bool fillChanged = true;
        private bool strokeChanged = true;
        private bool backgroundChanged = true;
        private bool maximumChanged = true;
        private bool minimumChanged = true;
        private bool orientationChanged = true;
        private DoubleAnimation _doubleAnimation;

        protected override void OnUpdate(RenderContext2D context)
        {
            base.OnUpdate(context);
            if (backgroundChanged)
            {
                (SceneNode as ProgressBarNode2D).Background = Background.ToD2DBrush(SceneNode.RenderSize, context.DeviceContext);
                backgroundChanged = false;
            }
            if (fillChanged)
            {
                (SceneNode as ProgressBarNode2D).Fill = Fill.ToD2DBrush(SceneNode.RenderSize, context.DeviceContext);
                fillChanged = false;
            }
            if (strokeChanged)
            {
                (SceneNode as ProgressBarNode2D).Stroke = Stroke.ToD2DBrush(SceneNode.RenderSize, context.DeviceContext);
                strokeChanged = false;
            }
            if (maximumChanged)
            {
                (SceneNode as ProgressBarNode2D).Maximum = Maximum;
                maximumChanged = false;
            }
            if (minimumChanged)
            {
                (SceneNode as ProgressBarNode2D).Minimum = Minimum;
                minimumChanged = false;
            }
            if (orientationChanged)
            {
                var orientation = Orientation == Orientation.Horizontal ? Model.Scene2D.Orientation.Horizontal : Model.Scene2D.Orientation.Vertical;
                (SceneNode as ProgressBarNode2D).Orientation = orientation;
                orientationChanged = false;
            }
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode2D node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            var c = node as ProgressBarNode2D;
            c.StrokeDashArray = StrokeDashArray == null ? new float[0] : StrokeDashArray.Select(x => (float)x).ToArray();
            c.StrokeDashCap = StrokeDashCap.ToD2DCapStyle();
            c.StrokeDashOffset = (float)StrokeDashOffset;
            c.StrokeEndLineCap = StrokeEndLineCap.ToD2DCapStyle();
            c.StrokeLineJoin = StrokeLineJoin.ToD2DLineJoin();
            c.StrokeMiterLimit = (float)StrokeMiterLimit;
            c.StrokeStartLineCap = StrokeStartLineCap.ToD2DCapStyle();
            c.StrokeThickness = (float)StrokeThickness;
            c.StrokeDashStyle = DashStyle.ToD2DDashStyle();
        }

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new ProgressBarNode2D();
        }
    }
}
