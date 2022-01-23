using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium.Model.Shape
{
    public struct FloatPoint : IEquatable<FloatPoint>
    {
        public FloatPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }

        public float Y { get; }

        public override int GetHashCode()
        {
            return InternalExtensions.CombineHashCodes(X.GetHashCode(), Y.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FloatPoint other))
                return false;
            return Equals(this, other);
        }


        public bool Equals(FloatPoint other)
        {
            return Equals(this, other);
        }


        public static bool Equals(FloatPoint a, FloatPoint b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator ==(FloatPoint left, FloatPoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FloatPoint left, FloatPoint right)
        {
            return !Equals(left, right);
        }

    }
}
