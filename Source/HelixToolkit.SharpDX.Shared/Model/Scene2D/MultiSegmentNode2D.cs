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


        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            return base.MeasureOverride(availableSize);
            //textRenderable.MaxWidth = availableSize.Width;
            //textRenderable.MaxHeight = availableSize.Height;
            //var metrices = textRenderable.Metrices;
            //return new Size2F(metrices.WidthIncludingTrailingWhitespace, metrices.Height);
        }

        protected override RectangleF ArrangeOverride(RectangleF finalSize)
        {
            return base.ArrangeOverride(finalSize);
            //textRenderable.MaxWidth = finalSize.Width;
            //textRenderable.MaxHeight = finalSize.Height;
            //var metrices = textRenderable.Metrices;
            //return finalSize;
        }
    }
}