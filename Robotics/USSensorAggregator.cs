using System;
using Microsoft.SPOT;
using Technobotts.Geometry;
using Technobotts.Utilities;

namespace Technobotts.Robotics
{
	public class USSensorAggregator
	{
		public static readonly Vector PitchSize = new Vector(122, 183);
		public static readonly double SensorSpacing = 15;
		public static readonly Range[] HeadingRanges = new Range[5];
		public static Range Tolerance = new Range(10);

		static USSensorAggregator()
		{
			//Middle half of a right angle
			Range range = new Range(0.0625);

			//Measured in turns
			HeadingRanges[0] = range;
			HeadingRanges[1] = range + 0.25;
			HeadingRanges[2] = range + 0.5;
			HeadingRanges[3] = range + 0.75;
			HeadingRanges[4] = range;
		}


		private IAngleFinder compass;


		public IRangeFinder Front;
		public IRangeFinder Right;
		public IRangeFinder Left;
		public IRangeFinder Back;

		public Vector PredictedPosition = null;

		private enum Orientation { North, East, South, West, Invalid }

		private class Distances {
			public int North, East, South, West;

			public Distances(int North, int East, int South, int West)
			{
				this.North = North;
				this.East = East;
				this.South = South;
				this.West = West;
			}

			public void IncrementBy(int x) {
				North += x;
				East += x;
				South += x;
				West += x;
			}

			public Vector ApproximatePosition
			{
				get
				{
					return new Vector(MathEx.Max(West, PitchSize.X - East), MathEx.Max(South, PitchSize.Y - North));
				}
			}
			public double ApproximateWidth { get { return West + East; } }
			public double ApproximateHeight { get { return North + South; } }

			public Distances ChangeFrom(Distances old)
			{
				return new Distances(
					System.Math.Abs(North - old.North),
					System.Math.Abs(East - old.East),
					System.Math.Abs(South - old.South),
					System.Math.Abs(West - old.West)
				);
			}
		}

		Distances lastDistances = null;

		public Vector GetPosition()
		{
			double heading = compass.Angle;
			heading /= MathEx.TwoPi;

			Orientation o = Orientation.Invalid;

			if (HeadingRanges[0].Contains(heading) || HeadingRanges[5].Contains(heading))
				o = Orientation.North;
			else if (HeadingRanges[1].Contains(heading))
				o = Orientation.East;
			else if (HeadingRanges[2].Contains(heading))
				o = Orientation.South;
			else if (HeadingRanges[3].Contains(heading))
				o = Orientation.West;

			if (o != Orientation.Invalid)
			{
				Distances d;

				if (o == Orientation.North)
					d = new Distances(Front.DistanceCM, Right.DistanceCM, Back.DistanceCM, Left.DistanceCM);
				else if (o == Orientation.East)
					d = new Distances(Left.DistanceCM, Front.DistanceCM, Right.DistanceCM, Back.DistanceCM);
				else if (o == Orientation.South)
					d = new Distances(Back.DistanceCM, Left.DistanceCM, Front.DistanceCM, Right.DistanceCM);
				else
					d = new Distances(Right.DistanceCM, Back.DistanceCM, Left.DistanceCM, Front.DistanceCM);

				int robotSize = (int)(SensorSpacing / 2 * MathEx.Cos(heading * MathEx.TwoPi));

				d.IncrementBy(robotSize);

				Vector pos = d.ApproximatePosition;

				if(lastDistances != null) {
					Distances changes = d.ChangeFrom(lastDistances);

					if (!Tolerance.Contains(d.ApproximateWidth - PitchSize.X))
					{
						if(changes.West > changes.East)
							pos = new Vector(PitchSize.X - d.East, pos.Y);
						else
							pos = new Vector(d.West, pos.Y);
					}
					if (!Tolerance.Contains(d.ApproximateHeight - PitchSize.Y)) {
						if(changes.South > changes.North)
							pos = new Vector(pos.X, PitchSize.Y - d.North);
						else
							pos = new Vector(pos.X, d.South);
					}
				}
				lastDistances = d;

				return pos;
			}
			return null;
		}
	}
}
