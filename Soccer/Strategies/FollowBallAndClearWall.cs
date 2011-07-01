using System;
using Math = System.Math;
using Microsoft.SPOT;
using Technobotts.Soccer;
using System.Threading;
using Technobotts.Utilities;
using Technobotts.Geometry;
using Technobotts.Robotics;

namespace Technobotts.Soccer.Strategies
{
	public class FollowBallAndClearWall : Strategy
	{
		public LowPassVectorFilter filter = new LowPassVectorFilter(0.1, 0.05);
		public override void ActiveInit()
		{
			robot.Drive.ControlEnabled = true;
		}
		public override void ActivePeriodic()
		{
			robot.SensorPoller.Poll();

			if (robot.LightGate.IsObstructed)
			{
				robot.Drive.ControlEnabled = false;

				robot.Drive.RotationPoint = Vector.I * 250;
				robot.Drive.DriveVelocity = 0;
				robot.Drive.TurnVelocity = -3;
				Thread.Sleep(500);
				robot.Drive.TurnVelocity = 0;
				Thread.Sleep(500);

				robot.Drive.ControlEnabled = true;
			}
			else
			{
				Vector direction = 20 * filter.apply(robot.BallDetector.Get());
				IRangeFinder[] sensors = robot.SensorPoller.USSensors;

				if (direction.Y > 0 && sensors[0].DistanceCM < 30)
					direction = new Vector(direction.X, -25);
				else if (direction.Y < 0 && sensors[2].DistanceCM < 30)
					direction = new Vector(direction.X, 25);

				if (direction.X > 0 && sensors[1].DistanceCM < 20)
					direction = new Vector(-25, direction.Y);
				else if (direction.X < 0 && sensors[3].DistanceCM < 20)
					direction = new Vector(25, direction.Y);

				robot.Drive.RotationPoint = 0;

				robot.Drive.DriveVelocity = direction;
				Thread.Sleep(10);
			}
		}
		public override void ActiveCleanUp()
		{
			robot.Drive.ControlEnabled = false;
		}

		public static void Main() { new FollowBallAndClearWall().Execute(); }
	}
}
