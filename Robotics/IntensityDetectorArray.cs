using System;
using Math = System.Math;
using Microsoft.SPOT;
using Technobotts.Geometry;
using GHIElectronics.NETMF.System;

namespace Technobotts.Robotics
{
	public class IntensityDetectorArray
	{
		public static IntensityDetectorArray FromRadialSensors(IIntensityDetector[] sensors)
		{
			double count = sensors.Length;
			double angle = Math.PI * 2 / sensors.Length;
			OrientedIntensityDetector[] preparedSensors = new OrientedIntensityDetector[sensors.Length];
			for (int i = 0; i < count; i++)
			{
				preparedSensors[i] = new OrientedIntensityDetector(
					sensors[i],
					Vector.FromPolarCoords(1, i * angle)
				);
			}
			return new IntensityDetectorArray(preparedSensors);
		}

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
			return sourceDirection.Length > 0 ? sourceDirection.Unit : 0;/// MathEx.Sqrt(sourceDirection.Length) : 0;
		}
	}
}
