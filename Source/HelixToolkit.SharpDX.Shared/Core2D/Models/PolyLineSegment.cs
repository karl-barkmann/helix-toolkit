using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;
using System.Linq;

namespace HelixToolkit.Wpf.SharpDX.Core2D
{
    class PolyLineSegment : Segment
    {
        private readonly Vector2Collection _points;

        public PolyLineSegment(IEnumerable<Vector2> points)
        {
            _points = new Vector2Collection(points);
        }

        public override void Create(GeometrySink sink)
        {
            sink.AddLines(_points.Select(x => (RawVector2)x).ToArray());
        }
    }
}
