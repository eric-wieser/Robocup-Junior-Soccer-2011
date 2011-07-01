using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace Technobotts
{
	class LaserTest
	{
		public static void Main()
		{
			Soccer.Robot.LEDGroup leds = new Technobotts.Soccer.Robot.LEDGroup(FEZ_Pin.Digital.An1, FEZ_Pin.Digital.An2, FEZ_Pin.Digital.An3, FEZ_Pin.Digital.An4);
			AnalogIn i = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An0);
			leds.State = true;
			while (true)
			{
				Debug.Print("Reading:"+i.Read());
			}
		}
	}
}
