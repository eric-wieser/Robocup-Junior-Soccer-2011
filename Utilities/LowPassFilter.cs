using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.System;
namespace Technobotts.Utilities
{
	class LowPassFilter
	{
		private bool _initialized = false;
		private double _lastTime;
		public double Output {get; private set;}

		public double Tau { get; private set; }

		public LowPassFilter(double tau) {
			Tau = tau;
		}

		public double Apply(double value)
		{
			SystemTime.Update();
			if (_initialized)
			{
				double dt = SystemTime.SecondsSince(_lastTime);
				double a = MathEx.Exp(-dt / Tau);
				Output = (1 - a) * value + a * Output;
			}
			else
			{
				Output = value;
				_initialized = true;
			}
			_lastTime = SystemTime.Seconds;
			return Output;
		}

		public void Reset()
		{
			_initialized = false;
		}
	}
}
