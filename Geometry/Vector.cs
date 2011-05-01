using System;
using GHIElectronics.NETMF.System;

namespace Technobotts.Geometry
{
	public class Vector
	{
		public static readonly Vector Zero = new Vector(0, 0);
		public static readonly Vector I = new Vector(1, 0);
		public static readonly Vector J = new Vector(0, 1);

		public readonly double X;
		public readonly double Y;

		public Vector(double x = 0, double y = 0)
		{
			X = x;
			Y = y;
		}

		public static Vector operator *(Vector v, double k)
		{
			return new Vector(v.X * k, v.Y * k);
		}

		public static Vector operator *(double k, Vector v)
		{
			return v * k;
		}

		public static Vector operator /(Vector v, double k)
		{
			return new Vector(v.X / k, v.Y / k);
		}

		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.X + b.X, a.Y + b.Y);
		}

		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.X - b.X, a.Y - b.Y);
		}

		public static Vector operator -(Vector v)
		{
			return new Vector(-v.X, -v.Y);
		}

		public double length()
		{
			return MathEx.Sqrt(X * X + Y * Y);
		}

		public Vector unit()
		{
			return this / this.length();
		}

		public double dot(Vector that)
		{
			return X * that.X + Y * that.Y;
		}
		public static Vector FromPolarCoords(double r, double theta)
		{
			return new Vector(r * MathEx.Sin(theta), r * MathEx.Cos(theta));
		}
		public static implicit operator Vector(double k)
		{
			return new Vector(k, k);
		}

		public static explicit operator double(Vector v)
		{
			return v.length();
		}

		public override string ToString()
		{
			return "(" + X + ", " + Y + ")";
		}

		public override bool Equals(Object obj)
		{
			Vector that = obj as Vector;
			return that != null && X == that.X && Y == that.Y;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() << 16 + Y.GetHashCode();
		}
	}
}