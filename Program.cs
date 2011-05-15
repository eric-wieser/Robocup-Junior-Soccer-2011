using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using Technobotts.Robotics;
using Technobotts.Hardware;

namespace Technobotts
{
	public class Program
	{
		public static void Main()
		{
			IRDetector sensor = new IRDetector((Cpu.Pin)FEZ_Pin.Digital.Di0);
			IRDetector sensorShielded = new IRDetector((Cpu.Pin)FEZ_Pin.Digital.Di1);
				/*
			UltrasonicSensor us = new UltrasonicSensor((Cpu.Pin) FEZ_Pin.Digital.Di0);
			while (true)
			{
				Debug.Print("Distance: "+us.GetDistance());
				Thread.Sleep(200);
			}*/
			Debug.Print("T"+System.Double.PositiveInfinity / System.Double.PositiveInfinity);
			while (true)
			{
				long p1 = sensor.Distance;
				long p2 = sensorShielded.Distance; ;
				Debug.Print("Shielded: "+p1+"\tNon-shielded: " + p2);
			}

		}

	}
}
