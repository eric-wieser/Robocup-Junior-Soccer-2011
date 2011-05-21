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
		public IRDetector(Cpu.Pin pin) : base(pin) { }
		new public int Period = 833;
		private long[] signalStrength = { 500, 400, 300, 200, 0 };
		public int Distance
		{
			get
			{
				long pulseWidth = (long) (this.DutyCycle * Period);
				//Debug.Print("t:"+pulseWidth);
				for (int i = 1; i < signalStrength.Length; i++)
					if (signalStrength[i - 1] >= pulseWidth && pulseWidth > signalStrength[i])
						return i;
				return signalStrength.Length;
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
