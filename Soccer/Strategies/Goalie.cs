using System;
using Microsoft.SPOT;
using Technobotts.Utilities;
using Technobotts.Robotics;
using Technobotts.Geometry;
using Technobotts.Soccer;

namespace Technobotts.Strategies
{
	public class Goalie : Strategy
	{
		PIDController positionPid;
		IRangeFinder backUS;

		public Goalie()
		{
			backUS = robot.Sensors.US[2];
			positionPid = new PIDController(0.5)
			{
				Input = new PIDController.InputFunction(
					() => backUS.DistanceCM,
					Range.Angle
				),
				Output = new PIDController.OutputFunction(
					(value) => robot.Drive.DriveVelocity = new Vector(value, robot.Drive.DriveVelocity.X),
					Range.SignedAngle * 2
				),
				Continuous = true,
				SetPoint = 20
			};
		}
		public override void Activated()
		{
			
		}
		public override void ActivePeriodic()
		{

		}
		public override void Disabled()
		{

		}
	}
}
