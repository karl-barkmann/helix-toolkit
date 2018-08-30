/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene2D
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    public enum HorizontalAlignment
    {
        Left, Right, Center, Stretch
    }

    public enum VerticalAlignment
    {
        Top, Bottom, Center, Stretch
    }

    public enum Visibility
    {
        Visible, Collapsed, Hidden
    }

    public enum Orientation
    {
        Horizontal, Vertical
    }

    public enum Stretch
    {
        /// <summary>
        ///     None - Preserve original size
        /// </summary>
        None = 0,

        /// <summary>
        ///     Fill - Aspect ratio is not preserved, source rect fills destination rect.
        /// </summary>
        Fill = 1,

        /// <summary>
        ///     Uniform - Aspect ratio is preserved, Source rect is uniformly scaled as large as 
        ///     possible such that both width and height fit within destination rect.  This will 
        ///     not cause source clipping, but it may result in unfilled areas of the destination 
        ///     rect, if the aspect ratio of source and destination are different.
        /// </summary>
        Uniform = 2,

        /// <summary>
        ///     UniformToFill - Aspect ratio is preserved, Source rect is uniformly scaled as small 
        ///     as possible such that the entire destination rect is filled.  This can cause source 
        ///     clipping, if the aspect ratio of source and destination are different.
        /// </summary>
        UniformToFill = 3,
    }

    public enum StretchDirection
    {
        /// <summary>
        /// Only scales the content upwards when the content is smaller than the Viewbox.
        /// If the content is larger, no scaling downwards is done.
        /// </summary>
        UpOnly,

        /// <summary>
        /// Only scales the content downwards when the content is larger than the Viewbox.
        /// If the content is smaller, no scaling upwards is done.
        /// </summary>
        DownOnly,

        /// <summary>
        /// Always stretches to fit the Viewbox according to the stretch mode.
        /// </summary>
        Both
    }

    public struct Thickness : IEquatable<Thickness>
    {
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;

        public Thickness(float size)
        {
            Left = size;
            Right = size;
            Top = size;
            Bottom = size;
        }

        public Thickness(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public bool Equals(Thickness other)
        {
            return this.Left == other.Left && this.Right == other.Right && this.Top == other.Top && this.Bottom == other.Bottom;
        }

        public static implicit operator Vector4(Thickness t)
        {
            return new Vector4(t.Left, t.Top, t.Right, t.Bottom);
        }
    }
}
