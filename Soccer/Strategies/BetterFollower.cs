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
		public const double epsilon = 0.0001;
		public const double exponent = 1.5;
		public const double speedlimit = 400.0;
		public int seconds;

		public LowPassVectorFilter filter = new LowPassVectorFilter(0.025);
		public override void Activated()
		{
			robot.Drive.ControlEnabled = true;
		}
		public override void ActivePeriodic()
		{
			Vector direction;

			if (false || robot.LightGate.IsObstructed)
			{
				direction = 800 * Vector.J;
			}
			else
			{
				direction = 200 * filter.apply(robot.BallDetector.Get());
				direction = new Vector(direction.X, direction.Y - MathEx.Abs(direction.X * 0.5));


				//#if DONT_BUMP_WALLS
			}
			IRangeFinder[] sensors = robot.Sensors.US;

			const int frontDist = 30;
			const int leftDist = 20;
			const int backDist = 20;
			const int righDist = 20;

			int cm;

			if (direction.Y > 0 && (cm = sensors[0].DistanceCM) < frontDist) // negative y moves away
				direction = new Vector(direction.X, (cm - frontDist) / frontDist / 4 * direction.Y);
			else if (direction.Y < 0 && (cm = sensors[2].DistanceCM) < backDist) // positive y moves away
				direction = new Vector(direction.X, (cm - backDist) / backDist * direction.Y);

			if (direction.X > 0 && (cm = sensors[1].DistanceCM) < 20) // negative x moves away
				direction = new Vector((cm - 20) / 80.0 * direction.X, direction.Y);
			else if (direction.X < 0 && (cm = sensors[3].DistanceCM) < 20) // positive x moves away.
				direction = new Vector((cm - 20) / 80.0 * direction.X, direction.Y);
			//#endif

			robot.Drive.RotationPoint = 0;
			robot.Drive.DriveVelocity = direction;
		}

		public static void Main() { new BetterFollower().Execute(); }
	}
}
