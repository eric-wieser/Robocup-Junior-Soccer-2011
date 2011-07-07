using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.System;
using GHIElectronics.NETMF.Hardware;
using Technobotts.Robotics;

namespace Technobotts.Tests
{
	class MotorTest
	{

		public static void Main()
		{
			Soccer.Robot r = new Soccer.Robot();
			double i = 0;
			double step = System.Math.PI / 16;

			r.Button.WaitForPress();
			r.MotorA.Speed = 1;
			Thread.Sleep(1000);
			r.MotorB.Speed = 1;
			Thread.Sleep(1000);
			r.MotorC.Speed = 1;
			Thread.Sleep(1000);
			r.Dispose();
		}
	}
}
