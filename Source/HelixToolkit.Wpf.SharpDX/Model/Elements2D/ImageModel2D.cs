using HelixToolkit.Wpf.SharpDX.Core2D;
using HelixToolkit.Wpf.SharpDX.Model.Scene2D;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Stretch = System.Windows.Media.Stretch;
using StretchDirection = System.Windows.Controls.StretchDirection;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element2D" />
    public class ImageModel2D : Element2D
    {
        /// <summary>
        /// DependencyProperty for Stretch property.
        /// </summary>
        /// <seealso cref="Viewbox.Stretch" />
        public static readonly DependencyProperty StretchProperty =
                Viewbox.StretchProperty.AddOwner(typeof(ImageModel2D),
                    new PropertyMetadata(Stretch.Uniform, StretchPropertyChanged));

        private static void StretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (((ImageModel2D)d).SceneNode as ImageNode2D).Stretch = (Model.Scene2D.Stretch)e.NewValue.GetHashCode();
        }

        /// <summary>
        /// DependencyProperty for StretchDirection property.
        /// </summary>
        /// <seealso cref="Viewbox.Stretch" />
        public static readonly DependencyProperty StretchDirectionProperty =
                Viewbox.StretchDirectionProperty.AddOwner(typeof(ImageModel2D), 
                    new PropertyMetadata(StretchDirection.Both, StretchDirectionPropertyChanged));

        private static void StretchDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (((ImageModel2D)d).SceneNode as ImageNode2D).StretchDirection = (Model.Scene2D.StretchDirection)e.NewValue.GetHashCode();
        }

        /// <summary>
        /// Gets/Sets the Stretch on this Image.
        /// The Stretch property determines how large the Image will be drawn.
        /// </summary>
        /// <seealso cref="Viewbox.StretchProperty" />
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        /// <summary>
        /// Gets/Sets the stretch direction of the Viewbox, which determines the restrictions on
        /// scaling that are applied to the content inside the Viewbox.  For instance, this property
        /// can be used to prevent the content from being smaller than its native size or larger than
        /// its native size.
        /// </summary>
        /// <seealso cref="Viewbox.StretchDirectionProperty" />
        public StretchDirection StretchDirection
        {
            get { return (StretchDirection)GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the image stream.
        /// </summary>
        /// <value>
        /// The image stream.
        /// </value>
        public Stream ImageStream
        {
            get { return (Stream)GetValue(ImageStreamProperty); }
            set { SetValue(ImageStreamProperty, value); }
        }

        /// <summary>
        /// The image stream property
        /// </summary>
        public static readonly DependencyProperty ImageStreamProperty =
            DependencyProperty.Register("ImageStream", typeof(Stream), typeof(ImageModel2D), new PropertyMetadata(null,
                (d,e)=> 
                {
                    ((d as Element2DCore).SceneNode as ImageNode2D).ImageStream = e.NewValue as Stream;
                }));


        /// <summary>
        /// Gets or sets the opacity.
        /// </summary>
        /// <value>
        /// The opacity.
        /// </value>
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        /// <summary>
        /// The opacity property
        /// </summary>
        public static readonly DependencyProperty OpacityProperty =
            DependencyProperty.Register("Opacity", typeof(double), typeof(ImageModel2D), new PropertyMetadata(1.0, (d,e)=> 
            {
                ((d as Element2DCore).SceneNode as ImageNode2D).Opacity = (float)(double)e.NewValue;
            }));


        protected override SceneNode2D OnCreateSceneNode()
        {
            return new ImageNode2D();
        }
    }
}
