using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using Technobotts.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace Technobotts.Robotics
{
	class IRDetector : PwmIn
	{
		public const int CarrierFrequency = 40; //kHz
		new public int Period = 833;
		public IRDetector(Cpu.Pin pin) : base(pin, 83) { }
		private long[] signalStrength = {0, 200, 300, 400, 500};
		public int Intensity
		{
			get
			{
				const long shortPulse = 1000 / 40;

				long pulseWidth = (long)(DutyCycle * Period);

				//No pulses
				if (pulseWidth < 4 * shortPulse)
					return 0;
				//Full power: 8 pulses
				else if (pulseWidth < 10 * shortPulse)
					return 1;
				//Quarter power: 4 pulses
				else if (pulseWidth < 14 * shortPulse)
					return 2;
				//Sixteenth power: 4 pulses
				else if (pulseWidth < 18 * shortPulse)
					return 3;
				//64th power: 4 pulses
				else
					return 4;
			}
		}
		public long ActualPeriod
		{
			get { return base.Period; }
		}

		public static void Main()
		{
			PinCapture sensor = new PinCapture((Cpu.Pin)FEZ_Pin.Digital.Di0, Port.ResistorMode.PullUp);
			uint[] data = new uint[256];
			bool state = true;
			long time = 0;
			int count = sensor.Read(state, data, 0, data.Length, 2000);
			for (int i = 0; i < count; i++)
			{
				Debug.Print(time + "\t" + (state ? 1 : 0));
				time += data[i];
				Debug.Print(time + "\t" + (state ? 1 : 0));
				state = !state;
			}
		}
	}
}
