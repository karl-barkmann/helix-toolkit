using System.Windows;
using System.Windows.Media;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Extensions;
    using HelixToolkit.Wpf.SharpDX.Core2D;
    using Model.Scene2D;
    using System;

    public class FrameStatisticsModel2D : Element2D
    {
        public static readonly DependencyProperty ForegroundProperty
            = DependencyProperty.Register("Foreground", typeof(Brush), typeof(FrameStatisticsModel2D),
        new PropertyMetadata(new SolidColorBrush(Colors.Black), (d, e) =>
        {
            var model = (d as FrameStatisticsModel2D);
            model.foregroundChanged = true;
        }));

        public Brush Foreground
        {
            set
            {
                SetValue(ForegroundProperty, value);
            }
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
        }

        public static readonly DependencyProperty BackgroundProperty
            = DependencyProperty.Register("Background", typeof(Brush), typeof(FrameStatisticsModel2D),
                new PropertyMetadata(new SolidColorBrush(Color.FromArgb(64, 32, 32, 32)), (d, e) =>
                {
                    var model = (d as FrameStatisticsModel2D);
                    model.backgroundChanged = true;
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


        public static readonly DependencyProperty FontSizeProperty
            = DependencyProperty.Register("FontSize", typeof(double), typeof(FrameStatisticsModel2D),
                new PropertyMetadata(12d, (d, e) =>
                {
                    var newValue = (double)e.NewValue;
                    var value = (int)Math.Round(newValue);
                    ((d as Element2DCore).SceneNode as FrameStatisticsNode2D).FontSize = Math.Max(1, value);
                }));

        public double FontSize
        {
            set
            {
                SetValue(FontSizeProperty, value);
            }
            get
            {
                return (double)GetValue(FontSizeProperty);
            }
        }

        private bool foregroundChanged = true;
        private bool backgroundChanged = true;

        protected override void OnAttached()
        {
            base.OnAttached();
            foregroundChanged = backgroundChanged = true;
        }

        protected override SceneNode2D OnCreateSceneNode()
        {
            return new FrameStatisticsNode2D();
        }

        protected override void OnUpdate(RenderContext2D context)
        {
            base.OnUpdate(context);
            if (foregroundChanged)
            {
                (SceneNode as FrameStatisticsNode2D).Foreground = Foreground?.ToD2DBrush(SceneNode.RenderSize, context.DeviceContext);
                foregroundChanged = false;
            }
            if (backgroundChanged)
            {
                (SceneNode as FrameStatisticsNode2D).Background = Background?.ToD2DBrush(SceneNode.RenderSize, context.DeviceContext);
                backgroundChanged = false;
            }
        }
    }
}
