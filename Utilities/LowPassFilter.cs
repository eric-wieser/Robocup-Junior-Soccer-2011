using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.System;
namespace Technobotts.Utilities
{
	class LowPassFilter
	{
		private bool initialized = false;

		private double a;

		private double _output;

		public LowPassFilter(double tau, double period) {
			a = MathEx.Exp(-period / tau);
		}

		public double apply(double value)
		{
			if (initialized)
			{
				_output = (1 - a) * value + a * _output;
			}
			else
			{
				_output = value;
				initialized = true;
			}
			return _output;
		}

		public void reset()
		{
			initialized = false;
		}
	}
}
