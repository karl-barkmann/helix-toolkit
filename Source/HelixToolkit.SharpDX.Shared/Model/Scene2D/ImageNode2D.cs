/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using global::SharpDX.Direct2D1;
using global::SharpDX.WIC;
using SharpDX;
using System;
using System.IO;
using Bitmap = SharpDX.Direct2D1.Bitmap;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene2D
#else

namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    using Core2D;
    using System.Windows;
    using System.Windows.Controls;

    public class ImageNode2D : SceneNode2D
    {
        private Stream imageStream;
        private Stretch _stretch = Stretch.Uniform;
        private StretchDirection _stretchDirection = StretchDirection.Both;

        public Stream ImageStream
        {
            set
            {
                if (SetAffectsMeasure(ref imageStream, value))
                {
                    BitmapChanged = true;
                }
            }
            get
            {
                return imageStream;
            }
        }

        public Stretch Stretch
        {
            get { return _stretch; }
            set
            {
                if (_stretch != value)
                {
                    _stretch = value;
                    InvalidateMeasure();
                    InvalidateArrange();
                }
            }
        }

        public StretchDirection StretchDirection
        {
            get { return _stretchDirection; }
            set
            {
                if (_stretchDirection != value)
                {
                    _stretchDirection = value;
                    InvalidateMeasure();
                    InvalidateArrange();
                }
            }
        }

        public float Opacity
        {
            set
            {
                (RenderCore as ImageRenderCore2D).Opacity = value;
            }
            get
            {
                return (RenderCore as ImageRenderCore2D).Opacity;
            }
        }

        protected bool BitmapChanged { private set; get; } = true;

        protected override RenderCore2D CreateRenderCore()
        {
            return new ImageRenderCore2D();
        }

        protected override bool OnAttach(IRenderHost host)
        {
            if (base.OnAttach(host))
            {
                BitmapChanged = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LoadBitmap(RenderContext2D context, Stream stream)
        {
            (RenderCore as ImageRenderCore2D).Bitmap = stream == null ? null : OnLoadImage(context, stream);
        }

        protected virtual Bitmap OnLoadImage(RenderContext2D context, Stream stream)
        {
            stream.Position = 0;
            using (var decoder = new BitmapDecoder(context.DeviceResources.WICImgFactory, stream, DecodeOptions.CacheOnLoad))
            {
                using (var frame = decoder.GetFrame(0))
                {
                    using (var converter = new FormatConverter(context.DeviceResources.WICImgFactory))
                    {
                        converter.Initialize(frame, global::SharpDX.WIC.PixelFormat.Format32bppPBGRA);
                        return Bitmap1.FromWicBitmap(context.DeviceContext, converter);
                    }
                }
            }
        }

        public override void Update(RenderContext2D context)
        {
            base.Update(context);
            if (BitmapChanged)
            {
                LoadBitmap(context, ImageStream);
                InvalidateMeasure();
                InvalidateArrange();
                BitmapChanged = false;
            }
        }

        protected override Size2F MeasureOverride(Size2F availableSize)
        {
            var inputSize = new Size(availableSize.Width, availableSize.Height);
            return MeasureArrangeHelper(inputSize).ToSize2F();
        }

        protected override RectangleF ArrangeOverride(RectangleF finalSize)
        {
            var inputSize = new Size(finalSize.Width, finalSize.Height);
            var arrangeSize = MeasureArrangeHelper(inputSize).ToSize2F();
            finalSize.Width = arrangeSize.Width;
            finalSize.Height = arrangeSize.Height;
            return finalSize;
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            hitResult = null;
            if (LayoutBoundWithTransform.Contains(mousePoint))
            {
                hitResult = new HitTest2DResult(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        private Size MeasureArrangeHelper(Size inputSize)
        {
            var renderCore = RenderCore as ImageRenderCore2D;
            Size naturalSize = new Size(Width,Height);
            if (renderCore.Bitmap == null)
            {
                return naturalSize;
            }

            naturalSize = new Size(renderCore.ImageSize.Width, renderCore.ImageSize.Height);

            //get computed scale factor
            Size scaleFactor = ComputeScaleFactor(inputSize,
                                                          naturalSize,
                                                          this.Stretch,
                                                          this.StretchDirection);

            // Returns our minimum size & sets DesiredSize.
            return new Size(naturalSize.Width * scaleFactor.Width, naturalSize.Height * scaleFactor.Height);
        }

        internal static Size ComputeScaleFactor(Size availableSize,
                                               Size contentSize,
                                               Stretch stretch,
                                               StretchDirection stretchDirection)
        {
            // Compute scaling factors to use for axes
            double scaleX = 1.0;
            double scaleY = 1.0;

            bool isConstrainedWidth = !Double.IsPositiveInfinity(availableSize.Width);
            bool isConstrainedHeight = !Double.IsPositiveInfinity(availableSize.Height);

            if ((stretch == Stretch.Uniform || stretch == Stretch.UniformToFill || stretch == Stretch.Fill)
                 && (isConstrainedWidth || isConstrainedHeight))
            {
                // Compute scaling factors for both axes
                scaleX = (DoubleUtil.IsZero(contentSize.Width)) ? 0.0 : availableSize.Width / contentSize.Width;
                scaleY = (DoubleUtil.IsZero(contentSize.Height)) ? 0.0 : availableSize.Height / contentSize.Height;

                if (!isConstrainedWidth) scaleX = scaleY;
                else if (!isConstrainedHeight) scaleY = scaleX;
                else
                {
                    // If not preserving aspect ratio, then just apply transform to fit
                    switch (stretch)
                    {
                        case Stretch.Uniform:       //Find minimum scale that we use for both axes
                            double minscale = scaleX < scaleY ? scaleX : scaleY;
                            scaleX = scaleY = minscale;
                            break;

                        case Stretch.UniformToFill: //Find maximum scale that we use for both axes
                            double maxscale = scaleX > scaleY ? scaleX : scaleY;
                            scaleX = scaleY = maxscale;
                            break;

                        case Stretch.Fill:          //We already computed the fill scale factors above, so just use them
                            break;
                    }
                }

                //Apply stretch direction by bounding scales.
                //In the uniform case, scaleX=scaleY, so this sort of clamping will maintain aspect ratio
                //In the uniform fill case, we have the same result too.
                //In the fill case, note that we change aspect ratio, but that is okay
                switch (stretchDirection)
                {
                    case StretchDirection.UpOnly:
                        if (scaleX < 1.0) scaleX = 1.0;
                        if (scaleY < 1.0) scaleY = 1.0;
                        break;

                    case StretchDirection.DownOnly:
                        if (scaleX > 1.0) scaleX = 1.0;
                        if (scaleY > 1.0) scaleY = 1.0;
                        break;

                    case StretchDirection.Both:
                        break;

                    default:
                        break;
                }
            }
            //Return this as a size now
            return new Size(scaleX, scaleY);
        }
    }
}