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

			while (!r.Button.Read()) ;
			while (r.Button.Read()) ;

			while (true)
			{
				r.MotorA.Speed = MathEx.Sin(i);
				i += step;
				Thread.Sleep(50);
			}
		}
	}
}
