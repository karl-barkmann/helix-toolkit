using HelixToolkit.Wpf.SharpDX.Extensions;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    public class Path2D : ShapeModel2D
    {
        protected override SceneNode2D OnCreateSceneNode()
        {
            return new PathNode2D();
        }

        private bool _opacityMaskChanged = false;

        #region OpacityMask
            
        /// <summary>
        /// Gets or sets the <see cref="OpacityMask"/> value.
        /// </summary>
        public Brush OpacityMask
        {
            get { return (Brush)GetValue(OpacityMaskProperty); }
            set { SetValue(OpacityMaskProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="OpacityMask"/> property.
        /// </summary>
        public static readonly DependencyProperty OpacityMaskProperty =
            DependencyProperty.Register(
                                nameof(OpacityMask),
                                typeof(Brush),
                                typeof(Path2D),
                                new PropertyMetadata(null, OpacityMaskPropertyChanged));

        private static void OpacityMaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Path2D)d).OnOpacityMaskChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        /// <summary>
        /// Called when <see cref="OpacityMask"/> changed.
        /// </summary>
        /// <param name="oldValue">Old value of <see cref="OpacityMask"/>.</param>
        /// <param name="newValue">New value of <see cref="OpacityMask"/>.</param>
        protected virtual void OnOpacityMaskChanged(Brush oldValue, Brush newValue)
        {
            _opacityMaskChanged = true;
        }

        #endregion

        #region Data Properties

        /// <summary>
        /// Data property
        /// </summary>
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(Geometry),
            typeof(Path2D),
            new FrameworkPropertyMetadata(
                null,
                DataPropertyChanged));

        private static void DataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Path2D)d).OnDataChanged((Geometry)e.OldValue, (Geometry)e.NewValue);
        }

        private void OnDataChanged(Geometry oldValue, Geometry newValue)
        {
            var pathNode2D = SceneNode as PathNode2D;
            pathNode2D.Geometry = newValue;
        }

        /// <summary>
        /// Data property
        /// </summary>
        public Geometry Data
        {
            get
            {
                return (Geometry)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }
        #endregion

        protected override void OnUpdate(RenderContext2D context)
        {
            base.OnUpdate(context);
            if (_opacityMaskChanged)
            {
                (SceneNode as PathNode2D).OpacityMask = OpacityMask.ToD2DBrush(SceneNode.RenderSize, context.DeviceContext);
                _opacityMaskChanged = false;
            }
        }
    }
}
