/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using D2D = global::SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public class EllipseRenderCore2D : ShapeRenderCore2DBase
    {
        private D2D.Ellipse fillEllipse = new D2D.Ellipse();
        private D2D.Ellipse strokeEllipse = new D2D.Ellipse();

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnRender(RenderContext2D context)
        {
            fillEllipse.Point = LayoutBound.Center;
            fillEllipse.RadiusX = LayoutBound.Width / 2.0f - StrokeWidth;
            fillEllipse.RadiusY = LayoutBound.Height / 2.0f - StrokeWidth;

            strokeEllipse.Point = LayoutBound.Center;
            strokeEllipse.RadiusX = (LayoutBound.Width - StrokeWidth) / 2.0f;
            strokeEllipse.RadiusY = (LayoutBound.Height - StrokeWidth) / 2.0f;

            if (FillBrush != null)
            {
                context.DeviceContext.FillEllipse(fillEllipse, FillBrush);
            }
            if (StrokeBrush != null && StrokeStyle != null)
            {
                context.DeviceContext.DrawEllipse(strokeEllipse, StrokeBrush, StrokeWidth, StrokeStyle);
            }
        }
    }
}
