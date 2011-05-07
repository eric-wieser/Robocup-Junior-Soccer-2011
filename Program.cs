using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace Technobotts
{
	public class Program
	{
		public static void Main()
		{
			PinCapture sensor = new PinCapture((Cpu.Pin)FEZ_Pin.Digital.Di0, Port.ResistorMode.Disabled);
			InputPort button = new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di1, true, Port.ResistorMode.PullDown);

			uint[] buffer = new uint[32];

			while (true)
			{
				int count = sensor.Read(true, buffer, 0, buffer.Length, 500);
				double period = 0;
				double highTime = 0;
				double duty = 0;
				for (int i = 1; i < count; i += 2)
				{
					period += buffer[i - 1] + buffer[i];
					highTime += buffer[i - 1];
				}
				if (count != 0)
				{
					period = period * 2 / count;
					highTime = highTime * 2 / count;
					duty = highTime / period;
				}
				Debug.Print("High: " + highTime + ",\tPeriod: " + period + ",\tDuty: " + duty);
				Thread.Sleep(10);
			}
		}

	}
}
