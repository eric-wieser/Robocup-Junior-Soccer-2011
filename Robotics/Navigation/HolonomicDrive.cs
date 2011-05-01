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
					Vector wheelVector = transformMatrix.Inverse() * value;
					TargetSpeed = wheelVector.Y;
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

		Vector driveVelocity = new Vector(0, 0);
		double turnVelocity;

		public void setDriveVelocity(Vector driveVelocity)
		{
			this.driveVelocity = driveVelocity;
			update();
		}

		public void setTurnVelocity(double turnVelocity)
		{
			this.turnVelocity = turnVelocity;
			update();
		}

		public void set(Vector driveVelocity, double turnVelocity)
		{
			this.driveVelocity = driveVelocity;
			this.turnVelocity = turnVelocity;
			update();
		}

		protected void update()
		{
			foreach (Wheel wheel in Wheels)
			{
				wheel.TargetVector = driveVelocity +
					Matrix.Rotate90 * wheel.position * turnVelocity;
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

		public void pidWrite(double output)
		{
			setTurnVelocity(output);
		}

	}
}
