using System;
using System.Threading;
using Microsoft.SPOT;
using Technobotts.Robotics;
using Technobotts.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts
{
	class IRActuallyPluggedIn
	{
		public static void Main()
		{
			SensorPoller s = new SensorPoller();
			LED l = new LED(FEZ_Pin.Digital.Di34);
			l.State = true;
			while (true)
			{
				s.Poll();
				int active = s.BrokenIRSensorCount;

				Debug.Print("IR Sensors borked: " + active);
				Thread.Sleep(100);
			}
		}
	}
}
