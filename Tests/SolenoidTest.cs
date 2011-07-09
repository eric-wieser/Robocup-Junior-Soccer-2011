using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF;
using GHIElectronics.NETMF.Hardware;

using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Tests
{
	class SolenoidTest
	{
		public static int fireTime = 250;
		public static uint period = 100;

		public static void Main()
		{
			/*
			InputPort button = new InputPort((Cpu.Pin)FEZ_Pin.Digital.LDR, true, Port.ResistorMode.PullUp);
			OutputPort solenoid = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.Di20, false);
			OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, false);
			PWM solenoidPulsed = new PWM((PWM.Pin) FEZ_Pin.PWM.Di10);
			while (true)
			{
				if (!button.Read())
				{
					led.Write(true);
					solenoidPulsed.SetPulse(period, period / 2);
					Thread.Sleep(fireTime);
					solenoidPulsed.Set(false);
					while (!button.Read()) Thread.Sleep(50) ;
					Thread.Sleep(500);
					led.Write(false);
				}
			}*/

		}
	}
}
