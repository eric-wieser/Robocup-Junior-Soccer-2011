using System;
using Technobotts.Geometry;
using GHIElectronics.NETMF.System;

namespace Technobotts.Robotics.Navigation
{
	public class HolonomicDrive
	{
		public class Wheel
		{
			public const double MaxSpeed = 1;

			public Vector position;
			private Matrix transformMatrix;
			private IMotor motor;

			public Wheel(Vector position, Vector driveAxis, Vector rollAxis, IMotor motor)
			{
				this.position = position;

				this.motor = motor;

				transformMatrix = Matrix.FromCoordinateAxes(driveAxis, rollAxis);
			}

			public double TargetSpeed { get; set; }

			public Vector TargetVector
			{
				set
				{
					Vector wheelVector = transformMatrix.Inverse * value;
					TargetSpeed = wheelVector.X;
				}
			}

			public void Update()
			{
				motor.Speed = TargetSpeed;
			}
		}

		Wheel[] Wheels;

		public HolonomicDrive(Wheel[] wheels)
		{
			Wheels = wheels;
		}

		private Vector _driveVelocity;
		public Vector DriveVelocity
		{
			get { return _driveVelocity; }
			set { _driveVelocity = value; update(); }
		}

		private double _turnVelocity;
		public double TurnVelocity
		{
			get { return _turnVelocity; }
			set { _turnVelocity = value; update(); }
		}


		public void set(Vector driveVelocity, double turnVelocity)
		{
			_driveVelocity = driveVelocity;
			_turnVelocity = turnVelocity;
			update();
		}

		protected void update()
		{
			foreach (Wheel wheel in Wheels)
			{
				wheel.TargetVector = DriveVelocity + wheel.position.Perpendicular * TurnVelocity;
			}

			normalize();

			foreach (Wheel wheel in Wheels)
			{
				wheel.Update();
			}
		}

		protected void normalize(bool scale = false)
		{
			double maxInputSpeed = 0;

			foreach (Wheel wheel in Wheels)
			{
				if (wheel.TargetSpeed > maxInputSpeed)
					maxInputSpeed = wheel.TargetSpeed;
				if (-wheel.TargetSpeed > maxInputSpeed)
					maxInputSpeed = -wheel.TargetSpeed;
			}

			double speedRatio = Wheel.MaxSpeed / maxInputSpeed;

			if (scale || maxInputSpeed > Wheel.MaxSpeed)
				foreach (Wheel wheel in Wheels)
					wheel.TargetSpeed *= speedRatio;
		}

	}
}
