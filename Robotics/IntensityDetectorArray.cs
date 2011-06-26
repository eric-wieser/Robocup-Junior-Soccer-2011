using System;
using Microsoft.SPOT;
using Technobotts.Geometry;
using GHIElectronics.NETMF.System;

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
			public Vector Intensity
			{
				get
				{
					int intensity = (int)Detector.Intensity;
					return intensity * intensity * Orientation;
				}
			}
			public void Recalculate()
			{
				Detector.Recalculate();
			}
		}

		protected IntensityDetectorArray() { }

		public IntensityDetectorArray(OrientedIntensityDetector[] sensors)
		{
			Sensors = sensors;
		}

		public OrientedIntensityDetector[] Sensors { get; protected set; }

		public Vector Get()
		{
			Vector sourceDirection = 0;
			foreach (OrientedIntensityDetector sensor in Sensors)
			{
				sensor.Recalculate();
			}
			foreach (OrientedIntensityDetector sensor in Sensors)
			{
				sourceDirection += sensor.Intensity;
			}
			return sourceDirection == (Vector) 0 ?  0 : sourceDirection / MathEx.Sqrt(sourceDirection.Length);
		}
	}
}
