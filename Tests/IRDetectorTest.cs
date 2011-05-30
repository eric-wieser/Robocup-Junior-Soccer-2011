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
		static double getReading(int count = 10)
		{
			int reading = 0;
			for (int i = 0; i < count; i++)
			{
				sensor.Recalculate();
				reading += sensor.Intensity;
			}
			return reading;
		}
		public static void TestRealSensor()
		{
			while (true)
			{
				Debug.Print("Reading: " + getReading());
				Thread.Sleep(50);
			}
		}
		public static void TestFakeSensor()
		{
			PWM pwm = new PWM(PWM.Pin.PWM1);

			for (byte d = 0; d < 100; d++)
			{
				pwm.Set(1200, d);
				for (int i = 0; i < 5; i++)
				{
					Debug.Print("Out: " + d + ",\tIn: " + getReading());
				}
				Thread.Sleep(100);

			}
		}
		public static void Main()
		{
			TestFakeSensor();
			//TestRealSensor();
		}
	}
}
