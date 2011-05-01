using System;
using Microsoft.SPOT;
using Technobotts.Geometry;

namespace Technobotts.Robotics
{
	public class IntensityDetectorArray
	{
		public class OrientedIntensityDetector
		{
			public OrientedIntensityDetector(IIntensityDetector detector, Vector orientation)
			{
				Detector = detector;
				Orientation = orientation;
			}

			public IIntensityDetector Detector;
			public Vector Orientation;
			public Vector Intensity { get { return Detector.Intensity * Orientation; } }
		}

		public IntensityDetectorArray() { }

		public IntensityDetectorArray(OrientedIntensityDetector[] sensors)
		{
			Sensors = sensors;
		}

		public OrientedIntensityDetector[] Sensors { get; protected set; }

		public Vector get()
		{
			Vector sourceDirection = 0;
			foreach (OrientedIntensityDetector sensor in Sensors)
			{
				sourceDirection += sensor.Intensity;
			}
			return sourceDirection;
		}
	}
}
