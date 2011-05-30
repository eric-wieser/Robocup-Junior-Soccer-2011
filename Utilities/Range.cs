using System;
using Microsoft.SPOT;

namespace Technobotts.Utilities
{
	public class Range
	{
		public double Max { get; private set; }
		public double Min { get; private set; }
		public double Span { get { return Max - Min; } }

		public Range(double min, double max)
		{
			if (min > max)
				throw new ArgumentOutOfRangeException(
					"max",
					"Lower bound must be lesser than upper bound!");
			Min = min;
			Max = max;
		}

		public Range(double tolerance) : this(-tolerance, tolerance) { }
		public Range() : this(DoubleEx.NegativeInfinity, DoubleEx.PositiveInfinity) { }


		public bool Contains(double d, bool inclusive = true)
		{
			if (inclusive)
				return d >= Min && d <= Max;
			else
				return d > Min && d < Max;

		}

		public double Clip(double d)
		{
			if (d < Min)
				d = Min;
			else if (d > Max)
				d = Max;
			return d;
		}

		public double Wrap(double d)
		{
			while (d >= Max)
				d -= Span;
			while (d < Min)
				d += Span;
			return d;
		}

		public bool IsFinite()
		{
			return Min != DoubleEx.NegativeInfinity && Max != DoubleEx.PositiveInfinity;
		}
	}
}
