using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using Technobotts.Hardware;
using GHIElectronics.NETMF.Hardware;
using Technobotts.Robotics;

namespace Technobotts.Hardware
{
	class TSOP1138 : PwmIn, IIntensityDetector
	{
		public const int CarrierFrequency = 40; //kHz
		new public int Period = 833;
		public TSOP1138(Cpu.Pin pin) : base(pin, 2) { }
		public int Intensity
		{
			get
			{
				const long shortPulse = 1000 / CarrierFrequency;

				long pulseWidth = (long)((1-DutyCycle) * Period);

				//No pulses
				if (pulseWidth < 4 * shortPulse)
					return 0;
				//Only 8 full power pulses
				else if (pulseWidth < 10 * shortPulse)
					return 1;
				//Also 4 quarter power pulses
				else if (pulseWidth < 14 * shortPulse)
					return 2;
				//Also 4 Sixteenth power pulses
				else if (pulseWidth < 18 * shortPulse)
					return 3;
				//Also 4 64th power pulses - Very close
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
