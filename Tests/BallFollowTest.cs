using System;
using Microsoft.SPOT;
using Technobotts.Soccer;

namespace Technobotts.Tests
{
	class BallFollowTest
	{
		public static void Main()
		{
			Robot r = new Robot();
			while (!r.Button.Read()) ;
			while (r.Button.Read()) ;
			while (true)
			{
				r.Drive.DriveVelocity = r.BallDetector.Get();
			}
		}
	}
}
