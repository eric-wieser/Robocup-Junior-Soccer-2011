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

		public ControlledHolonomicDrive Drive;
		public HolonomicDrive OldDrive;
		public Solenoid Kicker;
		public Button Button;
		public FieldHeadingFinder Compass;
		public LightGate LightGate;

		public SensorPoller Sensors;

		public class LEDGroup
		{
			public LED Purple {get; private set;}
			public LED Orange { get; private set; }
			public LED Green { get; private set; }
			public LED White { get; private set; }

			public LED LaserIndicator { get { return Purple; } }
			public LED IRIndicator { get { return Orange; } }
			public LED ModeIndicator { get { return Green; } }
			public LED Other { get { return White; } }

			public bool State { set { Purple.State = Orange.State = Green.State = White.State = value; } }

			public void StartBlinking(int period, double dutyCycle)
			{
				Purple.StartBlinking(period, dutyCycle);
				Orange.StartBlinking(period, dutyCycle);
				Green.StartBlinking(period, dutyCycle);
				White.StartBlinking(period, dutyCycle);
			}

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
			Sensors = new SensorPoller();
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

			Compass = new FieldHeadingFinder(new HMC6352());

			Drive = new ControlledHolonomicDrive(Compass,
				new HolonomicDrive.Wheel(
					Matrix.FromClockwiseRotation(Math.PI / 3) * wheelPosition,
					Matrix.FromClockwiseRotation(Math.PI / 3) * wheelMatrix,
					// MotorA 
					new RegulatedMotor(MotorA)
				),
				new HolonomicDrive.Wheel(
					Matrix.Rotate180 * wheelPosition,
					Matrix.Rotate180 * wheelMatrix,
					// MotorB 
					new RegulatedMotor(MotorB)
				),
				new HolonomicDrive.Wheel(
					Matrix.FromClockwiseRotation(-Math.PI / 3) * wheelPosition,
					Matrix.FromClockwiseRotation(-Math.PI / 3) * wheelMatrix,
					//MotorC 
					new RegulatedMotor(MotorC)
				)
			);

#if USEOLDDRIVE
			OldDrive = new HolonomicDrive(
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
#endif

			//Kicker = new Solenoid(PWM.Pin.PWM5);

			BallDetector = IntensityDetectorArray.FromRadialSensors(Sensors.IR);

			Button = new Button(FEZ_Pin.Digital.An5);


			LightGate = new LightGate(FEZ_Pin.AnalogIn.An0, 350, 160);

			
			LEDs = new LEDGroup(FEZ_Pin.Digital.An1, FEZ_Pin.Digital.An2, FEZ_Pin.Digital.An3, FEZ_Pin.Digital.An4);
			RegulatedMotor.EnableRegulatedMotorAlgorithm(true);
		}

		public void ShowDiagnostics()
		{
			int brokenIRCount = Sensors.BrokenIRSensorCount;

			if (brokenIRCount == 16)
				LEDs.IRIndicator.State = false;
			else if (brokenIRCount == 0)
				LEDs.IRIndicator.State = true;
			else
				LEDs.IRIndicator.StartBlinking(500, 1 - brokenIRCount / 16.0);

			LEDs.LaserIndicator.State = LightGate.IsObstructed;
		}

		public void Dispose()
		{
			Drive.Dispose();
			LightGate.Dispose();
//			Kicker.Dispose();
			Button.Dispose();
			LightGate.Dispose();
			Sensors.Dispose();
		}
	}
}
