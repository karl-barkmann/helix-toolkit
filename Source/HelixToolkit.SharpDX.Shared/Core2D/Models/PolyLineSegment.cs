using HelixToolkit.Wpf.SharpDX.Core;
using SharpDX;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;
using System.Linq;
using D2D = SharpDX.Direct2D1;

namespace HelixToolkit.Wpf.SharpDX.Core2D
{
    class PolyLineSegment : Segment
    {
        private readonly Vector2Collection _points;

        public PolyLineSegment(IEnumerable<Vector2> points)
        {
            _points = new Vector2Collection(points);
        }

        public override void Create(D2D.GeometrySink sink)
        {
            sink.AddLines(_points.Select(x => (RawVector2)x).ToArray());
        }
    }
}
