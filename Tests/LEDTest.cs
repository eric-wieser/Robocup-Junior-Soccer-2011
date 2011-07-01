using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace Technobotts.Tests
{
	class LEDTest
	{
		public static void Main()
		{
			Soccer.Robot.LEDGroup leds = new Technobotts.Soccer.Robot.LEDGroup(FEZ_Pin.Digital.An1, FEZ_Pin.Digital.An2, FEZ_Pin.Digital.An3, FEZ_Pin.Digital.An4);

			leds.Purple.StartBlinking(1000, 0.5);
			leds.Green.StartBlinking(500, 0.5);

			Thread.Sleep(2000);

			leds.Purple.State = false;
			leds.Orange.StartBlinking(500, 0.9);

			Thread.Sleep(2000);

			leds.Orange.StartBlinking(500, 0.1);

			Thread.Sleep(Timeout.Infinite);
		}
	}
}
