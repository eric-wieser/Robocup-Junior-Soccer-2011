using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using System.Threading;

namespace Technobotts.Hardware
{
	class PwmIn : IDisposable
	{
		PinCapture _in;
		DateTime _lastRise = DateTime.Now;
		public long Period { get; private set; }
		public long PulseWidth { get; private set; }
		public double DutyCycle { get; private set; }

		Timer _controlLoop;
		int _period;

		public PwmIn(Cpu.Pin pin, double updatePeriod = 0.05)
		{
			_in = new PinCapture(pin, Port.ResistorMode.PullDown);
			_period = (int)(updatePeriod * 1000);
			_controlLoop = new Timer((state) => ((PwmIn)state).recalculate(), this, 0, _period);
		}

		uint[] buffer = new uint[8];
		void recalculate()
		{
			bool state;
			int count = _in.Read(out state, buffer, 0, buffer.Length, _period);
			if (count < 2)
			{
				Period = 0;
				PulseWidth = 0;
				DutyCycle = state ? 1 : 0;
			}
			else
			{
				long totalA = 0;
				long totalB = 0;
				int i;
				for (i = 0; i + 1 < count; i += 2)
				{
					totalA += buffer[i];
					totalB += buffer[i + 1];
				}
				Period = (totalA + totalB)*2 / i;
				PulseWidth = (state ? totalA : totalB)*2 / i;
				DutyCycle = (double) PulseWidth / Period;
			}
		}

		public void Dispose()
		{
			_in.Dispose();
		}
	}
}
