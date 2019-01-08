using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class Arc2D : ShapeModel2D
    {
        #region StartAngle

        /// <summary>
        /// Gets or sets the <see cref="StartAngle"/> value.
        /// </summary>
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="StartAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(
                                nameof(StartAngle),
                                typeof(double),
                                typeof(Arc2D),
                                new PropertyMetadata(0d, StartAnglePropertyChanged));

        private static void StartAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Arc2D)d).OnStartAngleChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="StartAngle"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="StartAngle"/>.</param>
        /// <param name="newValue">New value of <see cref="StartAngle"/>.</param>
        protected virtual void OnStartAngleChanged(double oldValue, double newValue)
        {
            (SceneNode as ArcNode2D).StartAngle = (float)newValue;
        }

        #endregion


        #region EndAngle

        /// <summary>
        /// Gets or sets the <see cref="EndAngle"/> value.
        /// </summary>
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="EndAngle"/> property.
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register(
                                nameof(EndAngle),
                                typeof(double),
                                typeof(Arc2D),
                                new PropertyMetadata(90d, EndAnglePropertyChanged));

        private static void EndAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Arc2D)d).OnEndAngleChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="EndAngle"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="EndAngle"/>.</param>
        /// <param name="newValue">New value of <see cref="EndAngle"/>.</param>
        protected virtual void OnEndAngleChanged(double oldValue, double newValue)
        {
            (SceneNode as ArcNode2D).EndAngle = (float)newValue;
        }

        #endregion

        protected override void OnUpdate(RenderContext2D context)
        {
            base.OnUpdate(context);
            if (SceneNode is ArcNode2D arcNode)
            {
                arcNode.Fill = Fill;
                arcNode.Stroke = Stroke;
            }
        }

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new ArcNode2D();
        }
    }
}
