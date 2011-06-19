using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace Technobotts.Robotics
{
	public class Solenoid
	{
		public enum SolenoidState {
			Out,
			In
		}
		private PWM _port;
		public int Frequency {get; set; }

		private SolenoidState _state;
		public SolenoidState State {
			get {return _state; }
			set {
				if (value == SolenoidState.Out)
					_port.Set(Frequency, 50);
				else
					_port.Set(false);

				_state = value;
			}
		}

		public Solenoid(PWM port)
		{
			_port = port;
			Frequency = 10000;
			State = SolenoidState.In;
		}

		public Solenoid(PWM.Pin pin) :this(new PWM(pin)) { }
	}
}
