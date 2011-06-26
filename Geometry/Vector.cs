using System;
using GHIElectronics.NETMF.System;

namespace Technobotts.Geometry
{
	public class Vector
	{
		public static readonly Vector Zero = new Vector(0, 0);
		public static readonly Vector I = new Vector(1, 0);
		public static readonly Vector J = new Vector(0, 1);
		private static readonly Vector NaN = new Vector(DoubleEx.NaN, DoubleEx.NaN);

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
			return k == 0 ? NaN : new Vector(v.X / k, v.Y / k);
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

		public static bool operator ==(Vector a, Vector b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return a.X == b.X && a.Y == b.Y;
		}

		public static bool operator !=(Vector a, Vector b)
		{
			return !(a == b);
		}

		public Vector Perpendicular
		{
			get { return new Vector(-Y, X); }
		}

		public double Length
		{
			get { return MathEx.Sqrt(X * X + Y * Y); }
		}

		public Vector Unit
		{
			get { return this / this.Length; }
		}

		public double Dot(Vector that)
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
			return v.Length;
		}

		public override string ToString()
		{
			return ToString("");
		}
		public string ToString(string format)
		{
			return (X < 0 ? "(": "( ") + X.ToString(format) + (Y < 0 ? ",": ", ") + Y.ToString(format) + ")";
		}

		public override bool Equals(Object obj)
		{
			return obj as Vector == this;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() << 16 + Y.GetHashCode();
		}
	}
}