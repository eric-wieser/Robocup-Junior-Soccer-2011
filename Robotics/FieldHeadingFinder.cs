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
			ZeroAngle = AngleFinder.Angle;
		}

		public double Angle {
			get {
				return Range.Angle.Wrap(AngleFinder.Angle - ZeroAngle);
			}
		}
	}
}
