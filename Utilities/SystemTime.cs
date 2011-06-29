using System;
using Microsoft.SPOT;

namespace Technobotts.Utilities
{
	public static class SystemTime
	{
		private static DateTime lastCheck;

		public static long Ticks { get { return lastCheck.Ticks; } }
		public static double Seconds { get { return (double)Ticks / TimeSpan.TicksPerSecond; } }

		public static double SecondsSince(double then)
		{
			return Seconds - then;
		}
		public static long TicksSince(long then)
		{
			return Ticks - then;
		}

		public static void Update()
		{
			lastCheck = DateTime.UtcNow;
		}
	}
}
