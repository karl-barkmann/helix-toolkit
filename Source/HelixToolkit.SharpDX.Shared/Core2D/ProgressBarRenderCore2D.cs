using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using SharpDX.Mathematics.Interop;
using D2D = SharpDX.Direct2D1;

namespace HelixToolkit.Wpf.SharpDX.Core2D
{
    public class ProgressBarRenderCore2D : RenderCore2DBase
    {
        private D2D.Brush background;
        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public D2D.Brush Background
        {
            set
            {
                var old = background;
                if (SetAffectsRender(ref background, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get { return background; }
        }

        private D2D.Brush fillBrush = null;
        /// <summary>
        /// Gets or sets the fill brush.
        /// </summary>
        /// <value>
        /// The fill brush.
        /// </value>
        public D2D.Brush FillBrush
        {
            set
            {
                var old = fillBrush;
                if (SetAffectsRender(ref fillBrush, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get
            {
                return fillBrush;
            }
        }

        private D2D.Brush strokeBrush = null;
        /// <summary>
        /// Gets or sets the stroke brush.
        /// </summary>
        /// <value>
        /// The stroke brush.
        /// </value>
        public D2D.Brush StrokeBrush
        {
            set
            {
                var old = strokeBrush;
                if (SetAffectsRender(ref strokeBrush, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get
            {
                return strokeBrush;
            }
        }
        /// <summary>
        /// Gets or sets the width of the stroke.
        /// </summary>
        /// <value>
        /// The width of the stroke.
        /// </value>
        public float StrokeWidth
        {
            set; get;
        } = 1.0f;

        private D2D.StrokeStyle strokeStyle = null;

        /// <summary>
        /// Gets or sets the stroke style.
        /// </summary>
        /// <value>
        /// The stroke style.
        /// </value>
        public D2D.StrokeStyle StrokeStyle
        {
            set
            {
                var old = strokeStyle;
                if (SetAffectsRender(ref strokeStyle, value))
                {
                    RemoveAndDispose(ref old);
                    Collect(value);
                }
            }
            get
            {
                return strokeStyle;
            }
        }

        private double _maximum;
        public double Maximum
        {
            get => _maximum;
            set => SetAffectsRender(ref _maximum, value);
        }

        private double _minimum;
        public double Minimum
        {
            get => _minimum;
            set => SetAffectsRender(ref _minimum, value);
        }

        private double _value;

        public double Value
        {
            get => _value;
            set => SetAffectsRender(ref _value, value);
        }

        private Orientation _orientation;

        public Orientation Orientation
        {
            get => _orientation;
            set => SetAffectsRender(ref _orientation, value);
        }

        protected override void OnRender(RenderContext2D context)
        {
            if (Background != null)
            {
                context.DeviceContext.FillRectangle(LayoutBound, Background);
            }
            if (StrokeBrush != null && StrokeStyle != null)
            {
                context.DeviceContext.DrawRectangle(LayoutBound, StrokeBrush, StrokeWidth, StrokeStyle);
            }

            if (FillBrush != null)
            {
                if (Orientation == Orientation.Vertical)
                {
                    var fillHeight = (float)(((LayoutBound.Height - StrokeWidth) / (Maximum - Minimum)) * (Value - Minimum));
                    var fillWidth = LayoutBound.Width - StrokeWidth / 2;
                    var x = StrokeWidth / 2;
                    var y = LayoutBound.Height - StrokeWidth / 2 - fillHeight;
                    var layoutBounding = new RawRectangleF(x, y, fillWidth, y + fillHeight);
                    context.DeviceContext.FillRectangle(layoutBounding, FillBrush);
                }
                else
                {
                    var fillHeight = LayoutBound.Height - StrokeWidth / 2;
                    var fillWidth = (float)(((LayoutBound.Width - StrokeWidth) / (Maximum - Minimum)) * (Value - Minimum));
                    var x = StrokeWidth / 2;
                    var y = StrokeWidth / 2;
                    var layoutBounding = new RawRectangleF(x, y, x + fillWidth, fillHeight);
                    context.DeviceContext.FillRectangle(layoutBounding, FillBrush);
                }
            }
        }
    }
}
