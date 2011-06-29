using System;
using System.Threading;
using Microsoft.SPOT;
using Technobotts.Robotics;

namespace Technobotts
{
	class IRActuallyPluggedIn
	{
		public static void Main()
		{
			SensorPoller s = new SensorPoller();

			while (true)
			{
				s.Poll();
				int active = s.ActiveIRSensorCount;

				Debug.Print("IR Sensors functional: " + active);
				Thread.Sleep(100);
			}
		}
	}
}
