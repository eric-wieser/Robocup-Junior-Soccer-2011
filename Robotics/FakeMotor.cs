using System;
using Microsoft.SPOT;

namespace Technobotts.Robotics
{
	public class FakeMotor :IMotor
	{
		public string Name = "Motor";
		double _speed;
		public double Speed {
			get { return _speed; }
			set { _speed = value; Debug.Print(Name + ": " + value); }
		}

		public NeutralMode NeutralMode { get; set; }
	}
}
