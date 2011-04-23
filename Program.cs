using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;

namespace Technobotts
{
	public class Program
	{
		public static void Main()
		{
			// Blink board LED

			bool ledState = false;

			OutputPort led = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.LED, ledState);

			while (true)
			{
				// Sleep for 500 milliseconds
				Thread.Sleep(500);

				// toggle LED state
				ledState = !ledState;
				led.Write(ledState);
			}
		}

	}
}
