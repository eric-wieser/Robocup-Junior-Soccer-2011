using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Hardware
{
	public class LED : IDisposable
	{
		OutputPort led;
		Timer timer;
		int onTime, offTime;
		bool blinking = false;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Dispose()
		{
			blinking = false;
			timer.Dispose();
			led.Dispose();
		}

		public LED(FEZ_Pin.Digital pin)
		{
			led = new OutputPort((Cpu.Pin)pin, true);
			timer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
			State = false;
		}

		private bool _state;
		public bool State
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			set {
				if (blinking)
					StopBlinking();

				InternalState = value;
			}
			get {
				return InternalState;
			}
		}

		private bool InternalState
		{
			get { return _state; }
			set { _state = value; led.Write(_state); }
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void StartBlinking(int period, double dutyCycle)
		{
			onTime = (int)(period * dutyCycle);
			offTime = (int)(period * (1 - dutyCycle));

			if(!blinking) {
				blinking = true;
				timer.Change(0, Timeout.Infinite);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void StopBlinking()
		{
			blinking = false;
			timer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		void Callback(object o)
		{
			if (!blinking)
				return;

			InternalState = !InternalState;
			timer.Change(InternalState ? onTime : offTime, Timeout.Infinite);
		}
	}
}
