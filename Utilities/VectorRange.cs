using System;
using Microsoft.SPOT;
using Technobotts.Geometry;

namespace Technobotts.Utilities
{
	public class VectorRange
	{
		public Range X {get; private set; }
		public Range Y {get; private set; }

		public VectorRange(Vector min, Vector max)
		{
			
			X = max.X > min.X ? new Range(min.X, max.X) : new Range(max.X, min.X);
			Y = max.Y > min.Y ? new Range(min.Y, max.Y) : new Range(max.Y, min.Y);
		}

		public VectorRange(Range X, Range Y)
		{
			this.X = X;
			this.Y = Y;
		}
		public VectorRange(double tolerance) : this(new Vector(-tolerance, -tolerance), new Vector(tolerance,tolerance)) { }

		public bool Contains(Vector v, bool inclusive = true)
		{
			return X.Contains(v.X, inclusive) && Y.Contains(v.Y, inclusive);
		}

		public Vector Clip(Vector v)
		{
			return new Vector(X.Clip(v.X), Y.Clip(v.Y));
		}

		public Vector Wrap(Vector v)
		{
			return new Vector(X.Wrap(v.X), Y.Wrap(v.Y));
		}

		public bool IsFinite()
		{
			return X.IsFinite() && Y.IsFinite();
		}

		public static VectorRange operator +(VectorRange r, Vector v) { return new VectorRange(r.X + v.X, r.Y + v.Y); }
		public static VectorRange operator -(VectorRange r, Vector v) { return new VectorRange(r.X - v.X, r.Y - v.Y); }
	}
}

