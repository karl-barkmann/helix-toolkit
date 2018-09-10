using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using System.Collections.Generic;
using D2D = SharpDX.Direct2D1;

namespace HelixToolkit.Wpf.SharpDX.Core2D
{
    class PolyBezierSegment : Segment
    {
        private readonly Vector2Collection _bezierPoints;

        public PolyBezierSegment(IEnumerable<Vector2> bezierPoints)
        {
            _bezierPoints = new Vector2Collection(bezierPoints);
        }

        public override void Create(D2D.GeometrySink sink)
        {
            var bezierSegments = new List<D2D.BezierSegment>();
            for (int i = 0; i < _bezierPoints.Count; i += 3)
            {
                int index = i;
                var segment = new D2D.BezierSegment { };
                segment.Point1 = _bezierPoints[index];
                if ((index + 1) < _bezierPoints.Count)
                    segment.Point2 = _bezierPoints[index + 1];
                if ((index + 2) < _bezierPoints.Count)
                    segment.Point3 = _bezierPoints[index + 2];
                bezierSegments.Add(segment);
            }
            sink.AddBeziers(bezierSegments.ToArray());
        }
    }
}
