/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene2D
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    public class MultiSegmentNode2D : ShapeNode2D
    {
        public int SegmentNum
        {
            set => (RenderCore as MultiSegmentRenderCore2D).SegmentNum = value;
            get => (RenderCore as MultiSegmentRenderCore2D).SegmentNum;
        }

        public double SegmentAngle
        {
            set => (RenderCore as MultiSegmentRenderCore2D).SegmentAngle = value;
            get => (RenderCore as MultiSegmentRenderCore2D).SegmentAngle;
        }


        protected override ShapeRenderCore2DBase CreateShapeRenderCore()
        {
            return new MultiSegmentRenderCore2D();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }
    }
}