using System;
using Technobotts.Geometry;

namespace Technobotts.Robotics
{
	class RadialIntensityDetectorArray : IntensityDetectorArray
	{
		private static OrientedIntensityDetector[] prepareSensors(IIntensityDetector[] sensors)
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
			return preparedSensors;
		}

		public RadialIntensityDetectorArray(IIntensityDetector[] sensors) : base(prepareSensors(sensors)) { }
	}
}
