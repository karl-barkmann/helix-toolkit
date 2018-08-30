using System.Windows;
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