using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.System;
using Technobotts.Geometry;
namespace Technobotts.Utilities
{
	class LowPassVectorFilter
	{
		private bool initialized = false;

		private double a;

		private Vector _output;

		public LowPassVectorFilter(double tau, double period)
		{
			a = MathEx.Exp(-period / tau);
		}

		public Vector apply(Vector value)
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
