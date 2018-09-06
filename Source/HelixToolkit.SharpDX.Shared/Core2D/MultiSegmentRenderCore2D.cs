/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
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
            var logicalBound = ComputeLogicalBounds(LayoutBound);
            var radius = logicalBound.Width / 2f;
            if (logicalBound.Height < logicalBound.Width)
                radius = logicalBound.Height / 2f;
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
                context.DeviceContext.DrawGeometry(_geometry, StrokeBrush, StrokeWidth, StrokeStyle);
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
            var angle = degree * Math.PI / 180.0;
            return new Vector2((float) (0.5 + 0.5 * Math.Sin(angle)), (float) (0.5 - 0.5 * Math.Cos(angle)));
        }

        private RectangleF ComputeLogicalBounds(RectangleF layoutBounds)
        {
            RectangleF logicalBound = Inflate(layoutBounds, new Thickness(-StrokeWidth / 2));
            return GetStretchBound(logicalBound, Stretch.None, new Vector2(1.0f, 1.0f));
        }

        private static RectangleF GetStretchBound(RectangleF logicalBound, Stretch stretch, Vector2 aspectRatio)
        {
            if (stretch == Stretch.None)
            {
                stretch = Stretch.Fill;
            }
            if (stretch == Stretch.Fill || !HasValidArea(aspectRatio))
            {
                return logicalBound;
            }
            Vector2 point = Center(logicalBound);
            if (stretch == Stretch.Uniform)
            {
                if (aspectRatio.X * logicalBound.Height < logicalBound.X * aspectRatio.Y)
                {
                    logicalBound.Width = logicalBound.Height * aspectRatio.X / aspectRatio.Y;
                }
                else
                {
                    logicalBound.Height = logicalBound.Width * aspectRatio.Y / aspectRatio.X;
                }
            }
            else if (stretch == Stretch.UniformToFill)
            {
                if (aspectRatio.X * logicalBound.Height < logicalBound.Width * aspectRatio.Y)
                {
                    logicalBound.Height = logicalBound.Width * aspectRatio.Y / aspectRatio.X;
                }
                else
                {
                    logicalBound.Width = logicalBound.Height * aspectRatio.X / aspectRatio.Y;
                }
            }
            return new RectangleF(point.X - logicalBound.Width / 2.0f, point.Y - logicalBound.Height / 2.0f, logicalBound.Width, logicalBound.Height);
        }

        internal static Vector2 Center(RectangleF rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0f);
        }

        private static bool HasValidArea(Vector2 size)
        {
            return size.X > 0.0 && size.Y > 0.0 && !double.IsInfinity(size.X) && !double.IsInfinity(size.Y);
        }

        private static RectangleF Inflate(RectangleF rect, Thickness thickness)
        {
            float num = rect.Width + thickness.Left + thickness.Right;
            float num2 = rect.Height + thickness.Top + thickness.Bottom;
            float num3 = rect.X - thickness.Left;
            if (num < 0.0)
            {
                num3 += num / 2.0f;
                num = 0.0f;
            }
            float num4 = rect.Y - thickness.Top;
            if (num2 < 0.0)
            {
                num4 += num2 / 2.0f;
                num2 = 0.0f;
            }
            return new RectangleF(num3, num4, num, num2);
        }

        private Vector2 RelativeToAbsolutePoint(Vector2 relative, float radius)
        {
            var logicalBounds = ComputeLogicalBounds(LayoutBound);
            var point = new Vector2(logicalBounds.X + relative.X  * logicalBounds.Width,
                logicalBounds.Y + relative.Y * logicalBounds.Height);
            return point;
        }
    }
}