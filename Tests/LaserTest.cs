using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Threading;

namespace Technobotts.Tests
{
	class LaserTest
	{
		public static void Main()
		{
			int noBall = 350;
			int ball = 160;

			int threshold = (noBall + ball) / 2;
			AnalogIn photodiode = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An0);

			while (true)
			{
				int d = photodiode.Read();
				Debug.Print(d + ": "+ (d > threshold ? "No ball" : "Ball"));
				Thread.Sleep(50);
			}
		}
	}
}
