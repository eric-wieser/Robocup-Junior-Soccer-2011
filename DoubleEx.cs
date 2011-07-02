using System;
using Microsoft.SPOT;

namespace Technobotts
{
	public struct DoubleEx
	{
		public const double NaN = -0.0d / 0.0d;
		public const double NegativeInfinity = -1.0f / 0.0f;
		public const double PositiveInfinity = 1.0f / 0.0f;

		public const long NANMASK = 0x7FF0000000000000;

		public unsafe static bool IsNaN(double value)        
		{
			long rep = *((long*)&value);
			return ((rep & NANMASK) == NANMASK);		
		} 

		public static bool IsInfinite(double x)
		{
			return x == NegativeInfinity || x == PositiveInfinity;
		}
	}
}
