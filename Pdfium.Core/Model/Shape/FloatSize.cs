using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium.Model.Shape
{
    public struct FloatSize : IEquatable<FloatSize>
    {
        public FloatSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width { get; }

        public float Height { get; }


        public override int GetHashCode()
        {
            return InternalExtensions.CombineHashCodes(Width.GetHashCode(), Height.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FloatSize other))
                return false;
            return Equals(this, other);
        }


        public bool Equals(FloatSize other)
        {
            return Equals(this, other);
        }


        public static bool Equals(FloatSize a, FloatSize b)
        {
            return a.Width == b.Width && a.Height == b.Height;
        }

        public static bool operator ==(FloatSize left, FloatSize right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FloatSize left, FloatSize right)
        {
            return !Equals(left, right);
        }

    }
}
