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
		static InputPort button;

		public static void waitForButton(int time = 200)
		{
			while (button.Read()) ;
			Thread.Sleep(time);
			while (!button.Read()) ;
		}

		public static void Main()
		{
			Robot r = new Robot();
			int angle = 0;
			button = r.Button;

			r.Drive.Stop();
			waitForButton();
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