using System;
using Technobotts.Geometry;
using Technobotts.Robotics.Navigation;
using Technobotts.Robotics;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace Technobotts.Soccer
{
	public class Robot
	{
		AngleFinder Compass;
		IntensityDetectorArray BallDetector;
		Solenoid Kicker;
		IMotor MotorA;
		IMotor MotorB;
		IMotor MotorC;
		public HolonomicDrive Drive;
		public Robot()
		{
			MotorA = new DCMotor(PWM.Pin.PWM1, FEZ_Pin.Digital.Di21, FEZ_Pin.Digital.Di20);
			MotorB = new DCMotor(PWM.Pin.PWM2, FEZ_Pin.Digital.Di23, FEZ_Pin.Digital.Di22);
			MotorC = new DCMotor(PWM.Pin.PWM3, FEZ_Pin.Digital.Di25, FEZ_Pin.Digital.Di24);
			Drive = new HolonomicDrive(
				new HolonomicDrive.Wheel(
					Vector.FromPolarCoords(95, - Math.PI * 2 / 3),
					Matrix.FromRotation(- Math.PI * 2 / 3),
					MotorA
				),
				new HolonomicDrive.Wheel(
					Vector.FromPolarCoords(95, 0),
					Matrix.FromRotation(0),
					MotorB
				),
				new HolonomicDrive.Wheel(
					Vector.FromPolarCoords(95, Math.PI * 2 / 3),
					Matrix.FromRotation(Math.PI * 2 / 3),
					MotorC
				)
			);
		}
	}
}
