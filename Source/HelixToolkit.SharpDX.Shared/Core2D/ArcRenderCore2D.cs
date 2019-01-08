using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Extensions;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using System;
using D2D = SharpDX.Direct2D1;

#if NETFX_CORE
namespace HelixToolkit.UWP.Core2D
#else
namespace HelixToolkit.Wpf.SharpDX.Core2D
#endif
{
    class ArcRenderCore2D : ShapeRenderCore2DBase
    {
        private float _startAngle;
        private float _endAngle;
        private bool _isGeometryChanged;
        private D2D.PathGeometry1 _geometry;
        private D2D.Brush _stroke;
        private D2D.Brush _fill;

        public float StartAngle
        {
            get { return _startAngle; }
            set
            {
                if (SetAffectsRender(ref _startAngle, value))
                {
                    _isGeometryChanged = true;
                }
            }
        }

        public float EndAngle
        {
            get { return _endAngle; }
            set
            {
                if (SetAffectsRender(ref _endAngle, value))
                {
                    _isGeometryChanged = true;
                }
            }
        }

        public System.Windows.Media.Brush Stroke
        {
            get;
            set;
        }

        public System.Windows.Media.Brush Fill
        {
            get;
            set;
        }

        protected override bool OnAttach(IRenderHost host)
        {
            _isGeometryChanged = true;
            return base.OnAttach(host);
        }

        protected override void OnDetach()
        {
            base.OnDetach();
        }

        protected override bool CanRender(RenderContext2D context)
        {
            return base.CanRender(context) && !DoubleUtil.AreClose(StartAngle, EndAngle);
        }

        protected override void OnRender(RenderContext2D context)
        {
            if(_isGeometryChanged)
            {
                RemoveAndDispose(ref _geometry);
                RemoveAndDispose(ref _stroke);
                RemoveAndDispose(ref _fill);

                Vector2 geometrySize = new Vector2();
                if (DoubleUtil.IsVerySmall((NormalizeAngle(EndAngle) - NormalizeAngle(StartAngle)) % 360))
                {
                    var logicalBounds = ComputeLogicalBounds(LayoutBound);
                    Vector2[] array = new Vector2[2];
                    Size2F size = new Size2F(logicalBounds.Width / 2.0f, logicalBounds.Height / 2.0f);
                    Vector2 point = logicalBounds.Center;
                    if (size.Width > size.Height)
                    {
                        array[0] = new Vector2(logicalBounds.Left, point.Y);
                        array[1] = new Vector2(logicalBounds.Right, point.Y);
                    }
                    else
                    {
                        array[0] = new Vector2(point.X, logicalBounds.Top);
                        array[1] = new Vector2(point.X, logicalBounds.Bottom);
                    }

                    var leftArcSegment = new ArcSegment(array[1], size, 0, D2D.SweepDirection.Clockwise, D2D.ArcSize.Small);
                    var rightArcSegment = new ArcSegment(array[0], size, 0, D2D.SweepDirection.Clockwise, D2D.ArcSize.Small);
                    var figure = new Figure(array[0], false, true);
                    figure.AddSegment(leftArcSegment);
                    figure.AddSegment(rightArcSegment);
                    geometrySize = new Vector2(LayoutBound.Width, LayoutBound.Height);
                    _geometry = Collect(new D2D.PathGeometry1(context.DeviceResources.Factory2D));
                    using (var sink = _geometry.Open())
                    {
                        figure.Create(sink);
                        sink.Close();
                    }
                }
                else
                {
                    var radius = GetRadius();
                    var size2F = new Size2F(radius, radius);
                    var startPoint = GetArcPoint(StartAngle, radius);
                    var endPoint = GetArcPoint(EndAngle, radius);
                    var arcSize = D2D.ArcSize.Small;
                    if ((EndAngle - StartAngle) > 180d)
                        arcSize = D2D.ArcSize.Large;
                    var arcSegment = new ArcSegment(endPoint, size2F, 0, D2D.SweepDirection.Clockwise, arcSize);
                    var figure = new Figure(startPoint, false, false);
                    figure.AddSegment(arcSegment);
                    geometrySize = new Vector2(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y);
                    _geometry = Collect(new D2D.PathGeometry1(context.DeviceResources.Factory2D));
                    using (var sink = _geometry.Open())
                    {
                        figure.Create(sink);
                        sink.Close();
                    }
                }

                _stroke = Collect(Stroke.ToD2DBrush(geometrySize, context.DeviceContext));
                _fill = Collect(Fill.ToD2DBrush(geometrySize, context.DeviceContext));
                _isGeometryChanged = false;
            }

            if (StrokeBrush != null && StrokeWidth > 0 && StrokeStyle != null)
                context.DeviceContext.DrawGeometry(_geometry, _stroke, StrokeWidth, StrokeStyle);
            if (FillBrush != null)
                context.DeviceContext.FillGeometry(_geometry, _fill);
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
            return new Vector2((float)(0.5 + 0.5 * Math.Sin(angle)), (float)(0.5 - 0.5 * Math.Cos(angle)));
        }

        private Vector2 RelativeToAbsolutePoint(Vector2 relative, float radius)
        {
            var logicalBounds = ComputeLogicalBounds(LayoutBound);
            var point = new Vector2(logicalBounds.X + relative.X * logicalBounds.Width,
                logicalBounds.Y + relative.Y * logicalBounds.Height);
            return point;
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

        private float GetRadius()
        {
            var logicalBound = ComputeLogicalBounds(LayoutBound);
            var radius = logicalBound.Width / 2f;
            if (logicalBound.Height < logicalBound.Width)
                radius = logicalBound.Height / 2f;
            return radius;
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
    }
}
