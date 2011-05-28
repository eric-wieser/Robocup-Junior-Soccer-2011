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
			AnalogIn i = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An0);
			while (true)
			{
				Debug.Print("Reading:"+i.Read());
			}
		}
	}
}
