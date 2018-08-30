using System;

#if NETFX_CORE
namespace HelixToolkit.UWP.Model.Scene2D
#else
namespace HelixToolkit.Wpf.SharpDX.Model.Scene2D
#endif
{
    static class DoubleUtil
    {
        internal const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }
    }
}
