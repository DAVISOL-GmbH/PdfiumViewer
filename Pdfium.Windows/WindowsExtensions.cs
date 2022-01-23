using Davisol.Pdfium.Model.Shape;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Davisol.Pdfium.Windows
{
    public static class WindowsExtensions
    {
        public static SizeF ToSizeF(this FloatSize size)
        {
            return new SizeF(size.Width, size.Height);
        }

        public static PointF ToPointF(this FloatPoint point)
        {
            return new PointF(point.X, point.Y);
        }

    }
}
