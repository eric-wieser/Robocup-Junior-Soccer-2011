//#define NoMotors

using System;
using Math = System.Math;
using Button = Technobotts.Hardware.Button;
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
	public class Robot : IDisposable
	{
		public IntensityDetectorArray BallDetector;
		public IMotor MotorA;
		public IMotor MotorB;
		public IMotor MotorC;

		public HolonomicDrive Drive;
		public Solenoid Kicker;
		public Button Button;
		public AngleFinder Compass;
		public LightGate LightGate;

		public SensorPoller SensorPoller;

		public class LEDGroup
		{
			public LED Purple;
			public LED Orange;
			public LED Green;
			public LED White;

			public LEDGroup(FEZ_Pin.Digital purple, FEZ_Pin.Digital orange, FEZ_Pin.Digital green, FEZ_Pin.Digital white)
			{
				Purple = new LED(purple); //3.2-3.8V, 30mA => < 100 Ohms
				Orange = new LED(orange); //1.8-2.2V, 30mA => 37-50 ohms
				Green = new LED(green); //2.8-3V, 20mA => 15-25 ohms
				White = new LED(white); //3.5V, 20mA ????
			}
		}

		public LEDGroup LEDs;

		public Robot()
		{
			SensorPoller = new SensorPoller();
			#if NoMotors
				MotorA = new FakeMotor{Name = "A"};
				MotorB = new FakeMotor{Name = "B"};
				MotorC = new FakeMotor{Name = "C"};
			#else
			MotorA = new DCMotor(PWM.Pin.PWM1, FEZ_Pin.Digital.Di28, FEZ_Pin.Digital.Di29) { SpeedCharacteristic = DCMotor.Characteristic.Linear };
			MotorB = new DCMotor(PWM.Pin.PWM2, FEZ_Pin.Digital.Di30, FEZ_Pin.Digital.Di31) { SpeedCharacteristic = DCMotor.Characteristic.Linear };
			MotorC = new DCMotor(PWM.Pin.PWM3, FEZ_Pin.Digital.Di32, FEZ_Pin.Digital.Di33) { SpeedCharacteristic = DCMotor.Characteristic.Linear };

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

			//Kicker = new Solenoid(PWM.Pin.PWM5);

			BallDetector = IntensityDetectorArray.FromRadialSensors(SensorPoller.IRSensors);

			Button = new Button(FEZ_Pin.Digital.LDR);

			Compass = new HMC6352();

			LightGate = new LightGate(FEZ_Pin.AnalogIn.An0, 350, 160);

			
			LEDs = new LEDGroup(FEZ_Pin.Digital.Di4, FEZ_Pin.Digital.Di5, FEZ_Pin.Digital.Di6, FEZ_Pin.Digital.Di7);
		}

		public void ShowDiagnostics()
		{
			int brokenIRCount = SensorPoller.BrokenIRSensorCount;

			if (brokenIRCount > 12)
				LEDs.Orange.State = false;
			if (brokenIRCount > 8)
				LEDs.Orange.StartBlinking(500, 0.25);
			else if (brokenIRCount > 4)
				LEDs.Orange.StartBlinking(500, 0.5);
			else if (brokenIRCount > 0)
				LEDs.Orange.StartBlinking(500, 0.75);
			if (brokenIRCount == 0)
				LEDs.Orange.State = true;

			LEDs.Green.State = LightGate.IsObstructed;
		}

		public void Dispose()
		{
			Drive.Dispose();
			LightGate.Dispose();
			Kicker.Dispose();
			Button.Dispose();
			LightGate.Dispose();
			SensorPoller.Dispose();
		}
	}
}
