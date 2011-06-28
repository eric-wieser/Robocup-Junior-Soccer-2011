//#define NoMotors

using System;
using Math = System.Math;
using Microsoft.SPOT;
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
		public LightGate LightGate;

		public SensorPoller SensorPoller;

		public Robot()
		{
			SensorPoller = new SensorPoller();
			#if NoMotors
				MotorA = new FakeMotor{Name = "A"};
				MotorB = new FakeMotor{Name = "B"};
				MotorC = new FakeMotor{Name = "C"};
			#else
			DCMotor.SpeedToPWMFunction s = DCMotor.Characteristic.Linear;
			MotorA = new DCMotor(PWM.Pin.PWM1, FEZ_Pin.Digital.Di28, FEZ_Pin.Digital.Di29)
			{ SpeedCharacteristic = s };

			MotorB = new DCMotor(PWM.Pin.PWM2, FEZ_Pin.Digital.Di30, FEZ_Pin.Digital.Di31)
			{ SpeedCharacteristic = s };

			MotorC = new DCMotor(PWM.Pin.PWM3, FEZ_Pin.Digital.Di32, FEZ_Pin.Digital.Di33)
			{ SpeedCharacteristic = s };
			#endif


				Matrix wheelMatrix = new Matrix(-60 * Math.PI, 0, 0, 12 * Math.PI);
				Vector wheelPosition = Vector.J * 95;
			Drive = new HolonomicDrive(
				new HolonomicDrive.Wheel(
					Matrix.FromClockwiseRotation(Math.PI / 3) * wheelPosition,
					Matrix.FromClockwiseRotation(Math.PI / 3) * wheelMatrix,
					MotorA
				),
				new HolonomicDrive.Wheel(
					Matrix.Rotate180 * wheelPosition,
					Matrix.Rotate180 * wheelMatrix,
					MotorB
				),
				new HolonomicDrive.Wheel(
					Matrix.FromClockwiseRotation(-Math.PI / 3) * wheelPosition,
					Matrix.FromClockwiseRotation(-Math.PI / 3) * wheelMatrix,
					MotorC
				)
			);

			Kicker = new Solenoid(PWM.Pin.PWM5);

			BallDetector = IntensityDetectorArray.FromRadialSensors(SensorPoller.IRSensors);

			Button = new InputPort((Cpu.Pin)FEZ_Pin.Digital.LDR, true, Port.ResistorMode.PullUp);

			Compass = new HMC6352();

			LightGate = new LightGate(FEZ_Pin.AnalogIn.An0, 350, 160);
		}
	}
}
