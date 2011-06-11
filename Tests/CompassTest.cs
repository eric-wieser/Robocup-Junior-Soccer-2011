using System;
using System.Threading;
using Microsoft.SPOT;
using Technobotts.Hardware;
using Technobotts.Geometry;
namespace Technobotts.Tests
{
	class CompassTest
	{

		public static void Main()
		{
			HMC6352 compass = new HMC6352();
			compass.WakeUp();
			while (true)
			{
				double heading = compass.Heading;
				Vector v = compass.MagnetometerReading;
				Debug.Print("Heading: " + heading);
				Debug.Print("Vector: " + v);
				Thread.Sleep(200);
			}
		}
	}
}
