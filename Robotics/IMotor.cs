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
		public double Speed { get; set; }
		public NeutralMode NeutralMode { get; set; }
	}
}
