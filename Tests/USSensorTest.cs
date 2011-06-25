//#define Multi

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Technobotts.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Tests
{
	class USSensorTest
	{
		public static string ArrayToString(double[] array)
		{
			string data = "[";
			string sep = "";
			foreach (double d in array)
			{
				data += sep + (int)d;
				sep = ",\t";
			}
			data += "]";
			return data;
		}

		public static void Main()
		{
			UltrasonicSensor right = new UltrasonicSensor((Cpu.Pin)FEZ_Pin.Digital.Di49) { Unit = DistanceUnits.cm };
			UltrasonicSensor back = new UltrasonicSensor((Cpu.Pin)FEZ_Pin.Digital.Di50) { Unit = DistanceUnits.cm };
			UltrasonicSensor left = new UltrasonicSensor((Cpu.Pin)FEZ_Pin.Digital.Di51) { Unit = DistanceUnits.cm };
#if Multi
			SyncedUltrasonicSensors sensors = new SyncedUltrasonicSensors(right, back, left);
			while (true)
			{
				sensors.Update();

				Debug.Print(ArrayToString(sensors.Readings));
				Thread.Sleep(100);
			}
#else
			while (true)
			{
				Debug.Print("D\t" + left.GetDistance());
				Thread.Sleep(50);
			}
#endif
		}
	}
}
