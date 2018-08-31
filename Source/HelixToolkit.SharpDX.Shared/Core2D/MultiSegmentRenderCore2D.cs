/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using D2D = SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    public class MultiSegmentRenderCore2D : ShapeRenderCore2DBase
    {
        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool CanRender(RenderContext2D context)
        {
            return base.CanRender(context);
        }


        private int segmentNum;

        public int SegmentNum
        {
            get { return segmentNum; }
            set { SetAffectsRender(ref segmentNum, value); }
        }


        private double segmentAngle;

        public double SegmentAngle
        {
            get { return segmentAngle; }
            set { SetAffectsRender(ref segmentAngle, value); }
        }


        private double GetRadius()
        {
            var radius = LayoutBound.Width / 2d;
            if (LayoutBound.Height < LayoutBound.Width)
            {
                radius = LayoutBound.Height / 2d;
            }
            radius = radius - StrokeWidth;
            return radius;
        }

        protected override void OnRender(RenderContext2D context)
        {
            //VBI5中的多线段的半径使用的是控件的宽度
            var radius = GetRadius();
            var size2F = new Size2F((float) radius, (float) radius);
            double startAngle = 90d - segmentAngle / 2.0d;

            //double rangle = 0;
            double interval = (360d - (segmentAngle * segmentNum)) / (double) segmentNum;
            var figures = new List<Figure>();
            for (int i = 0; i < segmentNum; i++)
            {
                var startPoint = GetArcPoint(startAngle, radius);
                var figure = new Figure(startPoint, false, false);
                var endAngle = segmentAngle + startAngle;
                var endPoint = GetArcPoint(endAngle, radius);
                var arcSize = D2D.ArcSize.Small;
                if (segmentAngle > 180d)
                {
                    arcSize = D2D.ArcSize.Large;
                }
                var arcSegment =
                    new ArcSegment(endPoint, size2F, (float) 0, D2D.SweepDirection.Clockwise, arcSize);
                figure.AddSegment(arcSegment);
                figures.Add(figure);
                startAngle = endAngle + interval;
            }

            var geometry = Collect(new D2D.PathGeometry1(context.DeviceResources.Factory2D));
            using (var sink = geometry.Open())
            {
                foreach (var figure in figures)
                {
                    figure.Create(sink);
                }

                sink.Close();
            }
            if (StrokeBrush != null && StrokeWidth > 0 && StrokeStyle != null)
            {
                context.DeviceContext.DrawGeometry(geometry, StrokeBrush, StrokeWidth, StrokeStyle);
            }
            if (FillBrush != null)
            {
                context.DeviceContext.FillGeometry(geometry, FillBrush);
            }
        }

        private static double NormalizeAngle(double degree)
        {
            if (degree < 0.0 || degree > 360.0)
            {
                degree %= 360.0;
                if (degree < 0.0)
                {
                    degree += 360.0;
                }
            }
            return degree;
        }

        internal Vector2 GetArcPoint(double degree, double radius)
        {
            Vector2 arcPoint = ComputeCartesianCoordinate(degree, radius);
            return arcPoint;
            //CalArcPoint(degree);
            //return RelativeToAbsolutePoint(arcPoint);
        }


        private static Vector2 ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            double angleRad = (Math.PI / 180.0) * angle;
            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);
            return new Vector2((float) x, (float) y);
        }

        internal static Vector2 CalArcPoint(double degree)
        {
            degree = NormalizeAngle(degree);
            double num = degree * Math.PI / 180.0;
            return new Vector2((float) (0.5 + 0.5 * Math.Sin(num)), (float) (0.5 - 0.5 * Math.Cos(num)));
        }

        internal Vector2 RelativeToAbsolutePoint(Vector2 relative)
        {
            return new Vector2(LayoutBound.X + relative.X * LayoutBound.Width,
                LayoutBound.Y + relative.Y * LayoutBound.Height);
        }
    }
}