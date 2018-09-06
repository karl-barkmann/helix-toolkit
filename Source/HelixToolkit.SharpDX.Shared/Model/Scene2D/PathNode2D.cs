using HelixToolkit.SharpDX.Extensions;
using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Windows;

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
{
    public class PathNode2D : ShapeNode2D
    {
        private System.Windows.Media.Geometry _geometry;

        public System.Windows.Media.Geometry Geometry
        {
            get => _geometry;
            set
            {
                if(SetAffectsMeasure(ref _geometry, value))
                {
                    _geometry = value;
                    var pathRenderCore2D = RenderCore as PathRenderCore2D;
                    if (value == null)
                    {
                        pathRenderCore2D.Figures = null;
                        return;
                    }

                    var pathGeometry = System.Windows.Media.PathGeometry.CreateFromGeometry(value);
                    var d2dFigures = new System.Collections.Generic.List<Figure>();
                    foreach (var figure in pathGeometry.Figures)
                    {
                        var d2dFigure = new Figure(figure.StartPoint.ToVector2(), figure.IsFilled, figure.IsClosed);
                        foreach (var segment in figure.Segments)
                        {
                            d2dFigure.AddSegment(segment.ToD2DSegment(), segment.IsStroked, segment.IsSmoothJoin);
                        }
                        d2dFigures.Add(d2dFigure);
                    }

                    pathRenderCore2D.Figures = d2dFigures;
                }
            }
        }

        public Brush OpacityMask
        {
            set
            {
                (RenderCore as PathRenderCore2D).OpacityMask = value;
            }
            get
            {
                return (RenderCore as PathRenderCore2D).OpacityMask;
            }
        }

        protected override ShapeRenderCore2DBase CreateShapeRenderCore()
        {
            return new PathRenderCore2D();
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            if (Geometry == null)
                return base.MeasureOverride(availableSize);
            var size = GetNaturalSize();
            return new Size2F((float)size.Width, (float)size.Height);
        }

        /// <summary>
        /// Get the natural size of the geometry that defines this shape
        /// </summary>
        internal virtual Size GetNaturalSize()
        {
            System.Windows.Media.Geometry geometry = Geometry;

            //
            // For the purposes of computing layout size, don't consider dashing. This will give us
            // slightly different bounds, but the computation will be faster and more stable.
            //
            // NOTE: If GetPen() is ever made public, we will need to change this logic so the user
            // isn't affected by our surreptitious change of DashStyle.
            //
            System.Windows.Media.Pen pen = GetPen();
            System.Windows.Media.DashStyle style = null;

            Rect bounds = new Rect();
            if (pen != null)
            {
                style = pen.DashStyle;

                if (style != null)
                {
                    pen.DashStyle = null;
                }
                bounds = geometry.GetRenderBounds(pen);
            }
            else
            {
                bounds = geometry.Bounds;
            }

            if (style != null)
            {
                pen.DashStyle = style;
            }

            return new Size(Math.Max(bounds.Right, 0),
                Math.Max(bounds.Bottom - bounds.Top, 0));
        }

        internal bool IsPenNoOp
        {
            get
            {
                double strokeThickness = StrokeThickness;
                return (Stroke == null) || DoubleUtil.IsNaN(strokeThickness) || DoubleUtil.IsZero(strokeThickness);
            }
        }

        private System.Windows.Media.Pen _pen = null;
        private System.Windows.Media.DoubleCollection _empty = new System.Windows.Media.DoubleCollection();

        internal System.Windows.Media.Pen GetPen()
        {
            if (IsPenNoOp)
            {
                return null;
            }

            if (_pen == null)
            {
                double thickness = 0.0;
                double strokeThickness = StrokeThickness;

                thickness = Math.Abs(strokeThickness);

                // This pen is internal to the system and
                // must not participate in freezable treeness
                _pen = new System.Windows.Media.Pen();

                _pen.Thickness = thickness;
                _pen.Brush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                _pen.StartLineCap = System.Windows.Media.PenLineCap.Flat;
                _pen.EndLineCap = System.Windows.Media.PenLineCap.Flat;
                _pen.DashCap = System.Windows.Media.PenLineCap.Flat;
                _pen.LineJoin = System.Windows.Media.PenLineJoin.Miter;
                _pen.MiterLimit = StrokeMiterLimit;

                // StrokeDashArray is usually going to be its default value and GetValue
                // on a mutable default has a per-instance cost associated with it so we'll
                // try to avoid caching the default value
                _empty.Freeze();
                // Avoid creating the DashStyle if we can
                double strokeDashOffset = StrokeDashOffset;
                if (strokeDashOffset != 0.0)
                {
                    _pen.DashStyle = new System.Windows.Media.DashStyle(_empty, strokeDashOffset);
                }
            }

            return _pen;
        }
    }
}
