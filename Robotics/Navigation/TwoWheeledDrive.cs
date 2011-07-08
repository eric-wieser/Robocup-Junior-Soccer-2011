using System;
using Microsoft.SPOT;

namespace Technobotts.Robotics.Navigation
{
	class TwoWheeledDrive
	{
		public IMotor LeftMotor;
		public IMotor RightMotor;

		public TwoWheeledDrive(IMotor left, IMotor right)
		{
			LeftMotor = left;
			RightMotor = right;
		}

		private double _speed;
		private double _rotation;

		public double Speed
		{
			set
			{
				_speed = value;
				update();
			}
			get
			{
				return _speed;
			}
		}
		public double TurnVelocity
		{
			set
			{
				_rotation = value;
				update();
			}
			get
			{
				return _rotation;
			}
		}

		private void update()
		{
			LeftMotor.Speed = _speed + _rotation;
			RightMotor.Speed = _speed - _rotation;
		}
	}
}
