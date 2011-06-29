using System;
using Math = System.Math;
using Microsoft.SPOT;
using Technobotts.Soccer;
using System.Threading;
using Technobotts.Utilities;
using Technobotts.Geometry;
namespace Technobotts.Tests
{
	class BallFollowPIDTest
	{
		public static LowPassVectorFilter filter = new LowPassVectorFilter(0.1, 0.05);
		public static Range HeadingRange = new Range(-Math.PI, Math.PI);
		public static void Main()
		{
			Robot r = new Robot();

			PIDController pid = new PIDController(PIDController.CoefficientsFromZieglerNicholsMethod(1.15, 0.87))
			{
				Input = new PIDController.InputFunction(
					() => r.Compass.Angle,
					Range.Angle
				),
				Output = new PIDController.OutputFunction(
					(value) => r.Drive.TurnVelocity = -value,
					Range.SignedAngle * 2
				),
				Continuous = true,
				SetPoint = r.Compass.Angle
			};

			r.Button.WaitForPress();
			pid.Enabled = true;

			while (true)
			{
				r.SensorPoller.Poll();
				Vector raw = r.BallDetector.Get();
				Vector filtered = filter.apply(r.BallDetector.Get());

				if (r.LightGate.IsObstructed)
				{
					pid.Enabled = false;
					r.Drive.RotationPoint = Vector.I * 250;
					r.Drive.DriveVelocity = 0;
					r.Drive.TurnVelocity = -3;
					Thread.Sleep(500);
					r.Drive.TurnVelocity = 0;
					Thread.Sleep(500);
					pid.Enabled = true;
				}
				else
				{
					r.Drive.RotationPoint = 0;

					r.Drive.DriveVelocity = 20 * filtered;
					Thread.Sleep(10);
				}
			}
		}
	}
}
