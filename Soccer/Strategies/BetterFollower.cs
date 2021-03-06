#define DONT_BUMP_WALLS
#define TRY_PLAYING_FOOTBALL

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
	public class BetterFollower : Strategy
	{
		public int seconds;

		Distances min = new Distances
		{
			Top = 5,
			Bottom = 20,
			Left = 20,
			Right = 20
		};

		public LowPassVectorFilter filter = new LowPassVectorFilter(0.025);
		public override void Activated()
		{
			robot.Drive.ControlEnabled = true;
		}
		public override void ActivePeriodic()
		{
			Vector ballDirection = filter.apply(robot.BallDetector.Get());
			Vector direction;

			if (ballDirection.Y > 0 && robot.LightGate.IsObstructed)
			{
				direction = 800 * Vector.J;
			}
			else
			{
				direction = 800 * new Vector(ballDirection.X, ballDirection.Y - MathEx.Abs(ballDirection.X * 0.5));
			}






















































			Distances actual = robot.Sensors.USDistances;
			Distances delta = actual - min;

			double p = 20;

			if (direction.Y > 0 && delta.Top < 0)
				direction = new Vector(direction.X, p * delta.Top);
			else if (direction.Y < 0 && delta.Bottom < 0)
				direction = new Vector(direction.X, -p * delta.Bottom);
			
			if (direction.X > 0 && delta.Right < 0)
				direction = new Vector(p * delta.Right, direction.Y);
			else if (direction.X < 0 && delta.Left < 0)
				direction = new Vector(-p * delta.Left, direction.Y);
			
			robot.Drive.RotationPoint = 0;
			robot.Drive.DriveVelocity = direction;
		}

		public static void Main() { new BetterFollower().Execute(); }
	}
}
