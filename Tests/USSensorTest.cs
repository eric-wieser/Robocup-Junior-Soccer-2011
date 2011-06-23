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
		public static void Main()
		{
			UltrasonicSensor us = new UltrasonicSensor((Cpu.Pin)FEZ_Pin.Digital.Di50);

			while (true)
			{
				Debug.Print("D: " + us.GetDistance());
				Thread.Sleep(100);
			}
		}
	}
}
