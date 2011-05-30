using System;
using Microsoft.SPOT;

namespace Technobotts
{
	struct Double
	{
		public const double NaN = 0.0f / 0.0f;
		public const double NegativeInfinity = -1.0f / 0.0f;
		public const double PositiveInfinity = 1.0f / 0.0f;

		public bool isNaN(double x)
		{
			return x != x;
		}

		public bool isInfinity(double x)
		{
			return x == NegativeInfinity || x == PositiveInfinity;
		}
	}
}
