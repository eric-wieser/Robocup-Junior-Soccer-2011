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
			TSOP1138 sensor = new TSOP1138((Cpu.Pin)FEZ_Pin.Digital.Di0);
			TSOP1138 sensorShielded = new TSOP1138((Cpu.Pin)FEZ_Pin.Digital.Di1);
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
				long distance = sensor.Intensity;
				long period = 0;// sensor.ActualPeriod;
				long highTime = 0;//sensor.PulseWidth;
				long p2 = sensorShielded.Intensity;
				Debug.Print("Shielded: " + distance + ", " + period + ", " + highTime);
				Thread.Sleep(50);
			}

		}

	}
}
