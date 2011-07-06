using System;
using MathExGHI = GHIElectronics.NETMF.System.MathEx;
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

		public static double Sin(double angle) { return MathExGHI.Sin(angle); }
		public static double Cos(double angle) { return MathExGHI.Cos(angle); }
		public static double Sqrt(double angle) { return MathExGHI.Sqrt(angle); }
        public static double Exp(double angle) { return MathExGHI.Exp(angle); }
        public static double Atan2(double x, double y) { return MathExGHI.Atan2(x, y); }

		public static double Max(double a, double b) { return a > b ? a : b; }

	}
}
