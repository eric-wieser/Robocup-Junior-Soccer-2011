using System;
using Math = System.Math;
using Microsoft.SPOT;
using Technobotts.Soccer;
using System.Threading;
using Technobotts.Utilities;
using Technobotts.Geometry;
using Technobotts.Robotics;
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
					Vector direction = 20 * filtered;
					IRangeFinder[] sensors = r.SensorPoller.USSensors;

					if(direction.Y > 0 && sensors[0].DistanceCM < 30)
						direction = new Vector(direction.X, -5);
					else if (direction.Y < 0 && sensors[2].DistanceCM < 30)
						direction = new Vector(direction.X, 5);

					if (direction.X > 0 && sensors[1].DistanceCM < 20)
						direction = new Vector(-5, direction.Y);
					else if (direction.X < 0 && sensors[3].DistanceCM < 20)
						direction = new Vector(-5, direction.Y);

					r.Drive.RotationPoint = 0;

					r.Drive.DriveVelocity = direction;
					Thread.Sleep(10);
				}

				Debug.Print("" + r.Drive.TurnVelocity);
			}
		}
	}
}
