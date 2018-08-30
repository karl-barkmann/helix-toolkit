using HelixToolkit.Wpf.SharpDX.Core2D;
using D2D = SharpDX.Direct2D1;
using SharpDX;
using System;

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
{
    class ProgressBarNode2D : SceneNode2D
    {
        private bool strokeStyleChanged = true;

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
                (RenderCore as ProgressBarRenderCore2D).Background = value;
            }
            get
            {
                return (RenderCore as ProgressBarRenderCore2D).Background;
            }
        }

        public D2D.Brush Fill
        {
            set
            {
                (RenderCore as ProgressBarRenderCore2D).FillBrush = value;
            }
            get
            {
                return (RenderCore as ProgressBarRenderCore2D).FillBrush;
            }
        }

        public D2D.Brush Stroke
        {
            set { (RenderCore as ProgressBarRenderCore2D).StrokeBrush = value; }
            get { return (RenderCore as ProgressBarRenderCore2D).StrokeBrush; }
        }

        #region Stroke Style

        private D2D.CapStyle strokeDashCap = D2D.CapStyle.Flat;

        public D2D.CapStyle StrokeDashCap
        {
            set
            {
                if (SetAffectsRender(ref strokeDashCap, value))
                {
                    strokeStyleChanged = true;
                }
            }
            get { return strokeDashCap; }
        }

        private D2D.CapStyle strokeStartLineCap = D2D.CapStyle.Flat;

        public D2D.CapStyle StrokeStartLineCap
        {
            set
            {
                if (SetAffectsRender(ref strokeStartLineCap, value))
                {
                    strokeStyleChanged = true;
                }
            }
            get { return strokeStartLineCap; }
        }

        private D2D.CapStyle strokeEndLineCap = D2D.CapStyle.Flat;

        public D2D.CapStyle StrokeEndLineCap
        {
            set
            {
                if (SetAffectsRender(ref strokeEndLineCap, value))
                {
                    strokeStyleChanged = true;
                }
            }
            get { return strokeEndLineCap; }
        }

        private D2D.DashStyle strokeDashStyle = D2D.DashStyle.Solid;

        public D2D.DashStyle StrokeDashStyle
        {
            set
            {
                if (SetAffectsRender(ref strokeDashStyle, value))
                {
                    strokeStyleChanged = true;
                }
            }
            get { return strokeDashStyle; }
        }

        private float strokeDashOffset = 0;

        public float StrokeDashOffset
        {
            set
            {
                if (SetAffectsRender(ref strokeDashOffset, value))
                {
                    strokeStyleChanged = true;
                }
            }
            get { return strokeDashOffset; }
        }

        private D2D.LineJoin strokeLineJoin = D2D.LineJoin.Miter;

        public D2D.LineJoin StrokeLineJoin
        {
            set
            {
                if (SetAffectsRender(ref strokeLineJoin, value))
                {
                    strokeStyleChanged = true;
                }
            }
            get
            {
                return strokeLineJoin;
            }
        }

        private float strokeMiterLimit = 1;

        public float StrokeMiterLimit
        {
            set
            {
                if (SetAffectsRender(ref strokeMiterLimit, value))
                {
                    strokeStyleChanged = true;
                }
            }
            get { return strokeMiterLimit; }
        }

        public float StrokeThickness
        {
            set
            {
                (RenderCore as ProgressBarRenderCore2D).StrokeWidth = value;
            }
            get
            {
                return (RenderCore as ProgressBarRenderCore2D).StrokeWidth;
            }
        }

        private float[] strokeDashArray;

        public float[] StrokeDashArray
        {
            set
            {
                if (SetAffectsRender(ref strokeDashArray, value))
                {
                    strokeStyleChanged = true;
                }
            }
            get
            {
                return strokeDashArray;
            }
        }

        #endregion

        public double Maximum
        {
            set
            {
                (RenderCore as ProgressBarRenderCore2D).Maximum = value;
            }
            get
            {
                return (RenderCore as ProgressBarRenderCore2D).Maximum;
            }
        }

        public double Minimum
        {
            set
            {
                (RenderCore as ProgressBarRenderCore2D).Minimum = value;
            }
            get
            {
                return (RenderCore as ProgressBarRenderCore2D).Minimum;
            }
        }

        public double Value
        {
            set
            {
                (RenderCore as ProgressBarRenderCore2D).Value = value;
            }
            get
            {
                return (RenderCore as ProgressBarRenderCore2D).Value;
            }
        }

        public Orientation Orientation
        {
            get
            {
                return (RenderCore as ProgressBarRenderCore2D).Orientation;
            }
            set
            {
                (RenderCore as ProgressBarRenderCore2D).Orientation = value;
            }
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            return false;
        }

        protected override RenderCore2D CreateRenderCore()
        {
            return new ProgressBarRenderCore2D();
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                strokeStyleChanged = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Update(RenderContext2D context)
        {
            base.Update(context);
            if (strokeStyleChanged)
            {
                var renderCore = RenderCore as ProgressBarRenderCore2D;
                renderCore.StrokeStyle = new D2D.StrokeStyle(context.DeviceContext.Factory,
                    new D2D.StrokeStyleProperties()
                    {
                        DashCap = this.StrokeDashCap,
                        StartCap = StrokeStartLineCap,
                        EndCap = StrokeEndLineCap,
                        DashOffset = StrokeDashOffset,
                        LineJoin = StrokeLineJoin,
                        MiterLimit = Math.Max(1, (float)StrokeMiterLimit),
                        DashStyle = StrokeDashStyle
                    },
                    StrokeDashArray ?? (new float[0]));
                strokeStyleChanged = false;
            }
        }
    }
}
