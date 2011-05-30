using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Technobotts.Robotics;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Threading;

namespace Technobotts.Tests
{
	class IRDetectorTest
	{
		static IRDetector sensor = new IRDetector((Cpu.Pin)FEZ_Pin.Digital.Di0);
		public static void TestRealSensor()
		{
			while (true)
			{
				Debug.Print("Reading: " + sensor.Intensity);
				Thread.Sleep(50);
			}
		}
		public static void TestFakeSensor()
		{
			PWM pwm = new PWM(PWM.Pin.PWM1);

			for (byte d = 0; d < 100; d++)
			{
				pwm.Set(1200, d);
				for (int i = 0; i < 10; i++)
				{
					Debug.Print("Out: " + d + ",\tIn: " + sensor.Intensity);
					Thread.Sleep(50);
				}

			}
		}
		public static void Main()
		{
			//TestFakeSensor();
			TestRealSensor();
		}
	}
}
