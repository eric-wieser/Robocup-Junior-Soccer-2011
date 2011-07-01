using System;
using Microsoft.SPOT;
using Technobotts.Utilities;
using Technobotts.Geometry;

namespace Technobotts.Robotics.Navigation
{
	public class ControlledHolonomicDrive : HolonomicDrive
	{
		private PIDController _pid;
		private AngleFinder _compass;
		private readonly double[] pidConstants = PIDController.CoefficientsFromZieglerNicholsMethod(1.15, 0.87);

		public ControlledHolonomicDrive(AngleFinder compass, params Wheel[] wheels) : base(wheels)
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
					Range.SignedAngle * 2
				),
				Continuous = true,
				SetPoint = _compass.Angle
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
		public new void Stop()
		{
			ControlEnabled = false;
			Debug.Print("PID Disabled");
			base.Stop();
		}
		public Matrix GetOrientation()
		{
			return Matrix.FromClockwiseRotation(_compass.Angle);
		}
		public bool ControlEnabled { get { return _pid.Enabled; } set { _pid.Enabled = value; } }
	}
}
