using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Robotics
{
	public class DCMotor : IMotor, IDisposable
	{
		private PWM _pwm;
		private OutputPort _dir1;
		private OutputPort _dir2;

		private const int pwmFrequency = 100;

		private double _speed = 0;

		public DCMotor(PWM pwm, OutputPort dir1, OutputPort dir2)
		{
			_pwm = pwm;
			_dir1 = dir1;
			_dir2 = dir2;
		}
		public DCMotor(PWM.Pin pwmPin, FEZ_Pin.Digital dirPin1, FEZ_Pin.Digital dirPin2)
		{
			_pwm = new PWM(pwmPin);
			_dir1 = new OutputPort((Cpu.Pin)dirPin1, false);
			_dir2 = new OutputPort((Cpu.Pin)dirPin2, false);
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
			_dir1.Write(false);
			_dir2.Write(true);
		}

		private void setPWM(double dutyCycle)
		{
			if (dutyCycle == 1)
				_pwm.Set(true);
			else
				_pwm.Set(pwmFrequency, (byte)(dutyCycle * 100));
		}

		public void Dispose()
		{
			coast();
			_pwm.Dispose();
			_dir1.Dispose();
			_dir2.Dispose();
		}
	}
}
