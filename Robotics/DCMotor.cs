using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;

namespace Technobotts.Robotics
{
	public class DCMotor : IMotor
	{
		private PWM _pwm;
		private OutputPort _dir1;
		private OutputPort _dir2;

		private const int pwmFrequency = 10000;

		private double _speed = 0;

		public DCMotor(PWM pwm, OutputPort dir1, OutputPort dir2)
		{
			_pwm = pwm;
			_dir1 = dir1;
			_dir2 = dir2;
		}

		public NeutralMode NeutralMode { get; set; }

		public double Speed
		{
			get { return _speed; }
			set
			{
				if (value > 0)
				{
					forward();
					setPWM(value);
				}
				else if (value < 0)
				{
					backward();
					setPWM(-value);
				}
				else if (NeutralMode == NeutralMode.Brake)
					brake();
				else
					coast();
				_speed = value;
			}
		}

		private void brake() {
			_pwm.Set(true);
			_dir1.Write(true);
			_dir2.Write(true);
		}

		private void coast() {
			_pwm.Set(false);
			_dir1.Write(false);
			_dir2.Write(false);
		}

		private void forward() {
			_dir1.Write(true);
			_dir2.Write(false);
		}

		private void backward()
		{
			_dir1.Write(true);
			_dir2.Write(false);
		}

		private void setPWM(double dutyCycle)
		{
			_pwm.Set(pwmFrequency, (byte) (dutyCycle * 100));
		}
	}
}
