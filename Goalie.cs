using System;
using Microsoft.SPOT;
using Technobotts.Utilities;
using Technobotts.Robotics;
using Technobotts.Geometry;

namespace Technobotts
{
	public class Goalie
	{
		public static void Main()
		{
			Soccer.Robot r = new Soccer.Robot();
			IRangeFinder BackUS = r.SensorPoller.USSensors[2];

			PIDController headingPid = new PIDController(PIDController.CoefficientsFromZieglerNicholsMethod(1.15, 0.87))
			//pid = new PIDController(1.15)
			{
				Input = new PIDController.InputFunction(
					() => BackUS.DistanceCM,
					new Range(0, 255)
				),
				Output = new PIDController.OutputFunction(
					(value) => r.Drive.DriveVelocity = Vector.J * -value
				),
				Continuous = true,
				SetPoint = 25
			};

			PIDController positionPid = new PIDController(0.5)
			//pid = new PIDController(1.15)
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
		}
	}
}
