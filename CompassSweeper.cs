using System;
using Microsoft.SPOT;
using System.Threading;
using Technobotts.Hardware;
using Technobotts.Geometry;

namespace Technobotts
{
	class CompassSweeper
	{
		public static void Main()
		{
			using (Soccer.Robot r = new Soccer.Robot())
			{
				r.Button.WaitForPress();
				HMC6352 compass = r.Compass.AngleFinder as HMC6352;
				r.Drive.RotationPoint = Vector.J * -500;
				r.Drive.TurnVelocity = -0.25;
				compass.StartCalibration();
				Thread.Sleep(20000);
				compass.EndCalibration();
				r.Drive.TurnVelocity = 0;
			}
		}
	}
}
