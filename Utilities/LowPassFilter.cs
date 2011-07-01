using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.System;
namespace Technobotts.Utilities
{
	class LowPassFilter
	{
		private bool _initialized = false;
		private double _lastTime;
		private double _output;

		public double Tau { get; private set; }

		public LowPassFilter(double tau) {
			Tau = tau;
		}

		public double apply(double value)
		{
			if (_initialized)
			{
				double dt = SystemTime.SecondsSince(_lastTime);
				double a = MathEx.Exp(-dt / Tau);
				_output = (1 - a) * value + a * _output;
			}
			else
			{
				_output = value;
				_initialized = true;
			}
			_lastTime = SystemTime.Seconds;
			return _output;
		}

		public void reset()
		{
			_initialized = false;
		}
	}
}
