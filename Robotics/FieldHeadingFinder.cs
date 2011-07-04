using System;
using Microsoft.SPOT;
using Technobotts.Utilities;

namespace Technobotts.Robotics
{
	public class FieldHeadingFinder : IAngleFinder
	{
		public IAngleFinder AngleFinder {get; private set;}

		public double ZeroAngle { get; private set; }

		public FieldHeadingFinder(IAngleFinder angleFinder)
		{
			AngleFinder = angleFinder;
			RecordNorthDirection();
		}

		public void RecordNorthDirection()
		{
			double delta = 3 * System.Math.PI/180;	// get within 3 degrees.

			int i=0;
			double iVal ;
			do
			{
				iVal = AngleFinder.Angle;

				for (i = 0; i < 10; i++)
				{
					// wait for 10 iterations where angle differs by less than delta.
					if (MathEx.Abs(iVal - AngleFinder.Angle) > delta)
					{
						break;
					}
				}
			} while (i != 10);
			ZeroAngle =iVal;
		}

		public double Angle {
			get {
				double currentAngle = AngleFinder.Angle;
				return Range.Angle.Wrap(currentAngle - ZeroAngle);
			}
		}
	}
}
