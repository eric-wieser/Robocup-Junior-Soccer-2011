using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Technobotts.Geometry;
using Technobotts.Robotics;
using Technobotts.Robotics.Navigation;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;
using Technobotts.Soccer;

namespace Technobotts.Tests
{
	class DriveTest
	{
		public static void Main()
		{
			int angle = 0;

			Robot r = new Robot();
			r.Button.WaitForPress();

			while (true)
			{
				r.Drive.DriveVelocity = Vector.FromPolarCoords(2, System.Math.PI * angle / 3);
				Thread.Sleep(750);
				/*if (angle % 6 == 0)
				{
					r.Kicker.State = Solenoid.SolenoidState.Out;
					Thread.Sleep(200);
					r.Kicker.State = Solenoid.SolenoidState.In;
				}*/
				angle++;
			}
		}

	}
}