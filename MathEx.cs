using System;
using Microsoft.SPOT;

namespace Technobotts
{
	class MathEx
	{
		public const double TwoPi = 2 * System.Math.PI;

		public static double Sign(double val) { return val > 0 ? 1 : val < 0 ? -1 : val; }
		public static double Abs(double val) { return val < 0 ? -val : val; }
		public static double ToDegrees(double radians) { return radians * 360 / TwoPi; }
		public static double ToRadians(double degrees) { return degrees * TwoPi / 360; }

	}
}
