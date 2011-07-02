using System;
using Microsoft.SPOT;
using Technobotts.Geometry;
using Technobotts.Utilities;

namespace Technobotts.Robotics
{
	public class USSensorAggregator
	{
		public static readonly Vector PitchSize = new Vector(122, 183); //CHECK!
		public static readonly double SensorSpacing = 15;
		public static readonly Range[] UnusableHeadings = new Range[4];

		static USSensorAggregator()
		{
			//Middle half of a right angle
			Range badRange = new Range(0.25, 0.75) / 4;

			//Measured in turns
			UnusableHeadings[0] = badRange;
			UnusableHeadings[1] = badRange + 0.25;
			UnusableHeadings[2] = badRange + 0.5;
			UnusableHeadings[3] = badRange + 0.75;
		}


		private IAngleFinder compass;
		

		public IRangeFinder Front;
		public IRangeFinder Right;
		public IRangeFinder Left;
		public IRangeFinder Back;

		public Vector PredictedPosition = null;

		private bool isValidlyOriented(double heading) {
			heading /= MathEx.TwoPi;
			foreach(Range r in UnusableHeadings)
				if(r.Contains(heading))
					return false;
			return true;
		}

		public void Update()
		{

		}
	}
}
