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
		public static InputPort button;

		public static void Main()
		{
			button = new InputPort((Cpu.Pin)FEZ_Pin.Digital.LDR, true, Port.ResistorMode.PullUp);
			double i = 0;
			double step = System.Math.PI / 16;
			using (DCMotor motor = new DCMotor(PWM.Pin.PWM1, FEZ_Pin.Digital.Di20, FEZ_Pin.Digital.Di21))
			{
				while (button.Read())
				{
					motor.Speed = MathEx.Sin(i);
					i += step;
					Thread.Sleep(50);
				}
			}
		}
	}
}
