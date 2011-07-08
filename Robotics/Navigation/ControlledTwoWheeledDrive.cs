using System;
using Microsoft.SPOT;
using Technobotts.Utilities;

namespace Technobotts.Robotics.Navigation
{
	public class ControlledTwoWheeledDrive : TwoWheeledDrive
	{
		private IAngleFinder _compass;
		private PIDController _pid;
		private double[] pidConstants = new double[3] { 0.5, 0, 0};

		public ControlledTwoWheeledDrive(IMotor left, IMotor right, IAngleFinder compass)
			: base(left, right)
		{
			_compass = compass;
			_pid = new PIDController(pidConstants)
			{
				Input = new PIDController.InputFunction(
					() => _compass.Angle,
					Range.Angle
				),
				Output = new PIDController.OutputFunction(
					(value) => base.TurnVelocity = -value,
					new Range(0.5)
				),
				Continuous = true,
				SetPoint = 0,
				ErrorLimit = new Range(MathEx.TwoPi / 8)
			};
		}

		public double TargetHeading
		{
			get { return _pid.SetPoint; }
			set { _pid.SetPoint = value; ControlEnabled = true; }
		}
		public new double TurnVelocity
		{
			get { return base.TurnVelocity; }
			set { ControlEnabled = false; base.TurnVelocity = value; }
		}
		public bool ControlEnabled { get { return _pid.Enabled; } set { _pid.Enabled = value; } }
	}
}
