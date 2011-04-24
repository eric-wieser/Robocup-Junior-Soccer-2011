using System;

namespace Technobotts.Robotics
{
	public enum NeutralMode
	{
		Brake,
		Coast
	}
	public interface IMotor
	{
		double Speed { get; set; }
		NeutralMode NeutralMode { get; set; }
	}
}
