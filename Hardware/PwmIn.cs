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
		public int SampleTime { get; set; }

		Timer _controlLoop;

		public PwmIn(Cpu.Pin pin, int sampleTime = 100)
		{
			SampleTime = sampleTime;
			_in = new PinCapture(pin, Port.ResistorMode.PullUp);
			_controlLoop = new Timer(Recalculate, null, Timeout.Infinite, Timeout.Infinite);
		}

		public void StartPolling()
		{
			StartPolling(SampleTime);
		}

		public void StartPolling(int periodMs)
		{
			_controlLoop.Change(0, periodMs);
		}

		public void StopPolling()
		{
			_controlLoop.Change(Timeout.Infinite, Timeout.Infinite);
		}

		uint[] buffer = new uint[8];
		bool isSampling = false;

		public void Recalculate()
		{
			Recalculate(null);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		void Recalculate(object o)
		{
			if (isSampling) return;
			isSampling = true;

			bool state;
			int count = _in.Read(out state, buffer, 0, buffer.Length, SampleTime);
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

			isSampling = false;
		}

		public void Dispose()
		{
			_in.Dispose();
			_controlLoop.Dispose();
		}
	}
}
