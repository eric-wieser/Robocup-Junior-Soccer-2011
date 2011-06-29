using System;
using Technobotts.Geometry;
using GHIElectronics.NETMF.System;

namespace Technobotts.Robotics.Navigation
{
	public class HolonomicDrive : IDisposable
	{
		public class Wheel
		{
			public const double MaxSpeed = 1;

			public Vector Position {get; private set;}
			public Matrix TransformMatrix { get; private set; }
			public IMotor Motor { get; private set; }

			public Wheel(Vector position, Vector driveAxis, Vector rollAxis, IMotor motor)
				: this(position,  Matrix.FromCoordinateAxes(driveAxis, rollAxis), motor) { }

			public Wheel(Vector position, Matrix transformMatrix, IMotor motor)
			{
				Position = position;

				Motor = motor;
				Motor.NeutralMode = NeutralMode.Brake;

				TransformMatrix = transformMatrix;
			}

			public double TargetSpeed { get; set; }

			//private Vector v;
			public Vector TargetVector
			{
				//get { return v; }
				set
				{
					//v = value;
					Vector wheelVector = TransformMatrix.Inverse * value;
					TargetSpeed = wheelVector.X;
				}
			}

			public void Update()
			{
				Motor.Speed = TargetSpeed;
			}
		}

		Wheel[] Wheels;

		public Vector RotationPoint = 0;

		public HolonomicDrive(params Wheel[] wheels)
		{
			Wheels = wheels;
		}

		private Vector _driveVelocity = 0;
		public Vector DriveVelocity
		{
			get { return _driveVelocity; }
			set { _driveVelocity = value; update(); }
		}

		private double _turnVelocity = 0;
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
				wheel.TargetVector = DriveVelocity + (wheel.Position - RotationPoint).Perpendicular * TurnVelocity;
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


			if (maxInputSpeed != 0 && scale || maxInputSpeed > Wheel.MaxSpeed)
			{
				double speedRatio = Wheel.MaxSpeed / maxInputSpeed;
				foreach (Wheel wheel in Wheels)
					wheel.TargetSpeed *= speedRatio;
			}
		}

		public void Stop()
		{
			DriveVelocity = Vector.Zero;
		}

		public void Dispose()
		{
			foreach (Wheel wheel in Wheels)
			{
				IDisposable motor = wheel.Motor as IDisposable;
				if (motor != null) motor.Dispose();
			}
		}
	}
}
