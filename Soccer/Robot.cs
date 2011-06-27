//#define NoMotors

using System;
using Microsoft.SPOT.Hardware;
using Technobotts.Geometry;
using Technobotts.Robotics.Navigation;
using Technobotts.Robotics;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;
using Technobotts.Hardware;

namespace Technobotts.Soccer
{
	public class Robot
	{
		public IntensityDetectorArray BallDetector;
		public IMotor MotorA;
		public IMotor MotorB;
		public IMotor MotorC;

		public HolonomicDrive Drive;
		public Solenoid Kicker;
		public InputPort Button;
		public AngleFinder Compass;

		FEZ_Pin.Digital[] IRDetectorPins = new FEZ_Pin.Digital[] {
			FEZ_Pin.Digital.Di36, FEZ_Pin.Digital.Di37, FEZ_Pin.Digital.Di38, FEZ_Pin.Digital.Di39,
			FEZ_Pin.Digital.Di40, FEZ_Pin.Digital.Di41, FEZ_Pin.Digital.Di42, FEZ_Pin.Digital.Di43,
			FEZ_Pin.Digital.Di44, FEZ_Pin.Digital.Di45, FEZ_Pin.Digital.Di46, FEZ_Pin.Digital.Di47,
			FEZ_Pin.Digital.Di48, FEZ_Pin.Digital.Di49, FEZ_Pin.Digital.Di50, FEZ_Pin.Digital.Di51
		};

		public Robot()
		{
			#if NoMotors
				MotorA = new FakeMotor{Name = "A"};
				MotorB = new FakeMotor{Name = "B"};
				MotorC = new FakeMotor{Name = "C"};
			#else
				MotorA = new DCMotor(PWM.Pin.PWM1, FEZ_Pin.Digital.Di28, FEZ_Pin.Digital.Di29);
				MotorB = new DCMotor(PWM.Pin.PWM2, FEZ_Pin.Digital.Di30, FEZ_Pin.Digital.Di31);
				MotorC = new DCMotor(PWM.Pin.PWM3, FEZ_Pin.Digital.Di32, FEZ_Pin.Digital.Di33);
			#endif

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

			Kicker = new Solenoid(PWM.Pin.PWM5);


			IIntensityDetector[] detectors = new IRDetector[IRDetectorPins.Length];

			for (int i = 0; i < IRDetectorPins.Length; i++)
			{
				detectors[i] = new IRDetector((Cpu.Pin)IRDetectorPins[i]);
			}

			BallDetector = new RadialIntensityDetectorArray(detectors);

			Button = new InputPort((Cpu.Pin)FEZ_Pin.Digital.LDR, true, Port.ResistorMode.PullUp);

			Compass = new HMC6352();
		}
	}
}
