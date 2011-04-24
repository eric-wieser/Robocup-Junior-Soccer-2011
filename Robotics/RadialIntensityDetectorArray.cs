using System;
using Technobotts.Geometry;

namespace Technobotts.Robotics
{
	class RadialIntensityDetectorArray<T> : IntensityDetectorArray<T>
	{
		public RadialIntensityDetectorArray(T[] sensors)
		{
			double count = sensors.Length;
			double angle = Math.PI * 2 / sensors.Length;
			Sensors = new OrientedIntensityDetector[sensors.Length];
			for (int i = 0; i < count; i++)
			{
				Sensors[i] = new OrientedIntensityDetector(
					sensors[i],
					Vector.FromPolarCoords(1, i * angle)
				);
			}
		}
	}
}
