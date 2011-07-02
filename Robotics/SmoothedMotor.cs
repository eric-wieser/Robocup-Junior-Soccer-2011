using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.Hardware;
using Technobotts.Utilities;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Robotics
{
	class SmoothedDCMotor : DCMotor
	{
		public SmoothedDCMotor(PWM.Pin pwmPin, FEZ_Pin.Digital dirPin1, FEZ_Pin.Digital dirPin2)
			: base(pwmPin, dirPin1, dirPin2)
		{ }
		public LowPassFilter Filter = new LowPassFilter(0.05);
		public new double Speed
		{
			get { return base.Speed; }
			set { base.Speed = Filter.Apply(value); }
		}
	}
}
