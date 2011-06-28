using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Hardware
{
	public class LightGate : IDisposable
	{
		public int Threshold { get; set; } 
		public AnalogIn InnerPort { get; private set; }
		public bool IsObstructed
		{
			get
			{
				return InnerPort.Read() < Threshold;
			}
		}

		public LightGate(FEZ_Pin.AnalogIn pin, int openReading, int obstructedReading) :
			this(pin, (openReading + obstructedReading) / 2) { }

		public LightGate(FEZ_Pin.AnalogIn pin, int threshold) {
			InnerPort = new AnalogIn((AnalogIn.Pin)pin);
			Threshold = threshold;
		}

		public void Dispose() {
			InnerPort.Dispose();
		}
	}
}
