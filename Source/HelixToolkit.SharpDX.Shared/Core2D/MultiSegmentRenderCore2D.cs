/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using SharpDX;
using D2D = SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public class MultiSegmentRenderCore2D : ShapeRenderCore2DBase
    {
        private float _length;


        private double segmentAngle;


        private int segmentNum;

        public int SegmentNum
        {
            get => segmentNum;
            set => SetAffectsRender(ref segmentNum, value);
        }

        public double SegmentAngle
        {
            get => segmentAngle;
            set => SetAffectsRender(ref segmentAngle, value);
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
                return true;
            return false;
        }

        protected override bool CanRender(RenderContext2D context)
        {
            return base.CanRender(context);
        }


        private float GetRadius()
        {
            var radius = LayoutBound.Width / 2f;
            if (LayoutBound.Height < LayoutBound.Width)
                radius = LayoutBound.Height / 2f;
            radius = radius - StrokeWidth;
            return radius;
        }

        protected override void OnRender(RenderContext2D context)
        {
            //VBI5中的多线段的半径使用的是控件的宽度
            var radius = GetRadius();
            _length = radius * 2f;
            var size2F = new Size2F(radius, radius);
            var startAngle = 90d - segmentAngle / 2.0d;

            var interval = (360d - segmentAngle * segmentNum) / segmentNum;
            var figures = new List<Figure>();
            for (var i = 0; i < segmentNum; i++)
            {
                var startPoint = GetArcPoint(startAngle, radius);
                var figure = new Figure(startPoint, false, false);
                var endAngle = segmentAngle + startAngle;
                var endPoint = GetArcPoint(endAngle, radius);
                var arcSize = D2D.ArcSize.Small;
                if (segmentAngle > 180d)
                    arcSize = D2D.ArcSize.Large;
                var arcSegment =
                    new ArcSegment(endPoint, size2F, 0, D2D.SweepDirection.Clockwise, arcSize);
                figure.AddSegment(arcSegment);
                figures.Add(figure);
                startAngle = endAngle + interval;
            }

            var geometry = Collect(new D2D.PathGeometry1(context.DeviceResources.Factory2D));
            using (var sink = geometry.Open())
            {
                foreach (var figure in figures)
                    figure.Create(sink);

                sink.Close();
            }
            if (StrokeBrush != null && StrokeWidth > 0 && StrokeStyle != null)
                context.DeviceContext.DrawGeometry(geometry, StrokeBrush, StrokeWidth /2, StrokeStyle);
            if (FillBrush != null)
                context.DeviceContext.FillGeometry(geometry, FillBrush);
        }

        private static double NormalizeAngle(double degree)
        {
            if (degree < 0.0 || degree > 360.0)
            {
                degree %= 360.0;
                if (degree < 0.0)
                    degree += 360.0;
            }
            return degree;
        }

        internal Vector2 GetArcPoint(double degree, double radius)
        {
            var arcPoint = CalArcPoint(degree);
            return RelativeToAbsolutePoint(arcPoint);
        }

        internal static Vector2 CalArcPoint(double degree)
        {
            degree = NormalizeAngle(degree);
            var num = degree * Math.PI / 180.0;
            return new Vector2((float) (0.5 + 0.5 * Math.Sin(num)), (float) (0.5 - 0.5 * Math.Cos(num)));
        }

        internal Vector2 RelativeToAbsolutePoint(Vector2 relative)
        {
            return new Vector2(LayoutBound.X + StrokeWidth + relative.X * _length,
                LayoutBound.Y + StrokeWidth + relative.Y * _length);
        }
    }
}