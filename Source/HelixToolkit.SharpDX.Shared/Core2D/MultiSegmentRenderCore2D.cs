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
        private double segmentAngle;
        private int segmentNum;
        private D2D.PathGeometry1 _geometry;
        private bool _isGeometryChanged;

        public int SegmentNum
        {
            get => segmentNum;
            set
            {
                if (SetAffectsRender(ref segmentNum, value))
                {
                    _isGeometryChanged = true;
                }
            }
        }

        public double SegmentAngle
        {
            get => segmentAngle;
            set
            {
                if (SetAffectsRender(ref segmentAngle, value))
                {
                    _isGeometryChanged = true;
                }
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            _isGeometryChanged = true;
            return base.OnAttach(host);
        }

        protected override bool CanRender(RenderContext2D context)
        {
            return base.CanRender(context) && SegmentNum > 0;
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
            if (_isGeometryChanged)
            {
                RemoveAndDispose(ref _geometry);

                var radius = GetRadius();
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

                _geometry = Collect(new D2D.PathGeometry1(context.DeviceResources.Factory2D));
                using (var sink = _geometry.Open())
                {
                    foreach (var figure in figures)
                        figure.Create(sink);

                    sink.Close();
                }
                _isGeometryChanged = false;
            }

            if (StrokeBrush != null && StrokeWidth > 0 && StrokeStyle != null)
                context.DeviceContext.DrawGeometry(_geometry, StrokeBrush, StrokeWidth /2, StrokeStyle);
            if (FillBrush != null)
                context.DeviceContext.FillGeometry(_geometry, FillBrush);
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

        private Vector2 GetArcPoint(double degree, double radius)
        {
            var arcPoint = CalArcPoint(degree);
            return RelativeToAbsolutePoint(arcPoint, (float)radius);
        }

        private static Vector2 CalArcPoint(double degree)
        {
            degree = NormalizeAngle(degree);
            var num = degree * Math.PI / 180.0;
            return new Vector2((float) (0.5 + 0.5 * Math.Sin(num)), (float) (0.5 - 0.5 * Math.Cos(num)));
        }

        private Vector2 RelativeToAbsolutePoint(Vector2 relative, float radius)
        {
            return new Vector2(LayoutBound.X + StrokeWidth + relative.X * radius * 2,
                LayoutBound.Y + StrokeWidth + relative.Y * radius * 2);
        }
    }
}