using System;
using Microsoft.SPOT;
using Technobotts.Utilities;
using Technobotts.Geometry;

namespace Technobotts.Robotics.Navigation
{
	public class ControlledHolonomicDrive : HolonomicDrive
	{
		private PIDController _pid;
		private IAngleFinder _compass;
		// the next line is for unsmoothed motors
		private readonly double[] pidConstants = new double[3] { 0.7, 0.5, 0.06 };
		// the next line is for smoothed motors (we divide I by 6, and multiply D by 6 because it now 
		// step only a max of .33, however, we calculate this twice as fast (at 40 Hz) as we calculate the 
		// target: (20 Hz) 3

		// load of crap above.
		private const double Ku = 1.185;	// this is the value that it seemed to oscilate at (though the Regulated motor
											// code is messing with us.
		private const double Pu = 1.000;	// we think it's about one second for a full oscillation.
		// ziegler nichols method below for PID:
		// private readonly double[] pidConstants = new double[3] { 0.6* Ku, 0.6 * 2  * Ku / Pu, 0.6 * Ku * Pu / 8.0 };

		// original calculated values above... private readonly double[] pidConstants = new double[3] { 0.711, 1.422, .088875 };
		// private readonly double[] pidConstants = new double[3] { 0.711, 1.422, .088875 };
		//private readonly double[] pidConstants = new double[3] { 0.40, 1.10, .088875 };
		//private readonly double[] pidConstants = new double[3] { 0.6, 1, 0 };

		public ControlledHolonomicDrive(IAngleFinder compass, params Wheel[] wheels) : base(wheels)
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
				SetPoint = 0
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
