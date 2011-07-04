using System;
using Math = System.Math;
using Microsoft.SPOT;
using Technobotts.Soccer;
using System.Threading;
using Technobotts.Utilities;
using Technobotts.Geometry;
using Technobotts.Robotics;
using MathExGHI = GHIElectronics.NETMF.System.MathEx;

namespace Technobotts.Soccer.Strategies
{
	public class FollowBallAndClearWall : Strategy
	{
		public const double epsilon = 0.0001;

		public LowPassVectorFilter filter = new LowPassVectorFilter(0.1);
		public override void Activated()
		{
			robot.Drive.ControlEnabled = true;
		}
		public override void ActivePeriodic()
		{
			if (robot.LightGate.IsObstructed)
			{
				robot.Drive.ControlEnabled = false;

				robot.Drive.RotationPoint = Vector.I * 250;
				robot.Drive.DriveVelocity = 800;
				robot.Drive.TurnVelocity = 0;
				Thread.Sleep(500);
				robot.Drive.TurnVelocity = 0;
				Thread.Sleep(500);

				robot.Drive.ControlEnabled = true;
			}
 
			else
			{
				Vector direction = 20 * filter.apply(robot.BallDetector.Get());
/*				double angle = MathExGHI.Atan2(direction.X, direction.Y);
				Debug.Print("Ball at " + angle);
				double sign = Technobotts.MathEx.Sign(angle);
				double newAngle = Math.PI * 0.2 * sign + angle * 0.8;
				double x= 100.0 * MathExGHI.Tan(newAngle);
				Debug.Print("New Heading " + newAngle);
				if (DoubleEx.IsNaN(x))
				{
					direction.SetNewVector(Math.PI/200.0 * sign, 0.0);
				}
				else
				{
					direction.SetNewVector(x, 100.0);
				}
				
*/
				direction.SetNewVector(direction.X, direction.Y);

				IRangeFinder[] sensors = robot.Sensors.US;

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
			}
		}

		public static void Main() { new FollowBallAndClearWall().Execute(); }
	}
}
