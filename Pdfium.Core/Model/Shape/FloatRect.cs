using System;
using System.Collections.Generic;
using System.Text;

namespace Davisol.Pdfium.Model.Shape
{
    public struct FloatRect : IEquatable<FloatRect>
    {
        public FloatRect(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public FloatRect(FloatPoint pos, FloatSize size)
        {
            Left = pos.X;
            Top = pos.Y;
            Right = pos.X + size.Width;
            Bottom = pos.Y + size.Height;
        }

        public FloatRect(FloatPoint topLeft, FloatPoint bottomRight)
        {
            Left = topLeft.X;
            Top = topLeft.Y;
            Right = bottomRight.X;
            Bottom = bottomRight.Y;
        }

        public float Left { get; }

        public float Top { get; }

        public float Right { get; }

        public float Bottom { get; }

        public float Width => Right - Left;

        public float Height => Bottom - Top;

        public FloatPoint TopLeft => new FloatPoint(Left, Top);

        public FloatPoint BottomRight => new FloatPoint(Right, Bottom);

        public FloatPoint Position => TopLeft;

        public FloatSize Size => new FloatSize(Width, Height);

        public override int GetHashCode()
        {
            return InternalExtensions.CombineHashCodes(Left.GetHashCode(), Top.GetHashCode(), Right.GetHashCode(), Bottom.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FloatRect other))
                return false;
            return Equals(this, other);
        }


        public bool Equals(FloatRect other)
        {
            return Equals(this, other);
        }


        public static bool Equals(FloatRect a, FloatRect b)
        {
            return a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom;
        }

        public static bool operator ==(FloatRect left, FloatRect right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FloatRect left, FloatRect right)
        {
            return !Equals(left, right);
        }

    }
}
