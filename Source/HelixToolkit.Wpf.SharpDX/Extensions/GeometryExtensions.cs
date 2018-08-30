using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Linq;
using System.Windows.Media;
using CoreD2D = HelixToolkit.Wpf.SharpDX.Core2D;
using D2D = SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Extensions
{
    public static class GeometryExtensions
    {
        public static D2D.PathGeometry1 ToD2DPathGeometry(this Geometry geometry)
        {
            throw new NotImplementedException();
        }

        public static CoreD2D.Segment ToD2DSegment(this PathSegment pathSegment)
        {
            switch (pathSegment)
            {
                case LineSegment lineSegment:
                    CoreD2D.LineSegment core2DLineSegment = new CoreD2D.LineSegment(lineSegment.Point.ToVector2());
                    return core2DLineSegment;
                case ArcSegment arcSegment:
                    var size = new Size2F((float)arcSegment.Size.Width, (float)arcSegment.Size.Height);
                    CoreD2D.ArcSegment cor2DArcSegment = new CoreD2D.ArcSegment(
                        arcSegment.Point.ToVector2(),
                        size,
                        (float)arcSegment.RotationAngle,
                        GetSweepDirection(arcSegment.SweepDirection),
                        arcSegment.IsLargeArc ? D2D.ArcSize.Large : D2D.ArcSize.Small);
                    return cor2DArcSegment;
                case BezierSegment bezierSegment:
                    CoreD2D.BezierSegment core2DBezierSegment = new CoreD2D.BezierSegment(
                        bezierSegment.Point1.ToVector2(),
                        bezierSegment.Point2.ToVector2(),
                        bezierSegment.Point3.ToVector2());
                    return core2DBezierSegment;
                case PolyLineSegment polyLineSegment:
                    var points = polyLineSegment.Points.Select(x => x.ToVector2());
                    CoreD2D.PolyLineSegment core2DPolylineSegment = new CoreD2D.PolyLineSegment(points);
                    return core2DPolylineSegment;
                default:
                    throw new NotSupportedException();
            }
        }

        private static D2D.SweepDirection GetSweepDirection(SweepDirection sweepDirection)
        {
            switch (sweepDirection)
            {
                case SweepDirection.Counterclockwise:
                    return D2D.SweepDirection.CounterClockwise;
                case SweepDirection.Clockwise:
                    return D2D.SweepDirection.Clockwise;
            }

            return D2D.SweepDirection.CounterClockwise;
        }
    }
}
