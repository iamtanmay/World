using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldSimCS
{
    public static class FixedPoint
    {
        public const int FractionBits = 16;
        public const long One = 1L << FractionBits;

        public static long FromInt(int value) => (long)value << FractionBits;
        public static long FromFloat(float value) => (long)(value * One);
        public static long FromFloat(double value) => (long)(value * One);
        public static int ToInt(long value) => (int)(value >> FractionBits);
        public static float ToFloat(long value) => value / (float)One;

        public static long Add(long a, long b) => a + b;
        public static long Subtract(long a, long b) => a - b;
        public static long Multiply(long a, long b) => (a * b) >> FractionBits;
        public static long Divide(long a, long b) => (a << FractionBits) / b;
		public static long Sqrt(long x)
		{
			if (x == 0) return 0;
			long left = 0;
			long right = x;
			while (left < right)
			{
				long mid = (left + right + 1) / 2;
				if (mid * mid <= x)
				{
					left = mid;
				}
				else
				{
					right = mid - 1;
				}
			}
			return left;
		}
    }

    public struct Vector2
    {
        public long X;
        public long Y;

        public Vector2(long x, long y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 Zero = new Vector2(0, 0);
        public static Vector2 UnitX = new Vector2(FixedPoint.One, 0);
        public static Vector2 UnitY = new Vector2(0, FixedPoint.One);

        public Vector2 Add(Vector2 other) => new Vector2(FixedPoint.Add(X, other.X), FixedPoint.Add(Y, other.Y));
        public Vector2 Subtract(Vector2 other) => new Vector2(FixedPoint.Subtract(X, other.X), FixedPoint.Subtract(Y, other.Y));
        public Vector2 Multiply(long scalar) => new Vector2(FixedPoint.Multiply(X, scalar), FixedPoint.Multiply(Y, scalar));
    }

	public struct Vector3
	{
		public long X;
		public long Y;
		public long Z;

		public Vector3(long x, long y, long z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static Vector3 Zero = new Vector3(0, 0, 0);
		public static Vector3 UnitX = new Vector3(FixedPoint.One, 0, 0);
		public static Vector3 UnitY = new Vector3(0, FixedPoint.One, 0);
		public static Vector3 UnitZ = new Vector3(0, 0, FixedPoint.One);

		public Vector3 Add(Vector3 other) => new Vector3(FixedPoint.Add(X, other.X), FixedPoint.Add(Y, other.Y), FixedPoint.Add(Z, other.Z));
		public Vector3 Subtract(Vector3 other) => new Vector3(FixedPoint.Subtract(X, other.X), FixedPoint.Subtract(Y, other.Y), FixedPoint.Subtract(Z, other.Z));
		public Vector3 Multiply(long scalar) => new Vector3(FixedPoint.Multiply(X, scalar), FixedPoint.Multiply(Y, scalar), FixedPoint.Multiply(Z, scalar));
	}
}