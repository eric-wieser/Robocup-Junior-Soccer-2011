using System;
using Microsoft.SPOT;
using Technobotts.Utilities;

namespace Technobotts.Robotics
{
	class FieldHeadingFinder : IAngleFinder
	{
		private IAngleFinder _angleFinder;

		public double ZeroAngle {get; private set };

		public FieldHeadingFinder(IAngleFinder angleFinder)
		{
			_angleFinder = angleFinder;
			RecordNorthDirection();
		}

		public void RecordNorthDirection()
		{
			ZeroAngle = _angleFinder.Angle;
		}

		public double Angle {
			get {
				return Range.Angle.Wrap(_angleFinder.Angle - ZeroAngle);
			}
		}
	}
}
