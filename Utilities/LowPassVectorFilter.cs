using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.System;
using Technobotts.Geometry;
namespace Technobotts.Utilities
{
	public class LowPassVectorFilter
	{
		private Vector _output;
		private double _lastTime;

		public double Tau { get; private set; }

		public LowPassVectorFilter(double tau)
		{
			Tau = tau;
		}

		public Vector apply(Vector value)
		{
			if (_output == null)
				_output = value;
			else {
				double dt = SystemTime.SecondsSince(_lastTime);
				double a = MathEx.Exp(-dt / Tau);
				_output = (1 - a) * value + a * _output;
			}
			_lastTime = SystemTime.Seconds;
			return _output;
		}

		public void reset()
		{
			_output = null;
		}
	}
}
