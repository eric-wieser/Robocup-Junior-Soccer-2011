using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using Technobotts.Hardware;

namespace Technobotts.Robotics
{
	class IRDetector : PwmIn
	{
		public IRDetector(Cpu.Pin pin) : base(pin) { }
		public int Period = 833;
		private long[] signalStrength = { 500, 400, 300, 200, 0 };
		public int Distance {
			get {
				long pulseWidth = this.PulseWidth;
				for (int i = 1; i < signalStrength.Length; i++)
					if (signalStrength[i - 1] >= pulseWidth && pulseWidth > signalStrength[i])
						return i;
				return signalStrength.Length;
			}
		}
	}
}
