using System;
using System.Threading;
using Microsoft.SPOT;
using Technobotts.Robotics;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Tests
{
	public class RegulatedMotorTest
	{
		public static void Main()
		{
			RegulatedMotor motor = new RegulatedMotor(
				new DCMotor(PWM.Pin.PWM2, FEZ_Pin.Digital.Di30, FEZ_Pin.Digital.Di31)
				{ SpeedCharacteristic = DCMotor.Characteristic.Linear }
			);

			RegulatedMotor motork = new RegulatedMotor(
				new DCMotor(PWM.Pin.PWM3, FEZ_Pin.Digital.Di32, FEZ_Pin.Digital.Di33)
				{
					SpeedCharacteristic = DCMotor.Characteristic.Linear
				}
			);
			motor.Speed = 1;
			Thread.Sleep(1000);
			motor.Speed = -1;
			Thread.Sleep(1000);
			motor.Speed = 0;
			Thread.Sleep(1000);
		}
	}
}
