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
	public class FollowBallAndClearWall : Strategy
	{
		public const double epsilon = 0.0001;
		public const double exponent = 1.5;
		public const double speedlimit = 400.0;
		public int seconds;
		const int states = 8;

		public DateTime dtNextChange;
		public Vector[] vectors;
		public int curState;
		public  FollowBallAndClearWall()
		{
			vectors = new Vector[states]
			{
				Vector.I * speedlimit, Vector.Zero, 
				Vector.J * speedlimit, Vector.Zero, 
				Vector.I * -speedlimit, Vector.Zero,
				Vector.J * -speedlimit, Vector.Zero
			};
			curState = 0;
			seconds = 2;
			dtNextChange = (DateTime.UtcNow).AddSeconds(seconds);
		}

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

				// robot.Drive.RotationPoint = Vector.I * 250;
				robot.Drive.DriveVelocity = 800 * Vector.J;
				robot.Drive.TurnVelocity = 0;
				//Thread.Sleep(500);
				//robot.Drive.TurnVelocity = 0;
				Thread.Sleep(500);

				robot.Drive.ControlEnabled = true;
			}
			else
			{
				Vector direction;
#if TRY_PLAYING_FOOTBALL
				direction = 200 * filter.apply(robot.BallDetector.Get());

                double angle = MathEx.ToDegrees(direction.Heading);

				// Debug.Print("Ball at " + angle);


				double absAngle = MathEx.Abs(angle / 180);
				double sign = MathEx.Sign(angle);
				double newAngle = (exponent * absAngle - Math.Pow(absAngle, exponent)) /
									(exponent - 1) * sign * 180;
				double x = 100.0 * MathExGHI.Tan(MathEx.ToRadians(newAngle));

				// Debug.Print("New Heading " + newAngle);
				if (DoubleEx.IsNaN(x))
				{
					direction.SetNewVector(300.0 * sign, 0.0);
				}
				else
				{
					direction.SetNewVector(x, (absAngle > 0.5 ? -1 : 1) * 300.0);
				}
#endif

				/*				
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
				// direction.SetNewVector(direction.X, direction.Y);

				IRangeFinder[] sensors = robot.Sensors.US;

#if WALK_PITCH
				// robot.Drive.ControlEnabled = false;
				direction = vectors[curState];
				DateTime dtNow = DateTime.UtcNow;
				if (dtNow > dtNextChange)
				{
					curState = (curState+1) % states;
					Debug.Print("CurState=" + curState);
					dtNextChange = dtNow.AddSeconds(seconds * (((curState & 1) == 1) ?1 : 5));
				}
#endif

#if DONT_BUMP_WALLS

				int cm;

				if (direction.Y > 0 && (cm = sensors[0].DistanceCM) < 30) // negative y moves away
					direction = new Vector(direction.X, (30 - cm) / 120.0 * direction.Y);
				else if (direction.Y < 0 && (cm = sensors[2].DistanceCM) < 30) // positive y moves away
					direction = new Vector(direction.X,  (30 - cm) / 120.0 * direction.Y);

				if (direction.X > 0 && (cm = sensors[1].DistanceCM) < 20) // negative x moves away
					direction = new Vector((20 - cm) / 80.0 * direction.X, direction.Y);
				else if (direction.X < 0 && (cm = sensors[3].DistanceCM) < 20) // positive x moves away.
					direction = new Vector((20 - cm) / 80.0 * direction.X,direction.Y);
#endif
				robot.Drive.RotationPoint = 0;
				// robot.Drive.TurnVelocity = 0;
				robot.Drive.DriveVelocity = direction;
			}
		}

		public static void Main() { new FollowBallAndClearWall().Execute(); }
	}
}
