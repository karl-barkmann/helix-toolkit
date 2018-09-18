using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene2D
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    class ArcNode2D : ShapeNode2D
    {
        public float StartAngle
        {
            set => (RenderCore as ArcRenderCore2D).StartAngle = value;
            get => (RenderCore as ArcRenderCore2D).StartAngle;
        }

        public float EndAngle
        {
            set => (RenderCore as ArcRenderCore2D).EndAngle = value;
            get => (RenderCore as ArcRenderCore2D).EndAngle;
        }

        protected override ShapeRenderCore2DBase CreateShapeRenderCore()
        {
            return new ArcRenderCore2D();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }
    }
}
