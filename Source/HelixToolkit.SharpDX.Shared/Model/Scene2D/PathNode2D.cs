using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using SharpDX.Direct2D1;

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
{
    public class PathNode2D : ShapeNode2D
    {
        public System.Windows.Media.Geometry Geometry { get; internal set; }

        public Brush OpacityMask
        {
            set
            {
                (RenderCore as PathRenderCore2D).OpacityMask = value;
            }
            get
            {
                return (RenderCore as PathRenderCore2D).OpacityMask;
            }
        }

        protected override ShapeRenderCore2DBase CreateShapeRenderCore()
        {
            return new PathRenderCore2D();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            if (Geometry == null)
                return base.MeasureOverride(availableSize);
            var bounds = Geometry.Bounds;
            return new Size2F((float)bounds.Width, (float)bounds.Height);
        }
    }
}
