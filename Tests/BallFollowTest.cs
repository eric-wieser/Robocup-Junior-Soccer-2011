using System;
using Math = System.Math;
using Microsoft.SPOT;
using Technobotts.Soccer;
using System.Threading;
using Technobotts.Utilities;
using Technobotts.Geometry;
namespace Technobotts.Tests
{
	class BallFollowTest
	{
		public static LowPassVectorFilter filter = new LowPassVectorFilter(0.1, 0.05);
		public static Range HeadingRange = new Range(-Math.PI, Math.PI);
		public static void Main()
		{
			Robot r = new Robot();
			while (!r.Button.Read()) ;
			while (r.Button.Read()) ;

			while (true)
			{
				r.SensorPoller.Poll();
				Vector raw = r.BallDetector.Get();
				Vector filtered = filter.apply(r.BallDetector.Get());

				double headingError = HeadingRange.Wrap(r.Compass.Angle);

				Debug.Print((headingError / Math.PI).ToString("f1"));

				if (r.LightGate.IsObstructed)
				{
					r.Drive.RotationPoint = Vector.J * 150;
					r.Drive.DriveVelocity = 0;

					r.Drive.TurnVelocity = 1;
				}
				else
				{
					r.Drive.RotationPoint = 0;

					r.Drive.DriveVelocity = 20 * filtered;
					r.Drive.TurnVelocity = 0.5 * headingError;
				}

				Thread.Sleep(10);
			}
		}
	}
}
