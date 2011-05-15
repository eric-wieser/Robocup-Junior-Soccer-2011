using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace Technobotts
{
	public class Program
	{
		public static void Main()
		{
			// Blink board LED
			const uint period = 100;
			double duty = 0.5;

			//OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);
			PWM pwm = new PWM(PWM.Pin.PWM1);
			PinCapture pwmReader = new PinCapture((Cpu.Pin)FEZ_Pin.Digital.Di20, Port.ResistorMode.Disabled); 
			while (true)
			{
				pwm.SetPulse(period, (uint) (duty*period));
				uint[] data = new uint[16];
				pwmReader.Read(false, data, 0, data.Length, 2000);

				uint upTime = 0;
				uint downTime = 0;

				for (int i = 0; i < data.Length; i += 2)
				{
					upTime += data[i];
					downTime += data[i + 1];
				}

				double inDuty = (double) upTime / (upTime + downTime);
				Debug.Print("Intended duty: " + duty + ", actual duty: " + inDuty);
				Thread.Sleep(1000);
			}
		}

	}
}
