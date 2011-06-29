using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Technobotts;
using Technobotts.Robotics;
using GHIElectronics.NETMF.FEZ;
using Technobotts.Geometry;
using Technobotts.Utilities;
using Technobotts.Hardware;

namespace Technobotts.Tests
{
	class IRRingTest
	{
		public static void Main()
		{
			FEZ_Pin.Digital[] pins = new FEZ_Pin.Digital[] {
				FEZ_Pin.Digital.Di36, FEZ_Pin.Digital.Di37, FEZ_Pin.Digital.Di38, FEZ_Pin.Digital.Di39,
				FEZ_Pin.Digital.Di40, FEZ_Pin.Digital.Di41, FEZ_Pin.Digital.Di42, FEZ_Pin.Digital.Di43,
				FEZ_Pin.Digital.Di44, FEZ_Pin.Digital.Di45, FEZ_Pin.Digital.Di46, FEZ_Pin.Digital.Di47,
				FEZ_Pin.Digital.Di48, FEZ_Pin.Digital.Di49, FEZ_Pin.Digital.Di50, FEZ_Pin.Digital.Di51
			};

			IIntensityDetector[] detectors = new TSOP1138[pins.Length];

			for (int i = 0; i < pins.Length; i++)
			{
				detectors[i] = new TSOP1138((Cpu.Pin)pins[i]);
			}

			IntensityDetectorArray sensors = IntensityDetectorArray.FromRadialSensors(detectors);
#if false
			String[] intensities = new String[5];
			while (true)
			{
				foreach (IRDetector d in detectors)
				{
					d.Recalculate();
				}

				for (int i = 0; i < detectors.Length; i++)
				{
					int intensity = detectors[i].Intensity;
					intensities[intensity] += i + ",";
				}

				for (int i = 0; i < intensities.Length; i++)
				{
					Debug.Print(i + ": " + intensities[i]);
					intensities[i] = "";
				}
				Debug.Print("");
			}
#endif
			LowPassVectorFilter filter = new LowPassVectorFilter(0.5, 0.06);
			while (true)
			{
				DateTime t1 = DateTime.UtcNow;
				Vector intensity = sensors.Get();
				DateTime t2 = DateTime.UtcNow;
				intensity = filter.apply(intensity);
				Debug.Print(intensity.ToString("f1"));
				Debug.Print((t2-t1).ToString());
			}
		}
	}
}
