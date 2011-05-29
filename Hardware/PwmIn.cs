using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Technobotts.Hardware
{
	class PwmIn : IDisposable
	{
		PinCapture _in;
		DateTime _lastRise = DateTime.Now;
		public long Period {
			[MethodImpl(MethodImplOptions.Synchronized)]
			get;
			[MethodImpl(MethodImplOptions.Synchronized)]
			private set;
		}
		public long PulseWidth
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get;
			[MethodImpl(MethodImplOptions.Synchronized)]
			private set;
		}
		public double DutyCycle
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			get;
			[MethodImpl(MethodImplOptions.Synchronized)]
			private set;
		}

		Timer _controlLoop;
		int _period;

		public PwmIn(Cpu.Pin pin, double updatePeriod) : this(pin, (int) (updatePeriod * 1000))
		{ }

		public PwmIn(Cpu.Pin pin, int updatePeriod = 100)
		{
			_in = new PinCapture(pin, Port.ResistorMode.PullUp);
			_period = updatePeriod;
			_controlLoop = new Timer(Recalculate, this, 0, _period);
		}

		uint[] buffer = new uint[8];
		[MethodImpl(MethodImplOptions.Synchronized)]
		void Recalculate(object o)
		{
			bool state;
			int count = _in.Read(out state, buffer, 0, buffer.Length, _period);
			if (count < 4)
			{
				Period = 0;
				PulseWidth = 0;
				DutyCycle = state ? 1 : 0;
				Debug.Print("No signal!");
			}
			else
			{
				long totalA = 0;
				long totalB = 0;
				int i;
				count /= 2;
				for (i = 0; i < count; i ++)
				{
					totalA += buffer[2*i];
					totalB += buffer[2*i + 1];
				}
				
				Period = (totalA + totalB) / count;
				PulseWidth = (state ? totalA : totalB) / count;
				DutyCycle = (double) PulseWidth / Period;
			}
		}

		public void Dispose()
		{
			_in.Dispose();
		}
	}
}
