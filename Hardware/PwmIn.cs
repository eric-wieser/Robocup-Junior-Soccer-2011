using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Hardware
{
	class PwmIn : IDisposable
	{
		InputPort _in;
		DateTime _lastRise = DateTime.Now;

		public long Period { get; private set; }
		public long PulseWidth { get; private set; }
		public double DutyCycle { get { return (double) PulseWidth / Period; } }

		public PwmIn(FEZ_Pin.Interrupt pin)
		{
			_in = new InterruptPort((Cpu.Pin)pin, true,
				Port.ResistorMode.PullUp,
				Port.InterruptMode.InterruptEdgeBoth);

			_in.OnInterrupt += new NativeEventHandler(sensorInterrupt);
		}

		void sensorInterrupt(uint port, uint state, DateTime time)
		{
			const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

			long timeSpan = (time - _lastRise).Ticks / ticksPerMicrosecond;

			if(state == 1) //Rising Edge
			{
				Period = timeSpan;
				_lastRise = time;
			}
			else if (state == 0) //Falling Edge
			{
				PulseWidth = timeSpan;
			}
		}

		public void Dispose()
		{
			_in.Dispose();
		}
	}
}
